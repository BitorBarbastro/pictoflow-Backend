using Microsoft.EntityFrameworkCore;
using pictoflow_Backend.Services;
using Microsoft.OpenApi.Models;
using pictoflow_Backend;

var builder = WebApplication.CreateBuilder(args);

// Configurar los endpoints de Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5046);
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Inyectar el servicio de caché en memoria
builder.Services.AddMemoryCache();

// Agregar el contexto de la base de datos
builder.Services.AddDbContext<pictoflow_Backend.Models.ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))));

// Registrar UserManager como un servicio
builder.Services.AddScoped<UserManager>();

// Configurar CORS para permitir solicitudes desde React en el puerto 3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactOrigin", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseSession();
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowReactOrigin");

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();
