using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BazarStore.Areas.Admin.Models
{
    public class CategoryVM
    {
        public CategoryVM()
        {

        }

        public CategoryVM(Category row)
        {
            CategoryID = row.CategoryID;
            CategoryName = row.CategoryName;
            Description = row.Description;
            ImageUrl = row.ImageUrl;
        }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}