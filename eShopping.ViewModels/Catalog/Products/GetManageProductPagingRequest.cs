using eShopping.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopping.ViewModels.Catalog.Products
{
    public class GetManageProductPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
        public List<int> CategoryIds { get; set; }
        public string LanguageId { get; set; }
    }
}
