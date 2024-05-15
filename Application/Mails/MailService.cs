using Domain.Common;
using Domain.Mails.Requests;
using Infra.Utils.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Mails;

public class MailService(IOptionsSnapshot<MailSettings> mailSettings) : IMailService
{
    private readonly MailSettings _mailSettings = mailSettings.Value;

    public async Task<BaseResponse<object>> SendEmailAsync(MailRequest request)
    {
        var mail = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Mail),
            Subject = request.Subject
        };
        mail.To.AddRange(request.ToMail.Select(MailboxAddress.Parse));
        var builder = new BodyBuilder();
        if(request.Attachments != null)
        {
            foreach (var file in request.Attachments.Where(file => file.Length > 0))
            {
                byte[] fileBytes;
                using(var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }
                builder.Attachments.Add(file.Name, fileBytes, ContentType.Parse(file.ContentType));
            }
        }
        builder.HtmlBody = request.Body;
        mail.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _mailSettings.Host,
            _mailSettings.Port,
            SecureSocketOptions.StartTls
        );
        await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(mail);
        await smtp.DisconnectAsync(true);
        return new GenericResponse<object>(null);
    }
}