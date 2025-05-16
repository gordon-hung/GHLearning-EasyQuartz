using GHLearning.EasyQuartz.AppOpenTelemetry.JobHandlers;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton(TimeProvider.System);

// Learn more about configuring OpenAPI at https://www.quartz-scheduler.net/documentation/quartz-3.x/quick-start.html
builder.Services.AddQuartz(q =>
{
	q.UseSimpleTypeLoader();
	q.UseInMemoryStore();

	//建立 job
	var jobKey = new JobKey(nameof(PerSecondCronJobHandler));
	q.AddJob<PerSecondCronJobHandler>(jobKey);
	//建立 trigger(規則) 來觸發 job
	q.AddTrigger(t => t
		.WithIdentity(nameof(PerSecondCronJobHandler))
		.ForJob(jobKey)
		.StartNow()
		.WithCronSchedule("*/1 * * * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")))
	);
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

//Learn more about configuring OpenTelemetry at https://learn.microsoft.com/zh-tw/dotnet/core/diagnostics/observability-with-otel
builder.Services.AddOpenTelemetry()
	.ConfigureResource(resource => resource
	.AddService("EasyQuartz-AppOpenTelemetry"))
	.UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri("http://127.0.0.1:4317"))
	.WithMetrics(metrics => metrics
		.AddMeter("GHLearning.")
		.AddAspNetCoreInstrumentation()
		.AddRuntimeInstrumentation()
		.AddProcessInstrumentation()
		.AddPrometheusExporter())
	.WithTracing(tracing => tracing
		.AddQuartzInstrumentation()
		.AddEntityFrameworkCoreInstrumentation()
		.AddHttpClientInstrumentation()
		.AddAspNetCoreInstrumentation(options => options.Filter = (httpContext) => !httpContext.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/live", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/healthz", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/metrics", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/favicon.ico", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.Value!.Equals("/api/events/raw", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.Value!.EndsWith(".js", StringComparison.OrdinalIgnoreCase) &&
				!httpContext.Request.Path.StartsWithSegments("/_vs", StringComparison.OrdinalIgnoreCase)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
