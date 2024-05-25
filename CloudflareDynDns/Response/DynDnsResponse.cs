using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace CloudflareDynDns.Response;

public class DynDnsResponse
{
    public DynDnsResponse(string status, string message)
    {
        Status = status;
        Message = message;
    }

    public DynDnsResponse(IEnumerable<string> Errors)
    {
        Status = "error";
        Message = string.Join(", ", Errors);
    }

    public string Status { get; set; }

    public string Message { get; set; }

    [JsonIgnore]
    public IActionResult Result => new JsonResult(this);

    [JsonIgnore]
    public static DynDnsResponse SuccessResponseObject = new("success", "DNS Entry updated");
}



