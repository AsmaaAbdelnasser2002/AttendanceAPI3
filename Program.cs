using AttendanceAPI3.Models;
using AttendanceAPI3.Extentions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AttendanceContext>();

            builder.Services.AddCors(CorsOptions => {
                CorsOptions.AddPolicy("MyPolicy", CorsPolicyBuilder =>
                {
                    CorsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
           
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGenJwtAuth();
            builder.Services.AddCustomJwtAuth(builder.Configuration);

			builder.Services.AddControllers();
			builder.Services.AddDbContext<AttendanceContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("con")));

			builder.Services.AddSingleton<QRCodeService>();
			builder.Services.AddHostedService<QRCodeRegenerationService>();

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();

            //setting core policy
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
	}
}