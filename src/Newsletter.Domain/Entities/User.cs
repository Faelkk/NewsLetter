

    namespace Newsletter.Domain.Entities;

    public class User
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string Role { get; init; } 
        public required string[] Interests { get; init; }
    }

    
    
    
    
    