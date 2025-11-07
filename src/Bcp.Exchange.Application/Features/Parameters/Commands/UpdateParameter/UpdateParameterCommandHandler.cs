using Bcp.Exchange.Application.Features.Parameters.DTOs;
using Bcp.Exchange.Core.Configuration;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using Mapster;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Commands.UpdateParameter;

public sealed class UpdateParameterCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateParameterCommand, Result<ParameterDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ParameterDto>> Handle(
        UpdateParameterCommand request,
        CancellationToken cancellationToken
    )
    {
        var parameter = await _unitOfWork.Parameters.GetByIdAsync(
            request.ParameterId,
            cancellationToken
        );

        if (parameter is null)
        {
            return Result.Failure<ParameterDto>(ParameterErrors.NotFoundById(request.ParameterId));
        }

        parameter.Update(
            request.Description,
            request.LongDescription,
            request.DisplayOrder,
            request.NumericValue,
            request.TextValue,
            request.ModifiedBy
        );

        _unitOfWork.Parameters.Update(parameter);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(parameter.Adapt<ParameterDto>());
    }
}
