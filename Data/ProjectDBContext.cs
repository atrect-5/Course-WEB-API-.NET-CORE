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
            modelBuilder.Entity<User>()
                .HasIndex(f => f.Email)
                .IsUnique();

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

            // Si se intenta eliminar una Categoría que tiene Transacciones, la operación se bloqueará.
            // Esto previene que queden transacciones huérfanas y soluciona un posible ciclo de cascada.
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // No se puede eliminar una Cuenta de Dinero si tiene Transacciones asociadas.
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.MoneyAccount)
                .WithMany(ma => ma.Transactions)
                .HasForeignKey(t => t.MoneyAccountId)
                .OnDelete(DeleteBehavior.Restrict);

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
        
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MoneyAccount> MoneyAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
    }
}
