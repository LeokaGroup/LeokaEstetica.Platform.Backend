﻿using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Models.Dto.Communications.Output;

/// <summary>
/// Класс результата объектов группы абстрактной области.
/// </summary>
public class AbstractGroupResult
{
    /// <summary>
    /// Название группы объектов.
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// Объекты группы.
    /// </summary>
    public List<AbstractGroupOutput>? Objects { get; set; }

    /// <summary>
    /// Системное название группы объектов.
    /// </summary>
    public string? GroupSysName { get; set; }
    
    /// <summary>
    /// Тип группировки.
    /// </summary>
    public string? DialogGroupType { get; set; }
}