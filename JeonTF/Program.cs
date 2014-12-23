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
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
#endregion

namespace JeonTF
{
    class Program
    {

        public static Menu baseMenu;
        public static Obj_AI_Hero Player = ObjectManager.Player;


        public static card cards= card.none;
        public enum card {
            none,
            blue,
            red,
            gold
        }

        public static int stack=0;
        public static int pastTime = 0;
        public static int delayi = 0;
        public static int x = 0;





        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        public enum test
        {
            show_inventory,
            show_enemybuff,
            show_allybuff,
            show_mebuff
        }


        public static void OnGameLoad(EventArgs args) 
        {
            baseMenu = new Menu("JeonTF", "JeonTF",true);
            baseMenu.AddToMainMenu();

            var menu_q = new Menu("Q-Wild Cards", "Q-Wild Cards");
            var menu_autopicker = new Menu("W-Pick A Card", "W-Pick A Card");
            var menu_notifier = new Menu("Ult Notifier", "Ult Notifier");
            var menu_drawing = new Menu("drawing", "drawing");

            baseMenu.AddSubMenu(menu_q);
            menu_q.AddItem(new MenuItem("TF_q_enable", "Enable").SetValue(true));
            menu_q.AddItem(new MenuItem("TF_q_key", "Key:").SetValue(new KeyBind('T', KeyBindType.Press)));
            
            baseMenu.AddSubMenu(menu_autopicker);
            menu_autopicker.AddItem(new MenuItem("TF_Cardpicker", "Pick").SetValue(true));
            menu_autopicker.AddItem(new MenuItem("TF_Goldkey", "GoldKey:").SetValue(new KeyBind('T', KeyBindType.Press)));
            menu_autopicker.AddItem(new MenuItem("TF_Bluekey", "BlueKey:").SetValue(new KeyBind('E', KeyBindType.Press)));
            menu_autopicker.AddItem(new MenuItem("TF_Redkey", "RedKey").SetValue(new KeyBind('W', KeyBindType.Press)));
            menu_autopicker.AddItem(new MenuItem("TF_delay", "Delay X(ms)").SetValue(new Slider(100, 0, 400)));
            menu_autopicker.AddItem(new MenuItem("TF_delayrnd", "Random Delay(0~Xms)").SetValue(false));


            baseMenu.AddSubMenu(menu_notifier);
            menu_notifier.AddItem(new MenuItem("TF_notifier", "UltNotifier").SetValue(true));
            menu_notifier.AddItem(new MenuItem("TF_notifier_HP", "Notice On HP(%)").SetValue(new Slider(10, 0, 100)));

            baseMenu.AddSubMenu(menu_drawing);
            menu_drawing.AddItem(new MenuItem("TF_qRange", "Q-Range").SetValue(true));
            menu_drawing.AddItem(new MenuItem("TF_wRange", "Attack-Range(W)").SetValue(true));
            menu_drawing.AddItem(new MenuItem("TF_rRange", "R-Range").SetValue(true));
            menu_drawing.AddItem(new MenuItem("TF_killable", "KillableMark(W+Q)").SetValue(true));
            menu_drawing.AddItem(new MenuItem("TF_damage", "Q+WDamage").SetValue(true));

            if (Player.BaseSkinName == "TwistedFate")
            {
                Game.OnGameUpdate += Update;
                Drawing.OnEndScene += OnDraw_EndScene;
            }
            else
                Game.PrintChat("<font color ='#FF1010'>[Disable] You are not TwistedFate</font>");
            
        }

