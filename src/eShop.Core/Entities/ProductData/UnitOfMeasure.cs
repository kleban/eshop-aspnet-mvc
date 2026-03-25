using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Domain.Entities.ProductData
{
    public class UnitOfMeasure
    {

            public int Id { get; set; }
            public string Name { get; set; }   // кілограм, штука
            public string Symbol { get; set; } // кг, шт, см
        
    }
}
