using Microsoft.AspNetCore.Http;

namespace Domain.Mails.Requests;

public class MailRequest(IEnumerable<string> toMail, string subject, string body, IEnumerable<IFormFile>? attachments)
{
    public IEnumerable<string> ToMail { get; set; } = toMail;
    public string Subject { get; set; } = subject;
    public string Body { get; set; } = body;
    public IEnumerable<IFormFile>?  Attachments { get; set; } = attachments;
}