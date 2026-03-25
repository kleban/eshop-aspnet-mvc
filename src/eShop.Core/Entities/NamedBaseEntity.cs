using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Domain.Entities
{
    public abstract class NamedBaseEntity<TKey>
    {
        public TKey Id { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
    }
}
