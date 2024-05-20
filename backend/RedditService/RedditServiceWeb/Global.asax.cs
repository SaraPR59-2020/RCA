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
            //Dictionary<int, Comment> comments = new Dictionary<int, Comment>();

            users.Add(1, new User(1, "Marko", "Markovic", "", "", "", "", "marko.markovic@gmail.com", "marko", ""));
            topics.Add(1, new Topic(1, "Tema1", "tema", 1, 4, 3, 1, ""));
            //comments.Add(1, new Comment(1, "komentar1", 1));
            
            HttpContext.Current.Application["users"] = users;
            HttpContext.Current.Application["topics"] = topics;
            //HttpContext.Current.Application["comments"] = comments;
        }
    }
}
