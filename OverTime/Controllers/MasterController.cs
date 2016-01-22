using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OverTime.Models;

namespace OverTime.Controllers
{
    public class MasterController : Controller
    {
        OverTimeEntities db = new OverTimeEntities();

        public ActionResult User()
        {
            return View();
        }

        public ActionResult CustomGroup()
        {
            return View();
        }
    }
}
