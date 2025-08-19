using FluentValidation;

namespace Backend.API.Features.Users.Models
{
    public sealed class UserFilterValidator : AbstractValidator<UserFilter>
    {
        public UserFilterValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
            RuleFor(x => x.Order).Must(o => o is "asc" or "desc");
            RuleFor(x => x.Sort).Must(s => s is "name" or "age" or "country");
        }
    }
}
