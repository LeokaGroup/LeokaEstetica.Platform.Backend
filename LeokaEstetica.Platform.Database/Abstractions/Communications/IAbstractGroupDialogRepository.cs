﻿using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Database.Abstractions.Communications;

/// <summary>
/// Абстракция репозитория диалогов.
/// </summary>
public interface IAbstractGroupDialogRepository
{
    /// <summary>
    /// Метод создает диалог и добавляет в него участников.
    /// </summary>
    /// <param name="memberEmails">Список участников диалога.</param>
    /// <param name="dialogName">Название диалога.</param>
    /// <param name="dialogGroupType">Тип группировки чата.</param>
    /// <param name="abstractId">Id группы. Это может быть либо компании либо проекта и тд.</param>
    /// <returns>Созданный диалог.</returns>
    Task<GroupObjectDialogOutput?> CreateDialogAndAddDialogMembersAsync(IEnumerable<long> memberIds,
        string? dialogName, DialogGroupTypeEnum dialogGroupType, long? abstractId);

    /// <summary>
    /// Метод добавляет сообщение чата в БД.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="createdBy">Кто создал сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="isMyMessage">Признак сообщения текущего пользователя.</param>
    /// <returns>Добавленное сообщение.</returns>
    Task<GroupObjectDialogMessageOutput?> SaveMessageAsync(string? message, long createdBy, long dialogId,
        bool isMyMessage);
}