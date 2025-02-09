using Microsoft.EntityFrameworkCore;
using ShoppingListTracker.Models;


var builder = WebApplication.CreateBuilder(args);

var connectionString = "server=bv7lls09esqlxlfcgu5c-mysql.services.clever-cloud.com;database=bv7lls09esqlxlfcgu5c;user=utiq2jnexxcow3w4;password=KfOZhBkTFnwW2ZyIEWvT";
var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));

builder.Services.AddDbContext<ShoppingListContext>(opt =>
    opt.UseMySql(connectionString, serverVersion));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();  
builder.Services.AddSwaggerGen();            

var app = builder.Build();

if (app.Environment.IsDevelopment())  
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();
