namespace ThoiTrangNam.APIs
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Authentication.Facebook;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using global::ThoiTrangNam.Models;
    using System.Security.Claims;

    [ApiController]
    [Route("api/[controller]")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExternalAuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("GoogleLogin")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, Url.Action("GoogleResponse", new { returnUrl }));
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest("Error loading external login information.");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                // Retrieve tokens
                var tokens = info.AuthenticationTokens;
                var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token")?.Value;

                // Return tokens
                return Ok(new { access_token = accessToken });
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new ApplicationUser { UserName = email, Email = email };
            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddLoginAsync(user, info);
                if (identityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    // Retrieve tokens
                    var tokens = info.AuthenticationTokens;
                    var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token")?.Value;

                    // Return tokens
                    return Ok(new { access_token = accessToken });
                }
            }
            return BadRequest("Error creating user.");
        }

        [HttpGet("FacebookLogin")]
        public IActionResult FacebookLogin(string returnUrl = "/")
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(FacebookDefaults.AuthenticationScheme, Url.Action("FacebookResponse", new { returnUrl }));
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet("FacebookResponse")]
        public async Task<IActionResult> FacebookResponse(string returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest("Error loading external login information.");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                var tokens = info.AuthenticationTokens;
                var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token")?.Value;
                return Ok(new { access_token = accessToken });
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new ApplicationUser { UserName = email, Email = email };
            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddLoginAsync(user, info);
                if (identityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    var tokens = info.AuthenticationTokens;
                    var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token")?.Value;
                    return Ok(new { access_token = accessToken });
                }
            }
            return BadRequest("Error creating user.");
        }

        [HttpGet("OpenIdConnectLogin")]
        public IActionResult OpenIdConnectLogin(string returnUrl = "/")
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("oidc", Url.Action("OpenIdConnectResponse", new { returnUrl }));
            return Challenge(properties, "oidc");
        }

        [HttpGet("OpenIdConnectResponse")]
        public async Task<IActionResult> OpenIdConnectResponse(string returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest("Error loading external login information.");

            // Extract tokens from the external login info
            var tokens = info.AuthenticationTokens;
            var idToken = tokens.FirstOrDefault(t => t.Name == "id_token")?.Value;

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
            var phoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone); // Hoặc Claim khác chứa số điện thoại
            var address = info.Principal.FindFirstValue(ClaimTypes.StreetAddress); // Claim chứa địa chỉ (nếu có)

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = $"{firstName} {lastName}",
                PhoneNumber = phoneNumber,
                Address = address
            };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddLoginAsync(user, info);
                if (identityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return Ok(new { id_token = idToken });
                }
            }
            return BadRequest("Error creating user.");
        }
    }
}
