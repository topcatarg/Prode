using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prode.API.Services;

namespace Prode.API.Controllers
{

    public class FixtureController : Controller
    {

        private readonly IFixtureService _fixtureService;

        public FixtureController(IFixtureService fixtureService)
        {
            _fixtureService = fixtureService;
        }

        [HttpGet]
        [Route("api/fixture/allmatchs")]
        public async Task<IActionResult> AllMatchs()
        {
            return Json(await _fixtureService.GetAllMatchs());
        }

    }
}