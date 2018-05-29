using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prode.API.Models
{
    public class ValidResults
    {
        public int id { get; set; }
        public int team1goals { get; set; }
        public int team2goals { get; set; }
        public int userid { get; set; }
        public int usergoals1 { get; set; }
        public int usergoals2 { get; set; }
        public int score { get; set; }
    }
}
