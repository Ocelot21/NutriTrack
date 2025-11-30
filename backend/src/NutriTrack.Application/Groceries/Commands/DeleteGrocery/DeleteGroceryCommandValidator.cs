using FluentValidation;

namespace NutriTrack.Application.Groceries.Commands.DeleteGrocery;

public sealed class DeleteGroceryCommandValidator : AbstractValidator<DeleteGroceryCommand>
{
    public DeleteGroceryCommandValidator()
    {
        RuleFor(x => x.Id.Value).NotEmpty();
    }
}
