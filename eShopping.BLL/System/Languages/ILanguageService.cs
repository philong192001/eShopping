using eShopping.ViewModels.Common;
using eShopping.ViewModels.System.Languages;
using eShopping.ViewModels.System.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopping.BLL.System.Languages
{
    public interface ILanguageService
    {

        Task<ApiResult<List<LanguageVm>>> GetAll();
        
    }
}
