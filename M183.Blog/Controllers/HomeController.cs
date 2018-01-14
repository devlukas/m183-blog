using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using M183.Blog.Manager;
using M183.Blog.Models;

namespace M183.Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel viewModel)
        {
            if (await new UserManager().LoginAsync(viewModel))
            {
                Session["Username"] = viewModel.Username;
                return RedirectToAction("Index", "Home");
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> StartLogin(LoginViewModel viewModel)
        {
            var userManager = new UserManager();

            if (await userManager.ValidateCredentials(viewModel.Username, viewModel.Password))
            {
                try
                {
                    await userManager.GenerateAndSendLoginTokenAsync(viewModel.Username);
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                catch (BlogError error)
                {
                    return Json(new { result = false, error = error}, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new {result = false, error = "Falscher Benutzername oder Passwort!"}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}