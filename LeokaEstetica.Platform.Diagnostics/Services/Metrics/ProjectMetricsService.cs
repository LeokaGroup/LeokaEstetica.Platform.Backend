using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Diagnostics.Abstractions.Metrics;
using LeokaEstetica.Platform.Models.Dto.Output.Metrics;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Diagnostics.Services.Metrics;

/// <summary>
/// Класс реализует методы сервиса метрик проектов.
/// </summary>
internal sealed class ProjectMetricsService : IProjectMetricsService
{
    private readonly IProjectCommentsRepository _projectCommentsRepository;
    private readonly IMapper _mapper;
    private readonly IProjectRepository _projectRepository;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectCommentsRepository">Репозиторий комментариев проектов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    public ProjectMetricsService(IProjectCommentsRepository projectCommentsRepository,
        IMapper mapper,
        IProjectRepository projectRepository)
    {
        _projectCommentsRepository = projectCommentsRepository;
        _mapper = mapper;
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Метод получает последние 5 комментариев к проектам.
    /// Проекты не повторяются.
    /// </summary>
    /// <returns>Список комментариев.</returns>
    public async Task<IEnumerable<LastProjectCommentsOutput>> GetLastProjectCommentsAsync()
    {
        var comments = await _projectCommentsRepository.GetAllProjectCommentsAsync();

        if (!comments.Any())
        {
            return Enumerable.Empty<LastProjectCommentsOutput>();
        }

        var filterResult = await comments
            .Take(5)
            .GroupBy(g => g.ProjectId)
            .Select(x => x.OrderByDescending(o => o.Created).First())
            .ToListAsync();

        var result = _mapper.Map<List<LastProjectCommentsOutput>>(filterResult);

        // Заполняем названия проектов.
        foreach (var prj in result)
        {
            var project = await _projectRepository.GetProjectAsync(prj.ProjectId);
            prj.ProjectName = project.UserProject.ProjectName;
        }

        return result;
    }
}