using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace oAuthOidcOwinClient.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult Secured()
        {
            return View();
        }

    }
}