using Microsoft.EntityFrameworkCore;
using StitchArtisan.Backend.Models;
using BCrypt.Net;

namespace StitchArtisan.Backend.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync())
            {
                return; // DB already seeded with products
            }

            // 1. Roles
            if (!await context.Roles.AnyAsync())
            {
                var roleList = new List<Role>
                {
                    new Role { RoleName = "admin" },
                    new Role { RoleName = "staff" },
                    new Role { RoleName = "customer" }
                };
                await context.Roles.AddRangeAsync(roleList);
                await context.SaveChangesAsync();
            }

            // 2. Users
            if (!await context.Users.AnyAsync())
            {
                var adminUser = new User
                {
                    FullName = "Admin Bakery",
                    Email = "admin@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Phone = "0987654321",
                    RoleId = 1,
                    IsActive = true
                };
                var customerUser = new User
                {
                    FullName = "John Customer",
                    Email = "customer@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                    Phone = "0123456789",
                    RoleId = 3,
                    IsActive = true
                };
                await context.Users.AddRangeAsync(adminUser, customerUser);
                await context.SaveChangesAsync();
            }

            // 3. Categories
            if (!await context.Categories.AnyAsync())
            {
                var categoryList = new List<Category>
                {
                    new Category { Name = "Bánh Mì", Description = "Bánh mì tươi lên men tự nhiên" },
                    new Category { Name = "Bánh Ngọt", Description = "Bánh ngọt thơm ngon mỗi ngày" },
                    new Category { Name = "Bánh Kem", Description = "Bánh kem cho các dịp đặc biệt" }
                };
                await context.Categories.AddRangeAsync(categoryList);
                await context.SaveChangesAsync();
            }

            var categories = await context.Categories.ToListAsync();

            // 4. Products & Variants
            if (!await context.Products.AnyAsync())
            {
                var productsList = new List<Product>
                {
                    new Product { Name = "Bánh Mì Sourdough Tự Nhiên", Slug = "banh-mi-sourdough-tu-nhien", BaseDescription = "Lên men chậm 24h", CategoryId = categories[0].CategoryId },
                    new Product { Name = "Croissant Bơ Pháp", Slug = "croissant-bo-phap", BaseDescription = "Thơm mùi bơ AOP", CategoryId = categories[1].CategoryId },
                    new Product { Name = "Bánh Cuộn Quế", Slug = "banh-cuon-que", BaseDescription = "Lớp phủ kem phô mai", CategoryId = categories[1].CategoryId },
                    new Product { Name = "Bánh Mì Hoa Cúc", Slug = "banh-mi-hoa-cuc", BaseDescription = "Mềm xốp béo ngậy", CategoryId = categories[0].CategoryId },
                    new Product { Name = "Bánh Kem Dâu Tây", Slug = "banh-kem-dau-tay", BaseDescription = "Bánh kem tươi dâu Đà Lạt", CategoryId = categories[2].CategoryId }
                };
                await context.Products.AddRangeAsync(productsList);
                await context.SaveChangesAsync();

                var products = await context.Products.ToListAsync();

                var variantsList = new List<ProductVariant>
                {
                    new ProductVariant { ProductId = products[0].ProductId, Sku = "BREAD-001", Price = 85000, StockQty = 20, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDgEk1by2syfmSzk1FDqoggA2Sz8ePtOdJ5fOAoxKwwbdjqnrcEz9ELgh46cXDrtXUn394pYVOyL9nRiDNq1QSRTrZROQBlGJ4grJpgUcNTyn7Wa36srI7sxhhTrr_APtaLitzGuf2ct3PKc3U2vLSWpoeLgh5N74newi3_QSaKEGYL9bnPw3PkPO6I-pE9UhcuNHPKBnFKD0GDkJ_6WyEQStaJ7T7_En2kfRIh3poL2JpQ7GeUSVwk8M2g1X9u3Iv3dPQx2255_w" },
                    new ProductVariant { ProductId = products[1].ProductId, Sku = "PAS-001", Price = 45000, StockQty = 30, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAmp0zco70Ra6QFM7jwww2rUT2YCwS5EnQGfvst8HQYSNGrdnWYdwyDtiDuv5RJcZCUuGZ6LqHht2Ly-LEpdhDuOYOnAZmHwH1dUc368n8jTMJvcTRROg9nKbICTfayoF0UKJu1pxSpbampEotsLcK_-PGAGcPxKkkCyeU9CQ6YiAizgKY_jQDuiw1UFGZsQtPncBMoeveYhc34mx3yYjHxan8A0IndyxH9eMWOF05r8DssZrilFx5GZAwWwhzJziTpOuFvVuFPWQ" },
                    new ProductVariant { ProductId = products[2].ProductId, Sku = "PAS-002", Price = 35000, StockQty = 3, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDiACamZtXJe8Qdx8r-hiiOWokVnej1l0ALcWwWuajHWzXvXGXRQXFHRpb6JWesNIfcXUtgI_QrJJiTEA5v20o8wxEFpzBYKFEA_4jAN0rUE-1NQyyYrZHD9-HfATZ_4xOZ2yNzTmQaYXzFwGzaO3iYsmDrqn8qviBVK0R1PHGzc-OKmOTrkdQjePj7XrxRcZM7qIPrWHKPlOWOxtUWNCcJF-QzsBWd7UvrqDEHsNxaHmjPlZZqIx-rgM3r2Pjq1hlzRjijVQ3-vA" },
                    new ProductVariant { ProductId = products[3].ProductId, Sku = "BREAD-002", Price = 120000, StockQty = 15, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBn-ltScn6WKKgYWldzMDhtmkwqoOHUpbhygz_q3eWkdauGuSOYqYZlI9xscHlZ37WiD6tvPEjaAIPvYHymJ9o6Zmmpy1lBEwzQjFXPzcDGtxft-CThpJWCw-rlX-ltuMkULKtT7w-avkfMv2h4QJH9Lspyti8iDYfb38P-2cTele8JscQJSkgXKHgHvsGxODGwHA2WpfoAswHotbzEzwhgrIps8sE7QoiBnQVdUQ7ArgbK3BXUEzryPh9s_5lTHuenWdnNTIjW_A" },
                    new ProductVariant { ProductId = products[4].ProductId, Sku = "CAKE-001", Price = 450000, StockQty = 2, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCaCPYdwS7DuC7WmRivzXeHAmPhAr_wyJEJru-JJ2U__o2VwfQdRcsr2J-plF-GlwpFVTkh3aQWMHUfMwB9n8SOg_nGg5LcvwDiLRh7RvcEFu6cX9kt7V0llfTMrOJb6U5FYZ-nAsCJtO1rA4I0jbfEfses7mbQ4azC8Zc2F9LRhgQcUYoFIGRwzUQXocVEv9ReXVxPgEaZTl6qV7Oi2ujHDrHS_pKZ08sPK7v6QE1hzBqAjJrbIPurONahx9UOHy4m6Zr6foYLCQ" }
                };
                await context.ProductVariants.AddRangeAsync(variantsList);
                await context.SaveChangesAsync();
            }

            // 5. Mock Orders for Dashboard Data
            if (!await context.Orders.AnyAsync())
            {
                var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com") ?? await context.Users.FirstAsync();
                var customerUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "customer@gmail.com") ?? await context.Users.FirstAsync();
                var variants = await context.ProductVariants.ToListAsync();

            var order1 = new Order
            {
                UserId = customerUser.UserId,
                TotalPrice = 130000,
                OrderStatus = "delivered",
                ShippingAddress = "123 Quận 1, TPHCM",
                Note = "Giao trước 5h chiều",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            };
            var order2 = new Order
            {
                UserId = customerUser.UserId,
                TotalPrice = 450000,
                OrderStatus = "pending",
                ShippingAddress = "456 Quận 2, TPHCM",
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            };
            var order3 = new Order
            {
                UserId = adminUser.UserId, // Mock
                TotalPrice = 85000,
                OrderStatus = "processing",
                ShippingAddress = "Tại cửa hàng",
                CreatedAt = DateTime.UtcNow.AddMinutes(-45)
            };
            await context.Orders.AddRangeAsync(order1, order2, order3);
            await context.SaveChangesAsync();

            var orderItems = new List<OrderItem>
            {
                new OrderItem { OrderId = order1.OrderId, VariantId = variants[0].VariantId, Quantity = 1, PriceAtPurchase = 85000 },
                new OrderItem { OrderId = order1.OrderId, VariantId = variants[1].VariantId, Quantity = 1, PriceAtPurchase = 45000 },
                new OrderItem { OrderId = order2.OrderId, VariantId = variants[4].VariantId, Quantity = 1, PriceAtPurchase = 450000 },
                new OrderItem { OrderId = order3.OrderId, VariantId = variants[0].VariantId, Quantity = 1, PriceAtPurchase = 85000 }
            };
            await context.OrderItems.AddRangeAsync(orderItems);
            await context.SaveChangesAsync();

            var invoices = new List<Invoice>
            {
                new Invoice { OrderId = order1.OrderId, TotalAmount = 130000, Subtotal = 130000, PaymentStatus = "paid", InvoiceNumber = "INV-001", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Invoice { OrderId = order2.OrderId, TotalAmount = 450000, Subtotal = 450000, PaymentStatus = "unpaid", InvoiceNumber = "INV-002", CreatedAt = DateTime.UtcNow.AddHours(-3) },
                new Invoice { OrderId = order3.OrderId, TotalAmount = 85000, Subtotal = 85000, PaymentStatus = "paid", InvoiceNumber = "INV-003", CreatedAt = DateTime.UtcNow.AddMinutes(-45) }
            };
                await context.Invoices.AddRangeAsync(invoices);
                await context.SaveChangesAsync();
            }
        }
    }
}
