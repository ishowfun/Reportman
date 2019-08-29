using Reportman.Utils;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleClient
{
    public class Program
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
        public class ttt
        {
            [XmlElement("code")]
            public int code;
        }
        [Serializable]
        [XmlRoot("root")]
        public class tmp:ttt
        {            
            [XmlElement("msg")]
            public string msg;
            [XmlElement("uuid")]
            public string uuid;
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
        public class BuildData : Common
        {
            [XmlElement("building")]
            public Building[] builds;
        }
        static void Main(string[] args)
        {

            try
            {

                string str = "<root><code> 0000 </code><msg> 正常 </msg><building id = \"17\"><meter id = \"128\" > 10 </meter><meter id = \"129\" > 11 </meter></building><building id = \"18\"><meter id = \"130\" > 12 </meter><meter id = \"131\" > 13 </meter><meter id = \"132\" > 14 </meter></building></root>";
                StringReader stringReader = new StringReader(str);
                XmlSerializer serializer = new XmlSerializer(typeof(BuildData));
                var t = (BuildData)serializer.Deserialize(stringReader);
                
                Console.WriteLine(t.code);
            }
            catch(Exception ex)
            {

            }
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings["test"] == null)
                Console.Write("ssss");

            Console.ReadKey();
        }
    }
}
