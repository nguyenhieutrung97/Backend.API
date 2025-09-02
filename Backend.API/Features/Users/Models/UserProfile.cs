using AutoMapper;

namespace Backend.API.Features.Users.Models
{
    public sealed class UserProfile : Profile
    {
        private static int CalculateAge(DateOnly birthDate)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            int age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age;
        }
    }
}
