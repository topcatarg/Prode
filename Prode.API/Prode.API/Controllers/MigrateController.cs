using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prode.API.Helpers;
using Prode.API.Services;

namespace Prode.API.Controllers
{

    public class MigrateController : Controller
    {
        private readonly IMigrateService _migrateService;

        public MigrateController(IMigrateService migrateService)
        {
            _migrateService = migrateService;
        }

        [HttpGet]
        [Route("api/migrate/migrateAll")]
        public async Task<IActionResult> MigrateAll()
        {
            return Ok();
        }

        [HttpGet]
        [Route("api/migrate/migrate1")]
        public async Task<IActionResult> Migrate1()
        {
            if (await _migrateService.Migrate1Async())
            {
                return Ok();
            }
            return BadRequest();
        }


    }
}