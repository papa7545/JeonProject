#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
        // 목록 추가시 수정해야될 부분 SetSpellstatus,SaveSpellstatus,GetSpellstatus
        public static void SetSpellstatus(string targetSpell)
        {
            SetSettingValue(targetSpell, "Damagetype", "1", setFile.FullName);
            SetSettingValue(targetSpell, "IsCharged", "0", setFile.FullName);
            SetSettingValue(targetSpell, "ChargingTime", "0", setFile.FullName);
            SetSettingValue(targetSpell, "IsMissile", "0", setFile.FullName);
            SetSettingValue(targetSpell, "MissileType", "1", setFile.FullName);  // 1-Circle,2-Cone,3-Line
            SetSettingValue(targetSpell, "MissileDelay", "00", setFile.FullName);
            SetSettingValue(targetSpell, "IsBlockable", "0", setFile.FullName);
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
            SetSettingValue("Misc", "Combo", "1-2-3-4", setFile.FullName);
            SetSettingValue("Misc", "DrawQ", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawW", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawE", "1", setFile.FullName);
            SetSettingValue("Misc", "DrawR", "1", setFile.FullName);
        }

        
        public static void GetSpellstatus(ref SpellStatus targetSpell, String name)
        {
            targetSpell.Damagetype = (byte)GetSettingValue_Int(name, "Damagetype", setFile.FullName);
            targetSpell.IsCharging = GetSettingValue_Bool(name, "IsCharged", setFile.FullName);
            targetSpell.ChargingTime = GetSettingValue_Double(name, "ChargingTime", setFile.FullName);
            targetSpell.IsMissile = GetSettingValue_Bool(name, "IsMissile", setFile.FullName);
            targetSpell.MissileType = (byte)GetSettingValue_Int(name, "MissileType", setFile.FullName);
            targetSpell.MissileDelay = GetSettingValue_Int(name, "MissileDelay", setFile.FullName);
            targetSpell.IsBlockable = GetSettingValue_Bool(name, "IsBlockable", setFile.FullName);
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

            targetSpell.slot = GetSpellSlotByString(name);
            targetSpell.level = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).Level;
            targetSpell.manacost = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).ManaCost;
            
            GetSpellRange(ref targetSpell);

            targetSpell.name[0] = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).Name.Replace(Player.ChampionName,"");
            targetSpell.name[1] = GetChangeableSpellName(targetSpell.slot);

        }
        public static void GetMisc()
        {
            Misc.textCombo = GetSettingValue_String("Misc", "Combo", setFile.FullName)
                .Replace("1", "Q").Replace("2", "W").Replace("3", "E").Replace("4", "R")
                .Replace("5", "Q2").Replace("6","W2").Replace("7","E2");
            Misc.DrawQ = GetSettingValue_Bool("Misc", "DrawQ", setFile.FullName);
            Misc.DrawW = GetSettingValue_Bool("Misc", "DrawW", setFile.FullName);
            Misc.DrawE = GetSettingValue_Bool("Misc", "DrawE", setFile.FullName);
            Misc.DrawR = GetSettingValue_Bool("Misc", "DrawR", setFile.FullName);

            Misc.Combo = Misc.textCombo.Split('-');
        }
        public static void GetSpellRange(ref SpellStatus targetSpell)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).SData.CastRangeDisplayOverride[0] <= 0)
            {
                if (ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).SData.CastRange[0] <= 0)
                {
                    targetSpell.Range =
                        (int)ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).SData.CastRadius[0];
                }
                else
                {
                    if (!targetSpell.IsCharging)
                        targetSpell.Range =
                            (int)ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).SData.CastRange[0];
                    else
                        targetSpell.Range =
                            (int)ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).SData.CastRadius[0];
                }
            }
            else
                targetSpell.Range =
                    (int)ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).SData.CastRangeDisplayOverride[0];
        }
        private static SpellSlot GetSpellSlotByString(String temp)
        {
            if (temp == "Q")
                return SpellSlot.Q;
            if (temp == "W")
                return SpellSlot.W;
            if (temp == "E")
                return SpellSlot.E;
            if (temp == "R")
                return SpellSlot.R;
            return SpellSlot.Unknown;
        }

        private static string GetStringByBool(Bool temp)
        {
            if (!temp)
                return "0";
            else
                return "1";
        }
        private static string GetChangeableSpellName(SpellSlot slot)
        {
            string name = "LeeSin"; //"LeeSin","Elise","Jayce","Nidalee","RekSai"
            if (cName == name && slot == SpellSlot.Q)
                return "ResonatingStrike";
            if (cName == name && slot == SpellSlot.W)
                return "IronWill";
            if (cName == name && slot == SpellSlot.E)
                return "Cripple";

            name = "Elise";
            if (cName == name && slot == SpellSlot.Q)
                return "ResonatingStrike";
            if (cName == name && slot == SpellSlot.W)
                return "SkitteringFrenzy";
            if (cName == name && slot == SpellSlot.E)
                return "Rappel";

            name = "Jayce";
            if (cName == name && slot == SpellSlot.Q)
                return "ShockBlast";
            if (cName == name && slot == SpellSlot.W)
                return "HyperCharge";
            if (cName == name && slot == SpellSlot.E)
                return "AccelerationGate";

            name = "Nidalee";
            if (cName == name && slot == SpellSlot.Q)
                return "Takedown";
            if (cName == name && slot == SpellSlot.W)
                return "Pounce";
            if (cName == name && slot == SpellSlot.E)
                return "Swipe";

            name = "RekSai";
            if (cName == name && slot == SpellSlot.Q)
                return "PreySeeker";
            if (cName == name && slot == SpellSlot.W)
                return "Unburrow";
            if (cName == name && slot == SpellSlot.E)
                return "Tunnel";



            return "";
        }

    }
}
