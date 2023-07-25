using LeokaEstetica.Platform.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Core.Extensions;

/// <summary>
/// Класс настройки конфигураций маппингов.
/// </summary>
public static class MappingsExtensions
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        var assembliesMappings =
            AutoFac.GetAssembliesFromApplicationBaseDirectory(x =>
                x.FullName.StartsWith("LeokaEstetica.Platform.Models"));

        // Применяет все конфигурации маппингов.
        foreach (var item in assembliesMappings)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(item);
        }
    }
}