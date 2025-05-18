using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Peritos.Common.Data.BaseModels
{
    public abstract class Auditable
    {
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
        public DateTimeOffset? DateDeleted { get; set; }

        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }

        public bool IsDeleted => DateDeleted != null;
    }

    // Unfortunately EF Core does not allow a nice interface to add custom conventions, so this will have to do. 
    public static class AuditableConventionExtensions
    {
        public static void AddAuditableConventions(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            { 
                //If the actual entity is an auditable type. 
                if(typeof(Auditable).IsAssignableFrom(entityType.ClrType))
                {
                    //This adds (In a reflection type way), a Global Query Filter
                    //https://docs.microsoft.com/en-us/ef/core/querying/filters
                    //That always excludes deleted items. You can opt out by using dbSet.IgnoreQueryFilters()
                    var parameter = Expression.Parameter(entityType.ClrType, "p");
                    var deletedCheck = Expression.Lambda(Expression.Equal(Expression.Property(parameter, "DateDeleted"), Expression.Constant(null)), parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(deletedCheck);

                    //Loop through all properties. 
                    foreach (var property in entityType.GetProperties())
                    {
                        var columnName = property.GetColumnName();
                        if (columnName == "DateCreated")
                        {
                            property.SetDefaultValueSql("SYSDATETIMEOFFSET() AT TIME ZONE 'UTC'");
                        }
                    }

                }
            }
        }
    }
}
