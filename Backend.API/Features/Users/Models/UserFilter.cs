namespace Backend.API.Features.Users.Models
{
    public sealed class UserFilter
    {
        public string? Search { get; init; }   // name/email contains
        public string? Country { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public string? Sort { get; init; } = "name"; // name|age|country
        public string? Order { get; init; } = "asc"; // asc|desc
    }
}
