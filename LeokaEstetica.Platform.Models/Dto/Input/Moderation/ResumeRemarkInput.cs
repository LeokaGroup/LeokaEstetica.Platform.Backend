using LeokaEstetica.Platform.Models.Dto.Base.Moderation.Input;

namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс входной модели замечаний анкеты.
/// </summary>
public class ResumeRemarkInput : BaseRemarkInput
{
    /// <summary>
    /// Id анкеты.
    /// </summary>
    public long ProfileInfoId { get; set; }
}