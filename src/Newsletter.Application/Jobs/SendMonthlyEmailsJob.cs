using Newsletter.Application.Interfaces;
using Newsletter.Application.Jobs.Interfaces;
using Newsletter.Domain.Interfaces;


namespace Newsletter.Application.Jobs;

public class SendMonthlyEmailsJob : ISendMonthlyEmailsJob
{
    private readonly ISubscriptionRepository   _subscriptionRepository;
    private readonly IEmailService _emailService;
    private readonly IGenerateNewsLetters  _generateNewsLetters;

    public SendMonthlyEmailsJob(ISubscriptionRepository subscriptionRepository, IEmailService emailService,IGenerateNewsLetters generateNewsLetters)
    {
        _subscriptionRepository = subscriptionRepository;
        _emailService = emailService;
        _generateNewsLetters = generateNewsLetters;
    }

    public async Task Execute()
    {
        var today = DateTime.UtcNow.Date;
            
        var subscriptions = await _subscriptionRepository.GetActiveSubscriptionsWithUserEmail(today);
        
        var random = new Random();

        foreach (var sub in subscriptions)
        {
            
            var selectedInterest = sub.Interests[random.Next(sub.Interests.Length)];
            
             var newLetter = await _generateNewsLetters.GenerateNewsLetterAndSave(
                sub.UserId,
         selectedInterest 
            );
             
             await _emailService.SendMonthlyEmail(sub.UserEmail, newLetter);
            
             sub.NextDeliveryDate = DateTime.UtcNow.Date.AddMonths(1);
             await _subscriptionRepository.UpdateNextDeliveryDateAsync(sub.SubscriptionId, sub.NextDeliveryDate);
        }
    }
} 
 
