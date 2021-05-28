
using eShopping.DAL.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopping.ViewModels.Catalog.Products
{
    public class ProductCreateRequest
    {
        public decimal Price { get; set; }

        public decimal OriginalPrice { get; set; }

        public int Stock { get; set; }

        [Required(ErrorMessage = "Please Enter product name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please Enter Details")]
        public string Details { get; set; }

        [Required(ErrorMessage = "Please Enter SeoDescription")]
        public string SeoDescription { get; set; }

        [Required(ErrorMessage = "Please Enter SeoAlias")]
        public string SeoAlias { get; set; }

        [Required(ErrorMessage = "Please Enter SeoTitle")]
        public string SeoTitle { get; set; }
     
        public string LanguageId { get; set; }

        //public Product Product { get; set; }
        //public Language Language { get; set; }

        public IFormFile ThumbnailImage { get; set; }
    }
}
