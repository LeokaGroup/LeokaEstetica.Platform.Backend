using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Services.Abstractions.Project;

namespace LeokaEstetica.Platform.Services.Services.Project;

/// <summary>
/// Класс реализует методы сервиса проектов.
/// </summary>
public sealed class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    
    public ProjectService(IProjectRepository projectRepository, 
        ILogService logService, 
        IUserRepository userRepository, 
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _logService = logService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные нового проекта.</returns>
    public async Task<CreateProjectOutput> CreateProjectAsync(string projectName, string projectDetails, string account)
    {
        try
        {
            var result = new CreateProjectOutput();
            ValidateProject(projectName, projectDetails, account, ref result);

            if (result.Errors.Any())
            {
                return result;
            }

            var userId = await _userRepository.GetUserByEmailAsync(account);

            var project = await _projectRepository.CreateProjectAsync(projectName, projectDetails, userId);

            if (project is not null)
            {
                result = _mapper.Map<CreateProjectOutput>(project);
            }

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод валидация проекта при его создании.
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="projectDetails"></param>
    /// <param name="account"></param>
    /// <param name="result">Результирующая модель. Тут не возвращается, так как передана по ссылке сюда.</param>
    private void ValidateProject(string projectName, string projectDetails, string account, ref CreateProjectOutput result)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            result.Errors.Add("Не заполнено название проекта!");
        }
        
        if (string.IsNullOrEmpty(projectDetails))
        {
            result.Errors.Add("Не заполнено описание проекта!");
        }
        
        if (string.IsNullOrEmpty(account))
        {
            var ex = new ArgumentNullException($"Не передан аккаунт пользователя!");
            _logService.LogError(ex);
        }
    }
}