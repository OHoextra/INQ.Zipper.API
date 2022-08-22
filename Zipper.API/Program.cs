using System.Reflection;
using Zipper.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ZipperService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.IncludeXmlComments(
            Path.Combine(
                AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

             // TODO Read more on how this can be useful
            //options.MapType<FileContentResult>(() => new OpenApiSchema { Type = "file" });
    });

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
