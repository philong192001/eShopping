using eShopping.DAL.Entities;
using eShopping.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopping.DAL.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppConfig>().HasData(
                new AppConfig() { Key = "HomeTitle", Value = "This is home page of eShopSolution" },
                new AppConfig() { Key = "HomeKeyword", Value = "This is keyword of eShopSolution" },
                new AppConfig() { Key = "HomeDescription", Value = "This is description of eShopSolution" }
                //new AppConfig() { Key = "HomeTitle", Value = "This is home page of eShopSolution" },
                //new AppConfig() { Key = "HomeTitle", Value = "This is home page of eShopSolution" }
            );
            modelBuilder.Entity<Language>().HasData(
                 new Language() { Id = "vi-VN", Name = "Tiếng Việt", IsDefault = true },
                 new Language() { Id = "en-US", Name = "English", IsDefault = false }
                );

            modelBuilder.Entity<Category>().HasData(
                new Category()
                {
                    Id = 1,
                    IsShowHome = true,
                    ParentId = 0,
                    SortOrder = 1,
                    Status = Status.Active,
                    CategoryTranslations = new List<CategoryTranslation>()
                    {

                    }
                },
                new Category()
                {
                    Id = 2,
                    IsShowHome = true,
                    ParentId = 0,
                    SortOrder = 2,
                    Status = Status.Active,
                    CategoryTranslations = new List<CategoryTranslation>()
                    {

                    }
                }
                );
            modelBuilder.Entity<CategoryTranslation>().HasData(
                 new CategoryTranslation() { Id = 1, CategoryId = 1, Name = "Áo Nam", LanguageId = "vi-VN", SeoAlias = "ao-nam", SeoDescription = "Sản phẩm áo thời trang nam", SeoTitle = "Sản phẩm áo thời trang nam" },
                 new CategoryTranslation() { Id = 2, CategoryId = 1, Name = "Men Shirt", LanguageId = "en-US", SeoAlias = "men-shirt", SeoDescription = "The shirt products for men", SeoTitle = "The shirt products for men" },
                 new CategoryTranslation() { Id = 3, CategoryId = 2, Name = "Áo Nữ", LanguageId = "vi-VN", SeoAlias = "ao-nu", SeoDescription = "Sản phẩm áo thời trang nữ", SeoTitle = "Sản phẩm áo thời trang nữ" },
                 new CategoryTranslation() { Id = 4, CategoryId = 2, Name = "Women Shirt", LanguageId = "en-US", SeoAlias = "women-shirt", SeoDescription = "The shirt products for women", SeoTitle = "The shirt products for women" }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product()
                {
                    Id = 1,
                    DateCreated = DateTime.Now,
                    OriginalPrice = 100000,
                    Price = 200000,
                    Stock = 0,
                    ViewCount = 0
                });
            modelBuilder.Entity<ProductTranslation>().HasData(
                new ProductTranslation() { Id = 1, ProductId = 1, Name = "Áo Sơ Mi Nam Trắng Việt Tiệp", LanguageId = "vi-VN", SeoAlias = "ao-so-mi-trang-viet-tiep", SeoDescription = "Áo Sơ Mi Nam Trắng Việt Tiệp", SeoTitle = "Áo Sơ Mi Nam Trắng Việt Tiệp", Details = "Mô tả sản phẩm ", Description = "Áo Sơ Mi Nam Trắng Việt Tiệp" },
                new ProductTranslation() { Id = 2, ProductId = 1, Name = "Viet Tiep Men T-Shirt white", LanguageId = "en-US", SeoAlias = "viet-tiep-men-tshirt", SeoDescription = "Viet Tiep Men T-Shirt white", SeoTitle = "Viet Tiep Men T-Shirt white", Details = "Description of product", Description = "Viet Tiep Men T-Shirt white" }
                );
            modelBuilder.Entity<ProductInCategory>().HasData(
                new ProductInCategory() { ProductId = 1, CategoryId = 1 }
                );

            // any guid
            var roleId = new Guid("8D04DCE2-969A-435D-BBA4-DF3F325983DC");
            var adminId = new Guid("69BD714F-9576-45BA-B5B7-F00649BE00DE");
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = roleId,
                Name = "admin",
                NormalizedName = "admin",
                Description = "Administrator role"
            });

            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = adminId,
                UserName = "admin",
                NormalizedUserName = "admin",
                Email = "longgne@gmail.com",
                NormalizedEmail = "longgne@gmail.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Abcd1234$"),
                SecurityStamp = string.Empty,
                FirstName = "Longg",
                LastName = "Phii",
                Dob = new DateTime(2001, 09, 01)
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = roleId,
                UserId = adminId
            });
        }
    }
}
