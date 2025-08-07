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
        

        Console.WriteLine($"{today} entrou aq");

        var subscriptions = await _subscriptionRepository.GetActiveSubscriptionsWithUserEmail(today);

        Console.WriteLine($"{subscriptions} teste subs");

        var random = new Random();

        foreach (var sub in subscriptions)
        {
            Console.WriteLine($"{sub} dentro do sub");
            
            var selectedInterest = sub.Interests[random.Next(sub.Interests.Length)];

        
            Console.WriteLine($"{selectedInterest} selectedInterest");
            
             var newLetter = await _generateNewsLetters.GenerateNewsLetterAndSave(
                sub.UserId,
         selectedInterest 
            );
          
             Console.WriteLine($"{newLetter} new letter");
            
             Console.WriteLine($"{sub.UserEmail} email");
             
             await _emailService.SendMonthlyEmail(sub.UserEmail, newLetter);
            
             Console.WriteLine($"{newLetter} enviou");
            
             sub.NextDeliveryDate = (sub.NextDeliveryDate ?? DateTime.UtcNow).AddMonths(1);
             await _subscriptionRepository.UpdateNextDeliveryDateAsync(sub.SubscriptionId, sub.NextDeliveryDate);
        }
    }
} 