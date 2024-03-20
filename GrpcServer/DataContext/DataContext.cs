using GrpcServer.Protos;
using Microsoft.EntityFrameworkCore;

namespace GrpcServer.DataContext
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<MatchHistory> MatchHistories { get; set; }
        public DbSet<GameTransactions> GameTransactions { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username);

            modelBuilder.Entity<MatchHistory>()
               .HasIndex(m => m.Player1Id);

            modelBuilder.Entity<MatchHistory>()
                .HasIndex(m => m.Player2Id);

            modelBuilder.Entity<MatchHistory>()
                .HasIndex(m => m.WinnerId);

            modelBuilder.Entity<GameTransactions>()
                .HasIndex(t => t.MatchId);
        }
    }
}
