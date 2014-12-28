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
    class Program : CaptureLib
    {
        /*
         * Process :
         *  
         * 
         */


        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Obj_AI_Hero SelectedC = null;
        public static String cName = Player.BaseSkinName;
        public static String[] ChangeableHero = {
                                                    "LeeSin","Elise","Jayce","Nidalee","RekSai"
                                                };
        public static Bool IsChangeable = ChangeableHero.Contains(cName);
        public static Bool IsCharging = false;
        public static double ChargingRange = 0,speed = 0;
        public static Bool ChargingRange_set= false;
        public static int pastTime;


        public static DirectoryInfo dir = new DirectoryInfo(Config.LeagueSharpDirectory.ToString() + @"\JeonScriptor");
        public static FileInfo setFile = new FileInfo(dir + "/" + cName + ".ini");

        public class SpellStatus
        {
            //read info in ini file
            public string[] name = { "", "" };
            public byte Damagetype, MissileType;
            public int Range, MissileDelay;
            public double DmgLv1, DmgPer, totalAD, addAD, totalAP, EnemyAP, MaxMana, EnemyMaxHP, EnemyCurHP, EnemyMissHP, Per100AP,
                ChargingTime;
            public bool IsCharging, IsMissile, IsBlockable, IsNeedCalculate;

            //read info in Client
            public int level;
            public float manacost;
            public SpellSlot slot;
        }
        public class MiscStatus
        {
            public String[] Combo = { };
            public bool DrawQ, DrawW, DrawE, DrawR;
            public string textCombo;
        }


        public static SpellStatus Q = new SpellStatus(), W = new SpellStatus(), E = new SpellStatus(), R = new SpellStatus();
        public static SpellStatus Q2 = new SpellStatus(), W2 = new SpellStatus(), E2 = new SpellStatus(), R2 = new SpellStatus();
        public static MiscStatus Misc = new MiscStatus();
        public static Spell s_Q,s_W, s_E, s_R;



        public static List<Spell> c_Spells = new List<Spell>();
        public static List<SpellSlot> spell_orderlist = new List<SpellSlot>();
        public static HitChance h_chance;
        public static byte spell_ordernum = 0;

        public static Menu baseMenu;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color ='#33FFFF'>Jeon's ComboScriptor v1.0 </font>Loaded");



            try
            {
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
                    Readini.SetMisc();
                }

                DoReadini();
                Menus.CreateMenu();
                #endregion
            }
            catch
            {
                Game.PrintChat("THERE ARE BUG! PLZ CHECK YOUR INI FILE");
            }


            ComboSpells.getComboSpells();
            ComboSpells.getComboSpellList();

            foreach (var spell in c_Spells)
            {
                if (spell.Slot == SpellSlot.Q)
                    s_Q = spell;
                if (spell.Slot == SpellSlot.W)
                    s_W = spell;
                if (spell.Slot == SpellSlot.E)
                    s_E = spell;
                if (spell.Slot == SpellSlot.R)
                    s_R = spell;
            }



            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnEndScene += OnDraw_EndScene;
            Game.OnWndProc += OnWndProc;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            h_chance = Menus.GetHitchanceByInt(baseMenu.Item("HitChance").GetValue<Slider>().Value);



            if (baseMenu.Item("Combo_Key").GetValue<KeyBind>().Active)
                ComboSpells.CastComboSpells(SelectedC);

        }


        private static void OnDraw_EndScene(EventArgs args)
        {
            if (Misc.DrawQ)
                Utility.DrawCircle(Player.Position, Q.Range, Color.White, 5, 20);
            if (Misc.DrawW)
                Utility.DrawCircle(Player.Position, W.Range, Color.MidnightBlue, 5, 20);
            if (Misc.DrawE)
                Utility.DrawCircle(Player.Position, E.Range, Color.OrangeRed, 5, 20);

            if (Misc.DrawR)
            {
                Utility.DrawCircle(Player.Position, R.Range, Color.Red, 1, 20);
                if (R.Range > 3000)
                    Utility.DrawCircle(Player.Position, R.Range, Color.White, 1, 20, true);
            }

            if (SelectedC != null && SelectedC.IsVisible && !SelectedC.IsDead)
                Utility.DrawCircle(SelectedC.Position, 75, Color.Red, 30, 10);

            Drawing.DrawCircle(Player.Position, (float)ChargingRange, Color.Red);
        }
        private static void OnWndProc(WndEventArgs args)
        {
            if (args.Msg == 513) // Mouse L click or R click
                SelectedC = GetSelectedTarget();
        }

        private static void DoReadini()
        {
            Readini.GetMisc();
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
        }

        public static Obj_AI_Hero GetSelectedTarget()
        {
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => 
                Vector3.Distance(hero.Position,Game.CursorPos) <200 && hero.IsEnemy && !hero.IsDead 
                && hero.IsValidTarget()))
            {
                    return enemy;
            }
            return null;
        }
    }
}