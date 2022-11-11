using System.Linq.Expressions;
using AutoMapper;
using Core;
using Core.Dtos;
using Core.Dtos.Contracts;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Service.Exceptions;

namespace Caching;

public class ProjectServiceWithCaching : IProjectService
{

    private const string CacheProductKey = "productsCache";
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IProjectRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectServiceWithCaching(IUnitOfWork unitOfWork, IProjectRepository repository, IMemoryCache memoryCache, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _memoryCache = memoryCache;
        _mapper = mapper;

        if (!_memoryCache.TryGetValue(CacheProductKey, out _))
        {
            _memoryCache.Set(CacheProductKey, _repository.GetProjectsWithCategoryAsync().Result);
        }

    }


    public async Task<Project> AddAsync(Project entity)
    {
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        await CacheAllProjectsAsync();
        return entity;
    }

    public async Task<IEnumerable<Project>> AddRangeAsync(IEnumerable<Project> entities)
    {
            await _repository.AddRangeAsync(entities);
            await _unitOfWork.SaveChangesAsync();
            await CacheAllProjectsAsync();
            return entities;
    }

    public Task<bool> AnyAsync(Expression<Func<Project, bool>> expression)
    {
            throw new NotImplementedException();
    }

        public Task<IEnumerable<Project>> GetAllAsync()
        {

            var projects = _memoryCache.Get<IEnumerable<Project>>(CacheProductKey);
            return Task.FromResult(projects);
        }
    public Task<Project> GetByIdAsync(int id)
        {
            var project = _memoryCache.Get<List<Project>>(CacheProductKey).FirstOrDefault(x => x.Id == id);

            return Task.FromResult(project);
        }

        public Task<List<ProjectWithCategoryDto>> GetProjectsWithCategoryAsync()
        {
            var projects = _memoryCache.Get<IEnumerable<Project>>(CacheProductKey);

            var projectsWithCategoryDto = _mapper.Map<List<ProjectWithCategoryDto>>(projects);

            return Task.FromResult(projectsWithCategoryDto);
        }

        public async Task RemoveAsync(Project entity)
        {
            _repository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            await CacheAllProjectsAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<Project> entities)
        {
            _repository.RemoveRange(entities);
            await _unitOfWork.SaveChangesAsync();
            await CacheAllProjectsAsync();
        }

        public async Task UpdateAsync(Project entity)
        {
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            await CacheAllProjectsAsync();
        }

        public IQueryable<Project> Where(Expression<Func<Project, bool>> expression)
        {
            return _memoryCache.Get<List<Project>>(CacheProductKey).Where(expression.Compile()).AsQueryable();
        }

     public async Task CacheAllProjectsAsync()
     {
        _memoryCache.Set(CacheProductKey, await _repository.GetAll().ToListAsync());

    }

}
