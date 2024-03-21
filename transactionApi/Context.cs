using transactionApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace transactionApi
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(x => x.TransactionId);
                //entity.HasData(DatabaseGeneratedOption.None);
                entity.ToTable("Transaction");
            });

            modelBuilder.Entity<User>(entity =>
            { 
                entity.HasKey(x => x.userID);
                entity.ToTable("User");
            });
        }
    }
}
