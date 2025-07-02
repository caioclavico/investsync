using InvestSync.FinnhubWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<FinnhubWorker>();

var host = builder.Build();
host.Run();
