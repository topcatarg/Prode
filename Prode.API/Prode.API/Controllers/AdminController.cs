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

        [HttpGet]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/GetGroupList")]
        public async Task<IActionResult> GetGroupList()
        {
            return new OkObjectResult(await _adminService.GetGroups());
        }

        [HttpGet]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/GetUserList")]
        public async Task<IActionResult> GetUserList(int GroupId)
        {
            return new OkObjectResult(await _adminService.GetUsers(GroupId));
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
        [Route("api/admin/UpdateTeams")]
        public async Task<IActionResult> UpdateTeams([FromBody] MatchResult Match)
        {
            return new OkObjectResult(await _adminService.UpdateTeams(Match));
        }

        [HttpPost]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/UpdatePoints")]
        public async Task<IActionResult> UpdateScores()
        {
            return new OkObjectResult(await _adminService.UpdateScores());
        }

        [HttpPost]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/CreateGroup")]
        public async Task<IActionResult> CreateGroup(string GroupName)
        {
            if (await _adminService.CreateGroup(GroupName))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/ChangePaidValue")]
        public async Task<IActionResult> ChangePaidValue(int UserId)
        {
            await _adminService.ChangePaid(UserId);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/DeleteUserFromGroup")]
        public async Task<IActionResult> DeleteUserFromGroup(int UserId, int GroupId)
        {
            await _adminService.DeleteUserFromGroup(UserId, GroupId);
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = ProdePolicy.IsAdmin)]
        [Route("api/admin/BlankPass")]
        public async Task<IActionResult> BlankPass(int UserId)
        {
            await _adminService.BlankPass(UserId);
            return Ok();
        }
    }
}