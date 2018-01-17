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
            return View(new DashboardViewModel()
            {
                Posts = new PostManager().GetPosts()
            });
        }
    }
}