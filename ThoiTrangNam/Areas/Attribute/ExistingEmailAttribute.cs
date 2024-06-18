using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ThoiTrangNam.Models;

public class ExistingEmailAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var userManager = (UserManager<ApplicationUser>)validationContext
            .GetService(typeof(UserManager<ApplicationUser>));

        var email = (string)value;
        var user = userManager.FindByEmailAsync(email).Result;

        if (user == null)
        {
            return new ValidationResult(ErrorMessage ?? "Email không tồn tại trong hệ thống.");
        }

        return ValidationResult.Success;
    }
}
