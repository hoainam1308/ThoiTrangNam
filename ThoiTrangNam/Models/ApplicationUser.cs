using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ThoiTrangNam.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public string? Address { get; set; }
        public ushort? Age { get; set; }
    }
}
