#region
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Diagnostics;
using System.Windows.Input;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using Menu = LeagueSharp.Common.Menu;
using MenuItem = LeagueSharp.Common.MenuItem;
#endregion

namespace JeonUtilityReworked
{
    class Program
    {
        #region variable declaration
        public static Menu baseMenu;
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static SpellSlot[] SSpellSlots = { SpellSlot.Summoner1, SpellSlot.Summoner2 };
        public static SpellSlot[] SpellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

        public static Font font = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Calibri", Height = 16});

        #region Render variable
        public static List<Render.Circle> towerRanges = new List<Render.Circle>();
        public static Render.Text clock = new Render.Text("", new Vector2(Drawing.Width - 100, Drawing.Height * 8 / 100), (int)24, ColorBGRA.FromRgba(0xFFFFFFFF))
        {
            VisibleCondition = c => getMenuBool("NumPad1"),
            text = DateTime.Now.ToString("t"),
        };

        #endregion Render variable


        #region hardcc

        public static List<drawgrab> grabs = new List<drawgrab>();
        public class drawgrab
        {
            public Obj_AI_Base Caster = null;
            public float time = 0;
            public Vector3 Start = new Vector3(0, 0, 0);
            public Vector3 End = new Vector3(0, 0, 0);
            public float width = 0f;
            public Color color = Color.White;
        }

        #endregion

        #region Fake
        public static List<Fake> fakeList = new List<Fake>();
        public class Fake
        {
            public int id;
            public Vector3 position;
            public float time = 0;
            public Obj_AI_Minion target;

            public Render.Text text_fake { get; set; }

            public Fake()
            {
                text_fake = new Render.Text("", new Vector2(0, 0), 32
                    , SharpDX.Color.Red)
                {
                    VisibleCondition =
                    condition => getMenuBool("NumPad6") &&
                          !target.IsDead,
                    TextUpdate = () => "Fake!",
                    OutLined = true,
                    Centered = true
                };
                text_fake.Add();
            }
        }
        #endregion

        #region ShacoQ
        public static List<shacoDeceive> shacoQ = new List<shacoDeceive>();
        public class shacoDeceive
        {
            public Obj_AI_Base Caster = null;
            public float time = 0;
            public Vector3 End = new Vector3(0, 0, 0);

            public Render.Text text { get; set; }

            public shacoDeceive()
            {
                text = new Render.Text("", new Vector2(0, 0), 32
                    , SharpDX.Color.Red)
                {
                    VisibleCondition =
                    condition => getMenuBool("NumPad5"),
                    TextUpdate = () => "Shaco on here!",
                    PositionUpdate = delegate
                    {
                        Vector2 vec2 = Drawing.WorldToScreen(new Vector3(End.X, End.Y + 25, End.Z));
                        return vec2;
                    },
                    OutLined = true,
                    Centered = true
                };
                text.Add();
            }
        }
        #endregion

        #region Status
        public static List<Status> L_Status = new List<Status>();
        public class Status 
        {
            public Render.Text status;
            public Keys ID = 0;
            public byte order = 0;
            public bool colourCondition = false;

            public Status()
            {
                status = new Render.Text("", new Vector2(Drawing.Width * 20 / 100, Drawing.Width - 100), (int)12, ColorBGRA.FromRgba(0xFFFFFFFF))
                {
                    VisibleCondition = c => true,
                    PositionUpdate = () => new Vector2(Drawing.Width * (getMenuValue("x%") / 100f) + (order*12), Drawing.Height * (getMenuValue("y%") / 100f)),
                    text = "■",
                };
                
                status.Add();
            }
        }
        #endregion status

        #endregion

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            
            Console.Clear();

            #region Initialize
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Game.PrintChat("<font color ='#33FFFF'>JeonUtility v" + version + " </font>Loaded! ({0}x{1})", Drawing.Width, Drawing.Height);
            Console.WriteLine("JeanUtility Loaded.");
            #endregion

