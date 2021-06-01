using eShopping.ApiIntegration;
using eShopping.ViewModels.Common;
using eShopping.ViewModels.System.Languages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace eShopping.ApiIntegration
{
    public class LanguageApiClient : BaseApiClient,ILanguageApiClient
    {
       
        public LanguageApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, 
        IHttpContextAccessor httpContextAccessor) : base(httpClientFactory,configuration, httpContextAccessor)
        {
           
        }

        public async Task<ApiResult<List<LanguageVm>>> GetAll()
        {
            return await GetAsync<ApiResult<List<LanguageVm>>>("/api/languages");
        }
    }
}
