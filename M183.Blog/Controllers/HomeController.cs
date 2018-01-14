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
                await new UserManager().AddUserLoginAsync(viewModel.Username, Session.SessionID, GetIPAddress());
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

        private string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}