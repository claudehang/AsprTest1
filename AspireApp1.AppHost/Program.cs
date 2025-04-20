var builder = DistributedApplication.CreateBuilder(args);

// 添加PostgreSQL服务
var postgres = builder.AddPostgres("postgres")
    .AddDatabase("reservationdb");

var apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice")
    .WithHttpsHealthCheck("/health");

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpsHealthCheck("/health")
    .WithReference(apiService)
    .WithReference(postgres)
    .WaitFor(apiService);

builder.Build().Run();
