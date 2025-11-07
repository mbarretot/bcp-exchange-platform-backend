using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Bcp.Exchange.FunctionApp.Middleware;

public class CorsMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var requestData = await context.GetHttpRequestDataAsync();

        if (requestData != null)
        {
            if (requestData.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                var preflightResponse = requestData.CreateResponse();
                await AddCorsHeaders(preflightResponse);
                preflightResponse.StatusCode = System.Net.HttpStatusCode.OK;

                var invocationResult = context.GetInvocationResult();
                var httpOutputBindingFromMultipleOutputBindings =
                    GetHttpOutputBindingFromMultipleOutputBinding(context);
                if (httpOutputBindingFromMultipleOutputBindings is not null)
                {
                    httpOutputBindingFromMultipleOutputBindings.Value = preflightResponse;
                }
                else
                {
                    invocationResult.Value = preflightResponse;
                }

                return;
            }
        }

        await next(context);

        var httpReqData = await context.GetHttpRequestDataAsync();
        if (httpReqData != null)
        {
            var invocationResult = context.GetInvocationResult();
            var httpOutputBindingFromMultipleOutputBindings =
                GetHttpOutputBindingFromMultipleOutputBinding(context);

            HttpResponseData? response = null;
            if (httpOutputBindingFromMultipleOutputBindings is not null)
            {
                response = httpOutputBindingFromMultipleOutputBindings.Value as HttpResponseData;
            }
            else
            {
                response = invocationResult.Value as HttpResponseData;
            }

            if (response != null)
            {
                await AddCorsHeaders(response);
            }
        }
    }

    private static async Task AddCorsHeaders(HttpResponseData response)
    {
        response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add(
            "Access-Control-Allow-Headers",
            "Content-Type, Authorization, x-functions-key"
        );
        response.Headers.Add("Access-Control-Allow-Credentials", "true");
        await Task.CompletedTask;
    }

    private static OutputBindingData<HttpResponseData>? GetHttpOutputBindingFromMultipleOutputBinding(
        FunctionContext context
    )
    {
        var outputBindings = context.GetOutputBindings<HttpResponseData>();
        var httpOutputBinding = outputBindings.FirstOrDefault(b =>
            b.BindingType == "http" && b.Name != "$return"
        );
        return httpOutputBinding;
    }
}
