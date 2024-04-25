using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели стратегий представления рабочего пространства.
/// </summary>
public class ViewStrategyOutput : ViewStrategyEntity
{
	public ViewStrategyOutput(string viewStrategyName, string viewStrategySysName) : base(viewStrategyName, viewStrategySysName)
	{
	}
}