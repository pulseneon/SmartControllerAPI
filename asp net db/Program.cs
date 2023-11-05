using asp_net_db.Data;
using asp_net_db.Mapper;
using asp_net_db.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 192.168.95.211:7278

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var basePath = AppContext.BaseDirectory;
    var xmlPath = Path.Combine(basePath, "API.xml");
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

string connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

builder.Services.AddScoped<ServerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();


// 4XvL1RHinNFNM6jALE4hPLM - ncIWZxGfk
// https://26.0.102.28:7278/swagger/index.html