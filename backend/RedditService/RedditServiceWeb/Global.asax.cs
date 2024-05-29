using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Common;

namespace RedditServiceWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Dictionary<int, User> users = new Dictionary<int, User>();
            Dictionary<int, Topic> topics = new Dictionary<int, Topic>();
            Dictionary<int, Comment> comments = new Dictionary<int, Comment>();

            users.Add(0, new User(0, "Marko", "Markovic", "", "", "", "", "marko.markovic@gmail.com", "marko", "/Images/defaultUser.png"));
            topics.Add(0, new Topic(0, "Tema1", "tema", 0, 0, 0, 1, "/Images/defaultUser.png"));
            comments.Add(0, new Comment(0, "komentar1", 0, 0));

            HttpContext.Current.Application["users"] = users;
            HttpContext.Current.Application["topics"] = topics;
            HttpContext.Current.Application["comments"] = comments;
        }
    }
}
