using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prode.API.Services;
using Prode.API.Models;
using Prode.API.Helpers;
using System.Collections.Immutable;

namespace Prode.API.Controllers
{
    
    [Produces("application/json")]
    public class ForecastController : Controller
    {
        private readonly IForecastService _forecastService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMailServices _mailService;
        private readonly IUserService _userService;

        public ForecastController(IForecastService forecastService,
            IAuthorizationService authorizationService,
            IMailServices mailService,
            IUserService userService)
        {
            _forecastService = forecastService;
            _authorizationService = authorizationService;
            _mailService = mailService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        [Route("api/forecast/my")]
        public async Task<IActionResult> GetMyForecast(int UserId)
        {
            return new OkObjectResult(await _forecastService.GetUserMatchs(UserId));
        }

        [HttpPost]
        [Authorize]
        [Route("api/forecast/fillgame")]
        public async Task<IActionResult> FillGame([FromBody]Match MatchData )
        {
            if (await _forecastService.FillMatch(MatchData))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/forecast/fillall")]
        public async Task<IActionResult> FillAllGames([FromBody] Match[] MatchsData)
        {
            if (await _forecastService.FillAllGames(MatchsData))
            {
                int userId = User.GetClaim<int>(ClaimType.Id);
                string userMail = User.GetClaim<string>(ClaimType.Mail);
                string TeamName = User.GetClaim<string>(ClaimType.TeamName);
                //Send mails if needed
                string resul = User.GetClaim<string>(ClaimType.ReceiveMails);
                ImmutableArray<Matchs> m;
                if (resul != null || resul == "1")
                {
                    //send mail
                    m = await _forecastService.GetUserMatchs(userId);
                    await _mailService.SendMyResultsAsync(userMail, TeamName, m);
                }
                var isAdmin = (await _authorizationService.AuthorizeAsync(User, ProdePolicy.IsAdmin)).Succeeded;
                if (isAdmin)
                {
                    if (m == null)
                    {
                        m = await _forecastService.GetUserMatchs(userId);
                    }
                    var to = await _userService.GetMailsFromAdminForecastReceivers();
                    //send mail to users
                    await _mailService.SendAdminResultsAsync(to, TeamName, m);
                }
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        [Route("api/forecast/others")]
        public async Task<IActionResult> GetOtherForecast(int UserId)
        {
            return new OkObjectResult(await _forecastService.GetClosedUserMatchs(UserId));
        }

    }
}