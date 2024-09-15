using System;
using System.Reflection;
using Core.Entities;
using Infrastructure.Config;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class StoreContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<Product> Products { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        
         modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
         base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));
                    var dateTimeProperties = entityType.ClrType.GetProperties()
                        .Where(p => p.PropertyType == typeof(DateTimeOffset));

                    foreach (var property in properties)
                    {
                        modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion<double>();
                    }

                    // foreach (var property in dateTimeProperties)
                    // {
                    //     modelBuilder.Entity(entityType.Name).Property(property.Name)
                    //         .HasConversion(new DateTimeOffsetToBinaryConverter());
                    // }
                }
            }
        }
}