            #region 메뉴 - Menu
            #region 메인메뉴 - Main Menu
            baseMenu = new Menu("JeonUtility", "JeonUtility", true);
            baseMenu.AddToMainMenu();
            baseMenu.AddItem(new MenuItem("base_stat", "Status on hud").SetValue(true));
            baseMenu.AddItem(new MenuItem("x%", "HUD_X(%)").SetValue(new Slider(75, 0, 100)));
            baseMenu.AddItem(new MenuItem("y%", "HUD_Y(%)").SetValue(new Slider(55, 0, 100)));


            baseMenu.AddItem(new MenuItem("NumPad0", "N0 - TowerRange").SetValue(true));
            baseMenu.AddItem(new MenuItem("NumPad1", "N1 - Clock").SetValue(true));
            baseMenu.AddItem(new MenuItem("NumPad2", "N2 - SpellTracker").SetValue(true));
            baseMenu.AddItem(new MenuItem("NumPad3", "N3 - DrawHardCC").SetValue(true));
            baseMenu.AddItem(new MenuItem("NumPad4", "N4 - Stacks").SetValue(true));
            baseMenu.AddItem(new MenuItem("NumPad5", "N5 - ShacoQ").SetValue(true));
            baseMenu.AddItem(new MenuItem("NumPad6", "N6 - FakeDetector").SetValue(true));
            baseMenu.AddItem(new MenuItem("NumPad7", "N7 - DrawGlobalUltOnMinimap").SetValue(true));
            baseMenu.AddItem(new MenuItem("NumPad8", "N8").SetValue(true));
            baseMenu.AddItem(new MenuItem("NumPad9", "N9").SetValue(true));

            var menu_st = new Menu("Stacks", "Stacks");
            var menu_draw = new Menu("Draw", "Draw");
            #endregion

            #region 드로잉 메뉴 - menu for Draw
            baseMenu.AddSubMenu(menu_draw);
            menu_draw.AddItem(new MenuItem("draw_turret", "TurretRange").SetValue(true));
            menu_draw.AddItem(new MenuItem("draw_clock", "Clock").SetValue(true));

            #endregion

            #endregion

            #region Hotkey
            for (int i = 0; i <= 9; i++)
            {
                L_Status.Add(new Status()
                {
                    ID = (Keys)96+i,
                    order = (byte)i
                });
            }

            foreach(var t in L_Status)
            {
                if(getMenuBool(t.ID.ToString()))
                    t.status.Color = SharpDX.Color.ForestGreen;
            }
            #endregion

            #region Events Initialize
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Hero.OnProcessSpellCast += OnSpell;
            Drawing.OnEndScene += OnDraw_EndScene;
            Game.OnWndProc += OnWndProc;
            //Drawing.OnDraw += OnDraw;
            #endregion

            #region Initialize TowerRange
            foreach (var t in ObjectManager.Get<Obj_AI_Turret>().Where(t => !t.IsDead && t.IsEnemy &&
    (t.Name.StartsWith("Turret_T1") || t.Name.StartsWith("Turret_T2"))))
            {
                towerRanges.Add(new Render.Circle(t, 875, Color.Blue, 5)
                {
                    VisibleCondition = c => getMenuBool("NumPad0")
                });
                towerRanges.Last().Add();
            }
            #endregion


            clock.Add();
        }

        private static void OnWndProc(WndEventArgs args)
        {

            Keys _hotkey = 0;


            if (args.Msg != 257)
                return;


            for (int i = 0; i <= 9; i++)
            {
                _hotkey = (Keys)96 + i;
                if (args.WParam == Convert.ToInt32(_hotkey))
                {
                    baseMenu.Item(_hotkey.ToString()).SetValue(!getMenuBool(_hotkey.ToString()));
                    L_Status.First(t => t.ID == _hotkey).status.Color = getMenuBool(_hotkey.ToString()) ? SharpDX.Color.ForestGreen : SharpDX.Color.White;
                }
            }
            
        }
        
