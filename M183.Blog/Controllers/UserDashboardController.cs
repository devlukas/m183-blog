using System.Web.Mvc;
using M183.Blog.Manager;
using M183.Blog.Models;

namespace M183.Blog.Controllers
{
    public class UserDashboardController : Controller
    {
        public ActionResult Index()
        {
            if (Session["Username"] != null && new UserManager().HasRoles(Session["Username"].ToString(), "Default"))
            {
                string username = "basis"; //TODO: (string)Session["Username"];
                return View(new DashboardViewModel()
                {
                    Posts = new PostManager().GetPostsByUsername(username)
                });
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}