        public static void Update(EventArgs args)
        {

                #region get info
                float Player_bAD = Player.BaseAttackDamage;
                float Player_aAD = Player.FlatPhysicalDamageMod;
                float Player_totalAD = Player_bAD + Player_aAD;
                float Player_bAP = Player.BaseAbilityDamage;
                float Player_aAP = Player.FlatMagicDamageMod;
                float Player_totalAP = Player_bAP + Player_aAP;
                #endregion
                #region wildcards
                if (baseMenu.Item("TF_q_enable").GetValue<bool>() && baseMenu.Item("TF_q_key").GetValue<bool>())
                {
                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && !hero.IsDead && hero.IsValid && Vector3.Distance(Player.Position, hero.Position) <= 1450))
                    {
                        Player.Spellbook.CastSpell(SpellSlot.Q, hero);
                    }
                }
                #endregion
                #region pick a card
                if (baseMenu.Item("TF_Cardpicker").GetValue<bool>())
                {
                    Random delayt = new Random(DateTime.Now.Millisecond);
                    delayi = baseMenu.Item("TF_delay").GetValue<Slider>().Value;
                    if (baseMenu.Item("TF_delayrnd").GetValue<bool>())
                        delayi = delayt.Next(0, baseMenu.Item("TF_delay").GetValue<Slider>().Value);

                    if (stack >= 20)
                    {
                        cards = card.none;
                        stack = 0;
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Cooldown)
                    {
                        stack++;
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && cards == card.none
                        && Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Ready)
                    {
                        if (baseMenu.Item("TF_Goldkey").GetValue<KeyBind>().Active)
                        {
                            cards = card.gold;
                            stack = 0;
                            Player.Spellbook.CastSpell(SpellSlot.W);
                        }
                        if (baseMenu.Item("TF_Bluekey").GetValue<KeyBind>().Active)
                        {
                            cards = card.blue;
                            stack = 0;
                            Player.Spellbook.CastSpell(SpellSlot.W);
                        }
                        if (baseMenu.Item("TF_Redkey").GetValue<KeyBind>().Active)
                        {
                            cards = card.red;
                            stack = 0;
                            Player.Spellbook.CastSpell(SpellSlot.W);
                        }
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock" && cards == card.gold)
                    {

                        Utility.DelayAction.Add(delayi, () => { Player.Spellbook.CastSpell(SpellSlot.W); });
                        cards = card.none;
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock" && cards == card.blue)
                    {
                        Utility.DelayAction.Add(delayi, () => { Player.Spellbook.CastSpell(SpellSlot.W); });
                        cards = card.none;
                    }
                    if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock" && cards == card.red)
                    {
                        Utility.DelayAction.Add(delayi, () => { Player.Spellbook.CastSpell(SpellSlot.W); });
                        cards = card.none;
                    }
                }
                #endregion
                #region notifier
                if (baseMenu.Item("TF_notifier").GetValue<bool>())
                {
                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead && hero.IsValid))
                    {
                        if (hero.HealthPercentage() <= baseMenu.Item("TF_notifier_HP").GetValue<Slider>().Value)
                        {
                            targetPing(hero.Position.To2D(), Packet.PingType.AssistMe);
                        }
                    }
                }
                #endregion
        }
        public static void OnDraw_EndScene(EventArgs args)
        {
                if (baseMenu.Item("TF_rRange").GetValue<bool>())
                {
                    Utility.DrawCircle(Player.Position, 5500, Color.White, 1, 20, true);
                    Utility.DrawCircle(Player.Position, 5500, Color.White, 1, 20);
                }
                if (baseMenu.Item("TF_qRange").GetValue<bool>())
                {
                    Utility.DrawCircle(Player.Position, 1200, Color.White, 1, 20);
                }
                if (baseMenu.Item("TF_wRange").GetValue<bool>())
                {
                    Utility.DrawCircle(Player.Position, Player.AttackRange, Color.Red, 1, 20);
                }
                if (baseMenu.Item("TF_killable").GetValue<bool>() || baseMenu.Item("TF_damage").GetValue<bool>())
                    drawtarget();
        }


        #region 함수
        public static double getQDmg(Obj_AI_Base target, double AP, int s_level)
        {
            double[] spell_basedamage = { 0, 60, 110, 160, 210, 260 };

            double eDmg = AP * 0.65 + spell_basedamage[s_level];

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, eDmg);
        }
        public static double getWDmg(Obj_AI_Base target, double AP , double AD, int s_level,string name)
        {
            double[] spell_basedamage = { 0, 15, 22.5, 30, 37.5, 45 };
            switch (name)
            {
                case "goldcardlock":
                    spell_basedamage = new double[] { 0, 15, 22.5, 30, 37.5, 45 }; break;
                case "bluecardlock":
                    spell_basedamage = new double[] { 0, 40, 60, 80, 100, 120 }; break;
                case "redcardlock":
                    spell_basedamage = new double[] { 0, 30, 45, 60, 75, 90 }; break;
            }

            double eDmg = AP * 0.50 + AD * 1.00 + spell_basedamage[s_level];

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, eDmg);
        }
        public static double getEDmg(Obj_AI_Base target, double AP,double AD, int s_level)
        {
            double[] spell_basedamage = { 0, 55, 80, 105, 130, 155 };

            double eDmg = AP * 0.50 + spell_basedamage[s_level];

            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, eDmg) + ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical,AD);
        }

        public static void drawtarget()
        {
            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && !hero.IsDead && hero.IsVisible))
            {
                #region get info
                float Player_bAD = Player.BaseAttackDamage;
                float Player_aAD = Player.FlatPhysicalDamageMod;
                float Player_totalAD = Player_bAD + Player_aAD;
                float Player_bAP = Player.BaseAbilityDamage;
                float Player_aAP = Player.FlatMagicDamageMod;
                float Player_totalAP = Player_bAP + Player_aAP;
                #endregion

                    double dmg = 0;
                    if (Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Ready || Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed)
                        dmg = getQDmg(target, Player_totalAP, Player.Spellbook.GetSpell(SpellSlot.Q).Level);

                    if (Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Ready || Player.Spellbook.GetSpell(SpellSlot.W).State == SpellState.Surpressed)
                        dmg = dmg + getWDmg(target, Player_totalAP, Player_totalAD, Player.Spellbook.GetSpell(SpellSlot.W).Level, Player.Spellbook.GetSpell(SpellSlot.W).Name);

                    if (Player.HasBuff("CardMasterStackParticle"))
                        dmg = dmg + getEDmg(target, Player_totalAP, Player_totalAD, Player.Spellbook.GetSpell(SpellSlot.Q).Level);

                    


                   
                    if (target.Health <= dmg && baseMenu.Item("TF_killable").GetValue<bool>())
                        Utility.DrawCircle(target.Position, 100, Color.Peru, 50);

                    if (baseMenu.Item("TF_damage").GetValue<bool>())
                    {
                        var hpPercent = target.Health / target.MaxHealth;
                        var dmgPercent = (float)dmg / target.MaxHealth;

                        var barX = (target.HPBarPosition.X + 105 * hpPercent) +10 - (dmgPercent*105);
                        Drawing.DrawLine(Math.Max(barX, target.HPBarPosition.X + 10), target.HPBarPosition.Y + 20, Math.Max(barX, target.HPBarPosition.X + 10), target.HPBarPosition.Y + 28, 2, dmg > target.Health ? Color.White : Color.Blue);
                        Drawing.DrawText(target.HPBarPosition.X + 140, target.HPBarPosition.Y + 15, dmg > target.Health ? Color.Peru : Color.Blue, Convert.ToInt64(dmg).ToString());
                    }
                }
        }
       
        #endregion

        public static void targetPing(Vector2 Position, Packet.PingType ptype)
        {
            if (Environment.TickCount - pastTime < 2000)
                return;
            pastTime = Environment.TickCount;
            Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(Position.X, Position.Y, 0, 0, ptype)).Process();
        }
    }
}
