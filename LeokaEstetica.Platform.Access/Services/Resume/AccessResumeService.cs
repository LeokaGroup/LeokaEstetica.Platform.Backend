using LeokaEstetica.Platform.Access.Abstractions.Resume;

namespace LeokaEstetica.Platform.Access.Services.Resume;

/// <summary>
/// Класс реализует методы сервиса доступа к базе резюме.
/// </summary>
public class AccessResumeService : IAccessResumeService
{
    public Task<int> CheckAvailableResumesAsync(string account)
    {
        throw new NotImplementedException();
    }
}