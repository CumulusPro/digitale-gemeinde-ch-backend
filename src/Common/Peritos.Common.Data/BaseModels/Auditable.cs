using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Peritos.Common.Data.BaseModels
{
    /// <summary>
    /// Represents an entity that tracks audit information such as creation, update, and deletion timestamps,
    /// along with the user who performed these actions.
    /// </summary>
    public abstract class Auditable
    {
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
        public DateTimeOffset? DateDeleted { get; set; }

        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }

        /// <summary>
        /// Gets a value indicating whether the entity is considered deleted (soft delete).
        /// </summary>
        public bool IsDeleted => DateDeleted != null;
    }

    /// <summary>
    /// Extension methods to configure EF Core model conventions for entities that derive from <see cref="Auditable"/>.
    /// </summary>
    public static class AuditableConventionExtensions
    {
        /// <summary>
        /// Adds conventions for auditable entities to the EF Core <see cref="ModelBuilder"/>.
        /// This includes:
        /// <list type="bullet">
        /// <item>A global query filter to exclude soft-deleted entities (where <c>DateDeleted</c> is not null).</item>
        /// <item>Sets a default SQL value for the <c>DateCreated</c> column to use the current UTC time.</item>
        /// </list>
        /// </summary>
        /// <param name="modelBuilder">The EF Core model builder.</param>
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
