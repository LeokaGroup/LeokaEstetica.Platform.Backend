using System.Net.Http.Headers;
using System.Net.Http.Json;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
using LeokaEstetica.Platform.Processing.Consts;
using LeokaEstetica.Platform.Processing.Factories;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Processing.Services.PayMaster;

/// <summary>
/// Класс реализует методы сервиса работы с платежной системой PayMaster.
/// </summary>
public class PayMasterService : IPayMasterService
{
    private readonly ILogService _logService;
    private readonly IConfiguration _configuration;
    private readonly IFareRuleRepository _fareRuleRepository;

    public PayMasterService(ILogService logService,
        IConfiguration configuration,
        IFareRuleRepository fareRuleRepository)
    {
        _logService = logService;
        _configuration = configuration;
        _fareRuleRepository = fareRuleRepository;
    }

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="createOrderInput">Входная модель.</param>
    /// <returns>Данные платежа.</returns>
    public async Task<CreateOrderOutput> CreateOrderAsync(CreateOrderInput createOrderInput)
    {
        try
        {
            using var httpClient = new HttpClient();

            // Находим тариф, который оплачивает пользователь.
            var fareRule = await _fareRuleRepository.GetByIdAsync(createOrderInput.FareRuleId);

            // Заполняем модель для запроса в ПС.
            CreateOrderRequestFactory.Create(ref createOrderInput, _configuration, fareRule);

            // Устанавливаем заголовки.
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _configuration["Commerce:PayMaster:ApiToken"]);

            // Создаем платеж в ПС.
            using var response = await httpClient.PostAsJsonAsync(ApiConsts.CREATE_PAYMENT, createOrderInput);
            
            // Получаем результат из ПС.
            var result = await response.Content.ReadFromJsonAsync<CreateOrderOutput>();

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}