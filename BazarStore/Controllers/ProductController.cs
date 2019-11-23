using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BazarStore.Controllers
{
    public class ProductController : Controller
    {
        private BazarEntities db = new BazarEntities();
        // GET: Product/all-products
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

        // GET: Product/product-details/product-name
        [ActionName("product-details")]
        public ActionResult ProductDetails(int id)
        {
            var product = db.Products.Find(id);

            if(product == null)
            {
                ModelState.AddModelError("", "This product does not exist");
                return HttpNotFound("Sorry, the product you are seeking does not exist");
            }

            ViewBag.slug = product.ProductName.Replace(" ", "-").ToLower();

            product.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            return View(product);
        }

        public ActionResult GetProductDetailsPartial(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                ModelState.AddModelError("", "This product does not exist");
                return HttpNotFound("Sorry, the product you are seeking does not exist");
            }

            product.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));


            return PartialView("_GetProductDetailsPartial", product);
        }
    }
}