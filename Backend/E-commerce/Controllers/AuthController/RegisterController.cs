using E_commerce.DTOs.Auth;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers.AuthController
{
    [Route("api/auth")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _registerService.RegisterAsync(dto);

            if (result.IsSuccess)
            {
                return Ok(BaseResponse<UserInfoResponse>.Ok(result.Data, result.Message));
            }
            else
            {
                return BadRequest(BaseResponse<UserInfoResponse>.Fail(result.Message));
            }
        }
    }
}
