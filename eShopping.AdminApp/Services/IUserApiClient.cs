using eShopping.ViewModels.Common;
using eShopping.ViewModels.System.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopping.AdminApp.Services
{
    public interface IUserApiClient
    {
        Task<string> Authenticate(LoginRequest request);

        Task<PageResult<UserVm>> GetUserPaging(GetUserPagingRequest request);
    }
}
