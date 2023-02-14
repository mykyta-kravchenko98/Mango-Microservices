using Mango.Services.Email.DbContexts;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Models;
using Mango.Services.Email.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.Email.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContext;

    public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
    {
        //TODO implement an email sender
        EmailLog emailLog = new()
        {
            Email = message.Email,
            EmailSent = DateTime.Now,
            Log = $"Order - {message.OrderId} has been created successfully."
        };

        await using var db = new ApplicationDbContext(_dbContext);

        db.EmailLogs.Add(emailLog);
        await db.SaveChangesAsync();
    }
}