namespace Newsletter.Infrastructure.Settings;

public class EmailSettings
{
    public string SmtpServer { get; set; }
    public string From { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
}
