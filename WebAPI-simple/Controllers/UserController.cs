using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI_simple.Models.DTO;
using WebAPI_simple.Repositories;

namespace WebAPI_simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public UserController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        //POST:/api/Auth/Register - chức năng đăng ký user 
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO) // khai báo kiểu model cho Register 
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDTO.Username,
                Email = registerRequestDTO.Username
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDTO.Password);
            if (identityResult.Succeeded)
            {
                //add roles to this user 
                if (registerRequestDTO.Roles != null && registerRequestDTO.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDTO.Roles);
                }
                if (identityResult.Succeeded)
                {
                    return Ok("Register Successful! Let login!");
                }
            }
            return BadRequest("Something wrong!");
        }// end action Register 

        //POST: /api/Auth/Login -chức năng đăng nhập User 
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        // khai báo model cho Login 
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDTO.Username);
            if (user != null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);// kiểm tra password nhập vào khớp với password lưu ở database
                if (checkPasswordResult)
                {
                    //get roles for this user – lấy quyền của user từ database 
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        //create token – tạo token cho user này 
                        var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDTO
                        {
                            JwtToken = jwtToken
                        };

                        return Ok(response); // trả về chuỗi token 
                    }
                }
            }
            return BadRequest("Username or password incorrect");
        } // end action Login 
    } // end class User controller 
}
