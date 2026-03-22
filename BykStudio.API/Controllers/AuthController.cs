using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using BykStudio.data;
using BykStudio.data.DTOs;
using BykStudio.data.Interfaces;
using BykStudio.data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BykStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IEmailSender emailSender,
        IWebHostEnvironment env,
        ApplicationDbContext context) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var user = new ApplicationUser 
            { 
                UserName = model.Email, 
                Email = model.Email, 
                FullName = model.FullName,
                PhoneNumber = model.Phone
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Assign default role
            const string defaultRole = "User";
            if (!await roleManager.RoleExistsAsync(defaultRole))
            {
                await roleManager.CreateAsync(new IdentityRole(defaultRole));
            }
            await userManager.AddToRoleAsync(user, defaultRole);

            // Generate email confirmation token + URL
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                nameof(ConfirmEmail),
                "Auth",
                new { userId = user.Id, code = encodedCode },
                protocol: Request.Scheme);

            // Send confirmation email
            var emailSubject = "Confirm your email address";
            var emailBody = $"""
                Hi {user.FullName},<br/><br/>
                Please confirm your email by clicking the link below:<br/>
                <a href="{HtmlEncoder.Default.Encode(callbackUrl)}">Confirm Email</a>
                """;
            await emailSender.SendEmailAsync(user.Email!, emailSubject, emailBody);

            // Response
            var registerResponse = new RegisterResponse
            {
                Message = "User registered successfully. A confirmation email has been sent.",
                ConfirmationUrl = env.IsDevelopment() ? callbackUrl : null,
                UserId = user.Id,
                Code = encodedCode
            };
            return Ok(registerResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized();
            }
            // Optional manual check (redundant if RequireConfirmedEmail = true in options)
            if (!user.EmailConfirmed)
            {
                return BadRequest(new { message = "Please confirm your email before logging in." });
            }

            var signInResult = await signInManager.CheckPasswordSignInAsync(
                user, model.Password, lockoutOnFailure: true);

            if (signInResult.Succeeded)
            {
                var token = await GenerateJwtToken(user);
                return Ok(new { token });
            }

            if (signInResult.IsLockedOut)
            {
                return StatusCode(423, new { message = "Account is locked out due to multiple failed attempts." });
            }

            if (signInResult.IsNotAllowed)
            {
                return BadRequest(new { message = "Email not confirmed." });
            }

            return Unauthorized();
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Invalid email confirmation request.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ConfirmEmailAsync(user, decodedCode);

            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully. You can now log in.");
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var user = await context.Users
                    .Include(u => u.Bookings).ThenInclude(b => b.Room)
                    .Include(u => u.MakeupBookings).ThenInclude(mb => mb.MakeupTable)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return NotFound();

                var profileDto = new ProfileDto
                {
                    FullName = user.FullName ?? "",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber,
                    Bookings = user.Bookings.Select(b => new BookingSummaryDto
                    {
                        RoomName = b.Room?.Name,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime,
                        TotalPrice = b.TotalPrice
                    }).ToList(),
                    MakeupBookings = user.MakeupBookings.Select(mb => new MakeupBookingSummaryDto
                    {
                        MakeupTableName = mb.MakeupTable?.Name,
                        StartTime = mb.StartTime,
                        EndTime = mb.EndTime,
                        TotalPrice = mb.TotalPrice
                    }).ToList()
                };

                return Ok(profileDto);
            }
            catch (Exception ex)
            {
                // Log to console (or use ILogger)
                Console.WriteLine($"ERROR in GetProfile: {ex}");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            // Config validation
            var jwtKey = GetRequiredConfig("Jwt:Key");
            var issuer = GetRequiredConfig("Jwt:Issuer");
            var audience = GetRequiredConfig("Jwt:Audience");
            var expireMinutesStr = GetRequiredConfig("Jwt:ExpireMinutes");
            if (!double.TryParse(expireMinutesStr, out var expireMinutes) || expireMinutes <= 0)
            {
                throw new InvalidOperationException("Invalid Jwt:ExpireMinutes configuration.");
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(ClaimTypes.Name, user.UserName!),
                new("fullName", user.FullName ?? string.Empty)
            };

            // Add role claims
            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetRequiredConfig(string key)
        {
            var value = configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Configuration missing or empty: {key}");
            }
            return value;
        }
    }
}
