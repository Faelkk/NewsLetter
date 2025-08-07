namespace Newsletter.Application.Jobs.Interfaces;

public interface ICheckExpiredSubscriptionJob
{
    Task Execute();
}