#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

namespace JeonUtility
{
    class Program
    {
        #region variable declaration
        public static Menu baseMenu;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static System.Drawing.Rectangle Monitor = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

        public static SpellSlot[] SSpellSlots = { SpellSlot.Summoner1, SpellSlot.Summoner2 };
        public static SpellSlot[] SpellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static SpellSlot igniteSlot = SpellSlot.Unknown;
        public static SpellSlot defslot = SpellSlot.Unknown;
        public static Spell smite;
        public static Spell ignite;
        public static Spell defspell;
        public static Spell jumpspell;

        public static bool canw2j = false;
        public static bool rdyw2j = false;
        public static bool rdyward = false;
        public static bool text_Isrender = false;
        public static bool textsmite_Isrender = false;

        public static int req_ignitelevel { get { return Jlib.getm_value("igniteLv"); } }



        public static String[] DefSpellstr = { "barrier", "heal" };

        public static Render.Text text_notifier = new Render.Text("Can Ult to kill!", Player, new Vector2(0, 50), (int)32, SharpDX.ColorBGRA.FromRgba(0xFF00FFBB));
        public static Render.Text text_help = new Render.Text("Somebody Need Help!", Player, new Vector2(0, 50), (int)32, SharpDX.ColorBGRA.FromRgba(0xFF00FFBB));
        public static Render.Text text_smite = new Render.Text("AutoSmite!", Player, new Vector2(55, 50), (int)30, SharpDX.ColorBGRA.FromRgba(0xFF0000FF));
        #endregion

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<font color ='#33FFFF'>JeonUtility v1.0 </font>Loaded!");
            setSmiteSlot();
            setIgniteSlot();
            setDefSpellSlot();

            #region 메뉴
            #region 메인메뉴 - Main Menu
            baseMenu = new Menu("ProjectJ", "ProjectJ", true);
            baseMenu.AddToMainMenu();
            baseMenu.AddItem(new MenuItem("base_stat", "Status on hud").SetValue(true));
            baseMenu.AddItem(new MenuItem("x", "x").SetValue(new Slider(600, 0, Monitor.Width)));
            baseMenu.AddItem(new MenuItem("y", "y").SetValue(new Slider(250, 0, Monitor.Height)));

            var menu_smite = new Menu("Smite", "Smite");
            var menu_ignite = new Menu("Ignite", "Ignite");
            var menu_tracker = new Menu("Tracker", "Tracker");
            var menu_j2w = new Menu("Jump2Ward", "Jump2Ward");
            var menu_st = new Menu("Stacks", "Stacks");
            var menu_ins = new Menu("Item&Spell", "Item & Spell");
            var menu_noti = new Menu("Notifier", "Notifier");
            #endregion

            #region 스마이트 메뉴 - menu for smite
            baseMenu.AddSubMenu(menu_smite);
            menu_smite.AddItem(new MenuItem("AutoSmite", "AutoSmite").SetValue(true));
            menu_smite.AddItem(new MenuItem("smite_enablekey", "enableKey:").SetValue(new KeyBind('K', KeyBindType.Toggle)));// 32 - Space
            menu_smite.AddItem(new MenuItem("smite_holdkey", "HoldKey:").SetValue(new KeyBind(32, KeyBindType.Press)));// 32 - Space
            #endregion

            #region 점화 메뉴 - menu for ignite
            baseMenu.AddSubMenu(menu_ignite);
            menu_ignite.AddItem(new MenuItem("AutoIgnite", "AutoIgnite").SetValue(true));
            menu_ignite.AddItem(new MenuItem("igniteLv", "Req Level :").SetValue(new Slider(1, 1, 18)));
            #endregion

            #region 트래커 메뉴 - menu for tracker
            baseMenu.AddSubMenu(menu_tracker);
            menu_tracker.AddItem(new MenuItem("tracker_enemyspells", "EnemyStat").SetValue(true));

            #endregion

            #region 점프와드 메뉴 - menu for Jump2Ward
            baseMenu.AddSubMenu(menu_j2w);
            menu_j2w.AddItem(new MenuItem("j2w_bool", "Jump2Ward").SetValue(true));
            menu_j2w.AddItem(new MenuItem("j2w_hkey", "Key : ").SetValue(new KeyBind('T', KeyBindType.Press)));
            menu_j2w.AddItem(new MenuItem("j2w_info", "InFo").SetValue(false));
            #endregion

            #region 스택 메뉴 - menu for stacks
            baseMenu.AddSubMenu(menu_st);
            menu_st.AddItem(new MenuItem("st_bool", "ShowDamages").SetValue(true));
            menu_st.AddItem(new MenuItem("st_twitch", "Auto Twitch(E)").SetValue(false));
            menu_st.AddItem(new MenuItem("st_kalista", "Auto Kalista(E)").SetValue(false));
            #endregion

