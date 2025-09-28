using FluentValidation;

namespace Backend.API.Features.Seafile.Models;

public sealed class SeafileTrackFilterValidator : AbstractValidator<SeafileTrackFilter>
{
    public SeafileTrackFilterValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.CreatedFrom)
            .LessThan(x => x.CreatedTo)
            .When(x => x.CreatedFrom.HasValue && x.CreatedTo.HasValue)
            .WithMessage("CreatedFrom must be less than CreatedTo");
    }
}
