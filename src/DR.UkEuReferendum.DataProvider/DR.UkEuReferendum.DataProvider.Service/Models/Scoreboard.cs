using System;

namespace DR.UkEuReferendum.DataProvider.Service.Models
{
    public class Scoreboard
    {
        public Scoreboard()
        {
            UpdateId = 0;
            TotalCounsils = 0;
            DeclaredCounsils = 0;
            ParCode = "";
            RemainShare = "0";
            LeaveShare = "0";
        }

        public int UpdateId { get; set; }
        public int TotalCounsils { get; set; }
        public int DeclaredCounsils { get; set; }

        public string ParCode { get; set; }
        //public DateTime WinningMoment { get; set; }

        public string RemainShare { get; set; }
        public string LeaveShare { get; set; }
    }
}
