using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManagement.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }
        //public List<OrderLine> Lines { get; set; } = new();
    }
}
