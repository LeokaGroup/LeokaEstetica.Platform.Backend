namespace LeokaEstetica.Platform.Models.Dto.Output.Knowledge;

public class KnowledgeLandingResult
{
    /// <summary>
    /// Список результатов.
    /// </summary>
    public IEnumerable<KnowledgeLandingOutput> KnowledgeLanding { get; set; }

    /// <summary>
    /// Заголовок.
    /// </summary>
    public string Title { get; set; }
}