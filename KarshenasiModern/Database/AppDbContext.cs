using KarshenasiModern.Models;
using Microsoft.EntityFrameworkCore;

namespace KarshenasiModern.Database;

public class AppDbContext : DbContext
{
    // سازنده کلاس: دیتابیس را در صورت عدم وجود، از روی ساختار مدل‌ها می‌سازد
    public AppDbContext()
    {
        // این خط تضمین می‌کند که دیتابیس حذف شده با تمام جدول‌های جدید مجدداً ساخته شود
        Database.EnsureCreated();
    }

    // تعاریف جدول‌ها (DbSets)
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<BodyPartInspection> BodyPartInspections => Set<BodyPartInspection>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // تنظیم کانکشن استرینگ برای دیتابیس SQLite
        optionsBuilder.UseSqlite(@"Data Source=KarshenasiModern.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // رابطه یک به چند: خودرو به کارشناسی‌ها
        modelBuilder.Entity<Car>()
            .HasMany(x => x.Inspections)
            .WithOne(x => x.Car)
            .HasForeignKey(x => x.CarId);

        // رابطه یک به چند: کارشناسی به عکس‌ها
        modelBuilder.Entity<Inspection>()
            .HasMany(x => x.Photos)
            .WithOne(x => x.Inspection)
            .HasForeignKey(x => x.InspectionId);

        // رابطه یک به چند: کارشناسی به وضعیت قطعات بدنه خودرو
        modelBuilder.Entity<Inspection>()
            .HasMany(x => x.BodyParts)
            .WithOne(x => x.Inspection)
            .HasForeignKey(x => x.InspectionId);
    }
}