using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prode.API.Models
{
    public class MatchResult
    {
        public int MatchId { get; set; }
        public int Team1 { get; set; }
        public int Team2 { get; set; }
        public int Team1Goals { get; set; }
        public int Team2Goals { get; set; }

    }
}
