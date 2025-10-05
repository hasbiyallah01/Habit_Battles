using Habit_Battles.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Habit_Battles.Infrastructure.Context
{
    public class HabitBattlesContext : DbContext
    {
        public HabitBattlesContext(DbContextOptions<HabitBattlesContext> opt) : base(opt)
        {

        }

        public DbSet<Battle> Battles => Set<Battle>();
        public DbSet<User> Users => Set<User>();
        public DbSet<HabitLog> HabitLogs => Set<HabitLog>();
        public DbSet<Purchases> Purchases => Set<Purchases>();
        public DbSet<ShopItem> ShopItems => Set<ShopItem>();
        public DbSet<Strike> Strikes => Set<Strike>();
        public DbSet<UserBattle> UserBattles => Set<UserBattle>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }
                }
            }

            var concernsConverter = new ValueConverter<List<string>, string>(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
               );


            modelBuilder.Entity<UserBattle>()
                .HasKey(ub => new { ub.UserId, ub.BattleId });

            modelBuilder.Entity<UserBattle>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.Battles)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBattle>()
                .HasOne(ub => ub.Battle)
                .WithMany(b => b.UserBattles)
                .HasForeignKey(ub => ub.BattleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HabitLog>()
                .HasOne(h => h.User)
                .WithMany(u => u.HabitLogs)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HabitLog>()
                .HasOne(h => h.Battle)
                .WithMany()
                .HasForeignKey(h => h.BattleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Strike>()
                .HasOne(s => s.Battle)
                .WithMany()
                .HasForeignKey(s => s.BattleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Strike>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Purchases>()
                .HasOne(p => p.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Purchases>()
                .HasOne(p => p.ShopItem)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.ShopItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShopItem>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Coins)
                .HasDefaultValue(0);

            modelBuilder.Entity<User>()
                .Property(u => u.Streaks)
                .HasDefaultValue(0);

            modelBuilder.Entity<User>()
                .Property(u => u.Wins)
                .HasDefaultValue(0);

            modelBuilder.Entity<User>()
                .Property(u => u.Losses)
                .HasDefaultValue(0);
        }
    }
}
