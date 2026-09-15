using INCLUTEC.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();




// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<InclutecbdContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("INCLUTEBD"),
        sqlBuilder=>
        { 
        sqlBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            sqlBuilder.CommandTimeout(30);
            sqlBuilder.EnableRetryOnFailure();
        }
        ));
        




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "INCLUTEC.Api v1");  
    });

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