        private static void OnDraw_EndScene(EventArgs args)
        {
            if (Game.Time < 50)
                return;

            #region get info
            float Player_baseAD = Player.BaseAttackDamage;
            float Player_addAD = Player.FlatPhysicalDamageMod;
            float Player_totalAD = Player_baseAD + Player_addAD;
            float Player_totalAP = Player.FlatMagicDamageMod;
            #endregion

            #region ShacoQ
            if (shacoQ.Any())
            {
                foreach (var sQ in shacoQ.Where(t => t.End != Vector3.Zero))
                {
                    Render.Circle.DrawCircle(sQ.End, 50, Color.Red);

                    if (Game.Time > sQ.time)
                    {
                        sQ.text.Remove();
                        shacoQ.Remove(sQ);
                        break;
                    }
                }
            }
            #endregion

            #region 스택 - Stacks
            if (Player.ChampionName == "Kalista" && getMenuBool(Keys.NumPad4.ToString()))
            {

                Spell E = new Spell(SpellSlot.E, 900);

                var _List = ObjectManager.Get<Obj_AI_Hero>().Where(t =>
                    E.IsReady() &&
                    t.Distance(Player.Position) <= 900 &&
                        !t.IsDead && t.IsEnemy && t.HasBuff("KalistaExpungeMarker"));

                foreach (var target in _List)
                {
                    var Count = target.Buffs.First(buff => buff.DisplayName == "KalistaExpungeMarker").Count;
                    var damage = getKaliDmg(target, Count, Player_totalAD, E.Level);

                    String t_damage = Convert.ToInt64(damage).ToString() + "(" + Count + ")";

                    //font.DrawText(null, t_damage, (int)target.HPBarPosition.X, (int)target.HPBarPosition.Y - 5, SharpDX.Color.Red);

                    if (damage >= target.Health && !target.MagicImmune && !target.HasBuff("SivirE"))
                        E.Cast();
                }

                Obj_AI_Base mob = GetNearest(Player.ServerPosition);
                if (mob != null && mob.IsVisible)
                {
                    foreach (var venoms in mob.Buffs.Where(venoms => venoms.DisplayName == "KalistaExpungeMarker"))
                    {
                        var damage = getKaliDmg(mob, venoms.Count, Player_totalAD, E.Level);
                        
                        var font1 = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Calibri", Height = 20});
                        var font2 = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Calibri", Height = 20});
                        String t_damage = Convert.ToInt64(damage).ToString() + "(" + venoms.Count + ")";
                        //Drawtext_outlined(font1, mob.Health.ToString("0.0"), (int)mob.HPBarPosition.X + 100, (int)mob.HPBarPosition.Y - 5, SharpDX.Color.LightGreen);
                        //Drawtext_outlined(font2, t_damage, (int)mob.HPBarPosition.X+10, (int)mob.HPBarPosition.Y - 5, SharpDX.Color.Red);
                        font1.Dispose();
                        font2.Dispose();
                        if (damage - 50f >= mob.Health && Vector3.Distance(mob.Position, Player.Position) <= 900)
                            E.Cast();
                    }
                }
            }
            #endregion

            #region Spells
            foreach (var grab in grabs.Where(t => t.Start != Vector3.Zero))
            {
                if (grab.Caster.BaseSkinName == "Lux" && grab.width == 300)
                {
                    Render.Circle.DrawCircle(grab.End, grab.width, Color.White);

                    if (Game.Time > grab.time)
                    {
                        grabs.Remove(grab);
                        break;
                    }

                    return;
                }

                var start = grab.Start;
                var end = grab.End;
                var distance = start.Distance(end);
                var sin = (start.X - end.X) / distance;
                var cos = (start.Y - end.Y) / distance;
                Vector3 line1_start, line2_start, line1_end, line2_end;


                float endX = start.X - grab.width * (float)cos;
                float endY = start.Y + grab.width * (float)sin;
                line1_start = new Vector3(endX, endY, start.Z);

                endX = start.X + grab.width * (float)cos;
                endY = start.Y - grab.width * (float)sin;
                line2_start = new Vector3(endX, endY, start.Z);

                endX = end.X - grab.width * (float)cos;
                endY = end.Y + grab.width * (float)sin;
                line1_end = new Vector3(endX, endY, end.Z);

                endX = end.X + grab.width * (float)cos;
                endY = end.Y - grab.width * (float)sin;
                line2_end = new Vector3(endX, endY, end.Z);



                Drawing.DrawLine(Drawing.WorldToScreen(grab.Start),
                    Drawing.WorldToScreen(grab.End), 1, grab.color);

                Drawing.DrawLine(Drawing.WorldToScreen(line1_start),
                    Drawing.WorldToScreen(line1_end), 1, grab.color);

                Drawing.DrawLine(Drawing.WorldToScreen(line2_start),
                    Drawing.WorldToScreen(line2_end), 1, grab.color);

                Render.Circle.DrawCircle(grab.Start, grab.width, grab.color);
                Render.Circle.DrawCircle(grab.End, grab.width, grab.color);

                if (Game.Time > grab.time)
                {
                    grabs.Remove(grab);
                    break;
                }
            }


            if (getMenuBool("NumPad7"))
            {
                foreach (var t in ObjectManager.Get<Obj_SpellMissile>().Where(t => t.SData.Name == "EzrealTrueshotBarrage" || t.SData.Name == "JinxR"
                    || t.SData.Name == "EnchantedCrystalArrow" || t.SData.Name == "DravenR"))
                {
                    if (t.Position.X > 0 && t.Position.X < 20000)
                    {
                        DrawCircleOnMinimap(t.Position, 60, Color.Red, 5, 5);
                    }
                }
            }
            #endregion
        }

