using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAut.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
		public ApplicationContext()
		{
            
		}

        public IQueryable<Poll> GetPlanned()
		{
            var all = Polls;
            return all.Where(p => p.StartTime > DateTime.Now);
        }

		/*
protected override void OnModelCreating(ModelBuilder builder)
{
   builder.Entity<Vote>()
   .HasKey(c => new { c.UserId, c.OptionId });
   base.OnModelCreating(builder);

}
*/
		public IQueryable<Poll> GetCurrent()
		{
            var all = Polls;
            return all.Where(p => p.StartTime < DateTime.Now && p.EndTime > DateTime.Now);
		}
        public IQueryable<Poll> GetOld()
        {
            var all = Polls;
            return all.Where(p => p.EndTime < DateTime.Now);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

        }
    }
}
