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

namespace JeonProject
{
    class Program
    {
        public static Menu baseMenu;

        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static SpellSlot igniteSlot = SpellSlot.Unknown;
        public static Spell smite;
        public static Spell ignite;

        public static int X = 0;
        public static int Y = 0;



        public static SpellSlot[] SSpellSlots = { ((SpellSlot)4), ((SpellSlot)5) };
        public static SpellSlot[] SpellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E,SpellSlot.R };

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnEndScene += OnDraw_EndScene;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Game.PrintChat("J Project was Activited");
            Game.PrintChat("Champion : " + ObjectManager.Player.BaseSkinName);
            Game.PrintChat("Spells : " + filterspellname(ObjectManager.Player.SummonerSpellbook.GetSpell(SpellSlot.Summoner1).Name) + "," +
                filterspellname(ObjectManager.Player.SummonerSpellbook.GetSpell(SpellSlot.Summoner2).Name));
            setSmiteSlot();
            setIgniteSlot();



            //메인메뉴
            baseMenu = new Menu("ProjectJ", "ProjectJ", true);
            baseMenu.AddToMainMenu();

            var menu_smite = new Menu("AutoSmite", "AutoSmite");
            var menu_ignite = new Menu("AutoIgnite", "AutoIgnite");
            var menu_tracker = new Menu("Tracker", "Tracker");


            #region 스마이트 메뉴 - menu for smite
            baseMenu.AddSubMenu(menu_smite);
            menu_smite.AddItem(new MenuItem("AutoSmite", "AutoSmite").SetValue(true));
            menu_smite.AddItem(new MenuItem("smite_enablekey", "enableKey:").SetValue(new KeyBind('K', KeyBindType.Toggle)));// 32 - Space
            menu_smite.AddItem(new MenuItem("smite_holdkey", "HoldKey:").SetValue(new KeyBind(32, KeyBindType.Press)));// 32 - Space
            #endregion

            #region 점화 메뉴 - menu for ignite
            baseMenu.AddSubMenu(menu_ignite);
            menu_ignite.AddItem(new MenuItem("AutoIgnite", "AutoIgnite").SetValue(true));
            #endregion

            #region 트래커 메뉴 - menu for tracker
            baseMenu.AddSubMenu(menu_tracker);
            menu_tracker.AddItem(new MenuItem("tracker_enemyspells", "EnemyStat").SetValue(true));
            //menu_tracker.AddItem(new MenuItem("tracker_x", "X").SetValue(new Slider(0,0,100)));
            //menu_tracker.AddItem(new MenuItem("tracker_y", "Y").SetValue(new Slider(0,0,100)));

