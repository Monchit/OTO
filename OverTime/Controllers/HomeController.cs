using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using WebCommonFunction;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using OverTime.Models;

namespace OverTime.Controllers
{
    public class HomeController : Controller
    {
        private TNCSecurity secure = new TNCSecurity();
        OverTimeEntities db = new OverTimeEntities();

        public ActionResult Index()
        {
            return View();
        }

        [Chk_Authen]
        public ActionResult Main()
        {
            return View();
        }

        [HttpPost]
        public JsonResult MyOTList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var user = Session["OT_Authen"].ToString();
                var query = db.td_otdetail.Where(w => w.empcode == user);

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.id,
                        s.empcode,
                        s.date,
                        s.time,
                        s.ot_normal,
                        s.ot_holiday,
                        s.ot_description
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateMyOT()
        {
            try
            {
                var user = Session["OT_Authen"].ToString();
                DateTime formData = DateTime.ParseExact(Request.Form["date"], "dd-MM-yyyy", null);
                var dbData = db.td_otdetail.Where(w => w.empcode == user && w.date == formData).FirstOrDefault();
                if (dbData == null)
                {
                    td_otdetail data = new td_otdetail();
                    data.empcode = user;
                    data.date = formData;
                    data.time = Request.Form["time"];
                    data.ot_normal = double.Parse(Request.Form["ot_normal"]);
                    data.ot_holiday = double.Parse(Request.Form["ot_holiday"]);
                    data.ot_description = Request.Form["ot_description"];
                    data.group_id = int.Parse(Session["OT_Group"].ToString());

                    db.td_otdetail.Add(data);
                    db.SaveChanges();
                    return Json(new { Result = "OK", Record = db.td_otdetail.OrderByDescending(o => o.date).FirstOrDefault() });
                }
                else
                {
                    return Json(new { Result = "Invalid", Message = "This date has OT !!!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateMyOT()
        {
            try
            {
                //var user = Session["OT_Authen"].ToString();
                //DateTime formData = DateTime.ParseExact(Request.Form["date"], "dd-MM-yyyy", null);
                long id = long.Parse(Request.Form["id"]);
                var data = db.td_otdetail.Find(id);
                data.time = Request.Form["time"];
                data.ot_normal = double.Parse(Request.Form["ot_normal"]);
                data.ot_holiday = double.Parse(Request.Form["ot_holiday"]);
                data.ot_description = Request.Form["ot_description"];
                data.group_id = int.Parse(Session["OT_Group"].ToString());
                db.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteMyOT()
        {
            try
            {
                //var user = Session["OT_Authen"].ToString();
                //DateTime formData = DateTime.ParseExact(Request.Form["date"], "dd-MM-yyyy", null);
                long id = long.Parse(Request.Form["id"]);
                var data = db.td_otdetail.Find(id);
                db.td_otdetail.Remove(data);
                db.SaveChanges();

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DailyOTList(string seldate, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var temp = seldate.Split('-');
                DateTime sdate = DateTime.Parse(temp[2] + "-" + temp[1] + "-" + temp[0]);
                var groupid = int.Parse(Session["OT_Group"].ToString());
                var query = db.td_otdetail.Where(w => w.group_id == groupid && w.date == sdate);

                //Get data from database
                int TotalRecord = query.Count();

                // Paging
                var output = query
                    .Select(s => new
                    {
                        s.empcode,
                        s.time,
                        s.ot_normal,
                        s.ot_holiday,
                        s.ot_description
                    }).OrderBy(jtSorting).Skip(jtStartIndex).Take(jtPageSize);

                //Return result to jTable
                return Json(new { Result = "OK", Records = output, TotalRecordCount = TotalRecord });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [Chk_Authen]
        public ActionResult Print()
        {
            int group_id = int.Parse(Session["OT_Group"].ToString());
            ViewBag.SelectDate = (from a in db.td_otdetail
                                  where a.group_id == group_id
                                  orderby a.date descending
                                  select a.date).Distinct();
            return View();
        }

        public ActionResult OTReport(int id)
        {
            return View();
        }

        public ActionResult Login()
        {
            string username = Request.Form["Username"].ToString();
            string pass = Request.Form["Password"].ToString();
            var chklogin = secure.Login(username, pass, false);//set false to true for Real

            if (chklogin != null)
            {
                if (chklogin.group_id == 135 || chklogin.group_id == 1) // System Only
                {
                    Session["OT_Authen"] = chklogin.emp_code; // Employee ID
                    Session["OT_Group"] = chklogin.group_id;
                    Session["OT_Name"] = chklogin.emp_fname + " " + chklogin.emp_lname;    // Name & Lastname

                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    TempData["noty"] = "You are not authorized to access this system !!!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["noty"] = "Username and/or Password is incorrect !!!";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session.Remove("OT_Authen");
            Session.Remove("OT_Name");
            Session.Remove("OT_Group");
            return RedirectToAction("Index", "Home");
        }
    }
}
