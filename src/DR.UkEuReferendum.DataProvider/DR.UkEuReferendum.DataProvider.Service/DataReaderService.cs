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

        public DataReaderService(string ftpHostname, string ftpUserName, string ftpPassword)
        {
            _ftpHostname = ftpHostname;
            _ftpUserName = ftpUserName;
            _ftpPassword = ftpPassword;
        }

        public async Task<Scoreboard> GetLatestScoreBoard()
        {
            var filename = await GetLatestScoreboardFileName();
            if (string.IsNullOrWhiteSpace(filename))
                return new Scoreboard();
            ReferendumScoreboard latestScoreboardData;
            try
            {
                latestScoreboardData = await GetScoreboardData(filename);
            }
            catch (Exception exception)
            {
                throw new FileLoadException("Failed to load scoreboard file", filename, exception);                
            }
            var updateIdString = filename.Split('.').FirstOrDefault();
            int updateId;
            Int32.TryParse(updateIdString, out updateId);

            var remainShare = latestScoreboardData.ScoreboardResult.RemainShare.ToString("0.0", CultureInfo.GetCultureInfo("da-DK"));
            var leaveShare = latestScoreboardData.ScoreboardResult.LeaveShare.ToString("0.0", CultureInfo.GetCultureInfo("da-DK"));

            if (remainShare == "50,0" || leaveShare == "50,0")
            {
                remainShare = latestScoreboardData.ScoreboardResult.RemainShare.ToString("0.00", CultureInfo.GetCultureInfo("da-DK"));
                leaveShare = latestScoreboardData.ScoreboardResult.LeaveShare.ToString("0.00", CultureInfo.GetCultureInfo("da-DK"));
            }

            var scoreboard = new Scoreboard
            {
                UpdateId = updateId,
                TotalCounsils = latestScoreboardData.Scoreboard.TotalCounsils,
                DeclaredCounsils = latestScoreboardData.Scoreboard.DeclaredCounsils,
                ParCode = string.IsNullOrWhiteSpace(latestScoreboardData.Scoreboard.WinningMoment) ? "" : latestScoreboardData.Scoreboard.ParCode,
                RemainShare = remainShare,
                LeaveShare = leaveShare
            };
            return scoreboard;
        }

        private async Task<string> GetLatestScoreboardFileName()
        {

            var credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
            var request = (FtpWebRequest)WebRequest.Create(_ftpHostname);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = credentials;
            
                var response = (FtpWebResponse)(await request.GetResponseAsync());
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream);
                var files = await reader.ReadToEndAsync();

                reader.Close();
                response.Close();

                var latestFileName = files.Split(new[] { "\r\n" }, StringSplitOptions.None).Where(f => f.EndsWith(".RSG")).OrderByDescending(f => f).FirstOrDefault();
                return latestFileName;

        }

        private async Task<ReferendumScoreboard> GetScoreboardData(string fileName)
        {
            var credentials = new NetworkCredential(_ftpUserName, _ftpPassword);

            var request = (FtpWebRequest) WebRequest.Create(string.Format("{0}/{1}", _ftpHostname, fileName));
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

        //private bool VerifyFtpAccess()
        //{
        //    var credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
        //    var request = (FtpWebRequest)WebRequest.Create(_ftpHostname);
        //    request.Method = WebRequestMethods.Ftp.ListDirectory;
        //    request.Credentials = credentials;
        //    try
        //    {
        //        var response = (FtpWebResponse)(request.GetResponse());

        //        response.Close();
        //        return true;
        //    }
        //    catch (Exception exception)
        //    {
        //        return false;
        //    }
        //}
    }
}
