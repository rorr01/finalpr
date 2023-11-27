using finalpr.Models;
using Microsoft.VisualBasic;

namespace finalpr.Models
{
    public class orders
    {
        public int Id { get; set; }
        public int userid { get; set; }
        public int itemid { get; set; }
        public DateTime buyDate { get; set; }
        public int quantity { get; set; }
        
    }
}

