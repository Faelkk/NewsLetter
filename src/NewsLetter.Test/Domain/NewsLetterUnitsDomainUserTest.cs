using Newsletter.Domain.Entities;

namespace NewsLetter.Test.Domain;

public class NewsLetterUnitsDomainUserTest
{
    [Fact]
    public void Should_Create_User_With_Valid_Data()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Rafael",
            Email = "Rafael@gmail.com",
            Password = "123456", Interests = ["Tecnologia", "Desenhos", "Filmes,Séries", "Noticias"],
            Role = "User"
        };

        Assert.NotNull(user);
        Assert.Equal("Rafael", user.Name);
        Assert.Equal("Rafael@gmail.com", user.Email);
        Assert.Equal("123456", user.Password);
        Assert.Equal("User", user.Role);
        Assert.NotEmpty(user.Interests);
        Assert.Equal(4, user.Interests.Length);
        Assert.Contains("Tecnologia", user.Interests);
    }   
}