using BazarStore.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BazarStore.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private BazarEntities db = new BazarEntities();
        // GET: Admin/Category
        public ActionResult GetCategories()
        {
            var categoryList = new List<CategoryVM>();
            categoryList = db.Categories.ToArray().Select(x => new CategoryVM(x)).ToList();

            return View(categoryList);
        }

        public ActionResult Create()
        {
            var model = new CategoryVM();

            return View(model);
        }
    }
}