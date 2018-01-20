using System.Web.Mvc;
using M183.Blog.Manager;
using M183.Blog.Models;

namespace M183.Blog.Controllers
{
    public class PostController : Controller
    {
        public ActionResult Index(int id)
        {
            return View(new DetailViewModel()
            {
                Post = new PostManager().GetPostById(id),
                Comment = ""
            });
        }

        [HttpPost]
        public ActionResult Comment(DetailViewModel viewModel)
        {
            new PostManager().Comment(viewModel.Comment, viewModel.Post.Id, Session["Username"].ToString());
            return RedirectToAction("Index", new { viewModel.Post.Id });
        }
    }
}