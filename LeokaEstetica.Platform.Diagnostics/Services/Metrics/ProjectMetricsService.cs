using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Diagnostics.Abstractions.Metrics;
using LeokaEstetica.Platform.Models.Dto.Output.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<ProjectMetricsService>? _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectCommentsRepository">Репозиторий комментариев проектов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="logger">Логгер.</param>
    public ProjectMetricsService(IProjectCommentsRepository projectCommentsRepository,
        IMapper mapper,
        IProjectRepository projectRepository,
        ILogger<ProjectMetricsService>? logger)
    {
        _projectCommentsRepository = projectCommentsRepository;
        _mapper = mapper;
        _projectRepository = projectRepository;
        _logger = logger;
    }

    /// <summary>
    /// Метод получает последние 5 комментариев к проектам.
    /// Проекты не повторяются.
    /// </summary>
    /// <returns>Список комментариев.</returns>
    public async Task<IEnumerable<LastProjectCommentsOutput>> GetLastProjectCommentsAsync()
    {
        try
        {
            var comments = await _projectCommentsRepository.GetAllProjectCommentsAsync();

            if (!comments.Any())
            {
                return Enumerable.Empty<LastProjectCommentsOutput>();
            }

            var filterResult = await comments
                .GroupBy(g => g.ProjectId)
                .Select(x => x.OrderByDescending(o => o.Created).First())
                .Take(5)
                .ToListAsync();

            var result = _mapper.Map<List<LastProjectCommentsOutput>>(filterResult);

            // Заполняем названия проектов.
            foreach (var prj in result)
            {
                var project = await _projectRepository.GetProjectAsync(prj.ProjectId);
                prj.ProjectName = project.UserProject?.ProjectName ?? "Название проекта не указано";
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }
}