            #endregion


        }
        private static void OnGameUpdate(EventArgs args)
        {
            #region 오토스마이트-AutoSmite
            if (baseMenu.Item("AutoSmite").GetValue<bool>() && (baseMenu.Item("smite_holdkey").GetValue<KeyBind>().Active || baseMenu.Item("smite_enablekey").GetValue<KeyBind>().Active))
            {
                double smitedamage;
                bool smiteReady = false;
                smitedamage = setSmiteDamage();
                Drawing.DrawText(ObjectManager.Player.HPBarPosition.X + 55, ObjectManager.Player.HPBarPosition.Y + 25, System.Drawing.Color.Red, "AutoSmite!");
                Obj_AI_Base mob = GetNearest(ObjectManager.Player.ServerPosition);
                /*테스트
                testFind(ObjectManager.Player.ServerPosition);
                Game.PrintChat(smiteSlot.ToString() + "<damage>" + smitedamage);
                Game.PrintChat(ObjectManager.Player.SummonerSpellbook.GetSpell(SpellSlot.Summoner2).Name);
                */

                if (ObjectManager.Player.SummonerSpellbook.CanUseSpell(smiteSlot) == SpellState.Ready && Vector3.Distance(ObjectManager.Player.ServerPosition, mob.ServerPosition) < smite.Range)
                {
                    smiteReady = true;
                }

                if (smiteReady && mob.Health < smitedamage)
                {
                    setIgniteSlot();
                    ObjectManager.Player.SummonerSpellbook.CastSpell(smiteSlot, mob);
                }
            }
            #endregion
            
            #region 오토이그나이트-AutoIgnite
            if (baseMenu.Item("AutoIgnite").GetValue<bool>())
            {
                double ignitedamage;
                bool IgniteReady = false;
                ignitedamage = setigniteDamage();
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero != null && hero.IsValid && hero.Health < ignitedamage && !hero.IsDead && ObjectManager.Player.ServerPosition.Distance(hero.ServerPosition) < ignite.Range))
                {
                    if (ObjectManager.Player.SummonerSpellbook.CanUseSpell(smiteSlot) == SpellState.Ready && Vector3.Distance(ObjectManager.Player.ServerPosition, hero.ServerPosition) < ignite.Range)
                    {
                        IgniteReady = true;
                    }
                    if (IgniteReady)
                    {
                        setIgniteSlot();
                        ObjectManager.Player.SummonerSpellbook.CastSpell(igniteSlot, hero);
                    }
                }
            }
            #endregion
             
            #region 스펠트레커-Spelltracker
            if (baseMenu.Item("tracker_enemyspells").GetValue<bool>())
            {
                try
                {

                    foreach (var target in
                        ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero != null && hero.IsValid && (!hero.IsMe && hero.IsHPBarRendered)))
                    {

                        X = 10;
                        Y = 40;
                        foreach (var sSlot in SSpellSlots)
                        {
                            var spell = target.SummonerSpellbook.GetSpell(sSlot);
                            var t = spell.CooldownExpires - Game.Time;
                            if (t < 0)
                            {

                                Drawing.DrawText(target.HPBarPosition.X + X + 85, target.HPBarPosition.Y + Y, System.Drawing.Color.FromArgb(255, 0, 255, 0), filterspellname(spell.Name));
                            }
                            else
                            {
                                Drawing.DrawText(target.HPBarPosition.X + X + 85, target.HPBarPosition.Y + Y, System.Drawing.Color.Red, filterspellname(spell.Name));
                            }

                            Y += 15;
                        }
                        Y = 40;
                        foreach (var slot in SpellSlots)
                        {
                            var spell = target.Spellbook.GetSpell(slot);
                            var t = spell.CooldownExpires - Game.Time;
                            if (t < 0)
                            {
                                Drawing.DrawText(target.HPBarPosition.X + X, target.HPBarPosition.Y + Y, System.Drawing.Color.FromArgb(255, 0, 255, 0), Convert.ToString(spell.Level));
                            }
                            else
                            {
                                Drawing.DrawText(target.HPBarPosition.X + X, target.HPBarPosition.Y + Y, System.Drawing.Color.Red, Convert.ToString(spell.Level));
                            }
                            X += 20;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"/ff error : " + e);
                }
            }

            #endregion

        }
        public static void OnDraw_EndScene(EventArgs args)
        {

        }

        // Addional Function //
        #region 스마이트함수 - Smite Function

        public static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        public static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        public static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
        private static readonly string[] MinionNames =
        {
            "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith",
            "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon", "SRU_Baron","SRU_Crab"
        };


        public static void setSmiteSlot()
        {
            foreach (var spell in ObjectManager.Player.SummonerSpellbook.Spells.Where(spell => String.Equals(spell.Name, smitetype(), StringComparison.CurrentCultureIgnoreCase)))
            {

                smiteSlot = spell.Slot;
                smite = new Spell(smiteSlot, 700);
                return;
            }
        }
        public static string smitetype()
        {
            if (SmiteBlue.Any(Items.HasItem))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(Items.HasItem))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(Items.HasItem))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(Items.HasItem))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }
        public static Obj_AI_Minion GetNearest(Vector3 pos)
        {
            var minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(minion => minion.IsValid && MinionNames.Any(name => minion.Name.StartsWith(name)) && !MinionNames.Any(name => minion.Name.Contains("Mini")));
            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            Obj_AI_Minion sMinion = objAiMinions.FirstOrDefault();
            double? nearest = null;
            foreach (Obj_AI_Minion minion in objAiMinions)
            {
                double distance = Vector3.Distance(pos, minion.Position);
                if (nearest == null || nearest > distance)
                {
                    nearest = distance;
                    sMinion = minion;
                }
            }
            return sMinion;
        }
        public static double setSmiteDamage()
        {
            int level = ObjectManager.Player.Level;
            int[] damage =
            {
                20*level + 370,
                30*level + 330,
                40*level + 240,
                50*level + 100
            };
            return damage.Max();
        }
        public static void testFind(Vector3 pos)
        {
            double? nearest = null;
            var minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(minion => minion.IsValid);
            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            Obj_AI_Minion sMinion = objAiMinions.FirstOrDefault();
            foreach (Obj_AI_Minion minion in minions)
            {
                double distance = Vector3.Distance(pos, minion.Position);
                if (nearest == null || nearest > distance)
                {
                    nearest = distance;
                    sMinion = minion;
                }
            }
            Game.PrintChat("Minion name is: " + sMinion.Name);
        }
        #endregion

        #region 이그나이트 함수 - Ignite
        public static void setIgniteSlot()
        {
            foreach (var spell in ObjectManager.Player.SummonerSpellbook.Spells.Where(spell => String.Equals(spell.Name, "summonerdot", StringComparison.CurrentCultureIgnoreCase)))
            {
                igniteSlot = spell.Slot;
                ignite = new Spell(smiteSlot, 600);
                return;
            }
        }

        public static double setigniteDamage()
        {
            double dmg = 50 + 20 * ObjectManager.Player.Level;
            return dmg;
        }
        #endregion

        #region 트래커함수 - Tracker
        public static string filterspellname(String a)
        {
            switch (a)
            {
                case "s5_summonersmiteplayerganker":
                    a = "BSmite"; break;
                case "s5_summonersmiteduel":
                    a = "RSmite"; break;
                case "s5_summonersmitequick":
                    a = "Smite"; break;
                case "itemsmiteaoe":
                    a = "Smite"; break;
                default:
                    break;
            }
            a = a.Replace("summoner", "").Replace("dot", "ignite");

            return a;
        }
        #endregion
    }
}
