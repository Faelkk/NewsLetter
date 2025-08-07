namespace Newsletter.Application.Jobs.Interfaces;

public interface ISendMonthlyEmailsJob
{
    Task Execute();
}