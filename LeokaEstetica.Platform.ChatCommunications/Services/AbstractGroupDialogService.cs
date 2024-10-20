using System.Runtime.CompilerServices;
using Dapper;
using FluentValidation;
using FluentValidation.Results;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Communications.Services;

/// <summary>
/// Класс реализует методы сервиса диалогов.
/// </summary>
internal sealed class AbstractGroupDialogService : IAbstractGroupDialogService
{
    private readonly IAbstractGroupDialogRepository _abstractGroupDialogRepository;
    private readonly ILogger<AbstractGroupDialogService> _logger;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="abstractGroupDialogRepository">Репозиторий диалогов.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public AbstractGroupDialogService(IAbstractGroupDialogRepository abstractGroupDialogRepository,
        ILogger<AbstractGroupDialogService> logger,
        IUserRepository userRepository)
    {
        _abstractGroupDialogRepository = abstractGroupDialogRepository;
        _logger = logger;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<CreateDialogAndAddDialogMembersOutput> CreateDialogAndAddDialogMembersAsync(
        IEnumerable<string>? memberEmails, string? dialogName, string account)
    {
        try
        {
            var result = new CreateDialogAndAddDialogMembersOutput
            {
                Errors = new List<ValidationFailure>()
            };
            
            memberEmails ??= new List<string>();
            
            var memberEmailsList = memberEmails.AsList();

            var members = await _userRepository.GetUserByEmailsAsync(memberEmailsList.Union(new[] { account }));

            foreach (var me in memberEmailsList)
            {
                // Ошибки, если они есть.
                if (members.TryGet(me) <= 0)
                {
                    result.Errors.Add(new ValidationFailure
                    {
                        Severity = Severity.Error,
                        ErrorMessage = $"Пользователя {me} не существует в системе."
                    });
                }
            }

            // Если были ошибки, то просто возвращаем их. Диалог не будет создаваться.
            if (result.Errors.Count > 0)
            {
                result.IsSuccess = false;

                return result;
            }
            
            var memberIds = members.Select(x => x.Value);

            result.Dialog = await _abstractGroupDialogRepository.CreateDialogAndAddDialogMembersAsync(memberIds,
                dialogName);

            result.IsSuccess = true;

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}