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
            SetSettingValue(targetSpell, "ComboOrder", "0", setFile.FullName);
            SetSettingValue(targetSpell, "Range", "100", setFile.FullName);
            SetSettingValue(targetSpell, "Damagetype", "1", setFile.FullName);
            SetSettingValue(targetSpell, "IsMissile", "0", setFile.FullName);
            SetSettingValue(targetSpell, "IsBlockable", "0", setFile.FullName);
            SetSettingValue(targetSpell, "IsIgnorePrediction", "0", setFile.FullName);
            SetSettingValue(targetSpell, "DmgLv1", "00", setFile.FullName);
            SetSettingValue(targetSpell, "DmgPer", "00", setFile.FullName);
            SetSettingValue(targetSpell, "+totalAD(%)", "0.00", setFile.FullName);
            SetSettingValue(targetSpell, "+addAD(%)", "0.00", setFile.FullName);
            SetSettingValue(targetSpell, "+totalAP(%)", "0.00", setFile.FullName);
            SetSettingValue(targetSpell, "=SpeacialList((:D))", "", setFile.FullName);
            SetSettingValue(targetSpell, "+EnemyAP(%)", "0.00", setFile.FullName); // Veiga_R
            SetSettingValue(targetSpell, "+MaxMana(%)", "0.00", setFile.FullName); // Ryze
            SetSettingValue(targetSpell, "+EnemyMaxHP(%)", "0.00", setFile.FullName); // Vayne, Garen R...
            SetSettingValue(targetSpell, "+EnemyCurHP(%)", "0.00", setFile.FullName); // Dr.Mundo
            SetSettingValue(targetSpell, "+EnemyMissHP(%)", "0.00", setFile.FullName); // LeeSin Q
            SetSettingValue(targetSpell, "+AP(%)Per100AP", "0.00", setFile.FullName); // NasusR, Amumu ...
            SetSettingValue(targetSpell, "IsNeedCalculate", "0.00", setFile.FullName);
        }
        public static void GetSpellstatus(SpellStatus targetSpell, String name)
        {
            targetSpell.ComboOrder = GetSettingValue_Int(name, "ComboOrder", setFile.FullName);
            targetSpell.Range = GetSettingValue_Int(name, "Range", setFile.FullName);
            targetSpell.Damagetype = GetSettingValue_Int(name, "Damagetype", setFile.FullName);
            targetSpell.IsMissile = GetSettingValue_Bool(name, "IsMissile", setFile.FullName);
            targetSpell.IsBlockable = GetSettingValue_Bool(name, "IsBlockable", setFile.FullName);
            targetSpell.totalAD = GetSettingValue_Double(name, "+totalAD(%)", setFile.FullName);
            targetSpell.addAD = GetSettingValue_Double(name, "+addAD(%)", setFile.FullName);
            targetSpell.totalAP = GetSettingValue_Double(name, "+totalAP(%)", setFile.FullName);
            targetSpell.DmgLv1 = GetSettingValue_Double(name, "DmgLv1", setFile.FullName);
            targetSpell.DmgPer = GetSettingValue_Double(name, "DmgPer", setFile.FullName);
            targetSpell.IsNeedCalculate = GetSettingValue_Bool(name, "IsNeedCalculate", setFile.FullName);

            targetSpell.slot = GetSpellSlotByString(name);
            targetSpell.level = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).Level;
            targetSpell.manacost = ObjectManager.Player.Spellbook.GetSpell(targetSpell.slot).ManaCost;
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
