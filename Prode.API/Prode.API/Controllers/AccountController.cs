using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Prode.API.Helpers;
using Prode.API.Models;
using Prode.API.Services;
using Prode.API.Models.Enums;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Exceptional;

namespace Prode.API.Controllers
{
    public class AccountController : Controller
    {

        //private readonly ISEApiService _seApiService;
        //private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public AccountController(
            //IConfiguration configuration,
            //ISEApiService seApiService,
            IUserService userService,
            IAuthorizationService authorizationService
            )
        {
            //_seApiService = seApiService;
            //_configuration = configuration;
            _userService = userService;
            _authorizationService = authorizationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> LogInAsync([FromBody] UserInfo user)
        {
            //Logueo usr/pass
            user = await _userService.LoginUserAsync(user.Name, user.Password);
            if (user == null)
            {
                return BadRequest(1);
            }

            //Me devuelve un token
            var accessToken = "tokenvalido";

            //Genero las claims. Si pago, no pago, o si es admin!
            var claims = new List<Claim>
            {
                new Claim(ClaimType.Id, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimType.Name, user.Name),
                new Claim(ClaimType.Mail, user.Mail),
                new Claim(ClaimType.HasPaid, false.ToString())
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimType.IsAdmin, "1"));
            }

            var identity = new ClaimsIdentity(claims, "login");

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Ok();

            ////return Redirect(_seApiService.GetInitialOauthUrl(GetOauthReturnUrl(), returnUrl));
        }

        [HttpPost]
        [Route("api/create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserInfo user)
        {
            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Password) 
                || string.IsNullOrEmpty(user.Mail))
            {
                return BadRequest(CreateUserResult.BadParameters);
            }
            var success = await _userService.CreateUserAsync(user.Name, user.Password, user.Mail);
            if (success)
            {
                return new OkResult();
            }
            else
            {
                return BadRequest(CreateUserResult.ErrorOnDatabase);
            }
        }

        [HttpPost]
        [Route("api/logout")]
        public async Task<IActionResult> LogOut(string returnUrl = null)
        {
            await HttpContext.SignOutAsync();
            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        [Authorize]
        [Route("api/me")]
        public async Task<IActionResult> WhoAmI()
        {
            var isAdmin = (await _authorizationService.AuthorizeAsync(User, ProdePolicy.IsAdmin)).Succeeded;

            var v = new UserInfo
            {
                Name = User.GetClaim<string>(ClaimType.Name),
                Mail = User.GetClaim<string>(ClaimType.Mail),
                HasPaid = User.GetClaim<Boolean>(ClaimType.HasPaid),
                IsAdmin = isAdmin,
                Id = User.GetClaim<int>(ClaimType.Id)
            };

            return new OkObjectResult(v);

            //return Json(new UserInfo
            //{
            //    Name = User.GetClaim<string>(ClaimType.Name),
            //    Mail = User.GetClaim<string>(ClaimType.Mail),
            //    HasPaid = User.GetClaim<Boolean>(ClaimType.HasPaid),
            //    IsAdmin = isAdmin,
            //    Id = User.GetClaim<int>(ClaimType.Id)
            //});
        }

        [HttpGet]
        [Route("api/admin/errors/{path?}/{subPath?}")]
        public async Task Exceptions() => await ExceptionalMiddleware.HandleRequestAsync(HttpContext);

    }
}