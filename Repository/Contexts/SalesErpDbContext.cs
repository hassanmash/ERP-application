using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts
{
    public class SalesErpDbContext : DbContext
    {
        private readonly ICurrentTenantService _tenant;

        public SalesErpDbContext(DbContextOptions<SalesErpDbContext> options, ICurrentTenantService tenant)
            : base(options)
        {
            _tenant = tenant;
        }

        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<OrgModule> OrgModules => Set<OrgModule>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Lead> Leads => Set<Lead>();
        public DbSet<LeadStatusHistory> LeadStatusHistories => Set<LeadStatusHistory>();
        public DbSet<Activity> Activities => Set<Activity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Organization ---
            modelBuilder.Entity<Organization>(e =>
            {
                e.HasIndex(o => o.OrgCode).IsUnique();
                e.Property(o => o.Status).HasConversion<string>();
            });
            // No query filter here: Organization itself is the tenant root, not tenant-scoped.
            // Platform admins list/manage all orgs; org-scoped users never query this table directly.

            // --- OrgModule (tenant-scoped) ---
            modelBuilder.Entity<OrgModule>(e =>
            {
                e.HasOne(m => m.Organization).WithMany(o => o.OrgModules).HasForeignKey(m => m.OrganizationId);
                e.HasIndex(m => new { m.OrganizationId, m.ModuleName }).IsUnique();
                e.HasQueryFilter(m => _tenant.IsPlatformAdmin || m.OrganizationId == _tenant.OrganizationId);
            });

            // --- Team (tenant-scoped) ---
            modelBuilder.Entity<Team>(e =>
            {
                e.HasOne(t => t.Organization).WithMany(o => o.Teams).HasForeignKey(t => t.OrganizationId);
                e.HasOne(t => t.LeadUser).WithMany().HasForeignKey(t => t.LeadUserId).OnDelete(DeleteBehavior.SetNull);
                e.HasQueryFilter(t => _tenant.IsPlatformAdmin || t.OrganizationId == _tenant.OrganizationId);
            });

            // --- Role (tenant-scoped) ---
            modelBuilder.Entity<Role>(e =>
            {
                e.HasOne(r => r.Organization).WithMany(o => o.Roles).HasForeignKey(r => r.OrganizationId);
                e.Property(r => r.Permissions).HasColumnType("jsonb");
                e.HasIndex(r => new { r.OrganizationId, r.Name }).IsUnique();
                e.HasQueryFilter(r => _tenant.IsPlatformAdmin || r.OrganizationId == _tenant.OrganizationId);
            });

            // --- User (tenant-scoped) ---
            modelBuilder.Entity<User>(e =>
            {
                e.HasOne(u => u.Organization)
                    .WithMany(o => o.Users)
                    .HasForeignKey(u => u.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict); // platform admins have null OrganizationId; non-null FKs still enforced;
                e.HasOne(u => u.Team).WithMany(t => t.Members).HasForeignKey(u => u.TeamId).OnDelete(DeleteBehavior.SetNull);
                e.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.SetNull);
                e.HasIndex(u => new { u.OrganizationId, u.Email }).IsUnique();
                // Postgres treats NULL OrganizationId as distinct per row in a unique index,
                // so two platform admins could otherwise share an email undetected.
                // A separate partial unique index closes that gap.
                e.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasFilter("organization_id IS NULL");
                e.HasQueryFilter(u => _tenant.IsPlatformAdmin || u.OrganizationId == _tenant.OrganizationId);
            });

            // --- Lead (tenant-scoped) ---
            modelBuilder.Entity<Lead>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                e.Property(x => x.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                e.Property(x => x.Email)
                    .HasMaxLength(200);

                e.Property(x => x.Source)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(x => x.Project)
                    .IsRequired()
                    .HasMaxLength(200);

                e.Property(x => x.Status)
                    .HasConversion<int>();

                e.HasOne(x => x.Organization)
                    .WithMany()
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.AssignedUser)
                    .WithMany(u => u.AssignedLeads)
                    .HasForeignKey(x => x.AssignedUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(x => x.Activities)
                    .WithOne(x => x.Lead)
                    .HasForeignKey(x => x.LeadId);

                e.HasMany(x => x.StatusHistory)
                    .WithOne(x => x.Lead)
                    .HasForeignKey(x => x.LeadId);

                e.HasIndex(x => x.OrganizationId);

                e.HasIndex(x => x.AssignedUserId);

                e.HasIndex(x => x.Status);

                e.HasQueryFilter(x =>
                    _tenant.IsPlatformAdmin ||
                    x.OrganizationId == _tenant.OrganizationId);
            });

            // --- Activity (tenant-scoped) ---
            modelBuilder.Entity<Activity>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Type)
                    .HasConversion<int>();

                e.Property(x => x.Notes)
                    .IsRequired()
                    .HasMaxLength(1000);

                e.HasOne(x => x.Organization)
                    .WithMany()
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Lead)
                    .WithMany(x => x.Activities)
                    .HasForeignKey(x => x.LeadId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.CreatedByUser)
                    .WithMany(u => u.CreatedActivities)
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.OrganizationId);

                e.HasIndex(x => x.LeadId);

                e.HasIndex(x => x.CreatedByUserId);

                e.HasQueryFilter(x =>
                    _tenant.IsPlatformAdmin ||
                    x.OrganizationId == _tenant.OrganizationId);
            });

            // --- LeadStatusHistory (tenant-scoped) ---
            modelBuilder.Entity<LeadStatusHistory>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.FromStatus)
                    .HasConversion<int>();

                e.Property(x => x.ToStatus)
                    .HasConversion<int>();

                e.HasOne(x => x.Organization)
                    .WithMany()
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Lead)
                    .WithMany(x => x.StatusHistory)
                    .HasForeignKey(x => x.LeadId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.ChangedByUser)
                    .WithMany(u => u.LeadStatusChanges)
                    .HasForeignKey(x => x.ChangedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.OrganizationId);

                e.HasIndex(x => x.LeadId);

                e.HasIndex(x => x.ChangedByUserId);

                e.HasQueryFilter(x =>
                    _tenant.IsPlatformAdmin ||
                    x.OrganizationId == _tenant.OrganizationId);
            });
        }
    }
}