            #region 아이템사용 메뉴 - menu for UseItem&Spell
            baseMenu.AddSubMenu(menu_ins);


            var menu_Potion = new Menu("Potion", "Potion");
            menu_ins.AddSubMenu(menu_Potion);
            menu_Potion.AddItem(new MenuItem("useitem_flask", "Flask").SetValue(true));
            menu_Potion.AddItem(new MenuItem("useitem_p_fla", "Use On Hp(%)").SetValue(new Slider(50, 0, 100)));
            menu_Potion.AddItem(new MenuItem("useitem_hppotion", "HP-potion").SetValue(true));
            menu_Potion.AddItem(new MenuItem("useitem_p_hp", "Use On Hp(%)").SetValue(new Slider(50, 0, 100)));
            menu_Potion.AddItem(new MenuItem("useitem_manapotion", "Mana-potion").SetValue(true));
            menu_Potion.AddItem(new MenuItem("useitem_p_mana", "Use On Mana(%)").SetValue(new Slider(50, 0, 100)));

            var menu_items = new Menu("Items", "Items");
            menu_ins.AddSubMenu(menu_items);
            var item_zhonya = new Menu("Zhonya", "Zhonya");
            menu_items.AddSubMenu(item_zhonya);
            item_zhonya.AddItem(new MenuItem("useitem_zhonya", "UseZhonya").SetValue(true));
            item_zhonya.AddItem(new MenuItem("useitem_p_zhonya", "Use On Hp(%)").SetValue(new Slider(15, 0, 100)));

            var item_botrk = new Menu("BOTRK", "BOTRK");
            menu_items.AddSubMenu(item_botrk);
            item_botrk.AddItem(new MenuItem("useitem_botrk", "UseBOTRK").SetValue(true));
            item_botrk.AddItem(new MenuItem("useitem_p_botrk", "Use On Hp(%)").SetValue(new Slider(20, 0, 100)));
            item_botrk.AddItem(new MenuItem("useitem_botrk_atg", "Anti-Gap-Closer").SetValue(true));
            item_botrk.AddItem(new MenuItem("useitem_botrk_atg_p", "Gap :").SetValue(new Slider(150, 100, 450)));

            var item_mikaels = new Menu("Mikaels", "Mikaels");
            menu_items.AddSubMenu(item_mikaels);
            item_mikaels.AddItem(new MenuItem("useitem_mikaels", "ON?").SetValue(true));
            item_mikaels.AddItem(new MenuItem("useitem_p_mikaels", "Use On Hp(%)").SetValue(new Slider(15, 0, 100)));
            item_mikaels.AddItem(new MenuItem("useitem_p_mikaels_delay", "Mikaels Delay(ms)").SetValue(new Slider(100, 0, 1000)));

