using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MeoMeo.EntityFrameworkCore.Configurations.Contexts
{
    public class MeoMeoDbContextFactory : IDesignTimeDbContextFactory<MeoMeoDbContext>
    {


        MeoMeoDbContext IDesignTimeDbContextFactory<MeoMeoDbContext>.CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<MeoMeoDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new MeoMeoDbContext(optionsBuilder.Options);
        }
    }
}
