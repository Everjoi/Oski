using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oski.Application.DTOs;
using Oski.Application.Features.Tests.Queries.DTOsQueries;
using Oski.Application.Interfaces;
using System.Security.Claims;
using IAuthenticationService = Oski.Application.Interfaces.IAuthenticationService;

namespace Oski.Presentation.Controllers
{
    
    public class AuthController  : Controller
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login( LoginRequestDto model)
        {
            //var token = _authService.Authenticate(model.Email,model.Password);

            //if(token == null)
            //    return Unauthorized();

            //return Ok(new LoginResponseDto { Token = token });
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
            //if(!ModelState.IsValid)
            //    return BadRequest(ModelState);

            //var userRegistered = _authService.Register(model.FullName,model.Email,model.Password);

            //if(!userRegistered)
            //    return BadRequest("Username already exists or there was an issue with registration.");

            //return Ok("User registered successfully");

            var userRegistered = _authService.Register(model.FullName,model.Email,model.Password);

            if(!userRegistered)
                return BadRequest("Username already exists or there was an issue with registration.");

            var token = _authService.Authenticate(model.Email,model.Password);

            if(token == null)
                return BadRequest("Issue generating token after registration.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.NameIdentifier, token)
            };

            var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(identity));

            return RedirectToAction("GetAllTests","Test");


        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        public IActionResult Register()
        { 
            return View();
        }

    }

}
