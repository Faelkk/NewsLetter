using Newsletter.Application.Jobs.Interfaces;

namespace Newsletter.Presentation.Jobs.Wrappers;

public class SendMonthlyEmailsJobWrapper
{
    private readonly ISendMonthlyEmailsJob _job;

    public SendMonthlyEmailsJobWrapper(ISendMonthlyEmailsJob job)
    {
        _job = job;
    }

    public Task Execute()
    {
        return _job.Execute();
    }
}