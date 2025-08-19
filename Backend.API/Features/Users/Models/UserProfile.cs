using AutoMapper;

namespace Backend.API.Features.Users.Models
{
    public sealed class UserProfile : Profile
    {
        private static int CalculateAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age;
        }
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.FullName,
                    m => m.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(d => d.Age,
                    m => m.MapFrom(s => CalculateAge(s.BirthDate)));
        }
    }
}
