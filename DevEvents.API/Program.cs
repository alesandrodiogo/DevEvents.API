
using DevEvents.API.Domain.Repositories;
using DevEvents.API.Infrastructure.Persistence.Repositories;
using DevEvents.API.Infrastructure;
using DevEvents.API.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using DevEvents.API.Endpoints;

namespace DevEvents.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("AppDb");

            builder.Services
                .AddDbContext<AppDbContext>(o =>
                    o.UseSqlServer(connectionString)
                );

            builder.Services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

            builder.Services.RegisterMaps();

            builder.Services.AddScoped<IConferenceRepository, ConferenceRepository>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            
            app.AddConferenceEndpoints();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.Run();
        }
    }
}
