using FluentValidation.Results;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Validation;
using LeokaEstetica.Platform.Base.Abstractions.Services.Validation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Redis.Abstractions.Validation;
using LeokaEstetica.Platform.Redis.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Base.Services.Validation;

/// <summary>
/// Класс сервиса реализует методы сервиса для исключения параметров валидации, которые не нужно выдавать фронту.
/// </summary>
public sealed class ValidationExcludeErrorsService : IValidationExcludeErrorsService
{
    private readonly IValidationExcludeErrorsRepository _validationExcludeErrorsRepository;
    private readonly IValidationExcludeErrorsCacheService _validationExcludeErrorsCacheService;
    private readonly ILogger<ValidationExcludeErrorsService> _logger;
    
    public ValidationExcludeErrorsService(IValidationExcludeErrorsRepository validationExcludeErrorsRepository,
        IValidationExcludeErrorsCacheService validationExcludeErrorsCacheService, 
        ILogger<ValidationExcludeErrorsService> logger)
    {
        _validationExcludeErrorsRepository = validationExcludeErrorsRepository;
        _validationExcludeErrorsCacheService = validationExcludeErrorsCacheService;
        _logger = logger;
    }

    /// <summary>
    /// Метод исключает из списка ошибок те, которые есть в управляющей таблице исключения параметров валидации.
    /// </summary>
    /// <param name="errors">Список ошибок.</param>
    /// <returns>Список ошибок после фильтрации.</returns>
    public async Task<List<ValidationFailure>> ExcludeAsync(List<ValidationFailure> errors)
    {
        try
        {
            // Получаем список полей из кэша редиса, если они там есть.
            var cacheFields = await _validationExcludeErrorsCacheService
                .ValidationColumnsExcludeFromCacheAsync();

            // Если нашли в кэше, то продливаем время жизни и исключаем поля, затем возвращаем.
            if (cacheFields is not null && cacheFields.Any())
            {
                await _validationExcludeErrorsCacheService
                    .RefreshCacheAsync(GlobalConfigKeys.Cache.VALIDATION_EXCLUDE_KEY);
                
                var excludeCacheErrors = cacheFields.Select(x => x.ParamName);
                Exclude(ref errors, excludeCacheErrors);
                
                return errors;
            }
            
            // В кэше нет, получим из БД и исключим, затем добавим в кэш и вернем.
            var items = await _validationExcludeErrorsRepository
                .ValidationColumnsExcludeAsync();

            if (items.Any())
            {
                var serializeItems = JsonConvert.SerializeObject(items);
                var cacheItems = JsonConvert.DeserializeObject<ICollection<ValidationExcludeRedis>>(serializeItems);
                
                // Добавляем в кэш.
                await _validationExcludeErrorsCacheService.AddValidationColumnsExcludeToCacheAsync(cacheItems);
            }

            var excludeErrors = items.Select(x => x.ParamName);
            Exclude(ref errors, excludeErrors);

            return errors;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод исключает параметры валидации и удаляет дубли.
    /// </summary>
    /// <param name="errors">Список ошибок до исключения.</param>
    /// <param name="excludeCacheErrors">Список параметров дял исключения.</param>
    private void Exclude(ref List<ValidationFailure> errors, IEnumerable<string> excludeCacheErrors)
    {
        errors.RemoveAll(x => excludeCacheErrors.Contains(x.PropertyName));
        
        errors = errors
            .DistinctBy(d => d.PropertyName)
            .ToList();
    }
}