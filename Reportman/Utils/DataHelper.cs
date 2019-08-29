using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reportman.Utils
{
    public static class DataHelper
    {
        public static string ToDataString(this byte[] bt)
        {
            string str = "";
            foreach (var a in bt)
            {
                if (str != "")
                {
                    str += "-";
                }
                str += a.ToString();
            }
            return str;
        }

        public static byte[] ToBytes(this string str)
        {
            string[] strArray = str.Split('-');
            int len = strArray.Length;
            byte[] bt = new byte[len];
            for (int i = 0; i < len; i++)
            {
                bt[i] = Convert.ToByte(strArray[i]);
            }
            return bt;
        }

    }
}
