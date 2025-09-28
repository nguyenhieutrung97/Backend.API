using AutoMapper;

namespace Backend.API.Features.Seafile.Models;

public sealed class SeafileProfile : Profile
{
    public SeafileProfile()
    {
        CreateMap<SeafileTrack, SeafileTrackDto>();
    }
}
