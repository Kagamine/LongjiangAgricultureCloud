﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LongjiangAgricultureCloud.Models;
using LongjiangAgricultureCloud.Schema;
using LongjiangAgricultureCloud.Helpers;

namespace LongjiangAgricultureCloud.Controllers
{
    public class ServiceController : BaseController
    {
        // GET: Service
        public ActionResult Index(int? CatalogID ,DateTime? Begin, DateTime? End, InformationType? Type, int p = 0)
        {
            ViewBag.Level1 = (from c in DB.Catalogs
                              where c.Level == 0
                              && c.Type == CatalogType.农机服务分类
                              select c).ToList();
            ViewBag.Level2 = (from c in DB.Catalogs
                              where c.Level == 1
                              && c.Type == CatalogType.农机服务分类
                              select c).ToList();
            ViewBag.Level3 = (from c in DB.Catalogs
                              where c.Level == 2
                              && c.Type == CatalogType.农机服务分类
                              select c).ToList();
            ViewBag.Level4 = (from c in DB.Catalogs
                              where c.Level == 3
                              && c.Type == CatalogType.农机服务分类
                              select c).ToList();
            IEnumerable<Information> query = DB.Informations;
            if (CatalogID.HasValue)
                query = query.Where(x => x.CatalogID == CatalogID.Value);
            if (Begin.HasValue)
                query = query.Where(x => x.Time >= Begin.Value);
            if (End.HasValue)
                query = query.Where(x => x.Time <= End.Value);
            if (Type.HasValue)
                query = query.Where(x => x.Type == Type.Value);
            query = query.OrderByDescending(x => x.Time);
            ViewBag.PageInfo = PagerHelper.Do(ref query, 50, p);
            return View(query);
        }

        public ActionResult Delete(int id)
        {
            var information = DB.Informations.Find(id);
            DB.Informations.Remove(information);
            DB.SaveChanges();
            return Content("ok");
        }

        public ActionResult Verify(int id)
        {
            var information = DB.Informations.Find(id);
            return View(information);
        }

        public ActionResult Edit(int id)
        {
            var information = DB.Informations.Find(id);
            return View(information);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verify(int id, bool Verify)
        {
            var information = DB.Informations.Find(id);
            information.Verify = true;
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Title, string Description, int? CatalogID, string Name, string Phone, string Address)
        {
            var Information = new Information
            {
                Title = Title,
                Description = Description,
                Name = Name,
                Phone = Phone,
                Address = Address,
                CatalogID = CatalogID,
                UserID = CurrentUser.ID,
                Time = DateTime.Now
            };
            var Picture = Request.Files["Picture"];
            if (Picture != null)
            {
                using (var binaryReader = new BinaryReader(Picture.InputStream))
                {
                    Information.Picture = binaryReader.ReadBytes(Picture.ContentLength);
                }
            }
            else
            {
                Information.Picture = null;
            }
            DB.Informations.Add(Information);
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string Title, string Description, int? CatalogID, string Name, string Phone, string Address)
        {
            var information = DB.Informations.Find(id);
            information.Title = Title;
            information.Description = Description;
            information.CatalogID = CatalogID;
            information.Name = Name;
            information.Phone = Phone;
            information.Address = Address;
            var Picture = Request.Files["Picture"];
            if (Picture != null)
            {
                using (var binaryReader = new BinaryReader(Picture.InputStream))
                {
                    information.Picture = binaryReader.ReadBytes(Picture.ContentLength);
                }
            }
            DB.SaveChanges();
            return RedirectToAction("Success", "Shared");
        }
    }
}