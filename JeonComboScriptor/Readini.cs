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
    class Readini:Program
    {
        public static void SetSpellstatus(string targetSpell)
        {
            SetSettingValue(targetSpell, "Range", "100", setFile.FullName);
            SetSettingValue(targetSpell, "Damagetype", "1", setFile.FullName);
            SetSettingValue(targetSpell, "IsCharging", "0", setFile.FullName);
            SetSettingValue(targetSpell, "IsMissile", "0", setFile.FullName);
            SetSettingValue(targetSpell, "IsBlockable", "0", setFile.FullName);
            SetSettingValue(targetSpell, "IsIgnorePrediction", "0", setFile.FullName);
            SetSettingValue(targetSpell, "DmgLv1", "00", setFile.FullName);
            SetSettingValue(targetSpell, "DmgPer", "00", setFile.FullName);
            SetSettingValue(targetSpell, "+totalAD(%)", "0.00", setFile.FullName);
            SetSettingValue(targetSpell, "+addAD(%)", "0.00", setFile.FullName);
            SetSettingValue(targetSpell, "+totalAP(%)", "0.00", setFile.FullName);

            SetSettingValue(targetSpell, "=SpeacialList(:D)", "", setFile.FullName);
            SetSettingValue(targetSpell, "+EnemyAP(%)", "0.00", setFile.FullName); // Veiga_R
            SetSettingValue(targetSpell, "+MaxMana(%)", "0.00", setFile.FullName); // Ryze
            SetSettingValue(targetSpell, "+EnemyMaxHP(%)", "0.00", setFile.FullName); // Vayne, Garen R...
            SetSettingValue(targetSpell, "+EnemyCurHP(%)", "0.00", setFile.FullName); // Dr.Mundo
            SetSettingValue(targetSpell, "+EnemyMissHP(%)", "0.00", setFile.FullName); // LeeSin Q
            SetSettingValue(targetSpell, "+AP(%)Per100AP", "0.00", setFile.FullName); // NasusR, Amumu ...
            SetSettingValue(targetSpell, "IsNeedCalculate", "0", setFile.FullName);
        }
        public static void SetMisc()
        {
            SetSettingValue("Misc", "Combo", "1-2-3-4-1", setFile.FullName);
            SetSettingValue("Misc", "DrawQ", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawW", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawE", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawR", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawCombo", "1", setFile.FullName);
        }

        public static void SaveSpellstatus(SpellStatus spell,string targetSpell)
        {
            SetSettingValue(targetSpell, "Range", Convert.ToString(spell.Range), setFile.FullName);
            SetSettingValue(targetSpell, "Damagetype", Convert.ToString(spell.Range), setFile.FullName);
            SetSettingValue(targetSpell, "IsCharging", GetStringByBool(spell.IsCharging), setFile.FullName);
            SetSettingValue(targetSpell, "IsMissile", GetStringByBool(spell.IsMissile), setFile.FullName);
            SetSettingValue(targetSpell, "IsBlockable", GetStringByBool(spell.IsBlockable), setFile.FullName);
            SetSettingValue(targetSpell, "IsIgnorePrediction", GetStringByBool(spell.IsIgnorePrediction), setFile.FullName);
            SetSettingValue(targetSpell, "DmgLv1", Convert.ToString(spell.DmgLv1), setFile.FullName);
            SetSettingValue(targetSpell, "DmgPer", Convert.ToString(spell.DmgPer), setFile.FullName);
            SetSettingValue(targetSpell, "+totalAD(%)", Convert.ToString(spell.totalAD), setFile.FullName);
            SetSettingValue(targetSpell, "+addAD(%)", Convert.ToString(spell.addAD), setFile.FullName);
            SetSettingValue(targetSpell, "+totalAP(%)", Convert.ToString(spell.totalAP), setFile.FullName);

            SetSettingValue(targetSpell, "=SpeacialList(:D)", "", setFile.FullName);
            SetSettingValue(targetSpell, "+EnemyAP(%)", Convert.ToString(spell.EnemyAP), setFile.FullName); // Veiga_R
            SetSettingValue(targetSpell, "+MaxMana(%)", Convert.ToString(spell.MaxMana), setFile.FullName); // Ryze
            SetSettingValue(targetSpell, "+EnemyMaxHP(%)", Convert.ToString(spell.EnemyMaxHP), setFile.FullName); // Vayne, Garen R...
            SetSettingValue(targetSpell, "+EnemyCurHP(%)", Convert.ToString(spell.EnemyCurHP), setFile.FullName); // Dr.Mundo
            SetSettingValue(targetSpell, "+EnemyMissHP(%)", Convert.ToString(spell.EnemyMissHP), setFile.FullName); // LeeSin Q
            SetSettingValue(targetSpell, "+AP(%)Per100AP", Convert.ToString(spell.Per100AP), setFile.FullName); // NasusR, Amumu ...
            SetSettingValue(targetSpell, "IsNeedCalculate", GetStringByBool(spell.IsNeedCalculate), setFile.FullName);
        }
        public static void SaveMisc()
        {
            SetSettingValue("Misc", "Combo", "1-2-3-4-1", setFile.FullName);
            SetSettingValue("Misc", "DrawQ", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawW", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawE", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawR", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawCombo", "1", setFile.FullName);
        }

        public static void GetSpellstatus(ref SpellStatus targetSpell, String name)
        {
            targetSpell.Range = GetSettingValue_Int(name, "Range", setFile.FullName);
            targetSpell.Damagetype = GetSettingValue_Int(name, "Damagetype", setFile.FullName);
            targetSpell.IsCharging = GetSettingValue_Bool(name, "IsCharging", setFile.FullName);
            targetSpell.IsMissile = GetSettingValue_Bool(name, "IsMissile", setFile.FullName);
            targetSpell.IsBlockable = GetSettingValue_Bool(name, "IsBlockable", setFile.FullName);
            targetSpell.IsIgnorePrediction = GetSettingValue_Bool(name, "IsIgnorePrediction", setFile.FullName);
            targetSpell.DmgLv1 = GetSettingValue_Double(name, "DmgLv1", setFile.FullName);
            targetSpell.DmgPer = GetSettingValue_Double(name, "DmgPer", setFile.FullName);
            targetSpell.totalAD = GetSettingValue_Double(name, "+totalAD(%)", setFile.FullName);
            targetSpell.addAD = GetSettingValue_Double(name, "+addAD(%)", setFile.FullName);
            targetSpell.totalAP = GetSettingValue_Double(name, "+totalAP(%)", setFile.FullName);

            targetSpell.EnemyAP = GetSettingValue_Double(name, "+EnemyAP(%)", setFile.FullName);
            targetSpell.MaxMana = GetSettingValue_Double(name, "+MaxMana(%)", setFile.FullName);
            targetSpell.EnemyMaxHP = GetSettingValue_Double(name, "+EnemyMaxHP(%)", setFile.FullName);
            targetSpell.EnemyCurHP = GetSettingValue_Double(name, "+EnemyCurHP(%)", setFile.FullName);
            targetSpell.EnemyMissHP = GetSettingValue_Double(name, "+EnemyMissHP(%)", setFile.FullName);
            targetSpell.Per100AP = GetSettingValue_Double(name, "+AP(%)Per100AP", setFile.FullName);
            targetSpell.IsNeedCalculate = GetSettingValue_Bool(name, "IsNeedCalculate", setFile.FullName);

            //targetSpell.slot = GetSpellSlotByString(name);
            //targetSpell.level = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).Level;
            //targetSpell.manacost = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).ManaCost;
            
            //targetSpell.name[0] = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).Name;
            //targetSpell.name[1] = GetChangeableSpellName(targetSpell.slot);
        }
        public static void GetMisc()
        {
            GetSettingValue_Combo("Misc", "Combo", setFile.FullName);
            Misc.textCombo = GetSettingValue_String("Misc", "Combo", setFile.FullName);
            Misc.DrawQ = GetSettingValue_Bool("Misc", "DrawQ", setFile.FullName);
            Misc.DrawW = GetSettingValue_Bool("Misc", "DrawW", setFile.FullName);
            Misc.DrawE = GetSettingValue_Bool("Misc", "DrawE", setFile.FullName);
            Misc.DrawR = GetSettingValue_Bool("Misc", "DrawR", setFile.FullName);
            Misc.DrawCombo = GetSettingValue_Bool("Misc", "DrawCombo", setFile.FullName);


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

        public static string GetStringByBool(Bool temp)
        {
            if (!temp)
                return "0";
            else
                return "1";
        }
        public static string GetChangeableSpellName(SpellSlot slot)
        {
            string name = "LeeSin"; //"LeeSin","Elise","Jayce","Nidalee","RekSai"
            if (Player.BaseSkinName == name && slot == SpellSlot.Q)
                return "ResonatingStrike";
            if (Player.BaseSkinName == name && slot == SpellSlot.W)
                return "IronWill";
            if (Player.BaseSkinName == name && slot == SpellSlot.E)
                return "Cripple";

            name = "Elise";
            if (Player.BaseSkinName == name && slot == SpellSlot.Q)
                return "ResonatingStrike";
            if (Player.BaseSkinName == name && slot == SpellSlot.W)
                return "SkitteringFrenzy";
            if (Player.BaseSkinName == name && slot == SpellSlot.E)
                return "Rappel";

            name = "Jayce";
            if (Player.BaseSkinName == name && slot == SpellSlot.Q)
                return "ShockBlast";
            if (Player.BaseSkinName == name && slot == SpellSlot.W)
                return "HyperCharge";
            if (Player.BaseSkinName == name && slot == SpellSlot.E)
                return "AccelerationGate";

            name = "Nidalee";
            if (Player.BaseSkinName == name && slot == SpellSlot.Q)
                return "Takedown";
            if (Player.BaseSkinName == name && slot == SpellSlot.W)
                return "Pounce";
            if (Player.BaseSkinName == name && slot == SpellSlot.E)
                return "Swipe";

            name = "RekSai";
            if (Player.BaseSkinName == name && slot == SpellSlot.Q)
                return "PreySeeker";
            if (Player.BaseSkinName == name && slot == SpellSlot.W)
                return "Unburrow";
            if (Player.BaseSkinName == name && slot == SpellSlot.E)
                return "Tunnel";



            return "";
        }
    }
}
