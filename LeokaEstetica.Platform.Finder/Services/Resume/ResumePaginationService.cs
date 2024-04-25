using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Chains;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Models.Dto.Output.Pagination;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Finder.Services.Resume;

/// <summary>
/// Класс реализует методы сервиса пагинации резюме.
/// </summary>
public class ResumePaginationService : BaseIndexRamDirectory, IResumePaginationService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly ILogger<ResumePaginationService> _logger;
    private readonly IAccessUserService _accessUserService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="resumeRepository">Репозиторий резюме.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="accessUserService">Сервис доступа пользователей.</param>
    public ResumePaginationService(IResumeRepository resumeRepository,
        ILogger<ResumePaginationService> logger,
        IAccessUserService accessUserService)
    {
        _resumeRepository = resumeRepository;
        _logger = logger;
        _accessUserService = accessUserService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод пагинации резюме.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Список резюме.</returns>
    public async Task<PaginationResumeOutput> GetResumesPaginationAsync(int page)
    {
        try
        {
            var resumes = await _resumeRepository.GetFilterResumesAsync();

            if (!resumes.Any())
            {
                return new PaginationResumeOutput();
            }
            
            var result = new PaginationResumeOutput
            {
                IsVisiblePagination = true,
                PaginationInfo = new PaginationInfoOutput(resumes.Count(), page, PaginationConst.TAKE_COUNT)
            };
            
            // Получаем все анкеты из БД без выгрузки в память.
            ResumesDocumentLoader.Load(resumes, _index, _analyzer);

            using var reader = IndexReader.Open(_index.Value, true);
            using var searcher = new IndexSearcher(reader);
            var scoreDocs = CreateScoreDocsBuilder.CreateScoreDocsResult(page, searcher, resumes.Count());

            result.Resumes = CreateResumesSearchResultBuilder.CreateResumesSearchResult(scoreDocs, searcher).ToList();

            // Если первая страница и записей менее максимального на странице,
            // то надо скрыть пагинацию, так как смысл в пагинации теряется в этом кейсе.
            // Применяем именно к 1 странице, к последней нет (там это надо показывать).
            if (page == 1 && result.Resumes.Count < PaginationConst.TAKE_COUNT)
            {
                result.IsVisiblePagination = false;
            }

            if (result.Resumes.Any())
            {
                var removedUsers = new List<ResumeOutput>();
                
                foreach (var res in result.Resumes)
                {
                    var userId = res.UserId;
                    
                    // Проверяем заполнение анкеты.
                    var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);

                    // Удаляем анкеты из выборки, которые не заполнены.
                    if (isEmptyProfile)
                    {
                        removedUsers.Add(res);
                    }
                }

                if (removedUsers.Any())
                {
                    result.Resumes.RemoveAll(r => removedUsers.Contains(r));   
                }
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