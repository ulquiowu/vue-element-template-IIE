using iie_erp_assist_server.Authorities;
using iie_erp_assist_server.Converters;
using iie_erp_assist_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace iie_erp_assist_server
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
            services = services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "IIE.ERP.ASSIST.API",
                    Version = "v1",
                    Description = "锡凡-惟勤 ERP 辅助平台接口",
                    Contact = new OpenApiContact
                    {
                        Email = "jeremy.wu@foxmail.com",
                        Name = "作者: CIM 项目组"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Copyright © 2021 IIE - 保留所有权利"
                    }
                });

                // 添加Header验证信息
                var security = new OpenApiSecurityRequirement{
                        {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },
                                new string[] {}
                        }
                  };
                c.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });

                // 增加相应需要在Swagger中显示的帮助文档
                c.IncludeXmlComments(GetXmlCommnetsPath("iie-erp-assist-server"), true);
            });
            // 时间T格式问题
            services.AddControllers().AddJsonOptions(configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
            });

            #region Token

            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("System", policy => policy.RequireClaim("SystemType").Build());
                options.AddPolicy("Client", policy => policy.RequireClaim("ClientType").Build());
                options.AddPolicy("Admin", policy => policy.RequireClaim("AdminType").Build());
            });
            services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();
            #endregion

            // 追加跨域支持
            services.AddCors(_options => _options.AddPolicy("AllowCors", _builder => _builder.AllowAnyOrigin().AllowAnyMethod()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "iie_erp_assist_server v1");
                c.RoutePrefix = "api";
                // c.DefaultModelsExpandDepth(-1);
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "iie_erp_assist_server v1");
                c.RoutePrefix = "api";
                });

            #region TokenAuth
            app.UseMiddleware<TokenAuthHelper>();
            #endregion

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// 获取帮助文件路径
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        protected string GetXmlCommnetsPath(string name)
        {
            return string.Format(@"{0}\{1}.XML", AppDomain.CurrentDomain.BaseDirectory, name);
        }
    }
}
