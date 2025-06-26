using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class ProjectDBContext : DbContext
    {
        public ProjectDBContext(DbContextOptions<ProjectDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Category>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<MoneyAccount>().ToTable("MoneyAccount");
            modelBuilder.Entity<MoneyAccount>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Transaction>().ToTable("Transaction");
            modelBuilder.Entity<Transaction>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Transfer>().ToTable("Transfer");
            modelBuilder.Entity<Transfer>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.MoneyAccountSend)
                .WithMany(ma => ma.TransfersSent)
                .HasForeignKey(t => t.MoneyAccountSendId)
                .OnDelete(DeleteBehavior.Restrict); // evita conflictos de eliminación

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.MoneyAccountReceive)
                .WithMany(ma => ma.TransfersReceived)
                .HasForeignKey(t => t.MoneyAccountReceiveId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
        // Define DbSet properties for your entities
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}
