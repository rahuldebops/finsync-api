using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using finsyncapi.BAL.IServices;
using finsyncapi.BAL.Services;
using finsyncapi.DAL;
using finsyncapi.DAL.IRepositories;
using finsyncapi.DAL.Repositories;
using finsyncapi.Middlewares;
using System;
using finsyncapi.Extension;
using Hangfire;
using Hangfire.PostgreSql;
using finsyncapi.Models;


var builder = WebApplication.CreateBuilder(args);

// DB Context

builder.Services.AddDbContextPool<DB1Context>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DB1Connection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
        });
});

// Authentication - JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// REPOSITORIES
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IMasterRepository, MasterRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();

// SERVICES 
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMasterService, MasterService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<ISnowflakeService, SnowflakeService>();


builder.Services.AddSingleton<DapperContext>();


builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.Configure<AppSettingsModel>(builder.Configuration);

builder.Services.AddHangfire(config => config.UsePostgreSqlStorage( 
    builder.Configuration.GetConnectionString("Hangfire"),
    new PostgreSqlStorageOptions
    {
        QueuePollInterval = TimeSpan.FromHours(1)
    }));

builder.Services.AddHangfireServer(options =>
{
    options.SchedulePollingInterval = TimeSpan.FromHours(1);
    options.ServerCheckInterval = TimeSpan.FromHours(1); 
});

// LOGGING
builder.Logging.AddFilter("Npgsql", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
var app = builder.Build();

app.UseHangfireDashboard("/hangfire");
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();


app.Run();
