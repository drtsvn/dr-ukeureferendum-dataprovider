using System.Threading.Tasks;
using DR.UkEuReferendum.DataProvider.Service.Models;

namespace DR.UkEuReferendum.DataProvider.Service
{
    public interface IDataReaderService
    {
        Task<Scoreboard> GetLatestScoreBoard();
    }
}
