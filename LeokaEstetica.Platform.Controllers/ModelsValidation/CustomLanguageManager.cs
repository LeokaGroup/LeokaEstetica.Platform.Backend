using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.ModelsValidation;

public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
{
    public CustomLanguageManager() 
    {
        AddTranslation("ru", "NotNullValidator", ValidationConsts.NOT_VALID_EMAIL_ERROR);
        AddTranslation("ru", "NotNullValidator", ValidationConsts.EMPTY_PASSWORD_ERROR);
    }
}