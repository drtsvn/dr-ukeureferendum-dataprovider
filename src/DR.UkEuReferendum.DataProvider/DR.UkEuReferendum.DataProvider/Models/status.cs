using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DR.UkEuReferendum.DataProvider.Models
{
    [XmlRoot(ElementName = "status", Namespace = "www.dr.dk/status")]
    public class Status
    {
        [XmlElement("applicationname")]
        public string ApplicationName { get; set; }
        [XmlElement("applicationstatus")]
        public StatusEnum Applicationstatus { get; set; }
        [XmlElement("timestamp")]
        public DateTime Timestamp { get; set; }
        [XmlElement("servername")]
        public string ServerName { get; set; }
        [XmlElement("check")]
        public List<Check> Checks { get; set; }
    }

    public class Check
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("status")]
        public StatusEnum Status { get; set; }
        [XmlElement("responseinms")]
        public double ResponseInMilliSeconds { get; set; }
        [XmlElement("message")]
        public string Message { get; set; }
    }

    public enum StatusEnum
    {
        OK,
        ERROR
    }
}
