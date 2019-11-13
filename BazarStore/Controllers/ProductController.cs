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

            return View(productList);
        }
    }
}