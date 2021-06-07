using eShopping.ViewModels.Catalog.Categories;
using eShopping.ViewModels.Catalog.Products;
using eShopping.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopping.WebApp.Models
{
    public class ProductCategoryViewModel
    {
        public CategoryVm Category { get; set; }

        public PageResult<ProductVm> Products { get; set; }
    }
}
