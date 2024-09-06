using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceAPI3.Models
{
    public class AttendanceContext : DbContext
    {
        public AttendanceContext(DbContextOptions<AttendanceContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Sequance> Sequances { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Data Source=DESKTOP-LU175Q4\\SQLEXPRESS01;Initial Catalog=Attendance3;Integrated Security=True; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(a => a.User)
                .WithMany(u => u.AttendanceRecords)
                .HasForeignKey(a => a.User_Id)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(a => a.Session)
                .WithMany(s => s.AttendanceRecords)
                .HasForeignKey(a => a.Session_Id)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<Session>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.User_Id)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
                                                    // Unique constraint for PackageName in Package table
            modelBuilder.Entity<Package>()
                .HasIndex(p => p.PackageName)
                .IsUnique();

            // Unique constraint for SequanceName in Sequance table
            modelBuilder.Entity<Sequance>()
                .HasIndex(s => s.SequanceName)
                .IsUnique();

            // Unique constraint for SessionName in Session table
            modelBuilder.Entity<Session>()
                .HasIndex(s => s.SessionName)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
