#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
#endregion
namespace JeonChampions
{
    class Program
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static baseMenuItem m_Items = new baseMenuItem();
        public static Spell Q, W, E, R;
        public static String[] SupportChamps = { "Cassiopea", "Xerath" };
        public class baseMenuItem
        {
            public string COMBO_ACTIVE = "combo_active";
            public string HARASS_ACTIVE = "harass_active";
            public string COMBO_KEY = "combo_key";
            public string HARASS_KEY = "harass_key";
        }



        static void Main(string[] args)
        {
            if (SupportChamps.Contains(Jproject_base.Player.ChampionName))
            {
                if (Player.ChampionName == "Cassiopea")
                    Cassiopea._Cassiopea();
                if (Player.ChampionName == "Xerath")
                    Xerath._Xerath();
            }
            else
                Game.PrintChat("Jeon : There are not support your Champion("+Player.ChampionName+")");

        }
    }
}
