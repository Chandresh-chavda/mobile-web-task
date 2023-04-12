using Microsoft.AspNetCore.Identity;
using Models;
using System.ComponentModel.DataAnnotations;


namespace DataAccess
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        
        public string? Country { get; set; }
        public bool IsActive { get; set; }
        public Status.status status { get; set; }
       
        public string Reason { get; set; }

    }
    
}