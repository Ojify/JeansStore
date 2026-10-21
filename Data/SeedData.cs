using JeansStoreManagementSystem.Constants;
using JeansStoreManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace JeansStoreManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            string[] roleNames = { Roles.Guest, Roles.User, Roles.Admin };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            const string adminEmail = "admin@denimshop.com";
            const string adminPassword = "Admin@123456";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Super",
                    LastName = "Admin"
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Slim Fit", Description = "Зауженные джинсы" },
                    new Category { Name = "Straight Fit", Description = "Прямые джинсы" },
                    new Category { Name = "Skinny", Description = "Очень узкие джинсы" },
                    new Category { Name = "Bootcut", Description = "Расклешенные книзу" }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Brands.Any())
            {
                context.Brands.AddRange(
                    new Brand { Name = "Levi's", Country = "USA" },
                    new Brand { Name = "Wrangler", Country = "USA" },
                    new Brand { Name = "Lee", Country = "USA" },
                    new Brand { Name = "Diesel", Country = "Italy" }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                var cat = context.Categories.ToList();
                var brand = context.Brands.ToList();

                context.Products.AddRange(
                    new Product
                    {
                        Name = "Levi's 511 Slim",
                        Description = "Классические зауженные джинсы",
                        Price = 89.99m,
                        Stock = 25,
                        Size = "32",
                        Color = "Indigo",
                        CategoryId = cat.First(c => c.Name == "Slim Fit").Id,
                        BrandId = brand.First(b => b.Name == "Levi's").Id
                    },
                    new Product
                    {
                        Name = "Wrangler Texas Straight",
                        Description = "Прямые джинсы с высокой посадкой",
                        Price = 74.50m,
                        Stock = 18,
                        Size = "34",
                        Color = "Dark Blue",
                        CategoryId = cat.First(c => c.Name == "Straight Fit").Id,
                        BrandId = brand.First(b => b.Name == "Wrangler").Id
                    },
                    new Product
                    {
                        Name = "Diesel Skinzee",
                        Description = "Облегающие джинсы",
                        Price = 129.00m,
                        Stock = 12,
                        Size = "30",
                        Color = "Black",
                        CategoryId = cat.First(c => c.Name == "Skinny").Id,
                        BrandId = brand.First(b => b.Name == "Diesel").Id
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
