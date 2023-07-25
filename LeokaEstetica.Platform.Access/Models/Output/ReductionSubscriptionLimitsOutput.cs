namespace LeokaEstetica.Platform.Access.Models.Output;

/// <summary>
/// Класс выходной модели лимитов подписки.
/// </summary>
public class ReductionSubscriptionLimitsOutput
{
    /// <summary>
    /// Признак успешности прохождения по лимитам.
    /// </summary>
    public bool IsSuccessLimits { get; set; }

    /// <summary>
    /// Тип лимитов, по которым не прошли.
    /// </summary>
    public string ReductionSubscriptionLimits { get; set; }

    /// <summary>
    /// Кол-во на которое нужно уменьшить для прохождения по лимитам.
    /// </summary>
    public int FareLimitsCount { get; set; }
}