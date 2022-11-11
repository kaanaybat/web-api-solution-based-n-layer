using Core;
using Core.Repositories;
using Core.UnitOfWork;
using Core.Services;
using Core.Dtos;
using AutoMapper;

namespace Service.Services
{
    public class ProjectService:Service<Project>,IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        public ProjectService(
            IProjectRepository projectRepository,
            IMapper mapper,
            IGenericRepository<Project> genericRepository,
            IUnitOfWork uow):base(genericRepository,uow)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<List<ProjectWithCategoryDto>> GetProjectsWithCategoryAsync()
        {
            var projectsWithCategory = await _projectRepository.GetProjectsWithCategoryAsync();

            var projectsDto = _mapper.Map<List<ProjectWithCategoryDto>>(projectsWithCategory);

            return projectsDto;

        }

    }
}