        private static void OnSpell(Obj_AI_Base Caster, GameObjectProcessSpellCastEventArgs args)
        {
            #region Catch hard CC Spell
            if (getMenuBool("NumPad3"))
            {
                if (args.SData.Name == "ThreshQ")
                {
                    grabs.Add(new drawgrab()
                    {
                        Caster = Caster,
                        Start = args.Start,
                        End = args.Start.Extend(args.End, 1100),
                        time = Game.Time + 1f,
                        width = 70
                    });
                }
                if (args.SData.Name == "RocketGrab")
                {
                    grabs.Add(new drawgrab()
                    {
                        Caster = Caster,
                        Start = args.Start,
                        End = args.Start.Extend(args.End, 1100),
                        time = Game.Time + 1f,
                        width = 70
                    });
                }
                if (args.SData.Name == "DarkBindingMissile")
                {
                    grabs.Add(new drawgrab()
                    {
                        Caster = Caster,
                        Start = args.Start,
                        End = args.Start.Extend(args.End, 1300),
                        time = Game.Time + 1300f / 1200f,
                        width = 70
                    });
                }
                if (args.SData.Name == "BandageToss")
                {
                    grabs.Add(new drawgrab()
                    {
                        Caster = Caster,
                        Start = args.Start,
                        End = args.Start.Extend(args.End, 1100),
                        time = Game.Time + 1100f / 1800f,
                        width = 80
                    });
                }
                if (args.SData.Name == "JavelinToss")
                {
                    grabs.Add(new drawgrab()
                    {
                        Caster = Caster,
                        Start = args.Start,
                        End = args.Start.Extend(args.End, 1500),
                        time = Game.Time + 1500f / 1300f,
                        width = 40
                    });
                }
                if (args.SData.Name == "LuxLightBinding")
                {
                    grabs.Add(new drawgrab()
                    {
                        Caster = Caster,
                        Start = args.Start,
                        End = args.Start.Extend(args.End, 1175),
                        time = Game.Time + 1200f / 1175f,
                        width = 80
                    });
                }
                if (args.SData.Name == "LuxLightStrikeKugel")
                {
                    grabs.Add(new drawgrab()
                    {
                        Caster = Caster,
                        Start = args.Start,
                        End = args.End,
                        time = Game.Time + 1200f / 1175f,
                        width = 300
                    });
                }
            }

            #endregion

            #region Catch ShacoQ
            if (Caster.IsEnemy && (Caster.BaseSkinName == "Shaco" && args.SData.Name == "Deceive") && getMenuBool(Keys.NumPad5.ToString()))
            {
                var pos = args.End;
                if (args.Start.Distance(args.End) >= 400)
                    pos = args.Start.Extend(args.End, 400);

                shacoQ.Add(new shacoDeceive()
                {
                    Caster = Caster,
                    End = pos,
                    time = Game.Time + 3.0f
                });
            }
            #endregion
        }
        private static void OnGameUpdate(EventArgs args)
        {

            #region get info
            float Player_baseAD = Player.BaseAttackDamage;
            float Player_addAD = Player.FlatPhysicalDamageMod;
            float Player_totalAD = Player_baseAD + Player_addAD;
            float Player_totalAP = Player.FlatMagicDamageMod;
            #endregion

            #region 스펠트레커-Spelltracker
            if (getMenuBool("NumPad2"))
            {
                foreach (var target in
                    ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero != null && !hero.IsMe && hero.IsValid && (hero.IsHPBarRendered)))
                {

                    int X = 10;
                    int Y = 40;
                    foreach (var sSlot in SSpellSlots)
                    {

                        var spell = target.Spellbook.GetSpell(sSlot);
                        var t = spell.CooldownExpires - Game.Time;
                        if (t < 0)
                        {
                            Drawing.DrawText(target.HPBarPosition.X + X, target.HPBarPosition.Y + Y + 15, System.Drawing.Color.FromArgb(255, 0, 255, 0), filterspellname(spell.Name));
                        }
                        else
                        {
                            Drawing.DrawText(target.HPBarPosition.X + X, target.HPBarPosition.Y + Y + 15, System.Drawing.Color.Red, "{0}({1})", filterspellname(spell.Name), Convert.ToString((int)t + 1));
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
                            Drawing.DrawText(target.HPBarPosition.X + X, target.HPBarPosition.Y + Y, System.Drawing.Color.Red, "{0}({1})", Convert.ToString(spell.Level), Convert.ToString((int)t + 1));
                        }
                        X += 40;
                    }
                }
            }
            #endregion

