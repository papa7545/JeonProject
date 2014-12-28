using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


namespace JeonHelperForDev
{
    class Program
    {
        public static Menu baseMenu;
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static int pastTime = 0;
        public static List<PositionInfo> positions = new List<PositionInfo>();


        public class PositionInfo
        {
            public Vector3 position;
            public Byte count;
        }


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }
        private static void OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color ='#33FFFF'>JeonHelperForDev v1.0 </font>Loaded!");

            baseMenu = new Menu("JeonHelper", "JeonHelper", true);
            baseMenu.AddToMainMenu();

            
            var menu_spell_me = new Menu("Spell Info(me)", "Spell Info(me)");
            baseMenu.AddSubMenu(menu_spell_me);
            menu_spell_me.AddItem(new MenuItem("item_spell_me_bool", "Active").SetValue<bool>(true));
            menu_spell_me.AddItem(new MenuItem("item_spell_me_q", "Q").SetValue<bool>(false));
            menu_spell_me.AddItem(new MenuItem("item_spell_me_q_key", "Key").SetValue<KeyBind>(new KeyBind('Q',KeyBindType.Toggle)));
            menu_spell_me.AddItem(new MenuItem("item_spell_me_w", "W").SetValue<bool>(false));
            menu_spell_me.AddItem(new MenuItem("item_spell_me_w_key", "Key").SetValue<KeyBind>(new KeyBind('W', KeyBindType.Toggle)));
            menu_spell_me.AddItem(new MenuItem("item_spell_me_e", "E").SetValue<bool>(false));
            menu_spell_me.AddItem(new MenuItem("item_spell_me_e_key", "Key").SetValue<KeyBind>(new KeyBind('E', KeyBindType.Toggle)));
            menu_spell_me.AddItem(new MenuItem("item_spell_me_r", "R").SetValue<bool>(false));
            menu_spell_me.AddItem(new MenuItem("item_spell_me_r_key", "Key").SetValue<KeyBind>(new KeyBind('R', KeyBindType.Toggle)));


            var menu_buff = new Menu("Buff Info", "Buff Info");
            baseMenu.AddSubMenu(menu_buff);
            menu_buff.AddItem(new MenuItem("show_buff_me", "Buff Info(me)").SetValue(false));
            menu_buff.AddItem(new MenuItem("show_buff_select", "Buff Info(select)").SetValue(false));
            menu_buff.AddItem(new MenuItem("show_buff_type_displayname", "Show Buff DisplayName").SetValue(true));
            menu_buff.AddItem(new MenuItem("delay", "Refresh Delay(sec):").SetValue(new Slider(2, 0, 10)));

            baseMenu.AddItem(new MenuItem("show_player", "Player Info").SetValue(false));

            var menu_positions = new Menu("DrawingPositions", "DrawingPositions");
            baseMenu.AddSubMenu(menu_positions);
            menu_positions.AddItem(new MenuItem("item_positions_key", "Key : ").SetValue<KeyBind>(new KeyBind('T', KeyBindType.Toggle)));
            menu_positions.AddItem(new MenuItem("item_positions_clear", "Clear").SetValue<bool>(false));
           

