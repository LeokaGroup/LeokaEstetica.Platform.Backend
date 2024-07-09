using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Finder.Services.Resume;

/// <summary>
/// Класс реализует методы сервиса пагинации резюме.
/// </summary>
public class ResumePaginationService : IResumePaginationService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly ILogger<ResumePaginationService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="resumeRepository">Репозиторий резюме.</param>
    /// <param name="logger">Логгер.</param>
    public ResumePaginationService(IResumeRepository resumeRepository,
        ILogger<ResumePaginationService> logger)
    {
        _resumeRepository = resumeRepository;
        _logger = logger;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<PaginationResumeOutput> GetResumesPaginationAsync(int page, long? lastId)
    {
        try
        {
            var result = await _resumeRepository.GetPaginationResumesAsync(PaginationConst.TAKE_COUNT, lastId);

            if (result?.Resumes is null || result.Resumes.Count == 0)
            {
                return new PaginationResumeOutput { Resumes = new List<UserInfoOutput>() };
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}