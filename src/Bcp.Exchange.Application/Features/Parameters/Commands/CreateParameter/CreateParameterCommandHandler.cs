using Bcp.Exchange.Application.Features.Parameters.DTOs;
using Bcp.Exchange.Core.Configuration;
using Bcp.Exchange.Core.Configuration.Entities;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using Mapster;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Commands.CreateParameter;

public sealed class CreateParameterCommandHandler
    : IRequestHandler<CreateParameterCommand, Result<ParameterDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateParameterCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ParameterDto>> Handle(
        CreateParameterCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return Result.Failure<ParameterDto>(ParameterErrors.InvalidCode(request.Code));
        }

        var existingParameter = await _unitOfWork.Parameters.GetByCodeIncludingInactiveAsync(
            request.Code,
            cancellationToken
        );

        if (existingParameter is not null)
        {
            if (existingParameter.IsActive)
            {
                return Result.Failure<ParameterDto>(ParameterErrors.DuplicateCode(request.Code));
            }

            if (request.ParentId.HasValue)
            {
                var parent = await _unitOfWork.Parameters.GetByIdAsync(
                    request.ParentId.Value,
                    cancellationToken
                );
                if (parent is null)
                {
                    return Result.Failure<ParameterDto>(
                        ParameterErrors.ParentNotFound(request.ParentId.Value)
                    );
                }
            }

            existingParameter.Reactivate(
                request.Description,
                request.LongDescription,
                request.ParentId,
                request.DisplayOrder,
                request.NumericValue,
                request.TextValue,
                request.CreatedBy
            );

            _unitOfWork.Parameters.Update(existingParameter);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(existingParameter.Adapt<ParameterDto>());
        }

        if (request.ParentId.HasValue)
        {
            var parent = await _unitOfWork.Parameters.GetByIdAsync(
                request.ParentId.Value,
                cancellationToken
            );
            if (parent is null)
            {
                return Result.Failure<ParameterDto>(
                    ParameterErrors.ParentNotFound(request.ParentId.Value)
                );
            }
        }

        var parameter = Parameter.Create(
            request.Code,
            request.Description,
            request.LongDescription,
            request.ParentId,
            request.DisplayOrder,
            request.NumericValue,
            request.TextValue,
            request.CreatedBy
        );

        await _unitOfWork.Parameters.AddAsync(parameter, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(parameter.Adapt<ParameterDto>());
    }
}
