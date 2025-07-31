using Microsoft.EntityFrameworkCore;
using AutoServiceAPI.Models;

namespace AutoServiceAPI.Data
{
    public class AutoServiceDbContext : DbContext
    {
        public AutoServiceDbContext(DbContextOptions<AutoServiceDbContext> options) : base(options)
        {
        }

        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillService> BillServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Organisation)
                .WithMany(o => o.Users)
                .HasForeignKey(u => u.OrganisationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Car>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Cars)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Client)
                .WithMany(c => c.Bills)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Car)
                .WithMany(c => c.Bills)
                .HasForeignKey(b => b.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.CreatedBy)
                .WithMany(u => u.BillsCreated)
                .HasForeignKey(b => b.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BillService>()
                .HasOne(bs => bs.Bill)
                .WithMany(b => b.BillServices)
                .HasForeignKey(bs => bs.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BillService>()
                .HasOne(bs => bs.Service)
                .WithMany(s => s.BillServices)
                .HasForeignKey(bs => bs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Car>()
                .HasIndex(c => c.Vin)
                .IsUnique();

            modelBuilder.Entity<Bill>()
                .HasIndex(b => b.BillNumber)
                .IsUnique();

            // Configure indexes for better performance
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.PhoneNumber);

            modelBuilder.Entity<Car>()
                .HasIndex(c => c.LicensePlate);

            modelBuilder.Entity<Car>()
                .HasIndex(c => c.InsuranceNumber);

            modelBuilder.Entity<Bill>()
                .HasIndex(b => b.Date);
        }
    }
} 