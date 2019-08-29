using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Reportman.Models;
using Reportman.Utils;

namespace Reportman
{
    public class Requester
    {
        public Requester()
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            url = config.AppSettings.Settings["url"].Value;
            clientName = config.AppSettings.Settings["name"].Value;
            clientCode = config.AppSettings.Settings["code"].Value;
            if(config.AppSettings.Settings["uuid"] != null)
            {
                clientUUID = config.AppSettings.Settings["uuid"].Value;
            }

        }
        Configuration config;
        private string url;
        private string clientName;
        private string clientCode;
        private string clientUUID;
        private string clientPassword;
        public int heartBeatInterval;
        //private string 
        private string SyncPost(string url,string api,IDictionary<string,string> allParams)
        {
            Uri address = new Uri($"{url}/{api}");
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var data = string.Join("&", allParams.Select(p => $"{p.Key}={p.Value.AESEncrypt().ToDataString()}"));

            var bytes = UTF8Encoding.UTF8.GetBytes(data);
            request.ContentLength = bytes.Length;
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(bytes, 0, bytes.Length);
            }
            // WebService反馈
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string xml = reader.ReadToEnd();//MyAES.AESDecrypt(MyAES.StringToBytes(reader.ReadToEnd()));
                Log.Debug(xml);
                xml = xml.ToBytes().AESDecrypt();
                reader.Close();
                return xml;
            }
        }

        private T ParseXml<T>(string xml)
        {
            using(StringReader sr = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T obj = (T)serializer.Deserialize(sr);
                return obj;
            }
        }
        public string GetUUID()
        {            
            if (!string.IsNullOrEmpty(clientUUID))
                return clientUUID;
            else
            {
                Log.Info("获取UUID");
                var str = SyncPost(url, "GetUUID", new Dictionary<string, string>() { { "clientName", clientName }, { "clientCode", clientCode } });
                Result.UUID ret = ParseXml<Result.UUID>(str);
                Log.Info(str);
                if (ret.code == 0)
                {
                    clientUUID = ret.uuid;
                    config.AppSettings.Settings.Add("uuid", clientUUID);
                    config.Save();
                }
                else
                {
                    Log.Info($"获取UUID失败,{ret.msg}");
                }
            }
            return clientUUID;
        }

        public string GetPassword()
        {
            if (!string.IsNullOrEmpty(clientPassword))
                return clientPassword;
            var param = new Dictionary<string, string>()
            {
                { "clientName",clientName},
                { "clientCode",clientCode},
                { "clientUUID",GetUUID()}
            };
            Log.Info("获取登录密码");
            var str = SyncPost(url, "GetPassword", param);
            Log.Info($"登录密码 {str}");
            Result.Password ret = ParseXml<Result.Password>(str);
            if (ret.code == 0)
                clientPassword = ret.pw;
            return clientPassword;
        }

        public int Login()
        {
            var param = new Dictionary<string, string>()
            {
                { "clientName",clientName},
                { "clientCode",clientCode},
                { "clientUUID",GetUUID()},
                { "clientPassword",GetPassword()}
            };
            Log.Info("登录");
            var str = SyncPost(url, "Login", param);
            Log.Info($"登录结果 {str}");
            Result.Login ret = ParseXml<Result.Login>(str);
            if (ret.code == 0)
                heartBeatInterval = ret.heartbeattime;
            return ret.code;
        }


        public int SendHeartBeat()
        {
            var param = new Dictionary<string, string>()
            {
                { "clientName",clientName},
                { "clientCode",clientCode},
                { "clientUUID",GetUUID()},
                { "clientPassword",GetPassword()}
            };
            Log.Info("发送心跳");
            var str = SyncPost(url, "SendHeartBeat", param);
            Log.Info($"心跳结果 {str}");
            Result.Common ret = ParseXml<Result.Common>(str);
            return ret.code;
        }

        public Result.BuildData GetBuildingData()
        {
            var param = new Dictionary<string, string>()
            {
                { "clientName",clientName},
                { "clientCode",clientCode},
                { "clientUUID",GetUUID()},
                { "clientPassword",GetPassword()}
            };
            Log.Info("获取发送信息");
            var str= SyncPost(url, "GetBuildingData", param);
            Log.Info($"获取结果 {str}");
            Result.BuildData data = ParseXml<Result.BuildData>(str);
            return data;
        }

        public int SendMeterData(string data)
        {
            var param = new Dictionary<string, string>()
            {
                { "clientName",clientName},
                { "clientCode",clientCode},
                { "clientUUID",GetUUID()},
                { "clientPassword",GetPassword()},
                { "data",data }
            };
            Log.Info("发送数据");
            var str = SyncPost(url, "SendMeterData", param);
            Log.Info($"发送结果 {str}");
            Result.Common ret = ParseXml<Result.Common>(str);
            return ret.code;
        }
    }
}
