using ASP.NET_API.Helpers;
using ASP.NET_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP.NET_API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly HashService _hashService;
        private readonly IDataProtector _dataProtector;

        public AccountsController(UserManager<IdentityUser> userManager,
                                  IConfiguration _configuration,
                                  SignInManager<IdentityUser> signInManager,
                                  IDataProtectionProvider dataProtectionProvider,
                                  HashService hashService)
        {
            _userManager = userManager;
            _configuration = _configuration;
            _signInManager = signInManager;
            _hashService = hashService;
            _dataProtector = dataProtectionProvider.CreateProtector(_configuration["ProtectorKey"]);
        }

        [HttpGet("CalculateHash/{plainText}")]
        public ActionResult CalculateHash(string plainText)
        {
            var res1 = _hashService.Hash(plainText);
            var res2 = _hashService.Hash(plainText);

            return Ok(new
            {
                plainText = plainText,
                Hash1 = res1,
                Hash2 = res2
            });
        }

        [HttpGet("Encrypt")]
        public ActionResult Encrypt()
        {
            var plainText = "A plain text";
            var encryptedText = _dataProtector.Protect(plainText);
            var unencryptedText = _dataProtector.Unprotect(encryptedText);

            return Ok(new
            {
                plainText = plainText,
                encryptedText = encryptedText,
                unencryptedText = unencryptedText,
            });
        }

        [HttpGet("TimeLimitedEncryptor")]
        public ActionResult TimeLimitedEncryptor()
        {
            var limitedTimeProtector = _dataProtector.ToTimeLimitedDataProtector();

            var plainText = "A plain text";
            var encryptedText = limitedTimeProtector.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));
            var unencryptedText = limitedTimeProtector.Unprotect(encryptedText);

            return Ok(new
            {
                plainText = plainText,
                encryptedText = encryptedText,
                unencryptedText = unencryptedText,
            });
        }

        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentials userCredentials)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredentials.Email,
                                    userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
                return await BuildToken(userCredentials);
            else
                return BadRequest("Incorrect login");
        }

        [HttpPost("register", Name = "Register")]
        public async Task<ActionResult<AuthenticationResponse>> Register(UserCredentials userCredentials)
        {
            var user = new IdentityUser
            {
                UserName = userCredentials.Email,
                Email = userCredentials.Email
            };

            var result = await _userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
                return await BuildToken(userCredentials);
            else
                return BadRequest(result.Errors);
        }

        [HttpGet("RefreshToken", Name = "Refresh")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticationResponse>> Refresh()
        {
            var emailClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
            var email = emailClaim?.Value;

            var userCredencials = new UserCredentials
            {
                Email = email,

            };

            return await BuildToken(userCredencials);
        }

        [HttpPost("ConvertToAdmin", Name = "ConvertToAdmin")]
        public async Task<ActionResult> ConvertToAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await _userManager.FindByEmailAsync(editAdminDTO.Email);
            await _userManager.AddClaimAsync(user, new Claim(PoliciesHelper.IsAnAdmin, "1"));
            return NoContent();
        }

        [HttpPost("ConvertToNoAdmin", Name = "ConvertToNoAdmin")]
        public async Task<ActionResult> ConvertToNoAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await _userManager.FindByEmailAsync(editAdminDTO.Email);
            await _userManager.RemoveClaimAsync(user, new Claim(PoliciesHelper.IsAnAdmin, "1"));
            return NoContent();
        }

        private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email),
            };

            var user = await _userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDB = await _userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);

            return new AuthenticationResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration,
            };
        }
    }
}
