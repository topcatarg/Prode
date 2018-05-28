using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prode.API.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public int UserId { get; set; }
        public int Team1Forecast { get; set; }
        public int Team2Forecast { get; set; }
    }
}
