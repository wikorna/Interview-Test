using System.Security.Cryptography;
using System.Text;
namespace Interview_Test.Middlewares;

public class AuthenMiddleware : IMiddleware
{
    // SHA512 ที่ hash แล้วของ x-api-key 
    private const string hashedKey = "7DAF16EB5088006C9D55BE83CBE46586D7C7A5D68341683B6939389E2164C86227424ADD89C43F4101A41DF1D8CD4CC05BA99783C7199EDF6AD00C755BEF233C";
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