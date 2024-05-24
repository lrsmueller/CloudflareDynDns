using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace CloudflareDynDns;

public class ResponseObject
{
    public ResponseObject(string status, string message)
    {
        Status = status;
        Message = message;
    }

    public ResponseObject(IEnumerable<string> Errors)
    {
        Status = "error";
        Message = string.Join(", ", Errors);
    }

    public string Status { get; set; }

    public string Message { get; set; }

    public OkObjectResult Result => new OkObjectResult(JsonSerializer.Serialize(this));
    
    public static ResponseObject SuccessResponseObject = new("success", "DNS Entry updated");
}



