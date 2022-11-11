using Core.Repositories;

namespace Core.Repositories
{
    public interface IProjectRepository:IGenericRepository<Project>
    {
         Task<List<Project>> GetProjectsWithCategoryAsync();
    }
}