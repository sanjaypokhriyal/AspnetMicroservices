using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.Grpc.Protos;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Basket Microserice", Version = "v1" });
            });

            //Redis Confuguration
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetValue<String>("CacheSettings:ConnectionString");
            });
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddAutoMapper(typeof(Startup));

            //Adding GRPC Service Configuration
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(
                o => o.Address = new Uri(Configuration["GrpcSettings:DiscountUrl"]));
            services.AddScoped<DiscountGrpcServices>();


            //MassTransit- Rabbit MQ Configuration
            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    // connection string of the rabbit mq : amqp://<username>:<password>@<server>:<port>
                    cfg.Host(Configuration["EventBusSettings:HostAddress"]);
                });
            });
            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket MicroService");
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
