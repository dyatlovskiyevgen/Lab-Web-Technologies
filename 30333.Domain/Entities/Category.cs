using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OSS30333.Domain.Entities
{
    public class Category:Entity
    {
        public string NormalizedName { get; set; }
    }
}
