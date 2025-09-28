using FluentValidation;

namespace Backend.API.Features.Seafile.Models;

public sealed class UpdateSeafileTrackValidator : AbstractValidator<UpdateSeafileTrackRequest>
{
    public UpdateSeafileTrackValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Src)
            .NotEmpty()
            .WithMessage("Source URL is required")
            .Must(BeValidUrl)
            .WithMessage("Source must be a valid URL");

        RuleFor(x => x.Folder)
            .NotEmpty()
            .WithMessage("Folder is required")
            .MaximumLength(100)
            .WithMessage("Folder cannot exceed 100 characters");
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) && 
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}

public sealed record class UpdateSeafileTrackRequest(string Title, string Src, string Folder);
