using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Discount
    {
        [Key]
        public int? Id { get; set; } 
        public int ProductId { get; set; }
        public string? ProductName { get; set; } 
        public int ProductPrise { get; set; }
        public DateTime FromDate { get; set; }= DateTime.Now;
        public DateTime ToDate { get; set; } = DateTime.Now;   

        public enum DiscountType
        {
            Rupee,Percentage
        }
        public DiscountType discountType { get; set; }

        public double DiscountAmount { get; set; }
    }
}
