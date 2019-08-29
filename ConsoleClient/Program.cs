using Reportman;
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
    class Program
    {        
        static void Main(string[] args)
        {
            Logic logic = new Logic();
            logic.Start();
            Console.WriteLine("exit退出");
            while(true)
            {
                string key = Console.ReadLine();
                if (key == "exit")
                {
                    break;
                }
            }
            logic.Stop();
        }
    }
}
