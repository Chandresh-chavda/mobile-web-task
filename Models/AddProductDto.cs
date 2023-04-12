using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AddProductDto
    {
        [ValidateNever]
        public int? ProductID { get; set; }

        [ValidateNever]
        public string DealerId { get; set; }
        [Required]
       
        public string Name { get; set; }
       
        public string Description { get; set; }
        [Range(1,100)]
        public int Quentity { get; set; }
        public DateTime CreatedON { get; set; } = DateTime.Now;
        public DateTime UpdatedON { get; set; } = DateTime.Now;

        public double? Discount { get; set; }
        [Required]
       
        public int price { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; } 

        public bool IsActive { get; set; }
    }
}
