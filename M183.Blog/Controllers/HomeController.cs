using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using M183.Blog.Manager;
using M183.Blog.Models;

namespace M183.Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new DashboardViewModel()
            {
                Posts = new PostManager().GetPublishedPosts()
            });
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
            var userManager = new UserManager();

            try
            {
                if (await userManager.LoginWithTokenAsync(viewModel))
                {
                    Session["Username"] = viewModel.Username;
                    await new UserManager().AddUserLoginAsync(viewModel.Username, Session.SessionID, GetIPAddress());
                    string role = new UserManager().GetUserRole(viewModel.Username);
                    if (role == "Admin")
                    {
                        return RedirectToAction("Index", "AdminDashboard");
                    } else
                    {
                        return RedirectToAction("Index", "UserDashboard");
                    }
                }
            }
            catch (BlogError error)
            {
                viewModel.SmsToken = "";
                if (error.BlogErrorType == BlogErrorType.WrongUsernameOrPassword)
                {
                    viewModel.ShowSmsTokenField = true;
                }
                ModelState.AddModelError("", error.Message);
            }
            
            return View(viewModel);
        }

        /// <summary>
        /// JSON-API to Start the Login Process (Sends SMS-Token)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> StartLogin(LoginViewModel viewModel)
        {
            var userManager = new UserManager();
            try
            {
                // if the user is not blocked
                if (await userManager.BaseLogin(viewModel))
                {
                    await userManager.GenerateAndSendLoginTokenAsync(viewModel.Username);
                    return Json(new {result = true}, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = false, error = "Falscher Benutzername oder Passwort" }, JsonRequestBehavior.AllowGet);
            }
            catch (BlogError error)
            {
                await userManager.AddUserLogAsync(viewModel.Username, error.Message);
                return Json(new { result = false, error = error.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the Client Ip-Adress
        /// </summary>
        /// <returns></returns>
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