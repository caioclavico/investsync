using InvestSync.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<FinnhubWorker>();
// builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
