using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Application.Groceries.Queries.GetGroceryByCode
{
    public class GetGroceryByCodeQueryValidator : AbstractValidator<GetGroceryByCodeQuery>
    {
        public GetGroceryByCodeQueryValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Grocery code must not be empty.")
                .MaximumLength(100).WithMessage("Grocery code must not exceed 100 characters.");
        }
    }
}
