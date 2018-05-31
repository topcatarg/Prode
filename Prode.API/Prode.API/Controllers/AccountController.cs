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
        private readonly IMailServices _mailService;

        public AccountController(
            //IConfiguration configuration,
            //ISEApiService seApiService,
            IUserService userService,
            IAuthorizationService authorizationService,
            IMailServices mailService
            )
        {
            //_seApiService = seApiService;
            //_configuration = configuration;
            _userService = userService;
            _authorizationService = authorizationService;
            _mailService = mailService;
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
            //var accessToken = "tokenvalido";

            //Genero las claims. Si pago, no pago, o si es admin!
            var claims = new List<Claim>
            {
                new Claim(ClaimType.Id, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimType.Name, user.Name),
                new Claim(ClaimType.Mail, user.Mail),
                new Claim(ClaimType.TeamName, user.TeamName),
                new Claim(ClaimType.GameGroupId, user.GameGroupId.ToString(CultureInfo.InvariantCulture))
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimType.IsAdmin, "1"));
            }
            if (user.HasPaid)
            {
                claims.Add(new Claim(ClaimType.HasPaid, "1"));
            }
            if(user.ReceiveMails)
            {
                claims.Add(new Claim(ClaimType.ReceiveMails, "1"));
            }
            if(user.ReceiveAdminMails)
            {
                claims.Add(new Claim(ClaimType.ReceiveAdminMails, "1"));
            }
            var identity = new ClaimsIdentity(claims, "login");

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Ok();

            ////return Redirect(_seApiService.GetInitialOauthUrl(GetOauthReturnUrl(), returnUrl));
        }
        [HttpPost]
        [Route("api/logout")]
        public async Task<IActionResult> LogOut(string returnUrl = null)
        {
            await HttpContext.SignOutAsync();
            return new OkResult();
            //return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        [Authorize]
        [Route("api/me")]
        public async Task<IActionResult> WhoAmI()
        {
            var isAdmin = (await _authorizationService.AuthorizeAsync(User, ProdePolicy.IsAdmin)).Succeeded;
            string resul;
            resul = User.GetClaim<string>(ClaimType.HasPaid);
            bool hasPaid = false;
            if (resul != null || resul == "1")
            {
                hasPaid = true;
            }
            resul = User.GetClaim<string>(ClaimType.ReceiveMails);
            bool receiveMails = false;
            if (resul != null || resul == "1")
            {
                receiveMails = true;
            }
            resul = User.GetClaim<string>(ClaimType.ReceiveAdminMails);
            bool receiveAdminMails = false;
            if (resul != null || resul == "1")
            {
                receiveAdminMails = true;
            }


            var v = new UserInfo
            {
                Name = User.GetClaim<string>(ClaimType.Name),
                Mail = User.GetClaim<string>(ClaimType.Mail),
                HasPaid = hasPaid,
                IsAdmin = isAdmin,
                Id = User.GetClaim<int>(ClaimType.Id),
                TeamName = User.GetClaim<string>(ClaimType.TeamName),
                ReceiveMails = receiveMails,
                ReceiveAdminMails = receiveAdminMails,
                GameGroupId = User.GetClaim<int>(ClaimType.GameGroupId)
            };

            return new OkObjectResult(v);
        }

        [HttpGet]
        [Route("api/admin/errors/{path?}/{subPath?}")]
        public async Task Exceptions() => await ExceptionalMiddleware.HandleRequestAsync(HttpContext);

        [HttpGet]
        [Route("api/getstandartime")]
        public IActionResult GetStandarTime()
        {
            return new OkObjectResult(DateTime.UtcNow.AddMinutes(-175));
        }

        [HttpPost]
        [Route("api/recorverpassword")]
        public async Task<IActionResult> RecoverPassword(string mail)
        {
            //Check if the mail exists
            if (!await _userService.MailExists(mail))
            {
                return BadRequest();
            }
            //Create GUID
            Guid guid = Guid.NewGuid();
            var resul = await _mailService.SendRecoverPassword(mail,guid);
            if (resul == System.Net.HttpStatusCode.Accepted)
            {
                //Store guid on recovery table 
                if (await _userService.StoreGuidRecovery(guid, mail))
                {
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("api/setnewpassword")]
        public async Task<IActionResult> SetNewPassword(string pass, string guid)
        {
            if (await _userService.ChangePasswordAfterLost(pass, guid))
            {
                return Ok();
            }
            return BadRequest();
        }

        #region profile admin
        [HttpPost]
        [Authorize]
        [Route("api/changepassword")]
        public async Task<IActionResult> ChangePassword(int UserId, string NewPassword)
        {
            if (await _userService.ChangePassword(UserId, NewPassword))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        [Route("api/changeteamname")]
        public async Task<IActionResult> ChangeTeamName(int UserId, string TeamName)
        {
            if (await _userService.ChangeTeamName(UserId,TeamName))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        [Route("api/changemailreceived")]
        public async Task<IActionResult> ChangeMailReceived(int UserId, bool ReceiveMails, bool ReceiveAdminMails)
        {
            if (await _userService.ChangeReceiveMails(UserId, ReceiveMails?1:0, ReceiveAdminMails?1:0))
            {
                return Ok();
            }
            return BadRequest();
        }
        #endregion

        #region Create User

        [HttpPost]
        [Route("api/create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserInfo user)
        {
            if (string.IsNullOrEmpty(user.Name)
                || string.IsNullOrEmpty(user.Password)
                || string.IsNullOrEmpty(user.Mail)
                || string.IsNullOrEmpty(user.TeamName)
                || user.GameGroupId == 0)
            {
                return BadRequest(CreateUserResult.BadParameters);
            }
            //Check unique user and mail, better with unique in database
            var result = await _userService.UserExists(user.Name);
            if (result)
            {
                return BadRequest(CreateUserResult.UserAlreadyExist);
            }
            result = await _userService.MailExists(user.Mail);
            if (result)
            {
                return BadRequest(CreateUserResult.MailAlreadyExist);
            }
            var success = await _userService.CreateUserAsync(user.Name, user.Password, user.Mail, user.TeamName, user.GameGroupId);
            if (success)
            {
                await _mailService.SendHelloMail(user.Mail);
                return new OkResult();
            }
            else
            {
                return BadRequest(CreateUserResult.ErrorOnDatabase);
            }
        }


        [HttpGet]
        [Route("api/ExistGroup")]
        public async Task<IActionResult> ValidGroup(string group)
        {
            var v = await _userService.GroupExistAsync(group);
            if (v > 0)
            {
                return new OkObjectResult(v);
            }
            return BadRequest();
        }


        #endregion
    }
}