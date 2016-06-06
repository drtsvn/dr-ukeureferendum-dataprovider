using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DR.UkEuReferendum.DataProvider.Models;
using DR.UkEuReferendum.DataProvider.Service;
using System.Configuration;
using DR.UkEuReferendum.DataProvider.Service.Models;

namespace DR.UkEuReferendum.DataProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            var ftpHost = ConfigurationManager.AppSettings["FtpHost"];
            var ftpUserName = ConfigurationManager.AppSettings["FtpUserName"];
            var ftpPassword = ConfigurationManager.AppSettings["FtpPassword"];
            var dataReaderService = new DataReaderService(ftpHost, ftpUserName, ftpPassword);

            var firebaseUrl = ConfigurationManager.AppSettings["FirebaseUrl"];
            var firebaseSecret = ConfigurationManager.AppSettings["FirebaseSecret"];
            var firebaseService = new FirebaseService(firebaseUrl, firebaseSecret);

            var checks = new List<Check>();
            var timer = new Stopwatch();


#region Fetch Data

            var getDataSuccess = true;
            var scoreboardData = new Scoreboard();
            
            timer.Start();
            try
            {
                scoreboardData = await dataReaderService.GetLatestScoreBoard();
            }
            catch (Exception exception)
            {
                var logger = new Logging.Logging(enabled: true);
                logger.LogException(exception, "GetLatestScoreBoard() failed");
                getDataSuccess = false;
            }
            timer.Stop();

            checks.Add(new Check
            {
                Name = "GetData",
                Message = getDataSuccess ? "Fetched data successfully" : "Failed to fetch data",
                ResponseInMilliSeconds = timer.ElapsedMilliseconds,
                Status = getDataSuccess ? StatusEnum.OK : StatusEnum.ERROR
            });
            timer.Reset();
#endregion

#region Update Firebase

            if (getDataSuccess)
            {
                timer.Start();
                var firebaseSuccess = true;
                try
                {
                    firebaseSuccess = await firebaseService.UpdateFirebaseScoreboard(scoreboardData);
                }
                catch (Exception exception)
                {
                    var logger = new Logging.Logging(enabled: true);
                    logger.LogException(exception, "UpdateFirebaseScoreboard() failed");
                    firebaseSuccess = false;
                }
                timer.Stop();

                checks.Add(new Check
                {
                    Name = "UpdateFirebase",
                    Message = firebaseSuccess ? "Updated firebase successfully" : "Failed while updating firebase",
                    ResponseInMilliSeconds = timer.ElapsedMilliseconds,
                    Status = firebaseSuccess ? StatusEnum.OK : StatusEnum.ERROR
                });
                timer.Reset();
            }

#endregion


            WriteStatusXml(checks);

        }


        private static void WriteStatusXml(List<Check> checks)
        {
            var xmlPath = ConfigurationManager.AppSettings["XmlPath"];

#if DEBUG
            xmlPath = "C:/Temp/status.xml";
#endif
            var appStatus = checks.All(c => c.Status == StatusEnum.OK) ? StatusEnum.OK : StatusEnum.ERROR;
            var statusObj = new Status
            {
                ApplicationName = ConfigurationManager.AppSettings["ApplicationName"],
                ServerName = Environment.MachineName,
                Applicationstatus = appStatus,

                Timestamp = DateTime.UtcNow,
                Checks = checks
            };


            var x = new XmlSerializer(statusObj.GetType());
            System.IO.FileStream file = System.IO.File.Create(xmlPath);
            x.Serialize(file, statusObj);
            file.Close();

            




        }
    }
}
