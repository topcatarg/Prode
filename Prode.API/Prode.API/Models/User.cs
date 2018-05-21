using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prode.API.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Mail { get; set; }

        public bool HasPaid { get; set; }

        public bool IsAdmin { get; set; }

    }
}
