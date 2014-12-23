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
    class Program
    {

        public class SpellStatus
        {
            //read info in ini file
            public int range, order, dmgtype;
            public double ADper, APper, DmgLv1, DmgPer;
            public bool IsMissile, IsBlockable, IsNeedCalculate;

            //read info in Client
            public int level;
            public float manacost;
            //public SpellSlot slot;
        }

        //public static Obj_AI_Hero Player = ObjectManager.Player;
        public static String cName = "TwistedFate";//Player.baseSkinName
        public static DirectoryInfo dir = new DirectoryInfo(LeagueSharp.Common.Config.LeagueSharpDirectory.ToString() + "/JeonScriptor");
        public static FileInfo setFile = new FileInfo(dir + "/" + cName + ".ini");
        public static SpellStatus Q = new SpellStatus(), W = new SpellStatus(), E = new SpellStatus(), R = new SpellStatus();

        static void Main(string[] args)
        {
            Console.WriteLine("dir : " + dir.FullName);

            if (!dir.Exists)
                dir.Create();

            if (!setFile.Exists)
            {
                SetSpellstatus("Q");
                SetSpellstatus("W");
                SetSpellstatus("E");
                SetSpellstatus("R");
            }

            GetSpellstatus(Q, "Q");
            GetSpellstatus(W, "W");
            GetSpellstatus(E, "E");
            GetSpellstatus(R, "R");



            Console.WriteLine(Q.order);
            Console.ReadLine();
        }

        //public static double GetSpellDamage(SpellStatus Spell)
        //{

        //    #region get info
        //    float Player_bAD = Player.BaseAttackDamage;
        //    float Player_aAD = Player.FlatPhysicalDamageMod;
        //    float Player_totalAD = Player_bAD + Player_aAD;
        //    float Player_bAP = Player.BaseAbilityDamage;
        //    float Player_aAP = Player.FlatMagicDamageMod;
        //    float Player_totalAP = Player_bAP + Player_aAP;
        //    Obj_AI_Hero target = TargetSelector.GetSelectedTarget();
        //    #endregion

        //    if (Spell.IsNeedCalculate && Player.BaseSkinName == "Kalista" && Spell.slot == SpellSlot.E) // 칼리스타E - Kalista E
        //    {
        //        double eDmg = 0;
        //        foreach (var venoms in target.Buffs.Where(venoms => venoms.DisplayName == "KalistaExpungeMarker"))
        //        {
        //            int count = venoms.Count;
        //            double[] spell_basedamage = { 0, 20, 30, 40, 50, 60 };
        //            double[] spell_perdamage = { 0, 0.25, 0.30, 0.35, 0.40, 0.45 };
        //            eDmg = Player_totalAD * 0.60 + spell_basedamage[Spell.level];
        //            count -= 1;
        //            eDmg = eDmg + count * (eDmg * spell_perdamage[Spell.level]);
        //        }

        //        return Player.CalcDamage(target, Damage.DamageType.Physical, eDmg);
        //    }

        //    else if (Spell.IsNeedCalculate && Player.BaseSkinName == "Twitch" && Spell.slot == SpellSlot.E) // 트위치E - Twitch E
        //    {
        //        double eDmg = 0;
        //        foreach (var venoms in target.Buffs.Where(venoms => venoms.DisplayName == "TwitchDeadlyVenom"))
        //        {
        //            int count = venoms.Count;
        //            double[] spell_basedamage = { 0, 20, 35, 50, 65, 80 };
        //            double[] spell_stackdamage = { 0, 15, 20, 25, 30, 35 };
        //            count -= 1;
        //            eDmg = spell_basedamage[Spell.level] + count * (spell_stackdamage[Spell.level] + (Player_totalAP * 0.2) + (Player_totalAD * 0.25));
        //        }
        //        return Player.CalcDamage(target, Damage.DamageType.Physical, eDmg);
        //    }


        //    else
        //    {
        //        double[] spell_basedamage = {   Spell.DmgLv1,
        //                                    Spell.DmgLv1 + Spell.DmgPer,
        //                                    Spell.DmgLv1 + Spell.DmgPer * 2,
        //                                    Spell.DmgLv1 + Spell.DmgPer * 3,
        //                                    Spell.DmgLv1 + Spell.DmgPer * 4,
        //                                    Spell.DmgLv1 + Spell.DmgPer * 5
        //                                };

        //        double Dmg = Player_totalAD * Spell.APper + spell_basedamage[Spell.level];

        //        if (Spell.dmgtype == 1)
        //            return Player.CalcDamage(target, Damage.DamageType.Physical, Dmg);
        //        if (Spell.dmgtype == 2)
        //            return Player.CalcDamage(target, Damage.DamageType.Magical, Dmg);
        //        if (Spell.dmgtype == 3)
        //            return Player.CalcDamage(target, Damage.DamageType.True, Dmg);
        //        return 0;
        //    }
        //}

        public static void SetSpellstatus(string targetSpell)
        {
            Capture.SetSettingValue(targetSpell, "ComboOrder", "1", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "Range", "200", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "Damagetype", "1", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "IsMissile", "0", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "IsBlockable", "0", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "DmgLv1", "50", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "DmgPer", "30", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+AD(%)", "0.25", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+AP(%)", "0.45", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "=SpeacialList((:D))", "", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+EnemyAP(%)", "0", setFile.FullName); // Veiga_R
            Capture.SetSettingValue(targetSpell, "+MaxMana(%)", "0", setFile.FullName); // Ryze
            Capture.SetSettingValue(targetSpell, "+EnemyMaxHP(%)", "0", setFile.FullName); // Vayne, Garen R...
            Capture.SetSettingValue(targetSpell, "+EnemyCurHP(%)", "0", setFile.FullName); // Dr.Mundo
            Capture.SetSettingValue(targetSpell, "+EnemyMissHP(%)", "0", setFile.FullName); // LeeSin Q
            Capture.SetSettingValue(targetSpell, "+AP(%)Per100AP", "0", setFile.FullName); // NasusR, Amumu ...
            Capture.SetSettingValue(targetSpell, "IsNeedCalculate", "0", setFile.FullName);
        }
        public static void GetSpellstatus(SpellStatus targetSpell, String name)
        {
            targetSpell.order = Capture.GetSettingValue_Int(name, "ComboOrder", setFile.FullName);
            targetSpell.range = Capture.GetSettingValue_Int(name, "Range", setFile.FullName);
            targetSpell.dmgtype = Capture.GetSettingValue_Int(name, "Damagetype", setFile.FullName);
            targetSpell.IsMissile = Capture.GetSettingValue_Bool(name, "IsMissile", setFile.FullName);
            targetSpell.IsBlockable = Capture.GetSettingValue_Bool(name, "IsBlockable", setFile.FullName);
            targetSpell.ADper = Capture.GetSettingValue_Double(name, "+AD(%)", setFile.FullName) / 100f;
            targetSpell.APper = Capture.GetSettingValue_Double(name, "+AP(%)", setFile.FullName) / 100f;
            targetSpell.DmgLv1 = Capture.GetSettingValue_Double(name, "DmgLv1", setFile.FullName);
            targetSpell.DmgPer = Capture.GetSettingValue_Double(name, "DmgPer", setFile.FullName);
            targetSpell.IsNeedCalculate = Capture.GetSettingValue_Bool(name, "IsNeedCalculate", setFile.FullName);

            //targetSpell.slot = GetSpellSlotByString(name);
            //targetSpell.level = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).Level;
            //targetSpell.manacost = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).ManaCost;
        }
        public static SpellSlot GetSpellSlotByString(String temp)
        {
            if (temp == "Q")
                return SpellSlot.Q;
            if (temp == "W")
                return SpellSlot.Q;
            if (temp == "E")
                return SpellSlot.Q;
            if (temp == "R")
                return SpellSlot.Q;
            return SpellSlot.Unknown;
        }
    }
}
