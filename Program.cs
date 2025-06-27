using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using net_chat.Services;

var builder = WebApplication.CreateBuilder(args);

var signingKey = JwtService.GetSecurityKey();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlDb"))));

// Register AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddSession();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
    options.DefaultSignInScheme = "Cookies";
})
.AddCookie(options =>
{
    options.LoginPath = "/Index";
})
.AddJwtBearer(options =>
{
    var config = builder.Configuration;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = signingKey
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API", Version = "v1" });

    // ✅ Cấu hình hỗ trợ JWT Bearer
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: Bearer {token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetChat API V1");
        c.RoutePrefix = "swagger";
    });

}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
