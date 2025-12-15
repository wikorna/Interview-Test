using System.Security.Cryptography;
using System.Text;
namespace Interview_Test.Middlewares;

public class AuthenMiddleware : IMiddleware
{
    // SHA512 ที่ hash แล้วของ x-api-key 
    private const string hashedKey = "BC92C479E1FE12E40747F056B8E14FBC36C64A4D3A4611E174038F3251FF0602F5B3325F528B87AC2655484272BC8A6E52FC07486353A902441BDE3D21CDF0B6";
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Console.WriteLine($"[Auth] IN  {context.Request.Method} {context.Request.Path}");
        // allow CORS preflight
        if (HttpMethods.Options.Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }
        //var apiKeyHeader = context.Request.Headers["x-api-key"];
        
        if (!context.Request.Headers.TryGetValue("x-api-key", out var apiKeyHeader) || 
            string.IsNullOrWhiteSpace(apiKeyHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }
        var inputBytes = Encoding.UTF8.GetBytes(apiKeyHeader!);
        var hashBytes = SHA512.HashData(inputBytes);
        var computedHashHex = Convert.ToHexString(hashBytes);

        if (!FixedTimeEquals(computedHashHex, hashedKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
        // Implement validate x-api-key to authenticate the user here
        //throw new NotImplementedException();
        await next(context);
        Console.WriteLine($"[Auth] OUT {context.Response.StatusCode} {context.Request.Path}");
    }
    private static bool FixedTimeEquals(string a, string b)
    {
        // ป้องกัน timing attack
        var aBytes = Encoding.UTF8.GetBytes(a);
        var bBytes = Encoding.UTF8.GetBytes(b);

        // ถ้ายาวไม่เท่ากันให้รีบ return false แต่อย่าลืมให้เทียบด้วย
        if (aBytes.Length != bBytes.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }
}