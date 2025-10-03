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

        }
    }
}
