using Backend.API.Features.Users.Models;
using Bogus;

namespace Backend.API.Infrastructure.Data
{
    public interface IUserStore
    {
        IReadOnlyList<User> Users { get; }
    }

    public sealed class FakeUserSeeder : IUserStore
    {
        public IReadOnlyList<User> Users { get; }

        public FakeUserSeeder(int count = 1000)
        {
            Randomizer.Seed = new Random(12345);

            var faker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.BirthDate, f => f.Date.BetweenDateOnly(new DateOnly(1965, 1, 1), new DateOnly(2007, 12, 31)))
                .RuleFor(u => u.Country, f => f.Address.Country());

            Users = faker.Generate(count);
        }
    }
}
