#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

namespace JeonTF
{
    class Program
    {

        public static Menu menu_TF;
        public static String cards= "none";
        public static int stack=0;
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Game.OnGameUpdate += Update;
            Drawing.OnEndScene += OnDraw_EndScene;
        }

        public static void OnGameLoad(EventArgs args)
        {

            menu_TF = new Menu("JeonTF", "JeonTF",true);
            menu_TF.AddToMainMenu();
            var drawing = new Menu("drawing", "drawing");
            menu_TF.AddSubMenu(drawing);
            menu_TF.AddItem(new MenuItem("TF_Cardpicker", "Cardpicker").SetValue(true));
            menu_TF.AddItem(new MenuItem("TF_Goldkey", "GoldKey:").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            menu_TF.AddItem(new MenuItem("TF_Bluekey", "BlueKey:").SetValue(new KeyBind("E".ToCharArray()[0], KeyBindType.Press)));
            menu_TF.AddItem(new MenuItem("TF_Redkey", "RedKey").SetValue(new KeyBind("W".ToCharArray()[0], KeyBindType.Press)));

            drawing.AddItem(new MenuItem("TF_qRange", "Q-Range").SetValue(true));
            drawing.AddItem(new MenuItem("TF_rRange", "R-Range").SetValue(true));
        }

        public static void Update(EventArgs args)
        {
            if (stack >= 3)
            {
                cards = "none";
                stack = 0;
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).State.ToString() == "Cooldown")
            {
                stack++;
                Game.PrintChat("cool");
            }
           if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && cards == "none"
               && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).State.ToString() == "Ready")
           {
                if (menu_TF.Item("TF_Goldkey").GetValue<KeyBind>().Active)
                {
                    cards = "gold";
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                }
                if (menu_TF.Item("TF_Bluekey").GetValue<KeyBind>().Active)
                {
                    cards = "blue";
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
                }
                if (menu_TF.Item("TF_Redkey").GetValue<KeyBind>().Active)
                {
                    cards = "red";
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W);
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
        public static void OnDraw_EndScene(EventArgs args)
        {
            if (menu_TF.Item("TF_rRange").GetValue<bool>())
            {
                Utility.DrawCircle(ObjectManager.Player.Position, 5500, Color.White, 1, 20, true);
                Utility.DrawCircle(ObjectManager.Player.Position, 5500, Color.White, 1, 20);
            }
            if (menu_TF.Item("TF_qRange").GetValue<bool>())
            {
                Utility.DrawCircle(ObjectManager.Player.Position, 1450, Color.White, 1, 20);
            }

        }
    }
}
