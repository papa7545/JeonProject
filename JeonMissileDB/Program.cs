using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace Jeon_MissileDB
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        /// 
        public static List<String> ChampionNames = new List<string>();
        public static List<String> SpellSlots = new List<string>();
        public static List<String> Types = new List<string>();
        public static List<String> Delays = new List<string>();
        public static List<String> Ranges = new List<string>();
        public static List<String> Widths = new List<string>();
        public static List<String> Speeds = new List<string>();
        public static List<cInfo> Infos = new List<cInfo>();


        public class cInfo
        {
            public string cName;
            public string SlotName, Type, Delay, Range, Width, Speed;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string Getdown = Path.GetTempPath() + Environment.TickCount +".txt";


            WebClient TxtClient = new WebClient();
            TxtClient.DownloadFile("https://raw.githubusercontent.com/papa7545/JeonProject/master/misiledb.txt", Getdown);

            var text = File.ReadLines(Getdown);

            foreach (var t in text.Where(tx => tx.StartsWith("ChampionName")))
            {
                ChampionNames.Add(t.Replace("ChampionName_", ""));
            }
            foreach (var t in text.Where(tx => tx.StartsWith("Slot_SpellSlot.")))
            {
                SpellSlots.Add(t.Replace("Slot_SpellSlot.", ""));
            }
            foreach (var t in text.Where(tx => tx.StartsWith("Type_")))
            {
                Types.Add(t.Replace("Type_SkillShotType.", ""));
            }
            foreach (var t in text.Where(tx => tx.StartsWith("Delay_")))
            {
                Delays.Add(t.Replace("Delay_", ""));
            }
            foreach (var t in text.Where(tx => tx.StartsWith("Range_")))
            {
                Ranges.Add(t.Replace("Range_", ""));
            }
            foreach (var t in text.Where(tx => tx.StartsWith("Radius_")))
            {
                Widths.Add(t.Replace("Radius_", ""));
            }
            foreach (var t in text.Where(tx => tx.StartsWith("MissileSpeed_")))
            {
                Speeds.Add(t.Replace("MissileSpeed_", ""));
            }


            for (int i = 0; true; i++)
            {
                try
                {
                    cInfo temp = new cInfo();
                    temp.cName = ChampionNames[i];
                    if (i >= 1 && ChampionNames[i - 1] == ChampionNames[i] && SpellSlots[i] == SpellSlots[i - 1])
                    {
                        temp.SlotName = SpellSlots[i] + "2";
                    }
                    else
                    {
                        temp.SlotName = SpellSlots[i];
                    }
                    temp.Type = Types[i];
                    temp.Delay = Delays[i];
                    temp.Range = Ranges[i];
                    temp.Width = Widths[i];
                    temp.Speed = Speeds[i];
                    Infos.Add(temp);
                }
                catch
                {
                    break;
                }
            }
 
            Application.Run(new Form1());

        }
    }
}
