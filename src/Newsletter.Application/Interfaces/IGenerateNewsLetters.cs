
using Newsletter.Domain.Entities;

namespace Newsletter.Application.Interfaces;

public interface IGenerateNewsLetters
{
    Task<string> GenerateNewsLetterAndSave(Guid userId, string interests);
}
