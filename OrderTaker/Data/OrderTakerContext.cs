using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderTaker.Models;

namespace OrderTaker.Data
{
    public class OrderTakerContext : DbContext
    {
        public OrderTakerContext (DbContextOptions<OrderTakerContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            //    {
            //        var parameter = Expression.Parameter(entityType.ClrType, "e");
            //        var filter = Expression.Lambda(
            //            Expression.Equal(
            //                Expression.Property(parameter, nameof(BaseEntity.IsActive)),
            //                Expression.Constant(true)),
            //            parameter);

            //        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            //    }
            //}
        }
        public override int SaveChanges()
        {
            BaseSaveData();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BaseSaveData();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void BaseSaveData()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach(var entry in entries)
            {
                if(entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.Timestamp = DateTime.Now;
                }
                else if(entry.State == EntityState.Modified)
                {
                    entry.Entity.Timestamp = DateTime.Now;
                    entry.Property(e => e.CreatedDate).IsModified = false;
                }
            }
        }

        public DbSet<Customer> Customer { get; set; } = default!;
        public DbSet<Sku> Sku { get; set; } = default!;
        public DbSet<OrderItem> OrderItem { get; set; } = default!;
        public DbSet<Order> Order { get; set; } = default!;
    }
}
