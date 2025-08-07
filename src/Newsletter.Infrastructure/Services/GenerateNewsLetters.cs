using Newsletter.Application.Interfaces;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newsletter.Infrastructure.Services
{
    public class GenerateNewsLetters : IGenerateNewsLetters
    {
        private readonly INewsletterRepository _newsletterRepository;
        private readonly GeminiService _geminiService;

        public GenerateNewsLetters(INewsletterRepository newsletterRepository, GeminiService geminiService)
        {
            _newsletterRepository = newsletterRepository;
            _geminiService = geminiService;
        }

        public async Task<string> GenerateNewsLetterAndSave(Guid userId, string interests)
        {
            var generatedContent = await _geminiService.GenerateNewsletterContent(interests);
            
            var newsLetter = new NewsletterEntry
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Topics = new[] { interests }, 
                Sent = true,
                Content = generatedContent
            };

  
            var createdNewsLetter = await _newsletterRepository.GenerateAndSendAsync(newsLetter);


            return generatedContent;
        }
    }
}