            #region mikaels_cc
            var menu_mikaels_cc = new Menu("mikael_cc", "Use On CC");
            item_mikaels.AddSubMenu(menu_mikaels_cc);
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_bool", "On CC?").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_stun", "Stun").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_fear", "Fear(Flee)").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_charm", "Charm").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_taunt", "Taunt").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_snare", "Snare").SetValue(true));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_silence", "Slience").SetValue(false));
            menu_mikaels_cc.AddItem(new MenuItem("mikaels_cc_polymorph", "Polymorph").SetValue(true));
            #endregion


            var item_qs = new Menu("QuickSilver", "QuickSilver");
            menu_items.AddSubMenu(item_qs);
            item_qs.AddItem(new MenuItem("useitem_qs_bool", "UseQS").SetValue(true));
            item_qs.AddItem(new MenuItem("useitem_p_qs_delay", "QuickSilver Delay(ms)").SetValue(new Slider(100, 0, 1000)));
            #region qs_cc
            var menu_quicksilver_cc = new Menu("Use On CC", "Use On CC");
            item_qs.AddSubMenu(menu_quicksilver_cc);
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_stun", "Stun").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_fear", "Fear(Flee)").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_charm", "Charm").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_taunt", "Taunt").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_snare", "Snare").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_silence", "Slience").SetValue(false));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_polymorph", "Polymorph").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_suppression", "Suppression").SetValue(true));
            menu_quicksilver_cc.AddItem(new MenuItem("qs_cc_zedutl", "ZedUtl").SetValue(true));
            #endregion

            var menu_spell = new Menu("Spell", "Spell");
            menu_ins.AddSubMenu(menu_spell);
            menu_spell.AddItem(new MenuItem("usespell", "CastSpell").SetValue(true));
            menu_spell.AddItem(new MenuItem("usespell_hp", "Cast On Hp(%)").SetValue(new Slider(10, 0, 100)));

            #endregion

            #region 알림 메뉴 - menu for notifier
            baseMenu.AddSubMenu(menu_noti);
            menu_noti.AddItem(new MenuItem("noti_karthus", "KarthusUlt").SetValue(true));
            menu_noti.AddItem(new MenuItem("noti_ez", "EzrealUlt").SetValue(true));
            menu_noti.AddItem(new MenuItem("noti_cait", "CaitUlt").SetValue(true));
            menu_noti.AddItem(new MenuItem("noti_shen", "ShenUlt").SetValue(true));
            menu_noti.AddItem(new MenuItem("noti_shenhp", "Notice On Hp(%)").SetValue(new Slider(10, 0, 100)));
            #endregion
            #endregion

            Game.OnGameUpdate += OnGameUpdate;
            //Drawing.OnEndScene += OnDraw_EndScene;
            //Drawing.OnDraw += OnDraw;
        }
        private static void OnGameUpdate(EventArgs args)
        {

            #region get info
            float Player_baseAD = Player.BaseAttackDamage;
            float Player_addAD = Player.FlatPhysicalDamageMod;
            float Player_totalAD = Player_baseAD + Player_addAD;
            float Player_totalAP = Player.FlatMagicDamageMod;
            #endregion

            #region 오토스마이트-AutoSmite
            if (Jlib.getm_bool("AutoSmite") && smiteSlot != SpellSlot.Unknown)
            {
                if ((baseMenu.Item("smite_holdkey").GetValue<KeyBind>().Active || baseMenu.Item("smite_enablekey").GetValue<KeyBind>().Active))
                {
                    double smitedamage;
                    bool smiteReady = false;
                    smitedamage = setSmiteDamage();
                    Drawing.DrawText(Player.HPBarPosition.X + 55, Player.HPBarPosition.Y + 25, System.Drawing.Color.Red, "AutoSmite!");

                    Obj_AI_Base mob = GetNearest(Player.ServerPosition);

                    if (Player.Spellbook.CanUseSpell(smiteSlot) == SpellState.Ready && Vector3.Distance(Player.ServerPosition, mob.ServerPosition) < smite.Range)
                    {
                        smiteReady = true;
                    }

                    if (smiteReady && mob.Health < smitedamage)
                    {
                        setIgniteSlot();
                        Player.Spellbook.CastSpell(smiteSlot, mob);
                    }
                }
            }
            #endregion

            #region 오토이그나이트-AutoIgnite
            if (Jlib.getm_bool("AutoIgnite") && igniteSlot != SpellSlot.Unknown &&
                Player.Level >= req_ignitelevel)
            {
                float ignitedamage;
                bool IgniteReady = false;
                ignitedamage = setigniteDamage();
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero != null && hero.IsValid && !hero.IsDead && Player.ServerPosition.Distance(hero.ServerPosition) < ignite.Range
                    && !hero.IsMe && !hero.IsAlly && (hero.Health + hero.HPRegenRate * 2) <= ignitedamage))
                {

                    if (Player.Spellbook.CanUseSpell(igniteSlot) == SpellState.Ready)
                    {
                        IgniteReady = true;
                    }
                    if (IgniteReady)
                    {
                        setIgniteSlot();
                        Player.Spellbook.CastSpell(igniteSlot, hero);
                    }
                }
            }
            #endregion

            #region 스펠트레커-Spelltracker
            if (Jlib.getm_bool("tracker_enemyspells"))
            {
                foreach (var target in
                    ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero != null && hero.IsValid && (!hero.IsMe && hero.IsHPBarRendered)))
                {

                    int X = 10;
                    int Y = 40;
                    foreach (var sSlot in SSpellSlots)
                    {
                        var spell = target.Spellbook.GetSpell(sSlot);
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

            #endregion

            #region 점프와드- Jump2Ward (Jax,Kata,LeeSin)
            if (Jlib.getm_bool("j2w_bool"))
            {
                List<String> champs = new List<String>();
                champs.Add("LeeSin"); champs.Add("Katarina"); champs.Add("Jax");
                setj2wslots(champs);
                if (canw2j)
                {
                    checkE();
                    checkWard();
                    if (rdyw2j && baseMenu.Item("j2w_hkey").GetValue<KeyBind>().Active)
                    {
                        Vector3 cursor = Game.CursorPos;
                        Vector3 myPos = Player.ServerPosition;
                        Player.IssueOrder(GameObjectOrder.MoveTo, cursor);
                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(ward => ward.IsVisible && ward.IsAlly && !ward.IsMe &&
                            Vector3.DistanceSquared(cursor, ward.ServerPosition) <= 200 * 200 &&
                            ward.Distance(Player) <= 700 && ward.Name.IndexOf("Turret") == -1))
                        {
                            jumpspell.CastOnUnit(target);
                        }

                        if (rdyward)
                        {
                            Items.GetWardSlot().UseItem(cursor);
                        }
                    }
                }


                if (Jlib.getm_bool("j2w_info"))
                {
                    Game.PrintChat("Champion : " + Player.BaseSkinName);
                    Game.PrintChat("Can? : " + canw2j);
                    Game.PrintChat("Spell : " + jumpspell.Slot.ToString());
                    Game.PrintChat("WardStack : " + Items.GetWardSlot().Stacks);
                    baseMenu.Item("j2w_info").SetValue<bool>(false);
                }

            }

            #endregion

            #region 스택 - Stacks
            if (Jlib.getm_bool("st_twitch") && Player.BaseSkinName == "Twitch")
            {
                    Spell E = new Spell(SpellSlot.E, 1200);
                    var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget(E.Range))
                    {
                        foreach (var venoms in target.Buffs.Where(venoms => venoms.DisplayName == "TwitchDeadlyVenom"))
                        {
                            var damage = getTwitEDmg(target, venoms.Count, Player_addAD, Player_totalAP, E.Level);
                            //Game.PrintChat("d:{0} hp:{1}",damage,target.Health);
                            if (damage >= target.Health && E.IsReady())
                                E.Cast();

                            if (Jlib.getm_bool("st_bool"))
                            {
                                String t_damage = Convert.ToInt64(damage).ToString() + "(" + venoms.Count + ")";
                                Drawing.DrawText(target.HPBarPosition.X, target.HPBarPosition.Y - 5, Color.Red, t_damage);
                            }
                        }
                    }
            }

            if (Jlib.getm_bool("st_kalista") && Player.BaseSkinName == "Kalista")
            {
                Spell E = new Spell(SpellSlot.E, 900);
                if (E.IsReady())
                {
                    var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget(E.Range))
                    {
                        foreach (var venoms in target.Buffs.Where(venoms => venoms.DisplayName == "KalistaExpungeMarker"))
                        {
                            var damage = getKaliDmg(target, venoms.Count, Player_totalAD, E.Level);
                            if (damage >= target.Health)
                                E.Cast();
                            if (Jlib.getm_bool("st_bool"))
                            {
                                String t_damage = Convert.ToInt64(damage).ToString() + "(" + venoms.Count + ")";
                                Drawing.DrawText(target.HPBarPosition.X, target.HPBarPosition.Y - 5, Color.Red, t_damage);
                            }
                        }
                    }

                    Obj_AI_Base mob = GetNearest(Player.ServerPosition);
                    foreach (var venoms in mob.Buffs.Where(venoms => venoms.DisplayName == "KalistaExpungeMarker"))
                    {
                        var damage = getKaliDmg(mob, venoms.Count, Player_totalAD, E.Level);
                        if (damage >= mob.Health && Vector3.Distance(mob.Position, Player.Position) <= 900
                            && (mob.Name.Contains("SRU_Dragon") || mob.Name.Contains("SRU_Baron")))
                            E.Cast();

                        if (Jlib.getm_bool("st_bool"))
                        {
                            String t_damage = Convert.ToInt64(damage).ToString() + "(" + venoms.Count + ")";
                            Drawing.DrawText(mob.HPBarPosition.X, mob.HPBarPosition.Y - 5, Color.Red, t_damage);
                        }
                    }
                }
            }
            #endregion

            #region Items&spells
            //item
            int tempItemid = 3157;
            if (Jlib.getm_bool("useitem_zhonya") && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
            {
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Zhonyas_Hourglass))
                {
                    if (Player.HealthPercentage() <= (float)Jlib.getm_value("useitem_p_zhonya"))
                        p_item.UseItem();
                }
            }
            tempItemid = Convert.ToInt32(ItemId.Blade_of_the_Ruined_King);
            if (Jlib.getm_bool("useitem_botrk") && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid)
                                                        && Player.HealthPercentage() <= (float)Jlib.getm_value("useitem_p_botrk"))
            {
                Obj_AI_Hero target = null;
                Double max_healpoint = 0;
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Blade_of_the_Ruined_King))
                {
                    if (ObjectManager.Get<Obj_AI_Hero>().Any(h=>h.IsEnemy && !h.IsDead && h.IsVisible &&
                        Vector3.Distance(h.Position, Player.Position) <= Jlib.getm_value("useitem_botrk_atg_p")) && Jlib.getm_bool("useitem_botrk_atg"))
                        p_item.UseItem(target); ;

                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsEnemy && h.IsValid && h.IsVisible && Vector3.Distance(h.Position, Player.Position) <= 450))
                    {
                        var healpoint = Player.CalcDamage(hero, Damage.DamageType.Physical, hero.MaxHealth * 0.1);
                        if(max_healpoint < healpoint)
                        {
                            max_healpoint = healpoint;
                            target = hero;
                        }
                    }
                    p_item.UseItem(target);
                }
            }

            tempItemid = Convert.ToInt32(ItemId.Mikaels_Crucible);
            if (Jlib.getm_bool("useitem_mikaels") && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
            {
                List<BuffType> bufflist = new List<BuffType>();
                getbufflist(bufflist,ItemId.Mikaels_Crucible);
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Mikaels_Crucible))
                {
                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly && !h.IsMe && h.IsValid && h.IsVisible && Vector3.Distance(h.Position, Player.Position) <= 800))
                    {
                        if (hero.HealthPercentage() <= (float)Jlib.getm_value("useitem_p_mikaels"))
                            p_item.UseItem(hero);

                        if (Jlib.getm_bool("mikaels_cc_bool"))
                        {
                            foreach(var buff in hero.Buffs)
                            {
                                if (bufflist.Any(b => b == buff.Type))
                                    Utility.DelayAction.Add(Jlib.getm_value("useitem_p_mikaels_delay"), () => { p_item.UseItem(hero); }); 
                            }
                        }
                    }
                }
            }
            tempItemid = Convert.ToInt32(ItemId.Quicksilver_Sash);
            int tempItemid2 = Convert.ToInt32(ItemId.Mercurial_Scimitar);
            if (Jlib.getm_bool("useitem_qs_bool"))
            {
                if ((Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid)) || ((Items.HasItem(tempItemid2) && Items.CanUseItem(tempItemid2))))
                {
                    List<BuffType> bufflist = new List<BuffType>();
                    getbufflist(bufflist, ItemId.Quicksilver_Sash);
                    foreach (var p_item in Player.InventoryItems.Where(item => (item.Id == ItemId.Quicksilver_Sash || item.Id == ItemId.Mercurial_Scimitar)))
                    {
                        foreach (var buff in Player.Buffs)
                        {
                            Utility.DelayAction.Add(Jlib.getm_value("useitem_p_qs_delay"), () =>
                            {
                                if (bufflist.Any(b => b == buff.Type))
                                    p_item.UseItem();
                                if (buff.DisplayName == "zedulttargetmark")
                                    Game.PrintChat("zed! {0}", buff.Type.ToString());
                                if (buff.DisplayName == "fizzmarinerdoombomb")
                                    Game.PrintChat("fizz! {0}", buff.Type.ToString());
                                if (buff.DisplayName == "SoulShackles")
                                    Game.PrintChat("fizz! {0}", buff.Type.ToString());
                            });
                        }
                    }
                }
            }
                
            //potions
            tempItemid = Convert.ToInt32(ItemId.Crystalline_Flask);
            if (Jlib.getm_bool("useitem_flask") && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
            {
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Crystalline_Flask && !Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Player.HealthPercentage() <= (float)Jlib.getm_value("useitem_p_fla"))
                        p_item.UseItem();
                }
            }
            tempItemid = Convert.ToInt32(ItemId.Health_Potion);
            if (Jlib.getm_bool("useitem_hppotion") && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
            {
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Health_Potion && !Player.HasBuff("Health Potion") && !Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Player.HealthPercentage() <= (float)Jlib.getm_value("useitem_p_hp"))
                    {
                        p_item.UseItem();
                    }
                }
            }

            tempItemid = Convert.ToInt32(ItemId.Mana_Potion);
            if (Jlib.getm_bool("useitem_manapotion") && Items.HasItem(tempItemid) && Items.CanUseItem(tempItemid))
            {
                foreach (var p_item in Player.InventoryItems.Where(item => item.Id == ItemId.Mana_Potion && !Player.HasBuff("Mana Potion") && !Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Player.HealthPercentage() <= (float)Jlib.getm_value("useitem_p_mana"))
                    {
                        p_item.UseItem();
                    }
                }
            }
            //spell
            if (Jlib.getm_bool("usespell")&& defslot != SpellSlot.Unknown)
            {
                if (Player.HealthPercentage() <= (float)Jlib.getm_value("usespell_hp"))
                {
                    if (Player.Spellbook.CanUseSpell(defslot) == SpellState.Ready)
                        Player.Spellbook.CastSpell(defslot);
                }
            }

            #endregion

            #region ultnotifier
            //Karthus
            if (Player.BaseSkinName == "Karthus")
            {
                if (Jlib.getm_bool("noti_karthus") && Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                {
                    Spell R = new Spell(SpellSlot.R, 100000);
                    var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                    var damage = R.GetDamage(target);


                    if (target.IsValidTarget(R.Range) && target.IsVisible && damage >= target.Health + target.HPRegenRate * 3)
                    {
                        if (!text_Isrender)
                            text_notifier.Add();
                        text_Isrender = true;
                    }
                }
                else
                {
                    text_notifier.Remove();
                    text_Isrender = false;
                }
            }
            //cait
            if (Player.BaseSkinName == "Caitlyn")
            {
                if (Jlib.getm_bool("noti_cait") && Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                {
                    Spell R = new Spell(SpellSlot.R, 1500 + (500 * Player.Spellbook.GetSpell(SpellSlot.R).Level));
                    var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                    var damage = R.GetDamage(target);


                    if (target.IsValidTarget(R.Range) && target.IsVisible && damage >= target.Health + target.HPRegenRate)
                    {
                        if (!text_Isrender)
                            text_notifier.Add();
                        text_Isrender = true;
                        Jlib.targetPing(target.Position.To2D());


                    }
                }
                else
                {
                    text_notifier.Remove();
                    text_Isrender = false;
                }
            }
            //ez
            if (Player.BaseSkinName == "Ezreal")
            {
                if (Jlib.getm_bool("noti_ez") && Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                {
                    Spell R = new Spell(SpellSlot.R, 100000);
                    var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                    var damage = R.GetDamage(target);


                    if (target.IsValidTarget(R.Range) && target.IsVisible && damage >= target.Health + target.HPRegenRate * (2000f / Vector3.Distance(Player.ServerPosition, target.ServerPosition))) // time=speed/distance
                    {
                        if (!text_Isrender)
                            text_notifier.Add();
                        text_Isrender = true;
                        Jlib.targetPing(target.Position.To2D());
                    }
                }
                else
                {
                    text_notifier.Remove();
                    text_Isrender = false;
                }
            }
            //shen
            if (Player.BaseSkinName == "Shen")
            {
                if (Jlib.getm_bool("noti_shen") && Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready)
                {

                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead && hero.IsValid))
                    {
                        if (hero.HealthPercentage() <= Jlib.getm_value("noti_shenhp"))
                        {
                            if (!text_Isrender)
                                text_help.Add();
                            text_Isrender = true;

                            Jlib.targetPing(hero.Position.To2D(), Packet.PingType.AssistMe);
                        }
                        else
                        {
                            text_help.Remove();
                            text_Isrender = false;
                        }
                    }
                }
                else
                {
                    text_help.Remove();
                    text_Isrender = false;
                }
            }
            #endregion


            #region Status on hud
            if (Jlib.getm_bool("base_stat"))
            {
                /*
                 * 오토스마이트
                 * 오토이그나이트
                 * 점프와드
                 * 스택
                 * Items
                 * Spell
                 */

                int x = Monitor.Width - Jlib.getm_value("x");
                int y = Monitor.Height - Jlib.getm_value("y");
                int interval = 20;
                int i = 0;

                Game.PrintChat(x + "," + y);
                Drawing.DrawText(x, y, Color.Wheat, "Champion : " + Player.BaseSkinName);
                i++;


                if (smiteSlot != SpellSlot.Unknown)
                {
                    addText(y + (interval * i), (Jlib.getm_bool("AutoSmite") && smiteSlot != SpellSlot.Unknown), "AutoSmite");
                    i++;
                }

                if (igniteSlot != SpellSlot.Unknown)
                {
                    addText(y + (interval * i), (Jlib.getm_bool("AutoIgnite") && igniteSlot != SpellSlot.Unknown), "AutoIgnite");
                    i++;
                }

                if (jumpspell != null)
                {
                    addText(y + (interval * i), (Jlib.getm_bool("j2w_bool") && jumpspell != null), "Jump2Ward");
                    i++;
                }
                if (defslot != SpellSlot.Unknown)
                {
                    addText(y + (interval * i), (Jlib.getm_bool("usespell") && defslot != SpellSlot.Unknown), string.Format("SpellCast{0}", filterspellname(Player.Spellbook.GetSpell(defslot).Name).ToUpper()));
                    i++;
                }
                if (Items.HasItem(Convert.ToInt32(ItemId.Crystalline_Flask)))
                {
                    addText(y + (interval * i), (Jlib.getm_bool("useitem_flask")), string.Format("Use Flask({0}%)", Jlib.getm_value("useitem_p_flask")));
                    i++;
                }
                if (Items.HasItem(Convert.ToInt32(ItemId.Health_Potion)))
                {
                    addText(y + (interval * i), (Jlib.getm_bool("useitem_hppotion")), string.Format("Use HP Potion({0}%)", Jlib.getm_value("useitem_p_hp")));
                    i++;
                }
                if (Items.HasItem(Convert.ToInt32(ItemId.Mana_Potion)))
                {
                    addText(y + (interval * i), (Jlib.getm_bool("useitem_manapotion")), string.Format("Use Mana Potion({0}%)", Jlib.getm_value("useitem_p_mana")));
                    i++;
                }
                if (Items.HasItem(Convert.ToInt32(ItemId.Zhonyas_Hourglass)))
                {
                    addText(y + (interval * i), (Jlib.getm_bool("useitem_zhonya")), string.Format("UseZhonya({0}%)", Jlib.getm_value("useitem_p_zhonya")));
                    i++;
                }
                if (Items.HasItem(Convert.ToInt32(ItemId.Blade_of_the_Ruined_King)))
                {
                    addText(y + (interval * i), (Jlib.getm_bool("useitem_botrk")), string.Format("UseBOTRK({0}%)", Jlib.getm_value("useitem_p_botrk")));
                    i++;
                }
                if (Items.HasItem(Convert.ToInt32(ItemId.Mikaels_Crucible)))
                {
                    addText(y + (interval * i), (Jlib.getm_bool("useitem_mikaels")), string.Format("Use Mikaels({0}%{1}", Jlib.getm_value("useitem_p_mikaels"),
                        Jlib.getm_bool("mikaels_cc_bool") ? ",CC)" : ")"));
                    i++;
                }
                if (Items.HasItem(Convert.ToInt32(ItemId.Quicksilver_Sash)))
                {
                    addText(y + (interval * i), (Jlib.getm_bool("useitem_qs_bool")), string.Format("Use QS(delay:{0})", Jlib.getm_value("useitem_p_qs_delay")));
                    i++;
                }
                if (Items.HasItem(Convert.ToInt32(ItemId.Mercurial_Scimitar)))
                {
                    addText(y + (interval * i), (Jlib.getm_bool("useitem_qs_bool")), string.Format("Use Scimitar(delay:{0})", Jlib.getm_value("useitem_p_qs_delay")));
                    i++;
                }
                //champ
                #region stack
                if (Player.BaseSkinName == "Twitch")
                {
                    addText(y + (interval * i), (Jlib.getm_bool("st_twitch")), "CastTwitchE");
                    i++;
                }
                if (Player.BaseSkinName == "Kalista")
                {
                    addText(y + (interval * i), (Jlib.getm_bool("st_kalista")), "CastKalistaE");
                    i++;
                }
                #endregion
                #region notifier
                if (Player.BaseSkinName == "Karthus")
                {
                    addText(y + (interval * i), (Jlib.getm_bool("noti_karthus")), "UltNotifiler");
                }
                if (Player.BaseSkinName == "Ezreal")
                {
                    addText(y + (interval * i), (Jlib.getm_bool("noti_ez")), "UltNotifiler");
                }
                if (Player.BaseSkinName == "Caitlyn")
                {
                    addText(y + (interval * i), (Jlib.getm_bool("noti_cait")), "UltNotifiler");
                }
                if (Player.BaseSkinName == "Shen")
                {
                    addText(y + (interval * i), (Jlib.getm_bool("noti_shen")), "UltNotifiler");
                }
                #endregion
            }
            #endregion

            #region test

            /*
            if (baseMenu.Item("test")ssssssssssssssss){

                testf(test.show_mebuff);

                baseMenu.Item("test").SetValue<bool>(false);
            }
             */
            #endregion
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
            "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon", "SRU_BaronSpawn", "Sru_Crab"
        };


        public static void setSmiteSlot()
        {
            foreach (var spell in Player.Spellbook.Spells.Where(spell => String.Equals(spell.Name, smitetype(), StringComparison.CurrentCultureIgnoreCase)))
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
            int level = Player.Level;
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
            foreach (var spell in Player.Spellbook.Spells.Where(spell => String.Equals(spell.Name, "summonerdot", StringComparison.CurrentCultureIgnoreCase)))
            {
                igniteSlot = spell.Slot;
                ignite = new Spell(smiteSlot, 600);
                return;
            }
        }

        public static float setigniteDamage()
        {
            float dmg = 50 + 20 * Player.Level;
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

        #region 점프와드함수 - J2W
        public static void setj2wslots(List<String> a)
        {
            foreach (String champname in a)
            {
                if (champname == Player.BaseSkinName)
                {
                    canw2j = true;
                    switch (champname)
                    {
                        case "LeeSin":
                            jumpspell = new Spell(SpellSlot.W, 700);
                            return;
                        case "Katarina":
                            jumpspell = new Spell(SpellSlot.E, 700);
                            return;
                        case "Jax":
                            jumpspell = new Spell(SpellSlot.Q, 700);
                            return;
                    }
                }
            }
        }
        public static void checkE()
        {
            if (Player.BaseSkinName == "LeeSin")
            {
                rdyw2j = jumpspell.IsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne";
            }
            else
            {
                rdyw2j = jumpspell.IsReady();
            }
        }

        public static void checkWard()
        {
            var Slot = Items.GetWardSlot();
            rdyward = !(Slot == null || Slot.Stacks == 0);
        }


        #endregion

        #region 스펠함수 - Item & Spell
        public static void getbufflist(List<BuffType> list,ItemId whatitem)
        {
            if (whatitem == ItemId.Mikaels_Crucible)
            {
                if (Jlib.getm_bool("mikaels_cc_stun"))
                    list.Add(BuffType.Stun);
                if (Jlib.getm_bool("mikaels_cc_fear"))
                {
                    list.Add(BuffType.Fear);
                    list.Add(BuffType.Flee);
                }
                if (Jlib.getm_bool("mikaels_cc_charm"))
                    list.Add(BuffType.Charm);
                if (Jlib.getm_bool("mikaels_cc_taunt"))
                    list.Add(BuffType.Taunt);
                if (Jlib.getm_bool("mikaels_cc_snare"))
                    list.Add(BuffType.Snare);
                if (Jlib.getm_bool("mikaels_cc_silence"))
                    list.Add(BuffType.Silence);
                if (Jlib.getm_bool("mikaels_cc_polymorph"))
                    list.Add(BuffType.Polymorph);
            }
            if (whatitem == ItemId.Quicksilver_Sash)
            {
                if (Jlib.getm_bool("qs_cc_stun"))
                    list.Add(BuffType.Stun);
                if (Jlib.getm_bool("qs_cc_fear"))
                {
                    list.Add(BuffType.Fear);
                    list.Add(BuffType.Flee);
                }
                if (Jlib.getm_bool("qs_cc_charm"))
                    list.Add(BuffType.Charm);
                if (Jlib.getm_bool("qs_cc_taunt"))
                    list.Add(BuffType.Taunt);
                if (Jlib.getm_bool("qs_cc_snare"))
                    list.Add(BuffType.Snare);
                if (Jlib.getm_bool("qs_cc_silence"))
                    list.Add(BuffType.Silence);
                if (Jlib.getm_bool("qs_cc_polymorph"))
                    list.Add(BuffType.Polymorph);
                if (Jlib.getm_bool("qs_cc_suppression"))
                    list.Add(BuffType.Suppression);
            }
           
        }
        public static void setDefSpellSlot()
        {
            foreach (var spell in Player.Spellbook.Spells.Where(spell => spell.Name.Contains(DefSpellstr[0]) || spell.Name.Contains(DefSpellstr[1])))
            {
                defslot = spell.Slot;
                defspell = new Spell(defslot);
                return;
            }
        }
        #endregion

        #region 스택함수 - Stack
        public static double getKaliDmg(Obj_AI_Base target,int count,double AD,int s_level)
        {
            double[] spell_basedamage ={0,20,30,40,50,60};
            double[] spell_perdamage ={0,0.25,0.30,0.35,0.40,0.45};
            double eDmg = AD * 0.60 + spell_basedamage[s_level];
            count -= 1;
            eDmg = eDmg + count*(eDmg * spell_perdamage[s_level]);
            return Player.CalcDamage(target, Damage.DamageType.Physical,eDmg);
        }
        public static double getTwitEDmg(Obj_AI_Base target, int count, double AD,double AP, int s_level)
        {
            double[] spell_basedamage = { 0, 20, 35, 50, 65, 80 };
            double[] spell_stackdamage = { 0, 15, 20, 25, 30, 35 };
            double eDmg = spell_basedamage[s_level] + count * (spell_stackdamage[s_level] + (AP * 0.2) + (AD * 0.25));
            return Player.CalcDamage(target, Damage.DamageType.Physical, eDmg);
        }
        #endregion

        #region status 함수 - Status
        public static void addText(float y,bool a,String b)
        {
            Drawing.DrawText(Monitor.Width - Jlib.getm_value("x"), y, a ? Color.FromArgb(0, 255, 0) : Color.Red,
                b+"[" + (a ? "ON" :"OFF") + "]");
        }
        #endregion


        
    }
}



