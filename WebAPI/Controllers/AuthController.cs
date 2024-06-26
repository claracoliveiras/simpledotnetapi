﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/conta")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IOptions<JWTSettings> _jwtSettings;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<JWTSettings> jwtSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok(GerarJwt());
            }
            return Problem("Falha ao registrar o usuário");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                return Ok(GerarJwt());
            }
            return Problem("Falha ao registrar o usuário");
        }

        private string GerarJwt()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.Segredo);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings.Value.Emissor,
                Audience = _jwtSettings.Value.Audiencia,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.Value.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);
            return encodedToken;
        }
    }
}
