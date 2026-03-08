using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMApplication.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int CClient { get; set; }
        public DateTime? DateOrder { get; set; }
        public string? TypePayment { get; set; }        
    }
}
