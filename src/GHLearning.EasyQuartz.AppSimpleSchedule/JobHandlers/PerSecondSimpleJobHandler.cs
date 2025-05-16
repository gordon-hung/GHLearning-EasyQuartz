using Quartz;

namespace GHLearning.EasyQuartz.AppSimpleSchedule.JobHandlers;

public class PerSecondSimpleJobHandler(
	ILogger<PerSecondSimpleJobHandler> logger,
	TimeProvider timeProvider) : IJob
{
	public Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation("由Quartz排程發送，時間:{dateAt}", timeProvider.GetUtcNow());
		return Task.Delay(TimeSpan.FromSeconds(5));
	}
}
