using Core;
using Core.Repositories;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ProjectRepository : GenericRepository<Project>,IProjectRepository
    {
        private readonly ICategoryRepository _categoryRepository;
        public ProjectRepository(AppDbContext context,ICategoryRepository categoryRepository) : base(context)
        {
            _categoryRepository = categoryRepository;
        }

        
        public async Task<List<Project>> GetProjectsWithCategoryAsync()
        {

            // Entity framework based
            // return await _context.Project.Include(x => x.Category).ToListAsync();


            // Linq  based
            return  await (from project in GetAll() 
                           join category in _categoryRepository.GetAll() on project.CategoryId equals category.Id
                           select new Project { 
                                Id = project.Id,
                                Name = project.Name,
                                PublishDate = project.PublishDate,
                                CategoryId = project.CategoryId,
                                Category = category
                            }
                          ).ToListAsync();
        }

    }
}