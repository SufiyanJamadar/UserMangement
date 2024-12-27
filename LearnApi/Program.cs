
using AutoMapper;
using LearnApi.Container;
using LearnApi.Helper;
using LearnApi.Modal;
using LearnApi.Repos;
using LearnApi.Repos.Models;
using LearnApi.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace LearnApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<ICustomerService,CustomerService>();
            builder.Services.AddTransient<IRefreshHandler, RefreshHandler>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IUserRoleService, UserRoleService>();
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddDbContext<LearndataContext>(
            options => options.UseSqlServer(builder.Configuration.GetConnectionString("apicon")));

            //builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            var _authkey = builder.Configuration.GetValue<string>("JwtSettings:securitykey");
            builder.Services.AddAuthentication(item =>
            {
                item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(item =>
            {
                item.RequireHttpsMetadata = true;
                item.SaveToken = true;
                item.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                };
            });
              
               
             builder.Services.AddAutoMapper(typeof(MappingProfile));

            // getall users was not woking that why i comment out this code 
            //var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
            //IMapper mapper = automapper.CreateMapper();
            // builder.Services.AddSingleton(mapper);
            builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
            {
                build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));

            builder.Services.AddCors(p => p.AddPolicy("corspolicy1", build =>
            {
                build.WithOrigins("https://domain3.com").AllowAnyMethod().AllowAnyHeader();
            }));

            builder.Services.AddCors(p => p.AddDefaultPolicy(build =>
            {
                build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));

            builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
            {
                options.Window = TimeSpan.FromSeconds(10);
                options.PermitLimit = 1;
                options.QueueLimit = 0;
                options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;

            }).RejectionStatusCode=401);

            string logpath = builder.Configuration.GetSection("Logging:Logpath").Value;
            var _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("microsoft",Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(logpath)
                .CreateLogger();
            builder.Logging.AddSerilog(_logger);

            var _jwtsetting = builder.Configuration.GetSection("JwtSettings");
            builder.Services.Configure<JwtSettings>(_jwtsetting);

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            var app = builder.Build();

            app.MapGet("minimalapi",()=>"Sufiyan Jamadar");

            app.MapGet("/getchannel", (string channelname) => "Welcome To " + channelname);

            app.MapGet("/getcustomer", async (LearndataContext db) =>
            {
                return await db.TblCustomers.ToListAsync();
            });


            app.MapGet("/getcustomerbycode/{code}", async (LearndataContext db,string code) =>
            {
                return await db.TblCustomers.FindAsync(code);
            });



            app.MapPost("/createcustomer", async (LearndataContext db, TblCustomer customer) =>
            {
                 await db.TblCustomers.AddAsync(customer);
                await db.SaveChangesAsync();
            });


            app.MapPut("/updatecustomer/{code}", async (LearndataContext db, TblCustomer customer,string code) =>
            {
              var existdata=  await db.TblCustomers.FindAsync(code);
                if(existdata is not null)
                {
                    existdata.Name = customer.Name;
                    existdata.Email = customer.Email;
                    existdata.Phone = customer.Phone;
                }
                await db.SaveChangesAsync();
            });

            app.MapDelete("/removecustomer/{code}", async (LearndataContext db, string code) =>
            {
                var existdata = await db.TblCustomers.FindAsync(code);
                if (existdata is not null)
                {
                    db.TblCustomers.Remove(existdata);
                }
                await db.SaveChangesAsync();
            });


            app.UseRateLimiter();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
