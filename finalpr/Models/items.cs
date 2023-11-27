using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace finalpr.Models
{
    public class items
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public string discount { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public int category { get; set; }
        public string image { get; set; }
       
    }
}
