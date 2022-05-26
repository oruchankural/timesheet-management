﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TimeSheetManagement.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TimeSheetManagement.Controllers
{
    [Authorize]
    public class AuthenticationController : Controller
    {
        private readonly ILogger<AuthenticationController> _logger;
        Context c = new Context();
        IHostingEnvironment _env;
        public IConfiguration _config;
        public AuthenticationController(IHostingEnvironment environment, IConfiguration configuration)
        {
            _env = environment;
            _config = configuration;
            
        }
        /*--- Login ---*/
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string Response)
        {
            if (!string.IsNullOrEmpty(Response))
            {
                ViewBag.Response = Response;
            }
            else
            {
                ViewBag.Response = null;
            }
           
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(User p)
        {
            var logedUser = c.Users.FirstOrDefault(x => x.username == p.username);
            if (logedUser != null)
            {
                // Hashing Down !
                bool isValidPass = BCrypt.Net.BCrypt.Verify(p.password, logedUser.password);

                if (logedUser != null && isValidPass)
                {
                    var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name,p.username)
                    };
                    var useridentity = new ClaimsIdentity(claims, "Login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                    await HttpContext.SignInAsync(principal);

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {

                    return RedirectToAction("Login", new { Response = "Wrong Username or Password !" });
                }
            }


            else
            {
                return RedirectToAction("Login", new { Response = "An error occured !" });


            }

        }
        /*--- Login End ---*/




        /*--- SignUp ---*/
        [AllowAnonymous]
        [HttpGet]
        public IActionResult SignUp(string Response)
        {
            if (!string.IsNullOrEmpty(Response))
            {
                ViewBag.Response = Response;
            }
            else
            {
                ViewBag.Response = null;
            }
           

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult SignUp(User p)
        {
            var foundedUser = c.Users.FirstOrDefault(x => x.mail == p.mail || x.username == p.username);
            if(foundedUser == null)
            {
                // Hashing Up !
                p.password = BCrypt.Net.BCrypt.HashPassword(p.password);
                c.Users.Add(p);
                c.SaveChanges();
                return RedirectToAction("Login", new { Response = ""+p.username+" your account created succesfully" });
            }
            else
            {
                if(c.Users.Where(x=>x.username == p.username && x.mail == p.mail).Count() > 0 )
                {
                   // mail ve username aynı !

                }
                else
                {
                    string Response = "";
                    if(c.Users.Where(x => x.username == p.username).Count() > 0)
                    {
                        //
                        Response += "Username aynı";
                    }
                    if(c.Users.Where(x => x.mail == p.mail).Count() > 0)
                    {
                        //
                        Response += "Mail  aynı";
                    }


                    
                }
              
                return RedirectToAction("SignUp", new { Response = "" + p.mail + " this email is already used by another user." });
            }
            

        }

       

        /*--- SignUp End---*/




        /*[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }*/


    }
}
