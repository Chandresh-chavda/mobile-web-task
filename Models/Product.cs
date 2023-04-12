using DataAccess;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [ValidateNever]
        [ForeignKey("applicationUser")]

        public string DealerId { get; set; }
        public ApplicationUser applicationUser { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public int Quentity { get; set; }
        public DateTime CreatedON { get; set; }= DateTime.Now;
        public DateTime UpdatedON { get; set;}= DateTime.Now;

       
        [Required]
       
        public int price { get; set; }
        
        public string? ImageUrl { get; set; } = "";

        public bool IsActive { get; set; }   


    }
}
