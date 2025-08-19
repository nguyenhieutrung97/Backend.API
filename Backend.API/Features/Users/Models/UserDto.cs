namespace Backend.API.Features.Users.Models
{
    public sealed record class UserDto
    {
        public Guid Id { get; init; }
        public string FullName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Phone { get; init; } = default!;
        public int Age { get; init; }
        public string Country { get; init; } = default!;
    }
}
