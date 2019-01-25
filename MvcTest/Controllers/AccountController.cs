using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcTest.Models;

namespace MvcTest.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm]LoginUser user)
        {
            var list = new List<LoginUser> {
                 new LoginUser { UserName = "Admin", Password = "admin", Role = "admin" },
                 new LoginUser { UserName = "System", Password = "system", Role = "system" }
             };
            var loginUser = list.Find(x => x.UserName.Equals(user.UserName) && x.Password.Equals(user.Password));
            if (loginUser != null)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Sid, loginUser.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Name, loginUser.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, loginUser.Role));
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                return RedirectToAction("index", "home");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}