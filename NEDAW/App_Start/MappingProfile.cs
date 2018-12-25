using AutoMapper;
using NEDAW.Models;
using NEDAW.Dtos;

namespace NEDAW.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<source, destination>();
            Mapper.CreateMap<NewsCategory, NewsCategoryDtoForUpdate>();
            Mapper.CreateMap<News, NewsDtoForUpdate>();
        }
    }
}