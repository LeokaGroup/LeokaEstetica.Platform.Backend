using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Abstractions;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.TorchSharp;

namespace LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Services;

/// <summary>
/// Класс реализует методы сервиса нейросети Scrum Master AI.
/// </summary>
internal sealed class ScrumMasterAiService : IScrumMasterAiService
{
    private readonly ILogger<ScrumMasterAiService> _logger;

    /// <summary>
    /// Контроллер.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    public ScrumMasterAiService(ILogger<ScrumMasterAiService> logger)
    {
        _logger = logger;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task EducationFromCsvDatasetAsync()
    {
        try
        {
            var mlContext = new MLContext();
            var data = mlContext.Data.LoadFromTextFile<MessageClassification>(@"C:\temp\dataset.csv",
                separatorChar: ';', hasHeader: true, allowQuoting: true);
                
            Console.WriteLine("Начали обучение модели...");
            _logger.LogInformation("Начали обучение модели...");

            // Обучаем модель.
            ITransformer model = mlContext.Transforms.Conversion.MapValueToKey("Label", "Response")
                .Append(mlContext.MulticlassClassification.Trainers.TextClassification(labelColumnName: "Label",
                    sentence1ColumnName: "Message"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"))
                // TODO: Microsoft рекомендует убрать кэш при огромных данных. На малых и средних данных можно.
                .AppendCacheCheckpoint(mlContext)
                .Fit(data);
                
            Console.WriteLine("Закончили обучение модели...");

            // TODO: Пока сохраняем вручную на сервер - не забывать в БД проставлять новую версию
            // TODO: при обучении в таблице ai.scrum_master_ai_message_versions.
            // Сохраняем обученную модель в формате .zip на сервере.
            var modelPath = @"C:\temp\v.1.0.0.scrum_master_ai_message.zip";
            //
            // // TODO: Уберем, когда сделаем сохранение модели на сервер.
            // if(!Directory.Exists(modelPath))
            // {
            //     throw new InvalidOperationException(
            //         $"Путь {modelPath} не существует либо идет попытка сохранения модели нейросети вне" +
            //         " локальной среды.");
            // }

            #region Для проверки обучения (раскоментить).

            // // Нейросеть проводит прогнозирование.
            // var predEngine = mlContext.Model
            //     .CreatePredictionEngine<MessageClassification, MessageClassificationPrediction>(
            //         model);
            //
            // // Результат ответа нейросети после прогнозирования.
            // var prediction = predEngine.Predict(new MessageClassification
            // {
            //     Message = "test"
            // });
            //
            // Console.WriteLine(prediction.Message);

            #endregion
            
            mlContext.Model.Save(model, data.Schema, modelPath);

            Console.WriteLine("Модель успешно обучена...");
            _logger.LogInformation("Модель успешно обучена...");

            await Task.CompletedTask;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    #endregion
}