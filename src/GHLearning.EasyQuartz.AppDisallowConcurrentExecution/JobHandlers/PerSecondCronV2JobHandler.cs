using Quartz;

namespace GHLearning.EasyQuartz.AppDisallowConcurrentExecution.JobHandlers;

[DisallowConcurrentExecution]
public class PerSecondCronV2JobHandler(
	ILogger<PerSecondCronV2JobHandler> logger,
	TimeProvider timeProvider) : IJob
{
	public Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation("由Quartz排程發送，時間:{dateAt}", timeProvider.GetUtcNow());
		return Task.Delay(TimeSpan.FromSeconds(5));
	}
}
