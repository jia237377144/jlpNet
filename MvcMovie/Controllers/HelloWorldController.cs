using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MvcMovie.Controllers
{
    public class HelloWorldController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // 
        // GET: /HelloWorld/Welcome/  （手动高亮）

        public IActionResult Welcome(string message,int numTimes)
        {
            ViewData["message"] = message;
            ViewData["numTimes"] = numTimes;
            return View();
        }
    }
}