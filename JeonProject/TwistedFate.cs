#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
#endregion

namespace JeonProject
{


    class TwistedFate
    {

        public static String cards = "none";
        public static void AttachMenu(){

                var menu_TF = new Menu("menu_TF", "TwistedFate");
                Program.baseMenu.AddSubMenu(menu_TF);
                menu_TF.AddItem(new MenuItem("TF_Cardpicker", "Cardpicker").SetValue(true));
                menu_TF.AddItem(new MenuItem("TF_Goldkey", "GoldKey:").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                menu_TF.AddItem(new MenuItem("TF_Bluekey", "BlueKey:").SetValue(new KeyBind("E".ToCharArray()[0], KeyBindType.Press)));
                menu_TF.AddItem(new MenuItem("TF_Redkey", "RedKey").SetValue(new KeyBind("W".ToCharArray()[0], KeyBindType.Press)));
          
        }
        public static void Update()
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && cards =="none")
            {
                if (Program.baseMenu.Item("TF_Goldkey").GetValue<KeyBind>().Active)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                    cards = "gold";
                }
                if (Program.baseMenu.Item("TF_Bluekey").GetValue<KeyBind>().Active)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                    cards = "blue";
                }
                if (Program.baseMenu.Item("TF_Redkey").GetValue<KeyBind>().Active)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                    cards = "red";
                }

            }

            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock" && cards == "gold")
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                cards = "none";
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock" && cards == "blue")
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                cards = "none";
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock" && cards == "red")
            {
                ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                cards = "none";
            }

        }
    }
}
