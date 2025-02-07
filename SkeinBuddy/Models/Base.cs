using SkeinBuddy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.Models
{
    public class Base
    {
        [PgColumn("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [PgColumn("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [PgColumn("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
