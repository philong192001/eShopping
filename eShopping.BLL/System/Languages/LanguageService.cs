using eShopping.BLL.System.Languages;
using eShopping.DAL.EF;
using eShopping.DAL.Entities;
using eShopping.ViewModels.Common;
using eShopping.ViewModels.System.Languages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eShopping.BLL.System.Languages
{
    public class LanguageService : ILanguageService
    {
       
        private readonly IConfiguration _config;
        private readonly EShopDbContext _context;

        public LanguageService(EShopDbContext context,IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        public async Task<ApiResult<List<LanguageVm>>> GetAll()
        {
            var languages =  await _context.Languages.Select(x => new LanguageVm()
            {
                Id = x.Id,
                Name = x.Name,
                IsDefault = x.IsDefault
            }).ToListAsync();
            return new ApiSuccessResult<List<LanguageVm>>(languages);
        }
    }
}
