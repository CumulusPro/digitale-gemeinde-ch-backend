using Cpro.Forms.Data.Models;
using Cpro.Forms.Data.Models.Tenant;
using Cpro.Forms.Data.Models.User;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Data;

namespace Cpro.Forms.Data.Infrastructure;

public class SqlContext : BaseContext<SqlContext>
{
    public SqlContext(DbContextOptions<SqlContext> options, IRequestContext requestContext = null) : base(options, requestContext) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FormDesign>()
                    .HasMany(f => f.FormStates)
                    .WithOne()
                    .HasForeignKey(fs => fs.FormDesignId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormDesign>()
                    .HasMany(f => f.Processors)
                    .WithOne()
                    .HasForeignKey(p => p.FormDesignId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormDesign>()
                    .HasMany(f => f.Designers)
                    .WithOne()
                    .HasForeignKey(d => d.FormDesignId)
                    .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<FormDesignHistory>()
                    .HasMany(f => f.FormStates)
                    .WithOne()
                    .HasForeignKey(fs => fs.FormDesignId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormDesignHistory>()
                    .HasMany(f => f.Processors)
                    .WithOne()
                    .HasForeignKey(p => p.FormDesignId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormDesignHistory>()
                    .HasMany(f => f.Designers)
                    .WithOne()
                    .HasForeignKey(d => d.FormDesignId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
                    .HasIndex(u => new { u.TenantId, u.Email })
                    .IsUnique();

        modelBuilder.Entity<Tenant>()
                    .HasAlternateKey(t => t.TenantId);

        modelBuilder.Entity<User>()
                    .HasOne<Tenant>()
                    .WithMany()
                    .HasPrincipalKey(t => t.TenantId)
                    .HasForeignKey(u => u.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormDesignTag>()
            .HasKey(fdt => new { fdt.FormDesignId, fdt.TagId });

        modelBuilder.Entity<FormDesignTag>()
            .HasOne(fdt => fdt.FormDesign)
            .WithMany(fd => fd.Tags)
            .HasForeignKey(fdt => fdt.FormDesignId);

        modelBuilder.Entity<FormDesignTag>()
            .HasOne(fdt => fdt.Tag)
            .WithMany(t => t.FormDesignTags)
            .HasForeignKey(fdt => fdt.TagId);

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.TagName)
            .IsUnique();

        modelBuilder.HasSequence<int>("DocumentIdSequence", schema: "dbo")
            .StartsAt(100000)
            .IncrementsBy(1);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqlContext).Assembly);
    }

    public DbSet<FormTemplate> FormTemplates { get; set; }
    public DbSet<FormDesign> FormDesigns { get; set; }
    public DbSet<FormDesignHistory> FormDesignsHistory { get; set; }
    public DbSet<FormData> FormDatas { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<FormDesignTag> FormDesignTags { get; set; }
}
