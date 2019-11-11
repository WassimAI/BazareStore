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
    public class CategoryController : Controller
    {
        private BazarEntities db = new BazarEntities();

        // GET: Admin/Category
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please fill in the required fields below");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                Category newCategory = new Category()
                {
                    CategoryName = model.CategoryName,
                    Description = model.Description
                };

                db.Categories.Add(newCategory);
                db.SaveChanges();

                if(file !=null && file.ContentLength > 0)
                {
                    if (ImageProcessor.SaveImage(newCategory.CategoryID, file, "Categories"))
                    {
                        newCategory.ImageUrl = file.FileName;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sorry, the Image was not uploaded, please check the file type");
                        return View(model);
                    }
                }
                

                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Create");
            }

            return View(model);
        }

        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                Category categoryToChange = db.Categories.Find(model.CategoryID);
                categoryToChange.CategoryName = model.CategoryName;
                categoryToChange.Description = model.Description;

                if (!String.IsNullOrWhiteSpace(categoryToChange.ImageUrl))
                {
                    ImageProcessor.DeleteImage(categoryToChange.CategoryID, "Categories");
                }

                if(file != null && file.ContentLength > 0)
                {
                    if (ImageProcessor.SaveImage(categoryToChange.CategoryID, file, "Categories"))
                    {
                        categoryToChange.ImageUrl = file.FileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Image type is wrong, please upload correct image");
                        return View(model);
                    }
                }
                

                db.SaveChanges();

                TempData["success"] = "Category: " + categoryToChange.CategoryName + " is updated successfully";

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            ImageProcessor.DeleteImage(id, "Categories");
            TempData["success"] = "Category: " + category.CategoryName + " is successfully removed";
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
