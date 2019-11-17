using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BazarStore.Controllers
{
    public class ProductController : Controller
    {
        private BazarEntities db = new BazarEntities();
        // GET: Product
        [ActionName("all-products")]
        public ActionResult AllProducts()
        {
            var productList = new List<Product>();
            productList = db.Products.ToList();

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");

            return View(productList);
        }

        public ActionResult GetProductsPartial(string searchText="", int category=0)
        {
            var products = db.Products.AsQueryable();

            if (!String.IsNullOrEmpty(searchText))
                products = products.Where(x => x.ProductName.Contains(searchText));

            if (category != 0)
                products = products.Where(x => x.CategoryID == category);

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");

            return PartialView("_GetProductsPartial", products);
        }
    }
}