using ErrorOr;
using MediatR;
using NutriTrack.Domain.Common.Errors;

namespace NutriTrack.Application.Common.Behaviors;

public sealed class DomainExceptionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
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
            var responseType = typeof(TResponse);

            if (responseType.IsGenericType &&
                responseType.GetGenericTypeDefinition() == typeof(ErrorOr<>))
            {
                var error = Error.Failure(ex.Code ?? "General.Failure", ex.Message);

                // ErrorOr<T>.From(params Error[] errors)
                var fromMethod = responseType.GetMethod(
                    nameof(ErrorOr<>.From),
                    [typeof(IEnumerable<Error>)]);

                if (fromMethod is not null)
                {
                    var result = fromMethod.Invoke(null, [new[] { error }]);
                    return (TResponse)result!;
                }
            }

            throw;
        }
    }
}