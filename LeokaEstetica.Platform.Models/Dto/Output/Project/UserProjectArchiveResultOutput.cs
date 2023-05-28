﻿namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели результата архива проектов.
/// </summary>
public class UserProjectArchiveResultOutput
{
    /// <summary>
    /// Список проектов.
    /// </summary>
    public IEnumerable<ProjectArchiveOutput> ProjectArchiveOutputs { get; set; }

    /// <summary>
    /// Кол-во проектов.
    /// </summary>
    public int Total => ProjectArchiveOutputs.Count();
}
