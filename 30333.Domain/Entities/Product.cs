using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OSS30333.Domain.Entities
{
    public class Product:Entity
    {
        
        public string Description { get; set; } // описание  
        public int Price { get; set; } // цена 

        public string? Image { get; set; } // путь к файлу изображения    

        // Навигационные свойства 
        /// <summary> 
        /// группа товаров (например, корм, игрушки и т.д.) 
        /// </summary> 
        public int CategoryId { get; set; }
        //[JsonIgnore]
        public Category? Category { get; set; }
    }
}
