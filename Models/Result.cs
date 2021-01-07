using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Models
{
    public class Result
    {
        [Key]
        public int Id { get; set; }

        public string Oid { get; set; }

        public string OidName { get; set; }

        public string Value { get; set; }
    }
}
