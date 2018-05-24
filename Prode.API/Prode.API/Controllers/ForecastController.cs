using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prode.API.Services;

namespace Prode.API.Controllers
{
    
    public class ForecastController : Controller
    {
        private readonly IForecastService _forecateService;

        public ForecastController(IForecastService forecastService)
        {
            _forecateService = forecastService;
        }


    }
}