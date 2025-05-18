using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Data.BaseModels;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Peritos.Common.Data
{
    public class BaseContext<TContext> : DbContext where TContext : DbContext
    {
        private readonly IRequestContext _requestContext;

        public BaseContext(DbContextOptions<TContext> options, IRequestContext requestContext = null) : base(options)
        {
            _requestContext = requestContext;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddAuditableConventions();
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var insertedEntries = this.ChangeTracker.Entries()
                                   .Where(x => x.State == EntityState.Added)
                                   .Select(x => x.Entity);

            foreach(var insertedEntry in insertedEntries)
            {
                var auditableEntity = insertedEntry as Auditable;
                //If the inserted object is an Auditable. 
                if(auditableEntity != null)
                {
                    auditableEntity.DateCreated = DateTimeOffset.UtcNow;
                    auditableEntity.CreatedBy = _requestContext != null ? _requestContext.UserId : null;
                }
            }

            var modifiedEntries = this.ChangeTracker.Entries()
                       .Where(x => x.State == EntityState.Modified)
                       .Select(x => x.Entity);

            foreach (var modifiedEntry in modifiedEntries)
            {
                //If the inserted object is an Auditable. 
                var auditableEntity = modifiedEntry as Auditable;
                if (auditableEntity != null)
                {
                    auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
                    auditableEntity.UpdatedBy = _requestContext != null ? _requestContext.UserId : null;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
