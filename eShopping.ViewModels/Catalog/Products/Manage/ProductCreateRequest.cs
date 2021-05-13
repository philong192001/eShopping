﻿
using eShopping.DAL.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopping.ViewModels.Catalog.Products.Manage
{
    public class ProductCreateRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int Stock { get; set; }
        public int ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string SeoDescription { get; set; }
        public string SeoAlias { get; set; }
        public string SeoTitle { get; set; }
        public string LanguageId { get; set; }

        //public Product Product { get; set; }
        //public Language Language { get; set; }

        public IFormFile ThumbnailImage { get; set; }
    }
}
