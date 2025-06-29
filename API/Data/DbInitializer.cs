using Microsoft.EntityFrameworkCore;
using OSS30333.API.Data;
using OSS30333.Domain.Entities;


namespace OSS.API.Data
{
    public class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            // Uri проекта 
            var uri = "https://localhost:7002/";
            // Получение контекста БД 
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Выполнение миграций 
            await context.Database.MigrateAsync();
            // Заполнение данными 
            if (!context.Categories.Any() && !context.Products.Any())
            {
                var categories = new Category[]
                {           
                    new Category { Name="Сухие корма",  NormalizedName="dry-food"},
                    new Category { Name="Консервы",NormalizedName="canned-food"},
                    new Category { Name="Домики и лежанки",NormalizedName="houses-and-beds"},
                    new Category { Name="Игрушки",NormalizedName="toys"}
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var products = new List<Product>
            {               
                new Product {Name="Royal Canin Sterilised",
                    Description="сухой корм, для пожилых, основной компонент: злаки, образ жизни: домашний, для стерилизованных",
                    Price =53, Image=uri+"Images/1.png",
                    Category=categories.FirstOrDefault(c=>c.NormalizedName.Equals("dry-food"))},
                new Product { Name="Acana Pacifica CAT",
                    Description="сухой корм, для котят, для взрослых, для пожилых, основной компонент: рыба, образ жизни: домашний, беззерновой",
                    Price =92, Image=uri+"Images/2.png",                    
                    Category=categories.FirstOrDefault(c=>c.NormalizedName.Equals("dry-food"))},
                new Product { Name="Cennamo Prestige Sterilized Cat",
                    Description="Сухой корм для взрослых стерилизованных кошек с сельдью",
                    Price =36, Image=uri+"Images/3.png",                    
                    Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("dry-food"))},
                new Product { Name="Monge BWild Cat Hare",
                    Description="сухой корм, для взрослых, основной компонент: зайчатина, образ жизни: домашний",
                    Price =284, Image=uri+"Images/4.png",                    
                    Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("dry-food"))},
                new Product { Name="Purina Pro Plan Original Kitten",
                    Description="Сухой корм для котят с курицей",
                    Price =55, Image=uri+"Images/5.png",
                    Category=categories.FirstOrDefault(c=>c.NormalizedName.Equals("dry-food"))},
                new Product { Name="Лежанка Эстрада Цап-царап",
                    Description="Мягкая лежанка для кошек и собак",
                    Price =31, Image=uri+"Images/6.png",                    
                    Category=categories.FirstOrDefault(c=>c.NormalizedName.Equals("houses-and-beds"))},
                new Product { Name="Banditos Кусочки в желе для кошек (Сочный ягненок)",
                    Description="Влажный корм для кошек, ягненок",
                    Price =2, Image=uri+"Images/7.png",                    
                    Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("canned-food"))},
                new Product { Name="Prochoice Паштет для кошек (Сардины и анчоусы)",
                    Description="Влажный корм для взрослых кошек",
                    Price =6, Image=uri+"Images/8.png",
                    Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("canned-food"))},
            };
                await context.AddRangeAsync(products);
                await context.SaveChangesAsync();

            }
        }
    }
}
