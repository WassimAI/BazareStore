﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
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
                products = products.Where(x => x.ProductName.Contains(searchText));

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

            //For dropzone
            ViewBag.productID = id;

            product.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

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

        [HttpPost]
        //POST: Admin/Product/DeleteProducts
        public JsonResult DeleteProducts(int[] ids)
        {
            var listToDelete = db.Products.Where(x => ids.Contains(x.ProductID)).ToList();
            foreach(var product in listToDelete)
            {
                db.Products.Remove(product);
            }
            try
            {
                db.SaveChanges();
                return Json(new { response = "Ok" });
            }
            catch
            {
                return Json(new { response = "No" });
            }
            
        }

        // POST: Admin/Product/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Loop through files
            foreach (string fileName in Request.Files)
            {
                // Init the file
                HttpPostedFileBase file = Request.Files[fileName];

                // Check it's not null
                if (file != null && file.ContentLength > 0)
                {
                    // Set directory paths
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    //if (!Directory.Exists(pathString1))
                    //    Directory.CreateDirectory(pathString1);

                    //if (!Directory.Exists(pathString2))
                    //    Directory.CreateDirectory(pathString2);

                    // Set image paths
                    var path = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);

                    // Save original and thumb

                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(150, 150);
                    img.Save(path2);
                }

            }

        }

        // POST: Admin/Product/DeleteImage
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);
            string fullPath3 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/" + imageName);
            string fullPath4 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);

            if (System.IO.File.Exists(fullPath3))
                System.IO.File.Delete(fullPath3);

            if (System.IO.File.Exists(fullPath4))
                System.IO.File.Delete(fullPath4);
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
