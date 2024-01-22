using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SimpleApiWithData.Data;
using SimpleApiWithData.Data.Entities;

namespace SimpleApiWithData.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    private readonly LocalContext _context;

    public ProductValidator(LocalContext context)
    {
        _context = context;

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(p => p.Category)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .MaximumLength(20).WithMessage("{PropertyName} must not exceed 20 characters.");

        RuleFor(e => e.Name)
            .MustAsync(NameIsUnique)
            .WithMessage("A product with the same name already exists.");
    }

    private async Task<bool> NameIsUnique(string? productName, CancellationToken token)
    {
        return !await _context.Products.AnyAsync(p => p.Name == productName, token);
    }
}
