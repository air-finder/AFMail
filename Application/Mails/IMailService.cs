using Domain.Common;
using Domain.Mails.Requests;

namespace Application.Mails;

public interface IMailService
{
    Task<BaseResponse<object>> SendEmailAsync(MailRequest request);
}