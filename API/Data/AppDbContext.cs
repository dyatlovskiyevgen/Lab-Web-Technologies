using Microsoft.EntityFrameworkCore;
using OSS30333.Domain.Entities;  // Подключаем твои сущности

namespace OSS30333.API.Data
{
    public class AppDbContext : DbContext
    {
        // Конструктор с DbContextOptions
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet для всех сущностей
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        // Настройка моделей через Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка сущности Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);               // Первичный ключ (Id из Entity)
                entity.Property(c => c.Name)            // Настройка Name
                    .IsRequired()                       // Обязательное поле
                    .HasMaxLength(100);                 // Ограничение длины

                entity.Property(c => c.NormalizedName)  // Настройка NormalizedName
                    .IsRequired()                       // Обязательное поле
                    .HasMaxLength(100);                 // Ограничение длины
            });

            // Настройка сущности Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);               // Первичный ключ (Id из Entity)
                entity.Property(p => p.Name)            // Настройка Name
                    .IsRequired()                       // Обязательное поле
                    .HasMaxLength(100);                 // Ограничение длины

                entity.Property(p => p.Description)     // Настройка Description
                    .HasMaxLength(500);                // Ограничение длины

                entity.Property(p => p.Price)           // Настройка Price
                    .IsRequired();                      // Обязательное поле

                entity.Property(p => p.Image)           // Настройка Image (необязательное)
                    .IsRequired(false);                // Разрешено NULL

                // Связь Product -> Category (многие-к-одному)
                entity.HasOne(p => p.Category)          // У товара одна категория
                    .WithMany()                        // У категории много товаров (без навигации в Category)
                    .HasForeignKey(p => p.CategoryId)   // Внешний ключ
                    .OnDelete(DeleteBehavior.Restrict); // Запрет каскадного удаления
            });
        }
    }
}