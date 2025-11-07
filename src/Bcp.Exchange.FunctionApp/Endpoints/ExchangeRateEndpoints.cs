using Bcp.Exchange.Application.Features.ExchangeRates.Commands.CreateExchangeRate;
using Bcp.Exchange.Application.Features.ExchangeRates.Commands.DeleteExchangeRate;
using Bcp.Exchange.Application.Features.ExchangeRates.Commands.UpdateExchangeRate;
using Bcp.Exchange.Application.Features.ExchangeRates.Queries.GetAllExchangeRates;
using Bcp.Exchange.Application.Features.ExchangeRates.Queries.GetExchangeRateById;
using Bcp.Exchange.FunctionApp.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Bcp.Exchange.FunctionApp.Endpoints;

public class ExchangeRateEndpoints(IMediator mediator)
{
    [Function("CreateExchangeRate")]
    [RequireAdmin]
    public async Task<IActionResult> CreateExchangeRate(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "exchange-rates")] HttpRequest req
    )
    {
        var command = await req.ReadFromJsonAsync<CreateExchangeRateCommand>();
        if (command is null)
            return new BadRequestResult();

        var result = await mediator.Send(command);
        return new OkObjectResult(result.Value);
    }

    [Function("UpdateExchangeRate")]
    [RequireAdmin]
    public async Task<IActionResult> UpdateExchangeRate(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "exchange-rates/{id}")]
            HttpRequest req,
        Guid id
    )
    {
        var command = await req.ReadFromJsonAsync<UpdateExchangeRateCommand>();
        if (command is null)
            return new BadRequestResult();

        command.ExchangeRateId = id;
        var result = await mediator.Send(command);
        return new OkObjectResult(result.Value);
    }

    [Function("DeleteExchangeRate")]
    [RequireAdmin]
    public async Task<IActionResult> DeleteExchangeRate(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "exchange-rates/{id}")]
            HttpRequest req,
        Guid id
    )
    {
        var command = new DeleteExchangeRateCommand
        {
            ExchangeRateId = id,
            ModifiedBy = req.Query["modifiedBy"],
        };
        await mediator.Send(command);
        return new NoContentResult();
    }

    [Function("GetAllExchangeRates")]
    public async Task<IActionResult> GetAllExchangeRates(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "exchange-rates")] HttpRequest req
    )
    {
        var query = new GetAllExchangeRatesQuery();
        var result = await mediator.Send(query);
        return new OkObjectResult(result.Value);
    }

    [Function("GetExchangeRateById")]
    public async Task<IActionResult> GetExchangeRateById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "exchange-rates/{id}")]
            HttpRequest req,
        Guid id
    )
    {
        var query = new GetExchangeRateByIdQuery { Id = id };
        var result = await mediator.Send(query);
        return new OkObjectResult(result.Value);
    }
}
