using Bcp.Exchange.Application.Features.Parameters.Commands.CreateParameter;
using Bcp.Exchange.Application.Features.Parameters.Commands.DeleteParameter;
using Bcp.Exchange.Application.Features.Parameters.Commands.UpdateParameter;
using Bcp.Exchange.Application.Features.Parameters.Queries.GetAllParameters;
using Bcp.Exchange.Application.Features.Parameters.Queries.GetParametersByParentCode;
using Bcp.Exchange.FunctionApp.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Bcp.Exchange.FunctionApp.Endpoints;

public class ParameterEndpoints(IMediator mediator)
{
    [Function("CreateParameter")]
    [RequireAdmin]
    public async Task<IActionResult> CreateParameter(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "parameters")] HttpRequest req
    )
    {
        var command = await req.ReadFromJsonAsync<CreateParameterCommand>();
        if (command is null)
            return new BadRequestResult();

        var result = await mediator.Send(command);
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(result.Error);
    }

    [Function("UpdateParameter")]
    [RequireAdmin]
    public async Task<IActionResult> UpdateParameter(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "parameters/{id}")]
            HttpRequest req,
        Guid id
    )
    {
        var command = await req.ReadFromJsonAsync<UpdateParameterCommand>();
        if (command is null)
            return new BadRequestResult();

        command.ParameterId = id;
        var result = await mediator.Send(command);
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(result.Error);
    }

    [Function("DeleteParameter")]
    [RequireAdmin]
    public async Task<IActionResult> DeleteParameter(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "parameters/{id}")]
            HttpRequest req,
        Guid id
    )
    {
        var command = new DeleteParameterCommand
        {
            ParameterId = id,
            ModifiedBy = req.Query["modifiedBy"],
        };
        var result = await mediator.Send(command);
        return result.IsSuccess ? new NoContentResult() : new BadRequestObjectResult(result.Error);
    }

    [Function("GetAllParameters")]
    public async Task<IActionResult> GetAllParameters(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "parameters")] HttpRequest req
    )
    {
        var query = new GetAllParametersQuery();
        var result = await mediator.Send(query);
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(result.Error);
    }

    [Function("GetParametersByParentCode")]
    public async Task<IActionResult> GetParametersByParentCode(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "parameters/by-parent")]
            HttpRequest req
    )
    {
        var parentCode = req.Query["parentCode"].ToString();
        if (string.IsNullOrWhiteSpace(parentCode))
        {
            return new BadRequestObjectResult("parentCode query parameter is required");
        }

        var query = new GetParametersByParentCodeQuery { ParentCode = parentCode };
        var result = await mediator.Send(query);
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(result.Error);
    }
}
