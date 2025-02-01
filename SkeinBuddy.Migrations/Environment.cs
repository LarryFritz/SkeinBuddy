using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.Migrations
{
    public class Environment
    {
        public string HostName { get; set; } = null!;
        public string DbName { get; set; } = null!;
        public int Port { get; set; }
    }
}
