using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using coa_Wallet.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationship between User and Account
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)          // Each account belongs to one user
                .WithMany(u => u.Accounts)    // Each user can have many accounts
                .HasForeignKey(a => a.UserId) // Foreign key in the Account table
                .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, their accounts will also be deleted

                modelBuilder.Entity<Category>()
                .HasOne(c => c.Account)
                .WithMany()
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
