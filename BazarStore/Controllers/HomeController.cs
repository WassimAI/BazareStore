﻿using BazarStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BazarStore.Controllers
{
    public class HomeController : Controller
    {
        private BazarEntities db = new BazarEntities();

        public ActionResult Index()
        {
            var model = new HomePageVM
            {
                Categories = db.Categories.ToList()
            };

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}