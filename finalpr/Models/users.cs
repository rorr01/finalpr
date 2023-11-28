using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace finalpr.Models
{
    public class users
    {
        public int Id { get; set; }
        public string name { get; set; }

        public string password { get; set; }

        public string role { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime registerDate { get; set; }
    }
}

