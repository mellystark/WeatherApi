using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDo.Services;
using WeatherApi.Models;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly TokenService _tokenService;

        public AccountController(IAdminService adminService, TokenService tokenService)
        {
            _adminService = adminService;
            _tokenService = tokenService;
        }

        // Kayıt (Register)
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminService.RegisterAsync(model);

            if (result == "Kayıt başarılı!")
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // Giriş (Login)
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var validationResult = await _adminService.ValidateUserAsync(loginModel.Username, loginModel.Password);

            switch (validationResult)
            {
                case UserValidationResult.InvalidUsername:
                    return Unauthorized("Geçersiz kullanıcı adı.");
                case UserValidationResult.InvalidPassword:
                    return Unauthorized("Geçersiz şifre.");
                case UserValidationResult.UserNotFound:
                    return Unauthorized("Kullanıcı bulunamadı.");
                case UserValidationResult.Valid:

                    var token = _tokenService.GenerateToken();
                    return Ok(new { Token = token });
                default:
                    return Unauthorized("Geçersiz kullanıcı adı, şifre veya e-posta.");
            }
        }
    }
}
