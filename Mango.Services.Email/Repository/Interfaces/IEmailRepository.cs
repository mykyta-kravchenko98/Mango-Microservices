using Mango.Services.Email.Messages;

namespace Mango.Services.Email.Repository.Interfaces;

public interface IEmailRepository
{
    Task SendAndLogEmail(UpdatePaymentResultMessage message);
}