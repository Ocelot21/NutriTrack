using ErrorOr;
using MediatR;
using NutriTrack.Domain.Common.Errors;

namespace NutriTrack.Application.Common.Behaviors;

public sealed class DomainExceptionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainException ex)
        {
            var error = Error.Validation(
                ex.Code ?? "Domain.Validation",
                ex.Message);

            var errors = new List<Error> { error };

            return (dynamic)errors;
        }
    }
}
