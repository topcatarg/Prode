using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Prode.API.Services;
using Prode.API.Models;
using Prode.API.Helpers;

namespace Prode.API.Controllers
{

    public class AdminController : Controller
    {

        private readonly IAdminService _adminService;
        private readonly IAuthorizationService _authorizationService;

        public AdminController(IAdminService adminService,
            IAuthorizationService authorizationService)
        {
            _adminService = adminService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/GetAllResults")]
        public async Task<IActionResult> GetAllResults()
        {
            return new OkObjectResult(await _adminService.GetAllMatchs());
        }

        [HttpGet]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/GetTeamList")]
        public async Task<IActionResult> GetTeamList()
        {
            return new OkObjectResult(await _adminService.GetTeams());
        }

        [HttpPost]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/UpdateGame")]
        public async Task<IActionResult> UpdateGame([FromBody] MatchResult Match)
        {
            return new OkObjectResult(await _adminService.UpdateGame(Match));
        }

        [HttpPost]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/UpdatePoints")]
        public async Task<IActionResult> UpdateScores()
        {
            return new OkObjectResult(await _adminService.UpdateScores());
        }
    }
}