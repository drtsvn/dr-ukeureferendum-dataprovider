using System;
using System.Data.Odbc;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DR.UkEuReferendum.DataProvider.Service.Models;

namespace DR.UkEuReferendum.DataProvider.Service
{
    public class DataReaderService : IDataReaderService
    {
        private readonly string _ftpHostname;
        private readonly string _ftpUserName;
        private readonly string _ftpPassword;
        private readonly string _scoreboardPath;

        public DataReaderService(string ftpHostname, string ftpUserName, string ftpPassword)
        {
            _ftpHostname = ftpHostname;
            _ftpUserName = ftpUserName;
            _ftpPassword = ftpPassword;
            _scoreboardPath = string.Format("{0}/RSG", _ftpHostname);
        }

        public async Task<Scoreboard> GetLatestScoreBoard()
        {
            var filename = await GetLatestScoreboardFileName();
            if (string.IsNullOrWhiteSpace(filename))
                return new Scoreboard();

            var latestScoreboardData = await GetScoreboardData(filename);
            var updateIdString = filename.Split('.').FirstOrDefault();
            int updateId;
            Int32.TryParse(updateIdString, out updateId);

            var scoreboard = new Scoreboard
            {
                UpdateId = updateId,
                TotalCounsils = latestScoreboardData.Scoreboard.TotalCounsils,
                DeclaredCounsils = latestScoreboardData.Scoreboard.DeclaredCounsils,
                ParCode = string.IsNullOrWhiteSpace(latestScoreboardData.Scoreboard.WinningMoment) ? "" : latestScoreboardData.Scoreboard.ParCode,
                RemainShare = latestScoreboardData.ScoreboardResult.RemainShare.ToString("0.0", CultureInfo.GetCultureInfo("da-DK")),
                LeaveShare = latestScoreboardData.ScoreboardResult.LeaveShare.ToString("0.0", CultureInfo.GetCultureInfo("da-DK"))
            };
            return scoreboard;
        }

        private async Task<string> GetLatestScoreboardFileName()
        {

            var credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
            var request = (FtpWebRequest)WebRequest.Create(_scoreboardPath);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = credentials;
            
            try 
            {
                var response = (FtpWebResponse)(await request.GetResponseAsync());
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream);
                var files = await reader.ReadToEndAsync();

                reader.Close();
                response.Close();

                var latestFileName = files.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(f => f.Replace("RSG/","")).OrderByDescending(f => f).FirstOrDefault();
                return latestFileName;
            }
            catch(WebException exception)
            {
                //If we cannnot list the scoreboard directory it may simply be because it's not there yet. In this case verify that we have access to the ftp and continue
                var res = (FtpWebResponse)exception.Response;
                if (res.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable && VerifyFtpAccess())
                {
                    return "";
                }
                throw;
            }
        }

        private async Task<ReferendumScoreboard> GetScoreboardData(string fileName)
        {
            var credentials = new NetworkCredential(_ftpUserName, _ftpPassword);

            var request = (FtpWebRequest) WebRequest.Create(string.Format("{0}/{1}", _scoreboardPath, fileName));
            request.Credentials = credentials;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            var response = (FtpWebResponse) (await request.GetResponseAsync());
            var responseStream = response.GetResponseStream();
            var reader = new StreamReader(responseStream);
            
            var xmlSerializer = new XmlSerializer(typeof (ReferendumScoreboard));
            var scoreboardData = (ReferendumScoreboard) xmlSerializer.Deserialize(reader);
            
            reader.Close();
            response.Close();
            
            return scoreboardData;
        }

        private bool VerifyFtpAccess()
        {
            var credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
            var request = (FtpWebRequest)WebRequest.Create(_ftpHostname);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = credentials;
            try
            {
                var response = (FtpWebResponse)(request.GetResponse());

                response.Close();
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }
    }
}
