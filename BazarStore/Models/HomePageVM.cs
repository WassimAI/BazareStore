﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BazarStore.Models
{
    public class HomePageVM
    {
        public IEnumerable<Category> Categories { get; set; }
        //public IEnumerable<Product> Products { get; set; }
    }
}