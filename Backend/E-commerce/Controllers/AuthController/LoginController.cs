using E_commerce.DTOs.Login;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers.LoginController
{
    [Route("api/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _loginService.LoginAsync(dto);
            if (result.IsSuccess)
            {
                return Ok(BaseResponse<LoginResponse>.Ok(result.Data, result.Message));
            }
            else
            {
                return Unauthorized(BaseResponse<LoginResponse>.Fail(result.Message, 401));
            }
        }
    }
}