            var menu_ts = new Menu("TargetSelector", "TargetSelector");
            TargetSelector.AddToMenu(menu_ts);
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnDraw;

        }

        private static void OnGameUpdate(EventArgs args)
        {

            if(baseMenu.Item("show_player").GetValue<bool>())
            {
                Game.PrintChat("Play Champion : {0}", Player.BaseSkinName);
                Game.PrintChat("Position:{0}", Player.Position);
                baseMenu.Item("show_player").SetValue<bool>(false);
            }
            if (baseMenu.Item("show_buff_me").GetValue<bool>())
            {
                if (Environment.TickCount - pastTime < baseMenu.Item("delay").GetValue<Slider>().Value * 1000)
                    return;
                pastTime = Environment.TickCount;
                show_buff_me();
            }
            if (baseMenu.Item("show_buff_select").GetValue<bool>())
            {
                if (Environment.TickCount - pastTime < baseMenu.Item("delay").GetValue<Slider>().Value * 1000)
                    return;
                pastTime = Environment.TickCount;
                show_buff_select();
            }

            #region spell_me
            if (baseMenu.Item("item_spell_me_q").GetValue<bool>() || baseMenu.Item("item_spell_me_q_key").GetValue<KeyBind>().Active && baseMenu.Item("item_spell_me_bool").GetValue<bool>())
            {
                show_spell_me(SpellSlot.Q);
                baseMenu.Item("item_spell_me_q").SetValue<bool>(false);
                baseMenu.Item("item_spell_me_q_key").SetValue<KeyBind>(new KeyBind('Q', KeyBindType.Toggle));
            }
            if (baseMenu.Item("item_spell_me_w").GetValue<bool>() || baseMenu.Item("item_spell_me_w_key").GetValue<KeyBind>().Active && baseMenu.Item("item_spell_me_bool").GetValue<bool>())
            {
                show_spell_me(SpellSlot.W);
                baseMenu.Item("item_spell_me_w").SetValue<bool>(false);
                baseMenu.Item("item_spell_me_w_key").SetValue<KeyBind>(new KeyBind('W', KeyBindType.Toggle));
            }
            if (baseMenu.Item("item_spell_me_e").GetValue<bool>() || baseMenu.Item("item_spell_me_e_key").GetValue<KeyBind>().Active && baseMenu.Item("item_spell_me_bool").GetValue<bool>())
            {
                show_spell_me(SpellSlot.E);
                baseMenu.Item("item_spell_me_e").SetValue<bool>(false);
                baseMenu.Item("item_spell_me_e_key").SetValue<KeyBind>(new KeyBind('E', KeyBindType.Toggle));
            }
            if (baseMenu.Item("item_spell_me_r").GetValue<bool>() || baseMenu.Item("item_spell_me_r_key").GetValue<KeyBind>().Active && baseMenu.Item("item_spell_me_bool").GetValue<bool>())
            {
                show_spell_me(SpellSlot.R);
                baseMenu.Item("item_spell_me_r").SetValue<bool>(false);
                baseMenu.Item("item_spell_me_r_key").SetValue<KeyBind>(new KeyBind('R', KeyBindType.Toggle));
            }
            if (baseMenu.Item("item_positions_key").GetValue<KeyBind>().Active)
            {
                PositionInfo temp = new PositionInfo();
                temp.count = (byte)(positions.Count + 1);
                temp.position = Player.Position;
                positions.Add(temp);
                baseMenu.Item("item_positions_key").SetValue<KeyBind>(new KeyBind('T', KeyBindType.Toggle));
            }
            #endregion



        }

        private static void show_spell_me(SpellSlot slot)
        {
            SpellDataInst spell = Player.Spellbook.GetSpell(slot);
            SpellData data = Player.Spellbook.GetSpell(slot).SData;

            Game.PrintChat("<font color ='#FF2222'>_______Spell Info_____________________");
            Game.PrintChat(slot.ToString() + "-Name:{0}", spell.Name);
            Game.PrintChat(slot.ToString() + "-Level:{0} -CastName: {1}", spell.Level, data.Name);
            Game.PrintChat(slot.ToString() + "-Cooldown:{0}  -CastRange: {1}", spell.Cooldown, data.CastRange[3]);
            Game.PrintChat(slot.ToString() + "-MissileSpeed:{0} -CastRadius: {1}", data.MissileSpeed, data.CastRadius[3]);
            Game.PrintChat(slot.ToString() + "-MissileWith:{0} -CastDisplayRange: {1}", data.LineWidth, data.CastRangeDisplayOverride[3]);
            Game.PrintChat(slot.ToString() + "-SpellCastTime:{0} -SpellCastTotaltime: {1} , {2}", data.SpellCastTime,data.SpellTotalTime,data.CastFrame);
            Game.PrintChat("<font color ='#FF2222'>____________________________________");
        }
        private static void show_buff_me()
        {
            String temp = "";
            foreach (var buff in Player.Buffs)
            {
                if (baseMenu.Item("show_buff_type_displayname").GetValue<bool>())
                    temp += (buff.DisplayName + "(" + buff.Count + ")" + ", ");
                else
                    temp += (buff.Name + "(" + buff.Count + ")" + ", ");
            }

            Game.PrintChat("<font color ='#FF2222'>_______Buff(me) Info____________________");
            Game.PrintChat(temp);
            Game.PrintChat("<font color ='#FF2222'>____________________________________");
        }
        private static void show_buff_select()
        {
            String temp = "";
            foreach (var buff in TargetSelector.SelectedTarget.Buffs)
            {
                if (baseMenu.Item("show_buff_type_displayname").GetValue<bool>())
                    temp += (buff.DisplayName + "(" + buff.Count + ")" + ", ");
                else
                    temp += (buff.Name + "(" + buff.Count + ")" + ", ");
                
            }
            
            Game.PrintChat("<font color ='#FF2222'>_______Buff(target) Info____________________");
            Game.PrintChat(temp);
            Game.PrintChat("<font color ='#FF2222'>____________________________________");
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
                foreach (var missile in ObjectManager.Get<Obj_SpellLineMissile>())
                {


                    Vector2 startPos = Drawing.WorldToScreen(missile.SpellCaster.Position);
                    Vector2 endPos = Drawing.WorldToScreen(missile.EndPosition);
                    
                    
                    Drawing.DrawLine(startPos, endPos, 1.5f, System.Drawing.Color.Red);
                    

                    Utility.DrawCircle(missile.SpellCaster.Position, 100, System.Drawing.Color.Blue);
                    Utility.DrawCircle(missile.EndPosition, 100, System.Drawing.Color.Red);
                    Utility.DrawCircle(missile.StartPosition, 100, System.Drawing.Color.White);
                    
                }
        }
    }
}
