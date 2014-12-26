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
        public static List<MenuItem> MenuList = new List<MenuItem>();

        public static void CreateMenu()
        {
            baseMenu = new Menu("JCombo", "JCombo", true);
            baseMenu.AddToMainMenu();
            baseMenu.AddItem(new MenuItem("ReloadSciprt", "ReloadSciprt").SetValue(false));
            baseMenu.AddItem(new MenuItem("HitChange", "HitChange").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" })));
            baseMenu.AddItem(new MenuItem("ComboKey", "ComboKey").SetValue(new KeyBind(32, KeyBindType.Press)));

            var menu_combo = new Menu("ComboInfo", "ComboInfo");
            var menu_q = new Menu("Q", "Q");
            var menu_w = new Menu("W", "W");
            var menu_e = new Menu("E", "E");
            var menu_r = new Menu("R", "R");

            var menu_q2 = new Menu("Q2", "Q2");
            var menu_w2 = new Menu("W2", "W2");
            var menu_e2 = new Menu("E2", "E2");

            baseMenu.AddSubMenu(menu_combo);
            menu_combo.AddSubMenu(menu_q);
            menu_combo.AddSubMenu(menu_w);
            menu_combo.AddSubMenu(menu_e);
            menu_combo.AddSubMenu(menu_r);
            menu_combo.AddSubMenu(menu_q2);
            menu_combo.AddSubMenu(menu_w2);
            menu_combo.AddSubMenu(menu_e2);

            AddSpellSubmenu(menu_q, "Q", Q);
            AddSpellSubmenu(menu_w, "W", W);
            AddSpellSubmenu(menu_e, "E", E);
            AddSpellSubmenu(menu_e, "R", R);
            if(IsChangeable)
            {
                AddSpellSubmenu(menu_q2, "Q2", Q2);
                AddSpellSubmenu(menu_w2, "W2", W2);
                AddSpellSubmenu(menu_e2, "E2", E2);
            }



            var menu_misc = new Menu("Misc", "Misc");
            menu_misc.AddItem(SetMenuItem("Misc_combo", "Combo:").SetValue(Misc.textCombo));
            menu_misc.AddItem(SetMenuItem("Misc_DrawQ", "DrawQ").SetValue(Misc.DrawQ));
            menu_misc.AddItem(SetMenuItem("Misc_DrawW", "DrawW").SetValue(Misc.DrawW));
            menu_misc.AddItem(SetMenuItem("Misc_DrawE", "DrawE").SetValue(Misc.DrawE));
            menu_misc.AddItem(SetMenuItem("Misc_DrawR", "DrawR").SetValue(Misc.DrawR));

        }

        public static void AddSpellSubmenu(Menu menu,String spellslotname,SpellStatus spells)
        {
            if (spellslotname.Length == 1)
                menu.AddItem(SetMenuItem(spellslotname + "_name", "Name :").SetValue(spells.name[0]));
            else
                menu.AddItem(SetMenuItem(spellslotname + "_name", "Name :").SetValue(spells.name[1]));

            menu.AddItem(SetMenuItem(spellslotname + "_level", "Level :").SetValue(spells.level));
            menu.AddItem(SetMenuItem(spellslotname + "_Damagetype", "Damagetype :").SetValue(GetStringByDmgtype(spells.Damagetype)));
            menu.AddItem(SetMenuItem(spellslotname + "_Range", "Range :").SetValue(GetStringByDmgtype(spells.Range)));
            menu.AddItem(SetMenuItem(spellslotname + "_IsCharging", "IsCharging :").SetValue(spells.IsCharging));
            menu.AddItem(SetMenuItem(spellslotname + "_IsMissile", "IsMissile :").SetValue(spells.IsMissile));
            menu.AddItem(SetMenuItem(spellslotname + "_IsBlockable", "IsBlockable :").SetValue(spells.IsBlockable));
            menu.AddItem(SetMenuItem(spellslotname + "_IsIgnorePrediction", "IgnorePrediction :").SetValue(spells.IsIgnorePrediction));
            menu.AddItem(SetMenuItem(spellslotname + "_DmgLv1", "DmgLv1 :").SetValue(spells.DmgLv1));
            menu.AddItem(SetMenuItem(spellslotname + "_DmgPer", "DmgPer :").SetValue(spells.DmgPer));
            menu.AddItem(SetMenuItem(spellslotname + "_totalAD", "totalAD :").SetValue(spells.totalAD));
            menu.AddItem(SetMenuItem(spellslotname + "_addAD", "addAD :").SetValue(spells.addAD));
            menu.AddItem(SetMenuItem(spellslotname + "_totalAP", "totalAP :").SetValue(spells.totalAP));
            menu.AddItem(SetMenuItem("noneuse", "----SpeacialList----"));
            menu.AddItem(SetMenuItem(spellslotname + "_EnemyAP", "EnemyAP :").SetValue(spells.totalAP));
            menu.AddItem(SetMenuItem(spellslotname + "_MaxMana", "MaxMana :").SetValue(spells.totalAP));
            menu.AddItem(SetMenuItem(spellslotname + "_EnemyMaxHP", "EnemyMaxHP :").SetValue(spells.EnemyMaxHP));
            menu.AddItem(SetMenuItem(spellslotname + "_EnemyCurHP", "EnemyCurHP :").SetValue(spells.EnemyCurHP));
            menu.AddItem(SetMenuItem(spellslotname + "_EnemyMissHP", "EnemyMissHP :").SetValue(spells.EnemyMissHP));
            menu.AddItem(SetMenuItem(spellslotname + "_Per100AP", "Per100AP :").SetValue(spells.Per100AP));
            menu.AddItem(SetMenuItem(spellslotname + "_IsNeedCalculate", "IsNeedCalculate :").SetValue(spells.IsNeedCalculate));
        }
        public static MenuItem SetMenuItem(string id,string name)
        {
            MenuItem temp = new MenuItem(id, name);
            MenuList.Add(temp);
            return temp;
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
