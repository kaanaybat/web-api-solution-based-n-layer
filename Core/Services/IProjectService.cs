using Core.Dtos;

namespace Core.Services
{
    public interface IProjectService:IService<Project>
    {
        public Task<List<ProjectWithCategoryDto>> GetProjectsWithCategoryAsync();
    }
}