var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Web_API>("web-api");

builder.Build().Run();