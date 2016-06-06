using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace DR.UkEuReferendum.DataProvider.Service.Models
{
    [XmlRoot(ElementName = "refScoreboard")]
    public class ReferendumScoreboard
    {
        [XmlElement("scoreboard")]
        public ScoreboardBasis Scoreboard { get; set; }

        [XmlElement("ref_scoreboard_result")]
        public ScoreboardResult ScoreboardResult { get; set; }
    }

    public class ScoreboardBasis
    {
        [XmlElement("total_councils"), DefaultValue(700000000)]
        public int TotalCounsils { get; set; }

        [XmlElement("declared_councils")]
        public int DeclaredCounsils { get; set; }

        [XmlElement("par_code_now")]
        public string ParCode { get; set; }

        [XmlElement("winning_moment_ts")]
        public string WinningMoment { get; set; }
    }
    
    public class ScoreboardResult
    {     
        [XmlElement("share_remain_now")]
        public string _remainShare { get; set; }

        public decimal RemainShare
        {
            get
            {
                decimal retval;
                return !string.IsNullOrWhiteSpace(_remainShare) && decimal.TryParse(_remainShare, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-GB"), out retval) ? retval : 0.0m;
            }
        }

        [XmlElement("share_leave_now")]
        public string _leaveShare { get; set; }

        public decimal LeaveShare
        {
            get
            {
                decimal retval;
                return !string.IsNullOrWhiteSpace(_leaveShare) && decimal.TryParse(_leaveShare, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-GB"), out retval) ? retval : 0.0m;
            }
        }
    }
}
