using LeokaEstetica.Platform.LuceneNet.Factors;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace LeokaEstetica.Platform.LuceneNet.Chains;

/// <summary>
/// Базовый класс цепочек фильтров.
/// </summary>
public abstract class BaseFilterChain
{
    /// <summary>
    /// Версия люсины.
    /// </summary>
    private const Version _version = Version.LUCENE_CURRENT;

    /// <summary>
    /// Индекс в памяти.
    /// Объявляем как Lazy, чтобы дергался лишь при использовании.
    /// </summary>
    protected Lazy<RAMDirectory> _index { get; set; }

    /// <summary>
    /// Используем стандартный анализатор текста для люсины.
    /// </summary>
    protected readonly StandardAnalyzer _analyzer;

    protected BaseFilterChain()
    {
        _analyzer = new StandardAnalyzer(_version);
        _index = new Lazy<RAMDirectory>(CreateIndexRamDirectoryFactory.CreateNew<RAMDirectory>);
    }
}