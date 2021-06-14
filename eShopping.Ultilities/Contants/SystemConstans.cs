using System;
using System.Collections.Generic;
using System.Text;

namespace eShopping.Ultilities.Contants
{
    public class SystemConstans
    {
        public const string MainConnectionString = "eShopSolutionDb";
        public const string CartSession = "CartSession";

        public class AppSetting
        {
            public const string DefaultLanguageId = "DefaultLanguageId";
            public const string Token = "Token";
            public const string BaseAddress = "BaseAddress";
        }

        public class ProductSettings
        {
            public const int NumberOfFeaturedProducts = 10;
            public const int NumberOfLatestProducts = 10;
        }

        public class ProductConstants
        {
            public const string NA = "N/A";
        }
    }
}
