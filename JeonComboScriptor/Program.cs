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

namespace JeonComboScriptor
{
    class Program:CaptureLib
    {
        /*
         * Process :
         *  
         * 
         */

        public class SpellStatus
        {
            //read info in ini file
            public string[] name = {"",""};
            public int Range, Damagetype;
            public double DmgLv1, DmgPer, totalAD, addAD, totalAP, EnemyAP, MaxMana, EnemyMaxHP, EnemyCurHP, EnemyMissHP, Per100AP;
            public bool IsCharging,IsMissile, IsBlockable, IsIgnorePrediction,IsNeedCalculate;

            //read info in Client
            public int level;
            public float manacost;
            //public SpellSlot slot;
        }
        public class MiscStatus
        {
            public byte[] Combo={};
            public string textCombo;
            public bool DrawQ, DrawW, DrawE, DrawR,DrawCombo;
        }

        public static String cName = "TwistedFate";//Player.baseSkinName
        public static String[] ChangeableHero = {
                                                    "LeeSin","Elise","Jayce","Nidalee","RekSai"
                                                };
        public static Bool IsChangeable = ChangeableHero.Contains(cName);
        public static DirectoryInfo dir = new DirectoryInfo(LeagueSharp.Common.Config.LeagueSharpDirectory.ToString() + "/JeonScriptor");
        public static FileInfo setFile = new FileInfo(dir + "/" + cName + ".ini");
        //public static Obj_AI_Hero Player = ObjectManager.Player;
        public static SpellStatus Q = new SpellStatus(), W = new SpellStatus(), E = new SpellStatus(), R = new SpellStatus();
        public static SpellStatus Q2 = new SpellStatus(), W2 = new SpellStatus(), E2 = new SpellStatus(), R2 = new SpellStatus();
        public static MiscStatus Misc = new MiscStatus();

        //public static Menu baseMenu;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }
        private static void OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color ='#33FFFF'>Jeon's ComboScriptor v1.0 </font>Loaded");
            #region File Stream
            if (!dir.Exists)
                dir.Create();

            if (!setFile.Exists)
            {
                Readini.SetSpellstatus("Q");
                Readini.SetSpellstatus("W");
                Readini.SetSpellstatus("E");
                Readini.SetSpellstatus("R");
                if (IsChangeable)
                {
                    Readini.SetSpellstatus("Q2");
                    Readini.SetSpellstatus("W2");
                    Readini.SetSpellstatus("E2");
                }
            }

            Readini.GetSpellstatus(ref Q, "Q");
            Readini.GetSpellstatus(ref W, "W");
            Readini.GetSpellstatus(ref E, "E");
            Readini.GetSpellstatus(ref R, "R");
            if (IsChangeable)
            {
                Readini.GetSpellstatus(ref Q2, "Q2");
                Readini.GetSpellstatus(ref W2, "W2");
                Readini.GetSpellstatus(ref E2, "E2");
            }
            #endregion
            //Menus.CreateMenu();
            //Game.OnGameUpdate += OnGameUpdate;
        }
        private static void OnGameUpdate(EventArgs args)
        {
            //Menus.link_menuitem2var(); --쓸까말까고민중

            //if(baseMenu.Item("ReloadSciprt").GetValue<bool>())
            //{
            //    Readini.SaveSpellstatus(Q,"Q");
            //    Readini.SaveSpellstatus(W, "W");
            //    Readini.SaveSpellstatus(E, "E");
            //    Readini.SaveSpellstatus(R, "R");
            //    Readini.SaveSpellstatus(Q2, "Q2");
            //    Readini.SaveSpellstatus(W2, "W2");
            //    Readini.SaveSpellstatus(E2, "E2");
            //}
        }
    }
}
