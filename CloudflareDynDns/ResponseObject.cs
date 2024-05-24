using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
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
	
    [JsonIgnore]
	public IActionResult Result => new JsonResult(this);

	[JsonIgnore]
	public static ResponseObject SuccessResponseObject = new("success", "DNS Entry updated");
}



