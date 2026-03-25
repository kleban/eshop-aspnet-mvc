using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Domain.Entities.ProductData
{
    public enum ProductType
    {
        Countable,   // поштучно, без серійників
        Serialized,  // поштучно, серійні номери
        Bulk         // вага, об’єм
    }

    public static class ProductTypeExtensions
    {
        private static readonly Dictionary<ProductType, string> _names = new()
        {
            { ProductType.Countable, "Поштучний" },
            { ProductType.Serialized, "Серійний" },
            { ProductType.Bulk, "На вагу / об’єм" }
        };

        public static string ToDisplayName(this ProductType type)
        {
            return _names[type];
        }
    }
}
