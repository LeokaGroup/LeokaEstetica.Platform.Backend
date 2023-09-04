using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.ModelsValidation;

public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
{
    public CustomLanguageManager() 
    {
        // AddTranslation("ru", "NotNullValidator", $"'{nameof(UserSignInInput.Email)}' {ValidationConsts.NOT_VALID_EMAIL_ERROR}");
        // AddTranslation("ru", "NotNullValidator", $"'{nameof(UserSignInInput.Password)}' {ValidationConsts.NOT_VALID_PASSWORD}");
        AddTranslation("ru", "NotNullValidator", $"{nameof(UserSignInInput.Email)} {ValidationConsts.NOT_VALID_EMAIL_ERROR}");
        AddTranslation("ru", "NotNullValidator", $"{nameof(UserSignInInput.Password)} {ValidationConsts.NOT_VALID_PASSWORD}");
    }
}