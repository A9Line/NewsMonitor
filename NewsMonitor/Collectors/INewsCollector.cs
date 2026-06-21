using System.Collections.Generic;
using System.Threading.Tasks;
using NewsMonitor.Models;

namespace NewsMonitor.Collectors
{
    // Любой источник новостей обязан уметь "отдать" список новостей.
    // Благодаря этому интерфейсу оркестратор не знает деталей источников.
    public interface INewsCollector
    {
        string SourceName { get; }
        Task<List<NewsItem>> FetchAsync();
    }
}
