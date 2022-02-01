using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(p => p.Id)
             .NotEmpty().WithMessage("{Id} is required.")
             .NotNull().WithMessage("{Id} cannot be null.");
        }
    }
}
