using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMApplication.Model
{
    /// <summary>
    /// This script will correspond with the Model
    /// </summary>

    internal class Item
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int Quantity { get; set; }
    }
}
