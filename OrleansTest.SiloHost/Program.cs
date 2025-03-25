using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseAzureStorageClustering(configureOptions: options =>
        {
            options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
        });

        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "OrleansTestAppCluster";
            options.ServiceId = "OrleansTestAppService";
        });

        siloBuilder.AddAzureTableGrainStorage("accountStore", options =>
        {
            options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
        });

        siloBuilder.AddAzureTableGrainStorage("cameraStore", options =>
        {
            options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
        });

        siloBuilder.AddAzureQueueStreams("StreamProvider", optionsBuilder =>
        {
            optionsBuilder.Configure(options =>
            {
                options.QueueServiceClient = new Azure.Storage.Queues.QueueServiceClient("UseDevelopmentStorage=true;");
            });
        }).AddAzureTableGrainStorage("PubSubStore", configureOptions: options =>
        {
            options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
        });

        siloBuilder.UseAzureTableReminderService(options =>
        {
            options.TableServiceClient = new Azure.Data.Tables.TableServiceClient("UseDevelopmentStorage=true;");
        });

    }).RunConsoleAsync();