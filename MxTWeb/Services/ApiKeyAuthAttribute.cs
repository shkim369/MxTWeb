using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key"; // 클라이언트가 보낼 헤더 이름

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiKeyAuthAttribute>>();

        var serverApiKey = configuration.GetValue<string>("ApiKey");

        // 서버측 키가 비어 있으면 모든 요청을 거부 (운영 사고 방지)
        if (string.IsNullOrEmpty(serverApiKey))
        {
            logger.LogError("API Key가 서버 구성에 설정되어 있지 않습니다. 모든 요청을 거부합니다.");
            context.Result = new UnauthorizedObjectResult("API Key not configured on server");
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var clientApiKey) ||
            string.IsNullOrEmpty(clientApiKey))
        {
            context.Result = new UnauthorizedObjectResult("API Key missing");
            return;
        }

        // 타이밍 공격을 막기 위해 고정 시간 비교(FixedTimeEquals) 사용
        var serverBytes = Encoding.UTF8.GetBytes(serverApiKey);
        var clientBytes = Encoding.UTF8.GetBytes(clientApiKey.ToString());

        if (serverBytes.Length != clientBytes.Length ||
            !CryptographicOperations.FixedTimeEquals(serverBytes, clientBytes))
        {
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
            return;
        }

        await next();
    }
}
