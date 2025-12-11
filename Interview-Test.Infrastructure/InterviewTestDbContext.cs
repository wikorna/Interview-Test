using Interview_Test.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Interview_Test.Infrastructure;

public class InterviewTestDbContext : DbContext
{
    public InterviewTestDbContext(DbContextOptions<InterviewTestDbContext> options) : base(options)
    {
    }
    
    public DbSet<UserModel> UserTb { get; set; }
    public DbSet<UserProfileModel> UserProfileTb { get; set; }
    public DbSet<RoleModel> RoleTb { get; set; }
    public DbSet<UserRoleMappingModel> UserRoleMappingTb { get; set; }
    public DbSet<PermissionModel> PermissionTb { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        /*// =======================
        // ROLE SEED
        // =======================
        modelBuilder.Entity<RoleModel>().HasData(
            new RoleModel { RoleId = 1, RoleName = "pick operation" },
            new RoleModel { RoleId = 2, RoleName = "pack operation" },
            new RoleModel { RoleId = 3, RoleName = "document operation" }
        );

        // =======================
        // PERMISSION SEED
        // =======================
        modelBuilder.Entity<PermissionModel>().HasData(
            // Role 1
            new PermissionModel { PermissionId = 1, Permission = "1-01-picking-info", RoleId = 1 },
            new PermissionModel { PermissionId = 2, Permission = "1-02-picking-start", RoleId = 1 },
            new PermissionModel { PermissionId = 3, Permission = "1-03-picking-confirm", RoleId = 1 },
            new PermissionModel { PermissionId = 4, Permission = "1-04-picking-report", RoleId = 1 },

            // Role 2
            new PermissionModel { PermissionId = 5, Permission = "2-01-packing-info", RoleId = 2 },
            new PermissionModel { PermissionId = 6, Permission = "2-02-packing-start", RoleId = 2 },
            new PermissionModel { PermissionId = 7, Permission = "2-03-packing-confirm", RoleId = 2 },
            new PermissionModel { PermissionId = 8, Permission = "2-04-packing-report", RoleId = 2 },

            // Role 3
            new PermissionModel { PermissionId = 9, Permission = "1-04-picking-report", RoleId = 3 },
            new PermissionModel { PermissionId = 10, Permission = "2-04-packing-report", RoleId = 3 },
            new PermissionModel { PermissionId = 11, Permission = "3-01-printing-label", RoleId = 3 }
        );*/
        
        // User
        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.HasIndex(u => u.UserId).IsUnique();
            entity.Property(u => u.UserId).HasMaxLength(20).IsRequired();
            entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
        });
        // Role
        modelBuilder.Entity<RoleModel>(entity =>
        {
            entity.Property(r => r.RoleName).HasMaxLength(100).IsRequired();
        });

        // Permission: Role 1 - N Permission
        modelBuilder.Entity<PermissionModel>(entity =>
        {
            entity.HasKey(p => p.PermissionId);
            entity.Property(p => p.Permission)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.HasOne(p => p.Role)
                  .WithMany(r => r.Permissions)
                  .HasForeignKey(p => p.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserRoleMappingModel>(entity =>
        {
            entity.HasOne(urm => urm.User)
                  .WithMany(u => u.UserRoleMappings)
                  .HasForeignKey(u => u.UserId)
                  .HasPrincipalKey(u => u.Id)
                  .OnDelete(DeleteBehavior.Cascade);

            // Role 1 - N UserRoleMapping
            entity.HasOne(urm => urm.Role)
                  .WithMany(r => r.UserRoleMappings)
                  .HasForeignKey(urm => urm.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            // ป้องกันไม่ให้ 1 User มี Role เดียวกันซ้ำ
            entity.HasIndex(urm => new { urm.UserId, urm.RoleId }).IsUnique();
        });
    }
}

public class InterviewTestDbContextDesignFactory : IDesignTimeDbContextFactory<InterviewTestDbContext>
{
    public InterviewTestDbContext CreateDbContext(string[] args)
    {
        // current directory ตอนรัน dotnet ef จะอยู่ที่ project / solution
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString =
            config.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=InterviewDb;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<InterviewTestDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new InterviewTestDbContext(optionsBuilder.Options);
    }
}