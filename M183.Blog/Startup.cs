﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(M183.Blog.Startup))]
namespace M183.Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
