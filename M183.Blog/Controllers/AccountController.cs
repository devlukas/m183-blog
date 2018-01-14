using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using M183.Blog.Manager;
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
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        private void AddErrors(string message)
        {
            ModelState.AddModelError("", message);
        }
    }
}