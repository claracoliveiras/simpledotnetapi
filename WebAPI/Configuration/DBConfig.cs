using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;

namespace WebAPI.Configuration
{
    public static class DBConfig
    {
        public static WebApplicationBuilder AddDBConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddEntityFrameworkNpgsql().AddDbContext<APIDBContext>(
                opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            return builder;
        }
    }
}
