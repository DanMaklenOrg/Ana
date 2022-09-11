using Ana.DataLayer.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Ana.DataLayer;

public class AnaDbContext : DbContext
{
    private readonly DatabaseConfig config;

    public AnaDbContext(IOptions<DatabaseConfig> config)
    {
        this.config = config.Value;
    }

    public DbSet<UserDbModel> Users { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Database = "Ana",
            Host = this.config.Host,
            Port = this.config.Port,
            Username = this.config.Username,
            Password = this.config.Password,
        }.ToString();

        optionsBuilder.UseNpgsql(connectionString);
    }
}
