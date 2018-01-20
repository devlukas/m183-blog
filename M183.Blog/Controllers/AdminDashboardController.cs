using M183.Blog.Manager;
using M183.Blog.Models;
using System.Web.Mvc;

namespace M183.Blog.Controllers
{
    public class AdminDashboardController : Controller
    {
        // GET: AdminDashboard
        public ActionResult Index()
        {
            if (Session["Username"] != null && new UserManager().HasRoles(Session["Username"].ToString(), "Admin"))
            {
                return View(new DashboardViewModel()
                {
                    Posts = new PostManager().GetPosts()
                });
            } else
            {
                return RedirectToAction("Login", "Home");
            }
           
        }
    }
}