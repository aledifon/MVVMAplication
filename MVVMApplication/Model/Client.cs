using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMApplication.Model
{
    public class Client
    {
        public int Id { get; set; }
        public string? ClientName { get; set; }
        public string? Address { get; set; }
        public string? Location { get; set; }
        public string? Telephone { get; set; }                  
    }
}
