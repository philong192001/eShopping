using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopping.BLL.Catalog.Products;
using eShopping.BLL.System.Languages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopping.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var language = await _languageService.GetAll();
            return Ok(language);
        }
    }
}
