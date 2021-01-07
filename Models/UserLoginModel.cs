using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Models
{
    public class UserLoginModel // day la model request len
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        // em biet Required la j ko? la field nay bat buoc phai nhap
        //uh no la 1 cai validation bat buoc phai truyen vao
    }
}
