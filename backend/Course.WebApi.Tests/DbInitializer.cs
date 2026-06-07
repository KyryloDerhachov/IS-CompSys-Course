using Course.WebApi.Domain.Entities;
using Course.WebApi.Infrastructure.Persistence;
using Course.WebApi.Infrastructure.Services.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Course.WebApi.Tests;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync())
        {
            return;
        }

        var adminRole = new Role("Admin", "Адміністратор", "Full system access");
        var sellerRole = new Role("Seller", "Продавць", "Sales and order processing");
        var warehouseAdminRole = new Role("WarehouseAdmin", "Адміністратор складу", "Inventory and warehouse management");
        var managerRole = new Role("Manager", "Менеджер", "General management");

        await context.Roles.AddRangeAsync(adminRole, sellerRole, warehouseAdminRole, managerRole);
        await context.SaveChangesAsync();

        var adminUser = new User("admin", "admin@company.com", "System", "Administrator");
        var hasher = new BcryptPasswordHasher();
        adminUser.SetPassword(hasher.Hash("AdminPassword123"));
        adminUser.AddRole(adminRole);

        var sellerUser = new User("seller1", "seller1@company.com", "John", "Seller");
        sellerUser.SetPassword(hasher.Hash("SellerPassword123"));
        sellerUser.AddRole(sellerRole);

        await context.Users.AddRangeAsync(adminUser, sellerUser);
        await context.SaveChangesAsync();

        var classA = new ProductClass("CLASS_A", "Class A", "Primary product category", adminUser.Id);
        var classB = new ProductClass("CLASS_B", "Class B", "Secondary product category", adminUser.Id);

        await context.ProductClasses.AddRangeAsync(classA, classB);
        await context.SaveChangesAsync();

        var categoryX = new ProductCategory(null, "CAT_X", "Category X", "General items of type X", adminUser.Id);
        var categoryY = new ProductCategory(null, "CAT_Y", "Category Y", "General items of type Y", adminUser.Id);

        await context.ProductCategories.AddRangeAsync(categoryX, categoryY);
        await context.SaveChangesAsync();

        var productA = new Product(
            "ITEM-001",
            "1234567890123",
            "Product A",
            classA.Id,
            categoryX.Id,
            "unit",
            100.00m,
            150.00m,
            new Dictionary<string, string> 
            { 
                { "Attribute1", "Value1" }, 
                { "Attribute2", "Value2" }
            },
            adminUser.Id
        );

        var productB = new Product(
            "ITEM-002",
            "9876543210987",
            "Product B",
            classB.Id,
            categoryY.Id,
            "unit",
            50.00m,
            75.00m,
            new Dictionary<string, string> 
            { 
                { "Attribute3", "Value3" }, 
                { "Attribute4", "Value4" }
            },
            adminUser.Id
        );

        await context.Products.AddRangeAsync(productA, productB);
        await context.SaveChangesAsync();
    }
}