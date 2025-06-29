using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSS30333.Domain.Entities
{
    public class CartItem
    {
        public Product Item { get; set; }
        public int Qty { get; set; }
    }
}
