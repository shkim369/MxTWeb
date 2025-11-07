using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "X-Api-Key"; // 클라이언트가 보낼 헤더 이름

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 1. 설정에서 서버에 저장된 실제 API 키를 가져옵니다.
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var serverApiKey = configuration.GetValue<string>("ApiKey"); // appsettings.json의 "ApiKey"

        // 2. 클라이언트 요청의 헤더에서 API 키를 가져옵니다.
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var clientApiKey))
        {
            // 헤더에 키가 아예 없는 경우
            context.Result = new UnauthorizedObjectResult("API Key missing");
            return;
        }

        // 3. 두 키를 비교합니다.
        if (!serverApiKey.Equals(clientApiKey))
        {
            // 키가 일치하지 않는 경우
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
            return;
        }

        // 4. 키가 유효하면 컨트롤러의 원래 액션을 실행합니다.
        await next();
    }
}