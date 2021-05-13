using eShopping.ViewModels.Catalog.Products;
using eShopping.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopping.BLL.Catalog.Products
{
    public interface IPublicProductService
    {
       Task<PageResult<ProductViewModel>> GetAllByCategoryId(GetPublicProductPagingRequest request);

       Task<List<ProductViewModel>> GetAll(string languageId);
    }
}
