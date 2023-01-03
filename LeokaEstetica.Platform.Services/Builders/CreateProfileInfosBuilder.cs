using LeokaEstetica.Platform.Models.Entities.Profile;

namespace LeokaEstetica.Platform.Services.Builders;

/// <summary>
/// Класс билдера строит список анкет. Убирает анкеты, которые не проходят по условиям.
/// </summary>
public static class CreateProfileInfosBuilder
{
    /// <summary>
    /// Метод убирает анкеты, которые не проходят по условиям.
    /// </summary>
    /// <param name="profileInfos">Список анкет в исходном виде.</param>
    public static void CreateProfileInfosResult(ref List<ProfileInfoEntity> profileInfos)
    {
        profileInfos.RemoveAll(p => string.IsNullOrEmpty(p.FirstName)
                                    || string.IsNullOrEmpty(p.LastName)
                                    || string.IsNullOrEmpty(p.Patronymic)
                                    || string.IsNullOrEmpty(p.Job)
                                    || string.IsNullOrEmpty(p.Aboutme)
                                    || p.UserId <= 0);
    }
}