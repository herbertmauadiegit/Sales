using Microsoft.EntityFrameworkCore;
using Sales.Application;
using Sales.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SaleDbContext>(options =>
    options.UseInMemoryDatabase("SaleDb"));

builder.Services.AddControllers();
builder.Services.AddTransient<ISaleService, SaleService>();
builder.Services.AddSingleton<IInMemoryQueueService, InMemoryQueueService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
