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