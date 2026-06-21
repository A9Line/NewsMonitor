using System.Collections.Generic;
using System.Threading.Tasks;
using NewsMonitor.Models;

namespace NewsMonitor.Publishers
{
    // Любой канал доставки умеет опубликовать список новостей.
    // Завтра можно сделать EmailPublisher — остальной код не изменится.
    public interface INewsPublisher
    {
        Task PublishAsync(List<NewsItem> items);
    }
}
