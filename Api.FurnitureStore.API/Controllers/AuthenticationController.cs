﻿using Api.FurnitureStore.API.Configuration;
using Api.FurnitureStore.Shared.Auth;
using Api.FurnitureStore.Shared.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace Api.FurnitureStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AuthenticationController(UserManager<IdentityUser> userManager,
                                       IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest();

            //verify email exits
            var emailExists = await _userManager.FindByEmailAsync(request.EmailAdress);
            if (emailExists != null) return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                        {
                            "Email already Exits"
                        }
            });
            //create user
            var user = new IdentityUser()
            {
                Email = request.EmailAdress,
                UserName = request.EmailAdress
            };
            var isCreated = await _userManager.CreateAsync(user);
            if (isCreated.Succeeded)
            {
                var token = GenerateToken(user);
                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = token
                });

            }
            else
            {
                var errors = new List<string>();
                foreach (var err in isCreated.Errors)
                    errors.Add(err.Description);

                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = errors
                });
            }
            return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string> { "User  couldn't be created" }

            });
        }

        private string GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                })),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }
    }
}

