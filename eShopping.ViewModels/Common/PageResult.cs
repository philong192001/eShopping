using System;
using System.Collections.Generic;
using System.Text;

namespace eShopping.ViewModels.Common
{
    public class PageResult<T> : PagedResultBase
    {
        public List<T> Items { get; set; }
    }
}
