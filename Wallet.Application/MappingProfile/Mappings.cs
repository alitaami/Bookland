using AutoMapper;
using Entities.Base;

namespace Application.MappingProfiles
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<ServiceResult, int>().ReverseMap(); 
        }
    }
}
