#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion


namespace test
{
    class Program
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static String cName = Player.BaseSkinName; 
        public static DirectoryInfo dir = new DirectoryInfo(Config.LeagueSharpDirectory.ToString()+@"\JeonScriptor");
        public static FileInfo setFile = new FileInfo(dir + "/" + "ss"+ ".ini");

        public class SpellStatus
        {
            //read info in ini file
            public string[] name = { "", "" };
            public byte Damagetype, MissileType;
            public int Range, ChargingMin, ChargingMax, MissileDelay, MissileSpeed, MissileWidth;
            public double DmgLv1, DmgPer, totalAD, addAD, totalAP, EnemyAP, MaxMana, EnemyMaxHP, EnemyCurHP, EnemyMissHP, Per100AP,
                ChargingTime;
            public bool IsCharging, IsMissile, IsBlockable, IsIgnorePrediction, IsNeedCalculate;

            //read info in Client
            public int level;
            public float manacost;
            public SpellSlot slot;
        }
        public class MiscStatus
        {
            public byte[] Combo = { };
            public string textCombo;
            public bool DrawQ, DrawW, DrawE, DrawR, DrawCombo;
        }

        public static String[] ChangeableHero = {
                                                    "LeeSin","Elise","Jayce","Nidalee","RekSai"
                                                };
        public static Bool IsChangeable = ChangeableHero.Contains(Player.BaseSkinName);

        public static List<Spell> c_Spells = new List<Spell>();

        public static SpellStatus Q = new SpellStatus(), W = new SpellStatus(), E = new SpellStatus(), R = new SpellStatus();
        public static SpellStatus Q2 = new SpellStatus(), W2 = new SpellStatus(), E2 = new SpellStatus(), R2 = new SpellStatus();
        public static MiscStatus Misc = new MiscStatus();
        public static HitChance h_chance;

        public static Menu baseMenu;

        static void Main(string[] args)
        {
            dir.Create();
            setFile.Create();
            Game.PrintChat("good");
            Game.PrintChat(dir.FullName);
        }
    }
}
