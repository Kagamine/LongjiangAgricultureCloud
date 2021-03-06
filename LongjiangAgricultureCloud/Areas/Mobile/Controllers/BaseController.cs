﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LongjiangAgricultureCloud.Models;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class BaseController : Controller
    {
        public readonly LongjiangAgricultureCloudContext DB = new LongjiangAgricultureCloudContext();
        public User CurrentUser;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = (from u in DB.Users
                               where u.Username == User.Identity.Name
                               select u).Single();
                ViewBag.CurrentUser = CurrentUser;
            }
            ViewBag.Back = false;
            ViewBag.Refresh = false;
            ViewBag.Menu = false;
            ViewBag.Product = false;
            ViewBag.Cart = false;
            ViewBag.Add = false;
            ViewBag.Url = "#";
            ViewBag.Provinces = DB.Areas.Where(x => x.Level == AreaLevel.省).ToList();
            ViewBag.Cities = DB.Areas.Where(x => x.Level == AreaLevel.市).ToList();
            ViewBag.Districts = DB.Areas.Where(x => x.Level == AreaLevel.区县).ToList();
            ViewBag.Towns = DB.Areas.Where(x => x.Level == AreaLevel.乡镇).ToList();
            ViewBag.Hamlets = DB.Areas.Where(x => x.Level == AreaLevel.村).ToList();
            ViewBag.Villages = DB.Areas.Where(x => x.Level == AreaLevel.屯).ToList();
            ViewBag.VerifyProductComment = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyProductComment"]);
            ViewBag.VerifyLocalTongComment = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyLocalTongComment"]);
            ViewBag.VerifyService = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyService"]);
            ViewBag.VerifyLocalTong = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyLocalTong"]);
            ViewBag.InformationComment = Convert.ToBoolean(ConfigurationManager.AppSettings["InformationComment"]);
            ViewBag.AlipayAppKey = ConfigurationManager.AppSettings["AlipayAppKey"];
            ViewBag.WeixinPayAppKey = ConfigurationManager.AppSettings["WeixinPayAppKey"];
            ViewBag.VerifyInformationComment = Convert.ToBoolean(ConfigurationManager.AppSettings["VerifyInformationComment"]);
            ViewBag.ServiceTel = ConfigurationManager.AppSettings["ServiceTel"];
        }

        public ActionResult Msg(string msg)
        {
            return RedirectToAction("Message", "Mobile", new { msg = msg, sid = Session["sid"].ToString() });
        }
    }
}