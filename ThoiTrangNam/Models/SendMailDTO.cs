using System.ComponentModel.DataAnnotations;

namespace ThoiTrangNam.Models
{
    public class SendMailDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email = StaticClass.FromEmail;
        [Required]
        public string Password = StaticClass.Password;
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }

    }
}
