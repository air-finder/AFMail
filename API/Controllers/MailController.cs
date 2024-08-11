using Application.Mails;
using Domain.Mails.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MailController(IMailService mailService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> SendEmailAsync([FromBody] MailRequest request)
    {
        return Ok(await mailService.SendEmailAsync(request));
    }
}