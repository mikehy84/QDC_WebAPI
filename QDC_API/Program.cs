using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QDC_BLL.Interfaces;
using QDC_BLL.Mapper;
using QDC_BLL.Repository;
using QDC_BLL.Services;
using QDC_DAL.Data;
using QDC_DAL.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var conStrg = builder.Configuration.GetConnectionString("QdcDbCon");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(conStrg));

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.Configure<IdentityOptions>(options =>
//{
//    options.Password.RequireDigit= false;
//});



var key = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
var issuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer");
var audience = builder.Configuration.GetValue<string>("ApiSettings:Audience");

builder.Services.AddAuthentication(a =>
{
    a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    //opt.Authority = issuer;
    //opt.Audience = audience;
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        //ValidIssuer = issuer,
        //ValidAudience = audience,
    };
});


// adding UnitOfWork here, so we do not need to add all repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// verifies reCAPTCHA tokens against Google's siteverify endpoint on Login/Register
builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>();

// adding AutoMapper here affter installing AutoMapper DependencyInjection
builder.Services.AddAutoMapper(typeof(MappingProfile));







builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();





// this should be added to react would be able to get data from api
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(
                                "http://localhost:3000",
                                "https://qualitydraftingco.com/",
                                "https://qualitydraftingco.com/api/",
                                "https://localhost:7288"
                                )
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed((host) => true)
                            .AllowAnyMethod();
                      });
});



// to add Authorization button on Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n " +
            "Example: \"Bearer 12345abcdef \"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(MyAllowSpecificOrigins);
}

app.UseHttpsRedirection();

// to load react app

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
//app.UseCors(opt =>
//{
//    opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
//    //opt.AllowAnyHeader().AllowAnyMethod().WithOrigins(
//    //    "http://localhost:3000",
//    //    "https://mikelx.com/",
//    //    "https://localhost:7288"
//    //    );
//});


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapFallbackToController("Index", "Home");

app.Run();
