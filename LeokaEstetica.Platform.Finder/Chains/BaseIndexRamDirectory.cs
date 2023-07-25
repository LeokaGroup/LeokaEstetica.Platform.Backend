using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace LeokaEstetica.Platform.Finder.Chains;

/// <summary>
/// Класс строит индекс для работы с Lucene.
/// </summary>
public abstract class BaseIndexRamDirectory
{
    /// <summary>
    /// Версия люсины.
    /// </summary>
    protected const Version _version = Version.LUCENE_CURRENT;

    /// <summary>
    /// Индекс в памяти.
    /// Объявляем как Lazy, чтобы дергался лишь при использовании.
    /// </summary>
    protected Lazy<RAMDirectory> _index { get; set; }

    /// <summary>
    /// Используем стандартный анализатор текста для люсины.
    /// </summary>
    protected readonly StandardAnalyzer _analyzer;

    protected BaseIndexRamDirectory()
    {
        _analyzer = new StandardAnalyzer(_version);
        _index = new Lazy<RAMDirectory>();
    }
}