
namespace NewsLetter.Domain.Entities;
public class User
{
      public Guid Id { get; set; }
      public required string Name { get; set; } 
      public required string Email { get; set; }

      public string Plan { get; set; } = "free";
      public list<string> Interests  { get; set; }
}