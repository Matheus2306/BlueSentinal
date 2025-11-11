using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using BlueSentinal.Models;
using System;
using BlueSentinal.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Adicionar o serviço de autenticação
// Serviço de EndPoints do Identity Framework
builder.Services.AddIdentityApiEndpoints<Usuario>(options =>
{
    options.SignIn.RequireConfirmedEmail = false; // Exige confirmação de email
    options.SignIn.RequireConfirmedAccount = false; // Exige confirmação de conta
    options.User.RequireUniqueEmail = true; // Exige email único
    options.Password.RequireNonAlphanumeric = false; // Exige caracteres não alfanuméricos
    options.Password.RequireLowercase = false; // Exige letras minúsculas
    options.Password.RequireUppercase = false; // Exige letras maiúsculas
    options.Password.RequireDigit = false; // Exige dígitos numéricos
    options.Password.RequiredLength = 4; // Exige comprimento mínimo da senha
})

.AddRoles<IdentityRole>() // Adicionando o serviço de roles
.AddEntityFrameworkStores<APIContext>() // Adicionando o serviço de EntityFramework
.AddDefaultTokenProviders(); // Adiocionando o provedor de tokens padrão

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BlueSentinal", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Adicionar o serviço de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});


// Adicionar os Serviços de Autenticação e Autorização
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

app.UseSwagger(); // Habilita o Swagger

app.UseSwaggerUI(); // Habilita a interface do Swagger

app.UseHttpsRedirection(); // Redireciona requisições HTTP para HTTPS

app.UseCors("AllowAll"); // Habilita o CORS

app.UseAuthentication(); // Habilita a autenticação

app.UseAuthorization(); // Habilita a autorização

app.MapControllers(); // Mapeia os controladores

app.MapGroup("/Usuario").MapIdentityApi<Usuario>(); // Mapeia o grupo de endpoints de autenticação

app.Run(); // Executa o aplicativo
