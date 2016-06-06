using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DR.UkEuReferendum.DataProvider.Service.Models;

namespace DR.UkEuReferendum.DataProvider.Service
{
    public interface IFirebaseService
    {
        Task<bool> UpdateFirebaseScoreboard(Scoreboard runningTotal);
    }
}
