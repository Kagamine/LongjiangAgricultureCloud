﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Helpers;

namespace LongjiangAgricultureCloud.Areas.Mobile.Controllers
{
    public class MobileController : BaseController
    {
        /// <summary>
        /// 提示信息
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ActionResult Message(string sid, string msg)
        {
            if (Session["sid"].ToString() != sid)
                msg = "您无权执行本操作！";
            ViewBag.Msg = msg;
            return View();
        }

        /// <summary>
        /// 手机端首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Informations = (from i in DB.Informations
                                    where i.Type == LongjiangAgricultureCloud.Models.InformationType.农业信息
                                    && i.Recommend == true
                                    orderby i.Time descending
                                    select i).ToList();
            ViewBag.Products = (from p in DB.Products
                                where p.Recommend
                                && !p.Delete
                                orderby p.ID descending
                                select p).ToList();
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Member");
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string Username, string Password, string returnUrl)
        {
            var pwd = Helpers.Security.SHA1(Password);
            System.Diagnostics.Debug.WriteLine(pwd);
            var user = (from u in DB.Users
                        where u.Username == Username
                        && u.Password == pwd
                        && !u.Delete
                        select u).SingleOrDefault();
            if (user == null)
            {
                return Msg("用户名或密码错误！");
            }
            else
            {
                FormsAuthentication.SetAuthCookie(Username, false);
                if (string.IsNullOrEmpty(returnUrl))
                    return Redirect(Request.UrlReferrer == null ? "/Mobile/Member" : Request.UrlReferrer.ToString());
                else
                    return Redirect(returnUrl);
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Confirm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User User, string Confirm)
        {
            User.Role = UserRole.普通用户;
            if (User.Username.Length != 11)
            {
                return Msg("请输入合法的手机号码！");
            }
            if (string.IsNullOrEmpty(User.Question) || string.IsNullOrEmpty(User.Answer))
                return Msg("请填写密码提示问题或答案!");
            foreach (var x in User.Username)
            {
                if (!"1234567890".Contains(x))
                {
                    return Msg("手机号码不合法！");
                }
            }
            if (User.Password != Confirm)
                return Msg("两次密码输入不一致！");
            if (DB.Users.Any(x => x.Username == User.Username))
                return Msg("该手机号码已存在，请修改后重新尝试注册！");
            User.Password = Security.SHA1(Confirm);
            if (User.Name == null)
                User.Name = "";
            if (User.Address == null)
                User.Address = "";
            if (User.PostCode == null)
                User.PostCode = "";
            DB.Users.Add(User);
            DB.SaveChanges();
            return Msg("注册成功！您可以使用该账号进行登录了！");
        }

        /// <summary>
        /// 服务条款
        /// </summary>
        /// <returns></returns>
        public ActionResult License()
        {
            ViewBag.Content = System.IO.File.ReadAllText(Server.MapPath("~/Files/License.html"));
            return View();
        }

        /// <summary>
        /// 分类列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="Service"></param>
        /// <returns></returns>
        public ActionResult Catalog(string type, int? id, InformationType? Service)
        {
            ViewBag.ShopNav = false;
            CatalogType Type;
            Enum.TryParse(type, out Type);

            if (id.HasValue)
            {
                var catalog = DB.Catalogs.Find(id);
                if (catalog.Type == CatalogType.商品分类 && catalog.Level == 2)
                    return RedirectToAction("List", "Mall", new { id = catalog.ID });
                else if (catalog.Type == CatalogType.农业信息分类 && catalog.Level == 3)
                    return RedirectToAction("List", "MInformation", new { id = catalog.ID });
                else if (catalog.Type == CatalogType.农机服务分类 && catalog.Level == 3)
                    return RedirectToAction("List", "MService", new { id = catalog.ID, Service = Service });
                else if (catalog.Type == CatalogType.本地通分类 && catalog.Level == 2)
                    return RedirectToAction("List", "MLocal", new { id = catalog.ID });
                else
                {
                    if (catalog.Type == CatalogType.商品分类)
                    {
                        ViewBag.ShopNav = true;
                        ViewBag.Product = true;
                    }
                    ViewBag.Title = catalog.Title;
                    return View(catalog.Catalogs.Where(x => !x.Delete).ToList());
                }
            }
            else
            {
                if (Type == CatalogType.商品分类)
                {
                    ViewBag.ShopNav = true;
                    ViewBag.Product = true;
                }
                ViewBag.Title = Type.ToString();
                return View(DB.Catalogs.Where(x => x.Type == Type && x.Level == 0 && !x.Delete).ToList());
            }
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <returns></returns>
        public ActionResult Forgot()
        {
            return View();
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public ActionResult Forgot2(string Username)
        {
            var user = DB.Users.Where(x => x.Username == Username).SingleOrDefault();
            if (user == null) return Msg("没有找到该用户");
            return View(user);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Answer"></param>
        /// <param name="Password"></param>
        /// <param name="Confirm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Forgot3(string Username, string Answer, string Password, string Confirm)
        {
            if (string.IsNullOrEmpty(Password))
                return Msg("请输入密码");
            var user = DB.Users.Where(x => x.Username == Username).SingleOrDefault();
            if (user == null) return Msg("没有找到该用户");
            if (Answer != user.Answer)
                return Msg("问题回答错误，无法找回密码");
            if (Password != Confirm)
                return Msg("两次密码输入不一致");
            user.Password = Security.SHA1(Confirm);
            DB.SaveChanges();
            return Msg("密码已经成功重置！");
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Mobile");
        }

        /// <summary>
        /// 图片
        /// </summary>
        /// <returns></returns>
        public ActionResult Image()
        {
            return View();
        }

    }
}