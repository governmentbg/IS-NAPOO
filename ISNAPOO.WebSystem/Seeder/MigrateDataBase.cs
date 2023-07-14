using Data.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace ISNAPOO.WebSystem.Seeder
{
    public class MigrateDataBase
    {
        public static async Task UpdateDataBase(WebApplication applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
        }
    }
}
