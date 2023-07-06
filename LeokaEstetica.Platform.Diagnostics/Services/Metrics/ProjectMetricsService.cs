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
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectCommentsRepository">Репозиторий комментариев проектов.</param>
    /// <param name="mapper">Автомаппер.</param>
    public ProjectMetricsService(IProjectCommentsRepository projectCommentsRepository,
        IMapper mapper)
    {
        _projectCommentsRepository = projectCommentsRepository;
        _mapper = mapper;
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
            .OrderByDescending(o => o.Created)
            .Take(5)
            .GroupBy(g => g.ProjectId)
            .Select(x => x.First())
            .ToListAsync();
        
        var result = _mapper.Map<List<LastProjectCommentsOutput>>(filterResult);

        return result;
    }
}