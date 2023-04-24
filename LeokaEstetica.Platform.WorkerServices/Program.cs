var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(_ => { })
    .Build();

await host.RunAsync();