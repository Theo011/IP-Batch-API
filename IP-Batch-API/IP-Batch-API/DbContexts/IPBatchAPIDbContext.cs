using Microsoft.EntityFrameworkCore;
using IP_Batch_API.Entities;

namespace IP_Batch_API.DbContexts
{
    public class IPBatchAPIDbContext : DbContext
    {
        public DbSet<IPDetail> IPDetail { get; private set; } = null!;

        public IPBatchAPIDbContext(DbContextOptions<IPBatchAPIDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IPDetail>().HasData(new IPDetail(
                "0.0.0.0",
                "city name",
                "country name",
                "continent name",
                0,
                0));
        }
    }
}