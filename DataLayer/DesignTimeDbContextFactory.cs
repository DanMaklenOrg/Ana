using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace Ana.DataLayer;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "This class is used by dotnet-ef tools for migrations without Dependency Injection")]
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AnaDbContext>
{
    public AnaDbContext CreateDbContext(string[] args)
    {
        if (args.Length != 4) throw new ArgumentException("Expected 4 arguments. <Host> <Port> <Username> <Password>");
        var config = new DatabaseConfig
        {
            Host = args[0],
            Port = int.Parse(args[1]),
            Username = args[2],
            Password = args[3],
        };

        return new AnaDbContext(new OptionsWrapper<DatabaseConfig>(config));
    }
}
