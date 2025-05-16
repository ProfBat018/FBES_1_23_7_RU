using ControllerFirst.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);



// using (var scope = app.Services.CreateScope())
// {
//     var svc = scope.ServiceProvider.GetRequiredService<IUserElasticService>();
//     await svc.CreateIndexIfNotExistsAsync();
//     await svc.ReindexAllUsersAsync();
// }

app.Run();
