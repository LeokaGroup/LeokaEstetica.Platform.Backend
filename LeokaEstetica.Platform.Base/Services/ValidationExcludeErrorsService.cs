using FluentValidation.Results;
using LeokaEstetica.Platform.Base.Abstractions.Repositories;
using LeokaEstetica.Platform.Base.Abstractions.Services;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Redis.Abstractions.Validation;

namespace LeokaEstetica.Platform.Base.Services;

/// <summary>
/// Класс сервиса реализует методы сервиса для исключения параметров валидации, которые не нужно выдавать фронту.
/// </summary>
public sealed class ValidationExcludeErrorsService : IValidationExcludeErrorsService
{
    private readonly IValidationExcludeErrorsRepository _validationExcludeErrorsRepository;
    private readonly ILogService _logService;
    private readonly IValidationExcludeErrorsCacheService _validationExcludeErrorsCacheService;
    
    public ValidationExcludeErrorsService(IValidationExcludeErrorsRepository validationExcludeErrorsRepository, 
        ILogService logService, 
        IValidationExcludeErrorsCacheService validationExcludeErrorsCacheService)
    {
        _validationExcludeErrorsRepository = validationExcludeErrorsRepository;
        _logService = logService;
        _validationExcludeErrorsCacheService = validationExcludeErrorsCacheService;
    }

    /// <summary>
    /// Метод исключает из списка ошибок те, которые есть в управляющей таблице исключения параметров валидации.
    /// </summary>
    /// <param name="errors">Список ошибок.</param>
    public async Task<List<ValidationFailure>> ExcludeAsync(List<ValidationFailure> errors)
    {
        try
        {
            // Получаем список полей из кэша редиса, если они там есть.
            var cacheFields = await _validationExcludeErrorsCacheService
                .ValidationColumnsExcludeFromCacheAsync();

            // Если нашли в кэше, то продливаем время жизни и исключаем поля, затем возвращаем.
            if (cacheFields.Any())
            {
                await _validationExcludeErrorsCacheService
                    .RefreshCacheAsync(GlobalConfigKeys.Cache.VALIDATION_EXCLUDE_KEY);
                
                var excludeCacheErrors = cacheFields.Select(x => x.ParamName);
               
                // Исключаем поля из ошибок валидации, которые есть в кэше.
                var cacheResult = errors
                    .Where(x => excludeCacheErrors.Contains(x.PropertyName))
                    .ToList();
                
                return cacheResult;
            }
            
            // В кэше нет, получим из БД и исключим, затем вернем.
            var items = await _validationExcludeErrorsRepository
                .ValidationColumnsExcludeAsync();
            var excludeErrors = items.Select(x => x.ParamName);
            var result = errors
                .Where(x => excludeErrors.Contains(x.PropertyName))
                .ToList();

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}