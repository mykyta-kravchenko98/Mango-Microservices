namespace Mango.Services.Email.Models;

public class EmailLog
{
    public long Id { get; set; }
    public string Email { get; set; }
    public string Log { get; set; }
    public DateTime EmailSent { get; set; }
}