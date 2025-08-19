namespace Backend.API.Features.Users.Models
{

    public sealed record class User
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Phone { get; init; } = default!;
        public DateOnly BirthDate { get; init; }
        public string Country { get; init; } = default!;
    }
}
