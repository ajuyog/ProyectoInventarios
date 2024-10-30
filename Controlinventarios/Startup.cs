using Controlinventarios.Model;
using Microsoft.EntityFrameworkCore;

namespace Controlinventarios
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAutoMapper(typeof(Startup));
            //var connectionString = Configuration.GetConnectionString("DefaultConnectionSqlServer");

            //// Registra el DbContext usando SQL Server
            //services.AddDbContext<InventoryTIContext>(options =>
            //    options.UseSqlServer(connectionString));

            var connectionStringMysql = Configuration.GetConnectionString("DefaultConnectionMySqlValuez");
            services.AddDbContext<InventoryTIContext>(options => options.UseMySql(connectionStringMysql, ServerVersion.AutoDetect(connectionStringMysql)));

            //var connectionStrinMysql = Configuration.GetConnectionString("DefaultConnectionMySql");
            //services.AddDbContext<InventoryTIContext>(options => options.UseMySql(connectionStrinMysql, ServerVersion.AutoDetect(connectionStrinMysql)));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {   
                endpoints.MapControllers();
            });
        }

    }
}
