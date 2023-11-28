using finalpr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace finalpr.Models
{
    public class orders
    {
        public int Id { get; set; }

        public int userid { get; set; }

        public int itemid { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime buyDate { get; set; }

        public int quantity { get; set; }
    }
}

