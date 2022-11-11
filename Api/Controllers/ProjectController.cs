using System.Net;
using Api.Filters;
using Api.Settings;
using AutoMapper;
using Core;
using Core.Dtos;
using Core.Dtos.Contracts;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
   
    public class ProjectController:BaseController
    {
        private readonly IMapper _mapper;
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IOptions<CachingKeysSettings> _cachingKeys;

        public ProjectController(
            IProjectService projectService,
            IMapper mapper,
            ILogger<ProjectController> logger,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            IOptions<CachingKeysSettings> cachingKeys
            )
        {
            _mapper = mapper;
            _projectService = projectService;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _cachingKeys = cachingKeys;
        }

        [HttpGet("all")]
        [Cache(120,"projects")]
        public async Task<IActionResult> All(){

            var projects = await _projectService.GetAllAsync();

            var projectsDto = _mapper.Map<List<ProjectDto>>(projects);

            return CreateActionResult(CustomResponseDto<List<ProjectDto>>.Success(projectsDto));

        }

        [ApiKeyAuth]
        [ServiceFilter(typeof(NotFoundFilter<Project>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> ById(int id){
            
            var project = await _projectService.GetByIdAsync(id);

            var projecDto = _mapper.Map<ProjectDto>(project);

            return CreateActionResult(CustomResponseDto<ProjectDto>.Success(projecDto));

        }

        [HttpGet("category")]
        public async Task<IActionResult> ProjectsWithCategory(){
            
            var projects = await _projectService.GetProjectsWithCategoryAsync();

            var projectWithCategoryDto = _mapper.Map<List<ProjectWithCategoryDto>>(projects);

            return CreateActionResult(CustomResponseDto<List<ProjectWithCategoryDto>>.Success(projectWithCategoryDto));

        }

        [HttpPost]
        public async Task<IActionResult> Add(ProjectCreateDto projectCreateDto){

            _memoryCache.Remove("projects");            
            var project = await _projectService.AddAsync(_mapper.Map<Project>(projectCreateDto));
            var returnProjectDto = _mapper.Map<ProjectDto>(project);
            return CreateActionResult(CustomResponseDto<ProjectDto>.Success(returnProjectDto,(int)HttpStatusCode.Created));
            
        }

        [ServiceFilter(typeof(NotFoundFilter<Project>))]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id , ProjectUpdateDto projectDto)
        {

            var project = await _projectService.GetByIdAsync(id);

            await _projectService.UpdateAsync(_mapper.Map(projectDto,project));
          
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success((int)HttpStatusCode.NoContent));
        }


        [ServiceFilter(typeof(NotFoundFilter<Project>))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var project = await _projectService.GetByIdAsync(id);

            if(project == null)
                return NotFound();

            await _projectService.RemoveAsync(project);
          
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success((int)HttpStatusCode.NoContent));
        }

    
    }
}