            #region fake tracker
            if (getMenuBool(Keys.NumPad6.ToString()))
            {
                if (ObjectManager.Get<Obj_AI_Minion>().Any(t =>
                    (t.BaseSkinName == "Shaco" || t.BaseSkinName == "Leblanc") && !t.IsDead && t.IsEnemy))
                {
                    foreach (var t in ObjectManager.Get<Obj_AI_Minion>().Where(t =>
                        (t.BaseSkinName == "Shaco" || t.BaseSkinName == "Leblanc") && !t.IsDead && t.IsEnemy && t.IsVisible))
                    {
                        if (!fakeList.Any(s => s.id == t.NetworkId) && t.BaseSkinName == "Leblanc")
                        {
                            fakeList.Add(new Fake()
                            {
                                id = t.NetworkId,
                                position = t.Position,
                                target = t,
                                time = Game.Time + 8.0f
                            });
                        }
                        else if (!fakeList.Any(s => s.id == t.NetworkId) && t.BaseSkinName == "Shaco")
                        {
                            fakeList.Add(new Fake()
                            {
                                id = t.NetworkId,
                                position = t.Position,
                                target = t,
                                time = Game.Time + 18.0f
                            });
                        }
                    }
                }
                if (fakeList.Any())
                {
                    foreach (var temp in fakeList.Where(t => t.target.IsDead || Game.Time > t.time))
                    {
                        temp.text_fake.Remove();
                        fakeList.Remove(temp);
                        break;
                    }

                    foreach (var t in fakeList.Where(t => !t.target.IsDead))
                    {
                        t.text_fake.PositionUpdate = delegate
                        {
                            Vector2 vec2 = Drawing.WorldToScreen(new Vector3(t.target.Position.X,
                                t.target.Position.Y + 25, t.target.Position.Z));
                            return vec2;
                        };
                    }

                }
            }
            #endregion fake tracker

            #region Clock
           
