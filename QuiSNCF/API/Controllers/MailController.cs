using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Service;

namespace QuiSNCF.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MailController(MailService mail) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendEmail()
    {
        mail.SendMail();
        return Ok();
    }
}