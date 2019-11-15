using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BazarStore;
using BazarStore.Extensions;

namespace BazarStore.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private BazarEntities db = new BazarEntities();

        // GET: Admin/Product
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Supplier);

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");

            return View(products.ToList());
            //return PartialView("_GetProductsPartial", products.ToList());
        }

        public ActionResult GetProductsPartial(string searchText="", int category = 0)
        {
            var products = db.Products.AsQueryable();

            //products = db.Products.Include(p => p.Category).Include(p => p.Supplier).ToList();

            if (!String.IsNullOrEmpty(searchText))
                products = products.Where(x => x.ProductName.StartsWith(searchText));

            if (category!=0)
            {
                products = products.Where(x => x.CategoryID == category);
            }
                

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");

            return PartialView("_GetProductsPartial", products);
        }

        // GET: Admin/Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            var model = new Product();

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName");
            return View(model);
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product model, HttpPostedFileBase file)
        {
            if(db.Products.Any(x=>x.ProductName == model.ProductName))
            {
                ModelState.AddModelError("", "Sorry, product with the same name already exists");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var productToAdd = new Product()
                {
                    ProductName = model.ProductName,
                    CategoryID = model.CategoryID,
                    Category = model.Category,
                    SupplierID = model.SupplierID,
                    Supplier = model.Supplier,
                    QuantityPerUnit = model.QuantityPerUnit,
                    UnitPrice = model.UnitPrice,
                    ReorderLevel = model.ReorderLevel,
                    UnitsInStock = model.UnitsInStock,
                    UnitsOnOrder = model.UnitsOnOrder,
                    Discontinued = model.Discontinued

                };

                db.Products.Add(productToAdd);
                db.SaveChanges();

                if(file != null && file.ContentLength > 0)
                {
                    if (ImageProcessor.SaveImage(productToAdd.ProductID, file, "Products"))
                    {
                        productToAdd.ImageUrl = file.FileName;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sorry, the Image was not uploaded, please check the file type");
                        return View(model);
                    }
                }

                TempData["success"] = "Product: " + productToAdd.ProductName + " is added successfully.";
                return RedirectToAction("Create");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", model.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", model.SupplierID);

            ModelState.AddModelError("", "Please check the red notifications below");
            return View(model);
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", product.SupplierID);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var productToChange = db.Products.Find(model.ProductID);
                productToChange.ProductName = model.ProductName;
                productToChange.Supplier = model.Supplier;
                productToChange.Category = model.Category;
                productToChange.CategoryID = model.CategoryID;
                productToChange.UnitPrice = model.UnitPrice;
                productToChange.UnitsInStock = model.UnitsInStock;
                productToChange.QuantityPerUnit = model.QuantityPerUnit;
                productToChange.ReorderLevel = model.ReorderLevel;
                productToChange.Discontinued = model.Discontinued;
                productToChange.UnitsOnOrder = model.UnitsOnOrder;

                if (file != null && file.ContentLength > 0)
                {
                    ImageProcessor.DeleteImage(productToChange.ProductID, "Products");

                    if (ImageProcessor.SaveImage(productToChange.ProductID, file, "Products"))
                    {
                        productToChange.ImageUrl = file.FileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Image type is wrong, please upload correct image");
                        return View(model);
                    }
                }

                db.SaveChanges();
                TempData["success"] = "Product: " + productToChange.ProductName + " is updated successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", model.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", model.SupplierID);

            return View(model);
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();

            ImageProcessor.DeleteImage(id, "Products");
            TempData["success"] = "Category: " + product.ProductName + " is successfully removed";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
