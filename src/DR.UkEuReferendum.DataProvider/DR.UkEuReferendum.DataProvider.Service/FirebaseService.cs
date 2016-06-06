using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DR.UkEuReferendum.DataProvider.Service.Models;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;

namespace DR.UkEuReferendum.DataProvider.Service
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IFirebaseClient _firebaseClient;

        public FirebaseService(string firebaseUrl, string firebaseSecret)
        {
            var firebaseConfig = new FirebaseConfig {AuthSecret = firebaseSecret, BasePath = firebaseUrl};
            _firebaseClient = new FirebaseClient(firebaseConfig);
        }

        public async Task<bool> UpdateFirebaseScoreboard(Scoreboard scoreboard)
        {
            var firebasePath = string.Format("scoreboard");

            var response = await _firebaseClient.SetAsync(firebasePath, scoreboard);
            return response.StatusCode == HttpStatusCode.OK;

        }
    }
}
