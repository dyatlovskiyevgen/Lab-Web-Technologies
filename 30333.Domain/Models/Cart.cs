using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSS30333.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        // Список объектов в корзине,  key - идентификатор объекта          
        public Dictionary<int, CartItem> CartItems { get; set; } = new();

        // Добавить объект в корзину  <param name="product">Добавляемый объект</param> 
        public virtual void AddToCart(Product product)
        {
            if (CartItems.ContainsKey(product.Id))
            {
                CartItems[product.Id].Qty++;
            }
            else
            {
                CartItems.Add(product.Id, new CartItem
                {
                    Item = product,
                    Qty = 1
                });
            }
            ;
        }

        // Удалить объект из корзины    <param name="product">удаляемый объект</param> 
        public virtual void RemoveItems(int id)
        {
            CartItems.Remove(id);
        }

        // Очистить корзину 
        public virtual void ClearAll()
        {
            CartItems.Clear();
        }

        // Количество объектов в корзине 
        public int Count { get => CartItems.Sum(item => item.Value.Qty); }

        // Общее количество калорий         
        public double TotalPrice
        {
            get => CartItems.Sum(item => item.Value.Item.Price * item.Value.Qty);
        }
    }
}
