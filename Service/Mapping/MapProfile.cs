using AutoMapper;
using Core;
using Core.Dtos;

namespace Service.Mapping
{
    public class MapProfile:Profile
    {

        public MapProfile()
        {
            CreateMap<Project,ProjectDto>().ReverseMap();
            CreateMap<Project,ProjectCreateDto>().ReverseMap();
            CreateMap<Project,ProjectUpdateDto>().ReverseMap();
            CreateMap<Project,ProjectWithCategoryDto>().ReverseMap();
            CreateMap<Category,CategoryDto>().ReverseMap();
        }
        
    }

}