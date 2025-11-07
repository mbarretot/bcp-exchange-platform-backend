using Bcp.Exchange.Core.Configuration;
using Bcp.Exchange.Core.Shared;
using Bcp.Exchange.Core.Shared.Interfaces;
using MediatR;

namespace Bcp.Exchange.Application.Features.Parameters.Commands.DeleteParameter;

public sealed class DeleteParameterCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteParameterCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(
        DeleteParameterCommand request,
        CancellationToken cancellationToken
    )
    {
        var parameter = await _unitOfWork.Parameters.GetByIdAsync(
            request.ParameterId,
            cancellationToken
        );

        if (parameter is null)
        {
            return Result.Failure(ParameterErrors.NotFoundById(request.ParameterId));
        }

        if (parameter.ParentId is null)
        {
            var activeChildren = await _unitOfWork.Parameters.GetActiveChildrenByParentIdAsync(
                request.ParameterId,
                cancellationToken
            );

            if (activeChildren.Any())
            {
                return Result.Failure(ParameterErrors.CannotDeleteParent(parameter.Code));
            }
        }

        parameter.Delete(request.ModifiedBy);

        _unitOfWork.Parameters.Update(parameter);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
