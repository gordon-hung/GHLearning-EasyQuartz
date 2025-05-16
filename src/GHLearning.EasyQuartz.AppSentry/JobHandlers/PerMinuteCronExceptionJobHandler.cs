using Quartz;

namespace GHLearning.EasyQuartz.AppSentry.JobHandlers;

public class PerMinuteCronExceptionJobHandler(
	ILogger<PerMinuteCronExceptionJobHandler> logger,
	TimeProvider timeProvider) : IJob
{
	public async Task Execute(IJobExecutionContext context)
	{
		var checkInId = SentrySdk.CaptureCheckIn("second-cron-exception", CheckInStatus.InProgress);
		try
		{
			logger.LogInformation("由Quartz排程發送，時間:{dateAt}，Nameof:{nameof}", timeProvider.GetUtcNow(), nameof(PerMinuteCronExceptionJobHandler));
			await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
			throw new Exception("exception");
		}
		catch
		{
			SentrySdk.CaptureCheckIn("second-cron-exception", CheckInStatus.Error, checkInId);
		}
	}
}
