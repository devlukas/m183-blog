using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using M183.Blog.Manager;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using M183.Blog.Models;

namespace M183.Blog.Controllers
{
    [Authorize]
    public class AccountController : Controller
    { 
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                await new UserManager().RegisterAsync(model);
                return RedirectToAction("Index", "Home");
            }
            catch (BlogError error)
            {
                AddErrors(error.Message);
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            Session["Username"] = null;
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(string message)
        {
            ModelState.AddModelError("", message);
        }
    }
}