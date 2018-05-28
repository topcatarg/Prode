using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prode.API.Services;
using Prode.API.Models;

namespace Prode.API.Controllers
{
    
    public class ForecastController : Controller
    {
        private readonly IForecastService _forecastService;
        private readonly IAuthorizationService _authorizationService;

        public ForecastController(IForecastService forecastService,
            IAuthorizationService authorizationService)
        {
            _forecastService = forecastService;
            _authorizationService = authorizationService;
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
            await _forecastService.FillAllGames(MatchsData);
            return Ok();
        }

    }
}