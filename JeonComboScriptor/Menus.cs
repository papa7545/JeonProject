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
    class Menus:Program
    {

        public static void CreateMenu()
        {
            baseMenu = new Menu("JComboScriptor", "JComboScriptor", true);
            baseMenu.AddToMainMenu();


            var menu_ts = new Menu("TargetSelector", "TargetSelector");
            TargetSelector.AddToMenu(menu_ts);

            var menu_combo = new Menu("ComboInfo", "ComboInfo");
            var menu_q = new Menu("Q", "Q");
            var menu_w = new Menu("W", "W");
            var menu_e = new Menu("E", "E");
            var menu_r = new Menu("R", "R");

            var menu_q2 = new Menu("Q2", "Q2");
            var menu_w2 = new Menu("W2", "W2");
            var menu_e2 = new Menu("E2", "E2");


            var menu_comboset = new Menu("ComboSetting", "ComboSetting");
            menu_comboset.AddItem(new MenuItem("IgnorePrediction", "IgnorePrediction").SetValue(false));
            menu_comboset.AddItem(new MenuItem("HitChance", "HitChance").SetValue(new Slider(2, 1, 4)));
            menu_comboset.AddItem(new MenuItem("Combo_Key", "ComboKey : ").SetValue(new KeyBind(32, KeyBindType.Press)));



            baseMenu.AddSubMenu(menu_combo);
            baseMenu.AddSubMenu(menu_comboset);
            menu_combo.AddSubMenu(menu_q);
            menu_combo.AddSubMenu(menu_w);
            menu_combo.AddSubMenu(menu_e);
            menu_combo.AddSubMenu(menu_r);

            AddSpellSubmenu(menu_q, "Q", Q);
            AddSpellSubmenu(menu_w, "W", W);
            AddSpellSubmenu(menu_e, "E", E);
            AddSpellSubmenu(menu_r, "R", R);

            if(IsChangeable)
            {
                menu_combo.AddSubMenu(menu_q2);
                menu_combo.AddSubMenu(menu_w2);
                menu_combo.AddSubMenu(menu_e2);
                AddSpellSubmenu(menu_q2, "Q2", Q2);
                AddSpellSubmenu(menu_w2, "W2", W2);
                AddSpellSubmenu(menu_e2, "E2", E2);
            }



            var menu_misc = new Menu("Misc", "Misc");
            menu_misc.AddItem(SetMenuItem("Misc_combo", "Combo:" + Misc.textCombo));
            menu_misc.AddItem(SetMenuItem("Misc_DrawQ", "DrawQ - " + Misc.DrawQ));
            menu_misc.AddItem(SetMenuItem("Misc_DrawW", "DrawW - " + Misc.DrawW));
            menu_misc.AddItem(SetMenuItem("Misc_DrawE", "DrawE - " + Misc.DrawE));
            menu_misc.AddItem(SetMenuItem("Misc_DrawR", "DrawR - " + Misc.DrawR));

            baseMenu.AddSubMenu(menu_misc);

        }

        public static void AddSpellSubmenu(Menu menu,String spellslotname,SpellStatus spells)
        {
            if (spellslotname.Length == 1)
                menu.AddItem(SetMenuItem(spellslotname + "_name", "Name : " + spells.name[0]));
            else
                menu.AddItem(SetMenuItem(spellslotname + "_name", "Name : " + spells.name[1]));

            menu.AddItem(SetMenuItem(spellslotname + "_level", "Level : " + spells.level));
            menu.AddItem(SetMenuItem(spellslotname + "_Damagetype", "Damagetype : " + GetStringByDmgtype(spells.Damagetype)));
            menu.AddItem(SetMenuItem(spellslotname + "_Range", "Range : " + spells.Range));
            menu.AddItem(SetMenuItem(spellslotname + "_IsCharging", "IsCharging : " + spells.IsCharging));
            if (spells.IsCharging)
            {
                menu.AddItem(SetMenuItem(spellslotname + "_ChargingTime", "ChargingTime : " + spells.ChargingTime));
            }
            menu.AddItem(SetMenuItem(spellslotname + "_IsMissile", "IsMissile : " + spells.IsMissile));

            if (spells.IsMissile)
            {
                menu.AddItem(SetMenuItem(spellslotname + "_MissileType", "MissileType : " + ComboSpells.GetSStypeByByte(spells.MissileType).ToString().Replace("Skillshot", "")));
                menu.AddItem(SetMenuItem(spellslotname + "_MissileDelay", "MissileDelay  : " + spells.MissileDelay));
            }
            menu.AddItem(SetMenuItem(spellslotname + "_IsBlockable", "IsBlockable : " + spells.IsBlockable));


            SetMenuItem(spellslotname + "_DmgLv1", "DmgLv1 : ", spells.DmgLv1, menu);
            SetMenuItem(spellslotname + "_DmgPer", "DmgPer : ", spells.DmgPer, menu);
            SetMenuItem(spellslotname + "_totalAD", "totalAD : ", spells.totalAD, menu);
            SetMenuItem(spellslotname + "_addAD", "addAD : ", spells.addAD, menu);
            SetMenuItem(spellslotname + "_totalAP", "totalAP : ", spells.totalAP, menu);

            menu.AddItem(SetMenuItem(spellslotname + "noneuse", "----SpeacialList----"));

            SetMenuItem(spellslotname + "_EnemyAP", "EnemyAP : ", spells.totalAP, menu);
            SetMenuItem(spellslotname + "_MaxMana", "MaxMana : ", spells.totalAP, menu);
            SetMenuItem(spellslotname + "_EnemyMaxHP", "EnemyMaxHP : ", spells.EnemyMaxHP, menu);
            SetMenuItem(spellslotname + "_EnemyCurHP", "EnemyCurHP : ", spells.EnemyCurHP, menu);
            SetMenuItem(spellslotname + "_EnemyMissHP", "EnemyMissHP : ", spells.EnemyMissHP, menu);
            SetMenuItem(spellslotname + "_Per100AP", "Per100AP : ", spells.Per100AP, menu);
            menu.AddItem(SetMenuItem(spellslotname + "_IsNeedCalculate", "IsNeedCalculate : " + spells.IsNeedCalculate));
        }
        public static MenuItem SetMenuItem(string id,string name)
        {
            MenuItem temp = new MenuItem(id, name);
            return temp;
        }
        public static MenuItem SetMenuItem(string id, string name,double target,Menu menu)
        {
            if (!(target <= 0))
            {
                MenuItem temp = new MenuItem(id, name + target);
                baseMenu.AddItem(temp);
                return temp;
            }
            else
                return null;
        }
        private static string GetStringByDmgtype(int i)
        {
            switch(i){
                case 1:
                    return "AD";
                case 2:
                    return "AP";
                case 3:
                    return "TRUE";
                default:
                    return "Error";
            }
        }
        public static HitChance GetHitchanceByInt(int name)
        {
            switch (name)
            {
                case 1:
                    return HitChance.Low;
                case 2:
                    return HitChance.Medium;
                case 3:
                    return HitChance.High;
                case 4:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Low;
            }
        }
    }
}
//public static void link_menuitem2var()
//{
//    foreach (var item in MenuList)
//    {
//        if (item.Name.First() == 'Q')
//            link(Q, "Q");
//        if (item.Name.First() == 'W')
//            link(W, "W");
//        if (item.Name.First() == 'E')
//            link(E, "E");
//        if (item.Name.First() == 'R')
//            link(R, "R");

//        if (item.Name.IndexOf("Q2") != -1)
//            link(Q2, "Q2");
//        if (item.Name.IndexOf("W2") != -1)
//            link(W2, "W2");
//        if (item.Name.IndexOf("E2") != -1)
//            link(E2, "E2");
//        if (item.Name.IndexOf("R2") != -1)
//            link(R2, "R2");
//    }


//}
//private static void link(SpellStatus Spell,String SlotName)
//{
//    Spell.IsCharging = baseMenu.Item(SlotName+"_IsCharging").GetValue<bool>();
//    Spell.IsMissile =baseMenu.Item(SlotName+"_IsMissile").GetValue<bool>();
//    Spell.IsBlockable = baseMenu.Item(SlotName + "_IsBlockable").GetValue<bool>();
//    Spell.IsIgnorePrediction = baseMenu.Item(SlotName + "_IsIgnorePrediction").GetValue<bool>();
//    Spell.IsNeedCalculate = baseMenu.Item(SlotName + "_IsNeedCalculate").GetValue<bool>();
//}
