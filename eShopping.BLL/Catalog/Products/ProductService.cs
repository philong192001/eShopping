using eShopping.BLL.Common;
using eShopping.DAL.EF;
using eShopping.DAL.Entities;
using eShopping.Ultilities.Contants;
using eShopping.Ultilities.Exceptions;
using eShopping.ViewModels.Catalog;
using eShopping.ViewModels.Catalog.ProductImages;
using eShopping.ViewModels.Catalog.Products;
using eShopping.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopping.BLL.Catalog.Products
{
    public class ProductService : IProductService
    {
        private readonly EShopDbContext _eShopDbContext;
        private readonly IStorageService _storageService;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public ProductService(EShopDbContext context, IStorageService storageService)
        {
            _storageService = storageService;
            _eShopDbContext = context;

        }

        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {
            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                ProductId = productId,
                SortOrder = request.SortOrder
            };

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _eShopDbContext.ProductImages.Add(productImage);
            await _eShopDbContext.SaveChangesAsync();
            return productImage.Id;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _eShopDbContext.Products.FindAsync(productId);
            product.ViewCount += 1;
            await _eShopDbContext.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var languages = _eShopDbContext.Languages;
            var translations = new List<ProductTranslation>();

            foreach (var language in languages)
            {
                if (language.Id == request.LanguageId)
                {
                    translations.Add(new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    });
                }
                else
                {
                    translations.Add(new ProductTranslation()
                    {
                        Name = SystemConstans.ProductConstants.NA,
                        Description = SystemConstans.ProductConstants.NA,
                        SeoAlias = SystemConstans.ProductConstants.NA,
                        LanguageId = language.Id
                    });
                }
            }

            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = translations
            };
            //Save image
            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail Image",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await this.SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }
            _eShopDbContext.Products.Add(product);
            await _eShopDbContext.SaveChangesAsync();

            return product.Id;
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _eShopDbContext.Products.FindAsync(productId);
            if (product == null)
            {
                throw new EShopException($"Cannot find a product : {productId}");
            }
            else
            {
                var images = _eShopDbContext.ProductImages.Where(i => i.ProductId == productId);
                foreach (var image in images)
                {
                    await _storageService.DeleteFileAsync(image.ImagePath);
                }
                _eShopDbContext.Products.Remove(product);
            }
            return await _eShopDbContext.SaveChangesAsync();
        }


        public async Task<PageResult<ProductVm>> GetAllPaging(GetManageProductPagingRequest request)
        {
            //1.Select join
            var query = from p in _eShopDbContext.Products
                        join pt in _eShopDbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _eShopDbContext.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join c in _eShopDbContext.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        join pi in _eShopDbContext.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        where pt.LanguageId == request.LanguageId && pi.IsDefault == true
                        select new { p, pt, pic, pi };
            //2.Filterr 
            if (!String.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            }
            if (request.CategoryId != null && request.CategoryId != 0)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
            }
            //3.Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync();

            //4.Select and projection
            var pagedResult = new PageResult<ProductVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return pagedResult;
        }

        public async Task<ProductVm> GetById(int productId, string languageId)
        {
            var product = await _eShopDbContext.Products.FindAsync(productId);
            var productTranslation = await _eShopDbContext.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId
            && x.LanguageId == languageId);

            var categories = await (from c in _eShopDbContext.Categories
                                    join ct in _eShopDbContext.CategoryTranslations on c.Id equals ct.CategoryId
                                    join pic in _eShopDbContext.ProductInCategories on c.Id equals pic.CategoryId
                                    where pic.ProductId == productId && ct.LanguageId == languageId
                                    select ct.Name).ToListAsync();

            var image = await _eShopDbContext.ProductImages.Where(x => x.ProductId == productId && x.IsDefault == true).FirstOrDefaultAsync();

            var productViewModel = new ProductVm()
            {
                Id = product.Id,
                DateCreated = product.DateCreated,
                Description = productTranslation != null ? productTranslation.Description : null,
                LanguageId = productTranslation.LanguageId,
                Details = productTranslation != null ? productTranslation.Details : null,
                Name = productTranslation != null ? productTranslation.Name : null,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                SeoAlias = productTranslation != null ? productTranslation.SeoAlias : null,
                SeoDescription = productTranslation != null ? productTranslation.SeoDescription : null,
                SeoTitle = productTranslation != null ? productTranslation.SeoTitle : null,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                Categories = categories,
                ThumbnailImage = image != null ? image.ImagePath : "no-image.jpg"
            };
            return productViewModel;
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _eShopDbContext.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                throw new EShopException($"Cannot find an image with id {imageId}");
            }
            var viewModel = new ProductImageViewModel()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                FileSize = image.FileSize,
                Id = image.Id,
                ImagePath = image.ImagePath,
                IsDefault = image.IsDefault,
                ProductId = image.ProductId,
                SortOrder = image.SortOrder
            };
            return viewModel;
        }

        public async Task<List<ProductImageViewModel>> GetListImages(int productId)
        {
            return await _eShopDbContext.ProductImages.Where(x => x.ProductId == productId).Select(i => new ProductImageViewModel()
            {
                Caption = i.Caption,
                DateCreated = i.DateCreated,
                FileSize = i.FileSize,
                Id = i.Id,
                ImagePath = i.ImagePath,
                IsDefault = i.IsDefault,
                ProductId = i.ProductId,
                SortOrder = i.SortOrder
            }).ToListAsync();
        }

        public async Task<int> RemoveImage(int ImageId)
        {
            var productImage = await _eShopDbContext.ProductImages.FindAsync(ImageId);
            if (productImage == null)
            {
                throw new EShopException($"Cannot find an image with id {ImageId}");
            }
            else
            {
                _eShopDbContext.ProductImages.Remove(productImage);
                return await _eShopDbContext.SaveChangesAsync();
            }
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _eShopDbContext.Products.FindAsync(request.Id);
            var productTranslations = await _eShopDbContext.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId == request.LanguageId);
            if (product == null || productTranslations == null)
            {
                throw new EShopException($"Cannot find a product with id : { request.Id}");
            }
            else
            {
                productTranslations.Name = request.Name;
                productTranslations.SeoAlias = request.SeoAlias;
                productTranslations.SeoDescription = request.SeoDescription;
                productTranslations.SeoTitle = request.SeoTitle;
                productTranslations.Description = request.Description;
                productTranslations.Details = request.Details;

                if (request.ThumbnailImage != null)
                {
                    var thumbnailImage = await _eShopDbContext.ProductImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == request.Id);
                    if (thumbnailImage != null)
                    {
                        thumbnailImage.FileSize = request.ThumbnailImage.Length;
                        thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                        _eShopDbContext.ProductImages.Update(thumbnailImage);
                    }
                }

                return await _eShopDbContext.SaveChangesAsync();
            }
        }

        public async Task<int> UpdateImage(int ImageId, ProductImageUpdateRequest request)
        {
            var productImage = await _eShopDbContext.ProductImages.FindAsync(ImageId);
            if (productImage == null)
            {
                throw new EShopException($"Cannot find an image with id {ImageId}");
            }

            if (request.ImageFile != null)
            {
                productImage.ImagePath = await this.SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _eShopDbContext.ProductImages.Update(productImage);
            return await _eShopDbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _eShopDbContext.Products.FindAsync(productId);
            if (product == null)
            {
                throw new EShopException($"Cannot find a product with id : { productId}");
            }
            else
            {
                product.Price = newPrice;
                return await _eShopDbContext.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _eShopDbContext.Products.FindAsync(productId);
            if (product == null)
            {
                throw new EShopException($"Cannot find a product with id : { productId}");
            }
            else
            {
                product.Stock = addedQuantity;
                return await _eShopDbContext.SaveChangesAsync() > 0;
            }
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
        }

        public async Task<PageResult<ProductVm>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request)
        {
            //1.Select join
            var query = from p in _eShopDbContext.Products
                        join pt in _eShopDbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _eShopDbContext.ProductInCategories on p.Id equals pic.ProductId
                        join c in _eShopDbContext.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pic };
            //2.Filterr

            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
            }
            //3.Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount
                }).ToListAsync();

            //4.Select and projection
            var pagedResult = new PageResult<ProductVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return pagedResult;
        }

        public async Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            var user = await _eShopDbContext.Products.FindAsync(id);
            if (user == null)
            {
                return new ApiErrorResult<bool>($"Sản phẩm với id {id} không tồn tại");
            }
            foreach (var category in request.Categories)
            {
                var productInCategory = await _eShopDbContext.ProductInCategories
                    .FirstOrDefaultAsync(x => x.CategoryId == int.Parse(category.Id)
                    && x.ProductId == id);
                if (productInCategory != null && category.Selected == false)
                {
                    _eShopDbContext.ProductInCategories.Remove(productInCategory);
                }
                else if (productInCategory == null && category.Selected)
                {
                    await _eShopDbContext.ProductInCategories.AddAsync(new ProductInCategory()
                    {
                        CategoryId = int.Parse(category.Id),
                        ProductId = id
                    });
                }
            }
            await _eShopDbContext.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }

        public async Task<List<ProductVm>> GetFeaturedProducts(string languageId, int take)
        {
            //1. Select join
            var query = from p in _eShopDbContext.Products
                        join pt in _eShopDbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _eShopDbContext.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join pi in _eShopDbContext.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        join c in _eShopDbContext.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        where pt.LanguageId == languageId && (pi == null || pi.IsDefault == true)
                        && p.IsFeatured == true
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync();

            return data;
        }

        public async Task<List<ProductVm>> GetLatestProducts(string languageId, int take)
        {
            //1. Select join
            var query = from p in _eShopDbContext.Products
                        join pt in _eShopDbContext.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _eShopDbContext.ProductInCategories on p.Id equals pic.ProductId into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join pi in _eShopDbContext.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        join c in _eShopDbContext.Categories on pic.CategoryId equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        where pt.LanguageId == languageId && (pi == null || pi.IsDefault == true)
                        select new { p, pt, pic, pi };

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
                .Select(x => new ProductVm()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.ImagePath
                }).ToListAsync();

            return data;
        }
    }
}
