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
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<MoneyAccount>(entity =>
            {
                entity.ToTable("MoneyAccount");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Previene que se elimine una Categoría si tiene transacciones.
                entity.HasOne(t => t.Category)
                    .WithMany(c => c.Transactions)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Previene que se elimine una Cuenta de Dinero si tiene transacciones.
                entity.HasOne(t => t.MoneyAccount)
                    .WithMany(ma => ma.Transactions)
                    .HasForeignKey(t => t.MoneyAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transfer>(entity =>
            {
                entity.ToTable("Transfer");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Previene que se elimine una Cuenta de Dinero si está involucrada en una transferencia.
                entity.HasOne(t => t.MoneyAccountSend)
                    .WithMany(ma => ma.TransfersSent)
                    .HasForeignKey(t => t.MoneyAccountSendId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.MoneyAccountReceive)
                    .WithMany(ma => ma.TransfersReceived)
                    .HasForeignKey(t => t.MoneyAccountReceiveId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MoneyAccount> MoneyAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
    }
}
