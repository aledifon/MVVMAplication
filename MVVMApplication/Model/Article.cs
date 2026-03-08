using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMApplication.Model
{
    public class Article
    {
        public int Id { get; set; }
        public string? Section { get; set; }
        public string? ArticleName { get; set; }
        public decimal? Price { get; set; }
        public DateTime? Date { get; set; }
        public string? OriginCountry { get; set; }
    }
}
