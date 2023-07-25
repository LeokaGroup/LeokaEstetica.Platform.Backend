using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.Core.Exceptions;

/// <summary>
/// Исключение дубликата отклика на проект.
/// </summary>
public class DublicateProjectResponseException : Exception
{
    public DublicateProjectResponseException() : base(ValidationConst.ProjectResponse.DUBLICATE_PROJECT_RESPONSE)
    {
    }
}