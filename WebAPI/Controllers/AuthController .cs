using BoardCasterWebAPI.Interfaces;
using BoardCasterWebAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Entities;

namespace BoardCasterWebAPI.Controllers
{
   // [Authorize(Policy = "UserPolicy")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly AuthService _tokenService;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
         IAuthService authService, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_tokenService = tokenService;
            _authService = authService;
            _logger = logger;
        }

        //[Authorize]
       // [HttpPost("login")]
        //public async Task<IActionResult> Login(LoginDto model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.UserName);

        //    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //        var roles = await _userManager.GetRolesAsync(user);
        //        var token = _tokenService.GenerateJwtToken(user, roles);

        //        return Ok(new { Token = token });
        //    }

        //    return Unauthorized();
        //}
        //[HttpPost("login")]
        //public IActionResult Login()
        //{
        //    //TokenService jWTTokenService = new TokenService();
        //    //var token = jWTTokenService.GenerateJwtToken();
        //    //return Ok(new { token });
        //}
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterUser model)
        //{

        //    //ApplicationUser user = new() { UserName = "test@test.com", Email = "test@test.com" };
        //    ////var result = await _userManager.CreateAsync(user, model.Password);

        //    ////if (result.Succeeded)
        //    ////{
        //    ////   // await _userManager.AddToRoleAsync(user, "User");
        //    //return Ok("User registered successfully");
        //    ////}

        //    ////return BadRequest("Registration failed");
        //}




        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                var (status, message) = await _authService.Login(model);
                if (status == 0)
                    return BadRequest(message);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("registeration")]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                var (status, message) = await _authService.Registeration(model, UserRoles.User);
                if (status == 0)
                {
                    return BadRequest(message);
                }
                return CreatedAtAction(nameof(Register), model);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
