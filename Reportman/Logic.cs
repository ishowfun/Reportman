using Reportman.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Reportman.Models;
using System.Xml.Serialization;
using System.IO;

namespace Reportman
{
    public class Logic:IDisposable
    {
        private Requester Requester;
        private Thread Heartbeat;
        private Thread MainThread;
        private bool Running = false;
        private bool Logined = false;
        private AutoResetEvent ExitMain;
        private AutoResetEvent ExitHB;
        private int MainInterval;
        public Logic()
        {
            Requester = new Requester();
            ExitMain = new AutoResetEvent(false);
            ExitHB = new AutoResetEvent(false);
            MainInterval = int.Parse(ConfigurationManager.AppSettings["mainInterval"]);
        }       

        public void Start()
        {
            MainThread = new Thread(new ThreadStart(() =>
            {
                while(Running)
                {
                    var bds = Requester.GetBuildingData();
                    if (bds.code == 0)
                    {
                        foreach (var b in bds.builds)
                        {
                            string ids = string.Join(",", b.Meters.Select(m => $"'{m.value}'"));
                            string sql = $"SELECT* FROM(SELECT Result, Time, MeterNumber FROM NingDaSjy.dbo.WaterMeter WHERE MeterNumber IN ({ids})) a union SELECT* FROM(SELECT Result, Time, MeterNumber FROM NingDaSjy.dbo.ElectricMeter WHERE MeterNumber IN ({ids})) b ORDER BY MeterNumber asc;";
                            try
                            {
                                DataTable t = DBHelper.ExecuteReader(sql);
                                Result.SendBuilds sb = new Result.SendBuilds();
                                sb.build = new Result.SendBuilding();
                                sb.build.id = b.id;
                                sb.build.Meters = new Result.SendMeter[t.Rows.Count];
                                var ms = b.Meters.OrderBy(m => m.value);
                                for (int i = 0; i < t.Rows.Count; ++i)
                                {
                                    //Log.Info(i.ToString());
                                    string id = ms.ElementAt(i).id;
                                    sb.build.Meters[i] = new Result.SendMeter();
                                    sb.build.Meters[i].id = id;
                                    sb.build.Meters[i].extid = ms.ElementAt(i).value;
                                    sb.build.Meters[i].time = ((DateTime)t.Rows[i]["Time"]).ToString();
                                    sb.build.Meters[i].value = (string)t.Rows[i]["Result"];
                                }
                                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Result.SendBuilds));
                                StringBuilder stringBuilder = new StringBuilder();
                                StringWriter stringWriter = new StringWriter(stringBuilder);
                                xmlSerializer.Serialize(stringWriter, sb);
                                Requester.SendMeterData(stringBuilder.ToString());
                            }
                            catch(Exception ex)
                            {
                                Log.Error($"{ex.Message}\n{ex.StackTrace}");
                            }
                        }                        
                    }
                    else if(bds.code == 0004)
                    {
                        Requester.Login();
                    }
                    if(ExitMain.WaitOne(MainInterval))
                    {
                        break;
                    }
                    //Thread.Sleep(MainInterval);
                }
            }));
            Heartbeat = new Thread(new ThreadStart(() =>
            {
                while(Running)
                {
                    if(ExitHB.WaitOne(Requester.heartBeatInterval * 1000))
                    {
                        break;
                    }
                    //Thread.Sleep(Requester.heartBeatInterval * 1000);
                    if(Logined)
                    {
                        if(Requester.SendHeartBeat() != 0)
                        {
                            Logined = Requester.Login() == 0;
                        }
                        continue;
                    }
                    if ( Requester.Login() != 0)
                    {
                        continue;
                    }
                    else
                    {
                        Logined = true;                
                        MainThread.Start();                        
                    }                    
                }
            }));
            Running = true;
            Heartbeat.Start();            
        }

        public void Stop()
        {
            Running = false;
            ExitMain.Set();
            ExitHB.Set();
            Heartbeat.Join();
            MainThread.Join();
        }
        public void Dispose()
        {
            Stop();
        }
    }
}
