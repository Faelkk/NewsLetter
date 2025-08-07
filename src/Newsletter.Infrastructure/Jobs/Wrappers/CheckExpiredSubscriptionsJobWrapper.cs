using Newsletter.Application.Jobs.Interfaces;

namespace Newsletter.Presentation.Jobs.Wrappers;

public class CheckExpiredSubscriptionsJobWrapper
{
    private readonly ICheckExpiredSubscriptionJob _job;

    public CheckExpiredSubscriptionsJobWrapper(ICheckExpiredSubscriptionJob job)
    {
        _job = job;
    }

    public async Task Execute()
    {
        await _job.Execute();
    }
}