using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AngularCalculator.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NumberController : ControllerBase
{
    [HttpPost]
    public Calculator Post(string number)
    {
        return number switch
        {
            "+" or "-" or "*" or "/" or "%" => new Calculator(9999, $" {number} "),

            _ => new Calculator(9999, number),
        };
    }
}
