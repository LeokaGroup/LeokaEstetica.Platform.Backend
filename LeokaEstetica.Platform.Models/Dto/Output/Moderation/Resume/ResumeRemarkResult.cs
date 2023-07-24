namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

/// <summary>
/// Класс результата списка замечаний анкеты.
/// </summary>
public class ResumeRemarkResult
{
    /// <summary>
    /// Список замечаний анкеты.
    /// </summary>
    public IEnumerable<ResumeRemarkOutput> ResumeRemarks { get; set; }

    /// <summary>
    /// Кол-во замечаний всего.
    /// </summary>
    public int Total
    {
        get
        {
            if (ResumeRemarks is not null)
            {
                return ResumeRemarks.Count();
            }

            return 0;
        }

        set => Total = value;
    }
}