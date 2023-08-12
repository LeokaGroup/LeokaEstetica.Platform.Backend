namespace LeokaEstetica.Platform.Models.Dto.Input.Moderation;

/// <summary>
/// Класс входной модели управлением замечанием проекта (одобрение, отклонение).
/// </summary>
public class ManageProjectRemarkInput
{
    /// <summary>
    /// Id замечаний.
    /// </summary>
    public IEnumerable<long> RemarkIds { get; set; }

    /// <summary>
    /// Признак фиксации всех результатов. Т.е. когда одобряются либо отклоняются сразу все замечания.
    /// </summary>
    public bool IsFixed { get; set; }

    /// <summary>
    /// Признак не фиксации всех результатов. Т.е. когда одобряются либо отклоняются не все замечания, а частично.
    /// </summary>
    public bool IsUnfixed { get; set; }
}