            if (clock.text != DateTime.Now.ToString("t"))
            {
                clock.text = DateTime.Now.ToString("t");
            }
            if (clock.X != Drawing.Width - 100)
            {
                clock.X = Drawing.Width - 100;
                clock.Y = Drawing.Height * 8 / 100;
            }
            #endregion

        }


        // Addional Function //

        #region common
        public static bool getMenuBool(String menu)
        {
            return Program.baseMenu.Item(menu).GetValue<bool>();
        }
        public static int getMenuValue(String menu)
        {
            return Program.baseMenu.Item(menu).GetValue<Slider>().Value;
        }
        public static String getMenuStringValue(String menu)
        {
            return Program.baseMenu.Item(menu).GetValue<StringList>().SelectedValue;
        }

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

        
        public static void Drawtext_outlined(Font font, String text, int x, int y, SharpDX.Color color)
        {
            font.DrawText(null, text, x + 1, y + 1, SharpDX.Color.Black);
            font.DrawText(null, text, x, y + 1, SharpDX.Color.Black);
            font.DrawText(null, text, x - 1, y - 1, SharpDX.Color.Black);
            font.DrawText(null, text, x, y - 1, SharpDX.Color.Black);
            font.DrawText(null, text, x, y, color);
        }

        public static void DrawCircleOnMinimap(Vector3 center,
            float radius,
            System.Drawing.Color color,
            int thickness = 5,
            int quality = 30)
        {
            var pointList = new List<Vector3>();
            for (var i = 0; i < quality; i++)
            {
                var angle = i * Math.PI * 2 / quality;
                pointList.Add(
                    new Vector3(
                        center.X + radius * (float)Math.Cos(angle), center.Y + radius * (float)Math.Sin(angle),
                        center.Z));
            }

            for (var i = 0; i < pointList.Count; i++)
            {
                var a = pointList[i];
                var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];

                var aonScreen = Drawing.WorldToMinimap(a);
                var bonScreen = Drawing.WorldToMinimap(b);

                Drawing.DrawLine(aonScreen.X, aonScreen.Y, bonScreen.X, bonScreen.Y, thickness, color);
            }
        }
        #endregion

        #region about Jungle
        private static readonly string[] MinionNames =
        {
        "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith",
            "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon", "SRU_Baron"
        };
        public static Obj_AI_Minion GetNearest(Vector3 pos)
        {

            if (ObjectManager.Get<Obj_AI_Minion>().Any(t => MinionNames.Any(name => t.Name.StartsWith(name))
                && !MinionNames.Any(name => t.Name.Contains("Mini")) && t.IsValid && !t.IsDead))
            {
                return ObjectManager.Get<Obj_AI_Minion>().OrderBy(t => Player.Distance(t.Position))
                        .First(minion => minion.IsValid && !minion.IsDead &&
                            MinionNames.Any(name => minion.Name.StartsWith(name)) &&
                            !MinionNames.Any(name => minion.Name.Contains("Mini")));
            }
            else
                return null;
        }
        #endregion

        #region 스택함수 - Stack
        public static double getKaliDmg(Obj_AI_Base target, int count, double AD, int s_level)
        {
            double[] spell_basedamage = { 0, 20, 30, 40, 50, 60 };
            double[] spell_perdamage = { 0, 0.25, 0.30, 0.35, 0.40, 0.45 };
            double eDmg = AD * 0.60 + spell_basedamage[s_level];
            count -= 1;
            eDmg = eDmg + count * (eDmg * spell_perdamage[s_level]);
            return ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            //return Player.CalcDamage(target, Damage.DamageType.Physical,eDmg);
        }
        public static double getTwitEDmg(Obj_AI_Base target, int count, double AD, double AP, int s_level)
        {
            double[] spell_basedamage = { 0, 20, 35, 50, 65, 80 };
            double[] spell_stackdamage = { 0, 15, 20, 25, 30, 35 };
            double eDmg = spell_basedamage[s_level] + count * (spell_stackdamage[s_level] + (AP * 0.2) + (AD * 0.25));
            return Player.CalcDamage(target, Damage.DamageType.Physical, eDmg);
        }
        #endregion



    }
}



