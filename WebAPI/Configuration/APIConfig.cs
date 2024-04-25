namespace WebAPI.Configuration
{
    public static class APIConfig
    {
        public static WebApplicationBuilder AddAPIConfig (this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            return builder;
        }
    }
}
