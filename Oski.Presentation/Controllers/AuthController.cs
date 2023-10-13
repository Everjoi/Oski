using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oski.Application.DTOs;
using Oski.Application.Interfaces;
using System.Security.Claims;
using IAuthenticationService = Oski.Application.Interfaces.IAuthenticationService;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oski.Presentation.Controllers
{

    [ApiController]
    public class AuthController  : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginRequestDto model)
        {
            var token = _authService.Authenticate(model.Email,model.Password);

            if(token == null)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.NameIdentifier, token)
            };

             var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
             HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(identity));

            return RedirectToAction("GetAllTests","Test");
        }
        
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto model)
        {
            var userRegistered = _authService.Register(model.FullName,model.Email,model.Password);

            if(userRegistered.Result == Guid.Empty)
                return BadRequest("Username already exists or there was an issue with registration.");

            var token = _authService.Authenticate(model.Email,model.Password);

            if(token == null)
                return BadRequest("Issue generating token after registration.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.NameIdentifier, token),
                new Claim(ClaimTypes.Actor,userRegistered.ToString())
            };

            var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(identity));

            return RedirectToAction("GetAllTests","Test");

        }


        [HttpGet("")]
        public IActionResult Index()
        {
            return Ok();
        }



    }

}
