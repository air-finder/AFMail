using Application.Mails;
using Domain.Mails.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class MailController(IMailService mailService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> SendEmailAsync([FromBody] MailRequest request)
    {
        return Ok(await mailService.SendEmailAsync(request));
    }
}