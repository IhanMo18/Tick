using FluentValidation;
using MediatR;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Application.Abstractions;

public sealed class ValidationBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest,TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any()) return await next(ct);
        
        var context = new ValidationContext<TRequest>(request);
        var results  = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct)));
        var failures = results.SelectMany(r => r.Errors).Where(e => e is not null).ToList();

        if (failures.Count > 0)
            throw new FluentValidation.ValidationException(failures);
        return await next(ct);
    }
}