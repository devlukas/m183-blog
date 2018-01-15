﻿using System.Web.Mvc;
using M183.Blog.Manager;
using M183.Blog.Models;

namespace M183.Blog.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            string username = "basis"; //TODO: (string)Session["Username"];
            return View(new DashboardViewModel() {
                Posts = new PostManager().GetPostsByUsername(username)
            });
        }

        public ActionResult DetailPost(int id)
        {
            return View(new PostManager().GetPostById(id));
        }
    }
}