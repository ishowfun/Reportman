using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Reportman.Models
{
    public class Result
    {
        [Serializable]
        [XmlRoot("root")]
        public class Common
        {
            [XmlElement("code")]
            public int code;
            [XmlElement("msg")]
            public string msg;
        }
        [Serializable]
        [XmlRoot("root")]
        public class UUID:Common
        {            
            [XmlElement("uuid")]
            public string uuid;
        }
        [Serializable]
        [XmlRoot("root")]
        public class Password:Common
        {           
            [XmlElement("pw")]
            public string pw;
        }
        [Serializable]
        [XmlRoot("root")]
        public class Login:Common
        {
            /// <summary>
            /// 单位秒
            /// </summary>
            [XmlElement("heartbeattime")]
            public int heartbeattime;
        }
        [Serializable]        
        public class Meter
        {
            [XmlAttribute("id")]
            public string id;
            [XmlText]
            public string value;
        }
        [Serializable]
        public class Building
        {
            [XmlAttribute("id")]
            public int id;
            [XmlElement("meter")]
            public Meter[] Meters;
        }
        [Serializable]
        [XmlRoot("root")]
        public class BuildData:Common
        {
            [XmlElement("building")]
            public Building[] builds;
        }
        [Serializable]
        [XmlRoot("root")]
        public class SendBuilds
        {
            [XmlElement("building")]
            public SendBuilding build;
        }
        [Serializable]
        public class SendBuilding
        {
            [XmlAttribute("id")]
            public int id;
            [XmlElement("meter")]
            public SendMeter[] Meters;
        }
        [Serializable]        
        public class SendMeter
        {
            [XmlAttribute("id")]
            public string id;
            [XmlAttribute("extid")]
            public string extid;
            [XmlAttribute("time")]
            public string time;
            [XmlText]
            public string value;
        }
    }
}
