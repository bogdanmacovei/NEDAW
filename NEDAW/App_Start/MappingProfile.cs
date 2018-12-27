using AutoMapper;
using NEDAW.Models;
using NEDAW.Dtos;
using NEDAW.ViewModels;

namespace NEDAW.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<source, destination>();
            Mapper.CreateMap<NewsCategory, NewsCategoryDtoForUpdate>();
            Mapper.CreateMap<News, NewsDtoForUpdate>();
            Mapper.CreateMap<NewsForm, News>();
        }
    }
}