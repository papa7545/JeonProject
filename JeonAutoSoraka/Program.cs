using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace JeonAutoSoraka
{
    public class Program
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static Vector3 spawn;
        private static bool recalling = false;
        private static Menu menu;
        private static double count;
        private static bool stopfollowing = false;
        private static double foundturret;
        private static Obj_AI_Turret turret;
        private static double gamestart;
        private static ItemToShop nextItem;
        private static List<ItemToShop> buyThings;
        private static List<Obj_AI_Hero> allies;
        private static float pastTime, afktime;

        private static List<string> ad = new List<string>
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "KogMaw",
            "MissFortune", "Quinn", "Sivir", "Tristana", "Twitch", "Varus", "Vayne", "Jinx", "Lucian", "Kalista"
        };

        private static List<string> ap = new List<string>
        {
            "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana",
            "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus", "Kassadin", "Katarina", "Kayle", "Kennen",
            "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna", "Ryze", "Sion",
            "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra",
            "Velkoz"
        };

        private static List<string> bruiser = new List<string>
        {
            "Darius", "Elise", "Evelynn", "Fiora", "Gangplank", "Gnar", "Jayce",
            "Pantheon", "Irelia", "JarvanIV", "Jax", "Khazix", "LeeSin", "Nocturne", "Olaf", "Poppy", "Renekton",
            "Rengar", "Riven", "Shyvana", "Trundle", "Tryndamere", "Udyr", "Vi", "MonkeyKing", "XinZhao", "Aatrox",
            "Rumble", "Shaco", "MasterYi","Shen"
        };
        private static List<string> fllowlist = new List<string>();
        

        public static bool canBuyItems = true;
        private static Obj_AI_Hero follow;
        private static double followtime;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (!(Player.ChampionName.ToUpper() == "SORAKA"))
                return;

            if (Player.Team.ToString() == "Chaos")
            {
                spawn = new Vector3(14318f, 14354, 171.97f);
                Game.PrintChat("Set PurpleTeam Spawn");
            }
            else
            {
                spawn = new Vector3(415.33f, 453.38f, 182.66f);
                Game.PrintChat("Set BlueTeam Spawn");
            }


            allies = new List<Obj_AI_Hero>();

            Q = new Spell(SpellSlot.Q, 970);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R);

            menu = new Menu("JeonAutoSoraka", "JeonAutoSoraka", true);
            menu.AddItem(new MenuItem("user", "Use R?").SetValue(true));
            menu.AddItem(new MenuItem("usew", "Use W?").SetValue(true));
            menu.AddItem(new MenuItem("allyhpw", "Ally % HP for W").SetValue(new Slider(50, 0, 93)));
            menu.AddItem(new MenuItem("wabovehp", "Use W when my hp > x%").SetValue(new Slider(20, 0, 99)));
            menu.AddItem(new MenuItem("allyhpr", "Ally % HP for R").SetValue(new Slider(15, 0, 50)));
            menu.AddItem(new MenuItem("hpb", "B if hp < %").SetValue(new Slider(15, 0, 100)));

            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && !x.IsMe))
            {
                allies.Add(ally);
                fllowlist.Add(ally.ChampionName);
            }

            #region 아이템 트리
            buyThings = new List<ItemToShop>
            {
                new ItemToShop()
                {
                    goldReach = 500,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(3301)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(3096)}
                },
                new ItemToShop()
                {
                    goldReach = 180,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(3096)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(1004)}
                },
                new ItemToShop()
                {
                    goldReach = 500+180,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(1004)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(1033)}
                },
                new ItemToShop()
                {
                    goldReach = 180+180,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(1033),JeonItem.GetItemIdbyInt(1004)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(3028)}
                },
                new ItemToShop()
                {
                    goldReach = 325,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(3028)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(1001)}
                },
                new ItemToShop()
                {
                    goldReach = 675,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(1001)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(3009)}
                },
                new ItemToShop()
                {
                    goldReach = 400,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(3009)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(1028)}
                },
                new ItemToShop()
                {
                    goldReach = 450,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(1028)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(3067)}
                },
                new ItemToShop()
                {
                    goldReach = 400,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(3067)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(1028)}
                },
                new ItemToShop()
                {
                    goldReach = 800,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(1028)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(3211)}
                },
                new ItemToShop()
                {
                    goldReach = 700,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(3211)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(3065)}
                },
                new ItemToShop()
                {
                    goldReach = 2900,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(3065)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(3116)}
                }
            };
            #endregion

            var myAutoLevel = new AutoLevel(new[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 3, 4, 3, 3, 1, 1, 4, 1, 3 });
            
            gamestart = Game.Time;
            menu.AddToMainMenu();
            nextItem = buyThings[0];
            followtime = Game.Time;
            Game.OnProcessPacket += Game_OnGameProcessPacket;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += OnDraw_EndScene;
        }

        private static void OnDraw_EndScene(EventArgs args)
        {
            if (follow != null)
                Utility.DrawCircle(follow.Position, 100, System.Drawing.Color.Red,5,5);

            


            foreach (var t in ObjectManager.Get<Obj_AI_Turret>().Where(t => !t.IsDead && t.IsEnemy))
            {
                Utility.DrawCircle(t.Position, 775, System.Drawing.Color.Blue, 5, 5);
                if(Player.Distance(t.Position) <= 755 && follow.Distance(t.Position) >755)
                    Player.IssueOrder(GameObjectOrder.MoveTo, spawn);
            }


            Utility.DrawCircle(turret.Position, 100, System.Drawing.Color.Blue, 5, 5);
        }


        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {

            if (args.PacketData[0] == Packet.S2C.TowerAggro.Header)
            {
                Packet.S2C.TowerAggro.Struct aggroPacket = Packet.S2C.TowerAggro.Decoded(args.PacketData);
                Obj_AI_Turret turret = ObjectManager.Get<Obj_AI_Turret>().First(t => t.NetworkId == aggroPacket.TurretNetworkId);
                Obj_AI_Base target = ObjectManager.Get<Obj_AI_Base>().First(t => t.NetworkId == aggroPacket.TargetNetworkId);

                Game.PrintChat(turret.BaseSkinName + target.BaseSkinName);
            }



            GamePacket p = new GamePacket(args.PacketData);
            if (p.Header != Packet.S2C.TowerAggro.Header) return;
            if (Packet.S2C.TowerAggro.Decoded(args.PacketData).TargetNetworkId != Player.NetworkId)
                return;

            if (Game.Time - foundturret > 20 && !recalling)
            {
                var turret2 =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Where(x => x.Distance(Player.Position) < 3500 && x.IsAlly);

                if (turret2.Any())
                {
                    stopfollowing = true;
                    turret = turret2.First();
                    foundturret = Game.Time;
                }
            }


            if (!stopfollowing || recalling) //따라다니고 있고 리콜중이라면 패스
                return;

            Player.IssueOrder(GameObjectOrder.MoveTo, turret); // 터렛으로 움직여

            if ((Player.Distance(turret.Position) <= 350) || !(Game.Time - count > 15)) // 15초가 안지났고 터렛과의 거리가 350보다 크면
                return;
            Player.Spellbook.CastSpell(SpellSlot.Recall);

            recalling = true;
            count = Game.Time;
        }

        internal class ItemToShop
        {
            public int goldReach;
            public List<ItemId> itemIds;
            public List<ItemId> itemsMustHave;
        }

        private static bool checkItemcount(ItemToShop its)
        {
            bool[] usedItems = new bool[7];
            int itemsMatch = 0;
            foreach (int t in its.itemsMustHave)
            {
                for (int i = 0; i < Player.InventoryItems.Count(); i++)
                {
                    if (usedItems[i])
                        continue;
                    if (t != (decimal)Player.InventoryItems[i].Id) continue;
                    usedItems[i] = true;
                    itemsMatch++;
                    break;
                }
            }
            return itemsMatch == its.itemsMustHave.Count;
        }

        public static void checkItemInventory()
        {
            if (!canBuyItems)
                return;
            for (int i = buyThings.Count - 1; i >= 0; i--)
            {
                if (!checkItemcount(buyThings[i])) continue;
                nextItem = buyThings[i];
                if (i == buyThings.Count - 1)
                {
                    canBuyItems = false;
                }
                return;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            /////////////////check afk////////////////////////////
            if(Game.Time - pastTime >= 1 && !Player.IsDead && !Player.IsRecalling() && (Game.Time - gamestart > 70))
            {
                pastTime = Game.Time;
                afktime += 1;
                Game.PrintChat(afktime.ToString());
                if (afktime > 55) // 잠수 55초 경과
                {
                    Player.IssueOrder(GameObjectOrder.AttackTo,
                        new Vector3(4910f, 10268f, -71.24f));
                    follow = null;
                    afktime = 0;
                }


                #region 우물 잠수 45초 경과
                if (afktime > 45 && spawn.Distance(Player.Position) < 1500)
                {
                    Game.PrintChat("AFK DETECTED, Change Target");
                    if(fllowlist.Contains(follow.ChampionName))
                        fllowlist.Remove(follow.ChampionName);
                    follow = null;
                    afktime = 0;
                }
                #endregion
            }
            //////////////////////////////////////////////////////


            //////////////////check player in turret range///////////////////////




            ////////////////////////////////////////////////////////////////
            if (fllowlist.Count == 0)
            {
                foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && !x.IsMe))
                {
                    fllowlist.Add(ally.ChampionName);
                    Game.PrintChat("Follow LIST is NULL. ADD ALLY CHAMPIONS");
                }
            }




            #region 시작아이템 구입
            if (!(Items.HasItem(3301) || Items.HasItem(3096) || Items.HasItem(3069)))
            {
                Player.BuyItem(JeonItem.GetItemIdbyInt(3301));
                Player.BuyItem(JeonItem.GetItemIdbyInt(3340));
                Player.BuyItem(JeonItem.GetItemIdbyInt(2003));
                Player.BuyItem(JeonItem.GetItemIdbyInt(2003));
                Player.BuyItem(JeonItem.GetItemIdbyInt(2003));
            }
            #endregion

            #region 상점이용가능할때
            if (Utility.InShop(Player) || Player.IsDead) 
            {
                foreach (var item in nextItem.itemIds)
                {
                    if (!Items.HasItem(Convert.ToInt32(item)))
                    {
                        Player.BuyItem(item);
                    }
                }
                checkItemInventory();
            }
            #endregion

            #region 타겟이 안잡혀있음 - 타겟 설정 부분
            if (!stopfollowing)
            {
                if (ObjectManager.Get<Obj_AI_Hero>().Any(x => !x.IsMe && fllowlist.Contains(x.ChampionName) && ad.Contains(x.ChampionName)))
                {
                    follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && fllowlist.Contains(x.ChampionName) && ad.Contains(x.ChampionName));
                }
                else if (follow == null && ObjectManager.Get<Obj_AI_Hero>().Any(x => !x.IsMe && fllowlist.Contains(x.ChampionName) && ap.Contains(x.ChampionName)))
                {
                    follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && fllowlist.Contains(x.ChampionName) && ap.Contains(x.ChampionName));
                }
                else if (follow == null && ObjectManager.Get<Obj_AI_Hero>().Any(x => !x.IsMe && fllowlist.Contains(x.ChampionName) && bruiser.Contains(x.ChampionName)))
                {
                    follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && fllowlist.Contains(x.ChampionName) && bruiser.Contains(x.ChampionName));
                }
                else if (follow == null)
                {
                    follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && fllowlist.Contains(x.ChampionName));
                }
            }
            #endregion





            if (Game.Time - foundturret > 25) // 터렛발견하고 25초 경과 
            {
                stopfollowing = false;
            }


            #region 타겟이 죽어 있는 경우 (임시) 타겟 체인지
            if (follow.IsDead)
            {
                Game.PrintChat("Target is dead. Change Targeting");
                if (ObjectManager.Get<Obj_AI_Hero>().Any(x => !x.IsMe && x.IsAlly && !x.IsDead && x.Distance(Player.Position) < 1800))
                {
                    follow =
                        ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly && !x.IsDead && x.Distance(Player.Position) < 1800);
                }
                else
                {
                    follow =
                        ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly && !x.IsDead);
                }
            }
            #endregion

            #region follow가 죽었거나. (타겟과의거리가 5000이상이고. 내가 상점범위에 없고.타겟의 포지션이 상점내)이거나. 플레이어의 체력퍼센트가 설정된것 이하 일경우
            if (follow.IsDead ||
                 (follow.Distance(Player.Position) > 5000 && !Utility.InShop(Player) &&  spawn.Distance(follow.Position) < 1500) ||
                 Player.HealthPercentage() <
                 menu.Item("hpb").GetValue<Slider>().Value && !Player.IsDead)
            {
                if (Game.Time - foundturret > 20 && !recalling)
                {
                    Game.PrintChat("WARNNING! MUST RECALL");
                    var turret2 =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(x => x.IsAlly);

                    if (turret2.Any())
                    {
                        stopfollowing = true;
                        turret = turret2.OrderBy(x => x.Distance(Player.Position)).First();
                        foundturret = Game.Time;
                    }
                }


                if (stopfollowing && !recalling)
                {
                    Game.PrintChat("WARNNING! MOVE TURRET");
                    Player.IssueOrder(GameObjectOrder.MoveTo, turret);
                    if (Player.Distance(turret.Position) <= 350 && Game.Time - count > 15)
                    {
                        Player.Spellbook.CastSpell(SpellSlot.Recall);

                        recalling = true;
                        count = Game.Time;
                    }
                }
            }
            #endregion

            

            if ((Game.Time - count > 15)) //터렛이동한지 15초~17초 사이일때
            {
                stopfollowing = false;
                recalling = false;
            }

            if (Player.ChampionName.ToUpper() == "SORAKA")
            {

                #region W발동부분 - 아군
                if (!recalling && !stopfollowing && W.IsReady())
                {

                    var allies2 =
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                x =>
                                    x.IsAlly && x.Health / x.MaxHealth * 100 < menu.Item("allyhpw").GetValue<Slider>().Value &&
                                    !x.IsDead && x.Distance(Player.Position) < 550);
                    var objAiHeroes = allies2 as Obj_AI_Hero[] ?? allies2.ToArray();
                    if (objAiHeroes.Any() &&
                        Player.Health / Player.MaxHealth * 100 >
                        menu.Item("wabovehp").GetValue<Slider>().Value)
                        W.Cast(objAiHeroes.First());
                }
                #endregion
                
                #region R발동
                if (menu.Item("user").GetValue<bool>() && R.IsReady())
                {
                    var allies =
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                x =>
                                    x.IsAlly && x.Health / x.MaxHealth * 100 < menu.Item("allyhpr").GetValue<Slider>().Value &&
                                    !x.IsDead);
                    if (allies.Any())
                    {
                        if (R.IsReady())
                            R.Cast();
                    }
                }
                #endregion
            }
            #region 리콜중도 아니고 따라다니는중일때
            if (!recalling && !stopfollowing)
            {
                #region 거리가 500이상 5500이하일때
                if (follow.Distance(Player.Position) > 300 && follow.Distance(Player.Position) < 5500)
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, follow.Position.To2D().Extend(spawn.To2D(), 200).To3D());
                    afktime = 0;
                }
                else // 거리가 안으로 도달했을때 위치설정
                {
                    
                }
                #endregion

                #region 타겟과 거리가 5500이상일때
                if (follow.Distance(Player.Position) > 5500)
                {
                    if(spawn.Distance(follow.Position) <= 3000) // 타겟이 기지에 있다
                    {
                        Random y = new Random();
                        var turret =
                            ObjectManager.Get<Obj_AI_Turret>().First(x => x.Distance(Player.Position) < 3500 && x.IsAlly);

                        var xPos = ((spawn.X - turret.Position.X) / Vector3.Distance(turret.Position, spawn)) * 300 +
                               turret.Position.X -
                               y.Next(25, 150);
                        var yPos = ((spawn.Y - turret.Position.Y) / Vector3.Distance(turret.Position, spawn)) * 300 +
                               turret.Position.Y -
                               y.Next(25, 150);
                        
                        var vec = new Vector3(xPos, yPos, follow.Position.Z);
                        Player.IssueOrder(GameObjectOrder.MoveTo, vec);
                        afktime = 0;
                    }
                    else // 타겟이 기지 밖에 있다
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, follow.Position.To2D().Extend(spawn.To2D(), 200).To3D());
                        afktime = 0;
                    }
                }
                #endregion

                #region 타겟이 죽지않았을때 (평상시)
                if (!follow.IsDead)
                {
                    if (Player.ChampionName.ToUpper() == "SORAKA")
                    {
                        #region W발동-타겟만
                        if (W.IsReady() && menu.Item("usew").GetValue<bool>() &&
                            Player.Health / Player.MaxHealth * 100 >
                            menu.Item("wabovehp").GetValue<Slider>().Value)
                        {
                            if (follow.Health / follow.MaxHealth * 100 < menu.Item("allyhpw").GetValue<Slider>().Value &&
                                follow.Distance(Player.Position) < 550 &&
                                Player.Health / Player.MaxHealth * 100 >
                                menu.Item("wabovehp").GetValue<Slider>().Value)
                            {
                                W.Cast(follow);
                                afktime = 0;
                            }
                            else if (follow.Health / follow.MaxHealth * 100 < menu.Item("allyhpw").GetValue<Slider>().Value &&
                                     follow.Distance(Player.Position) > 550)
                            {
                                Player.IssueOrder(GameObjectOrder.MoveTo, follow.Position);
                                afktime = 0;
                            }
                        }
                        #endregion

                        #region 오토Q
                        if (Q.IsReady() && !Utility.UnderTurret(Player, true))
                        {
                            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                            Q.Cast(target);
                            afktime = 0;
                        }
                        #endregion

                        #region 오토E
                        if (E.IsReady() && !Utility.UnderTurret(Player, true))
                        {

                            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                            E.Cast(target);
                            afktime = 0;
                        }
                        #endregion
                    }
                    else
                    {
                        var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                        var wTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                        var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                        var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                        if (Q.CanCast(qTarget))
                            Q.Cast(qTarget);
                        if (W.CanCast(qTarget))
                            W.Cast(qTarget);
                        if (E.CanCast(qTarget))
                            E.Cast(qTarget);
                        if (R.CanCast(qTarget))
                            R.Cast(qTarget);
                    }
                }
                #endregion

                #region 타겟이 죽었을때
                else
                {
                    Game.PrintChat("Target is dead. Change Targeting");
                    if (ObjectManager.Get<Obj_AI_Hero>().Any(x => !x.IsMe && x.IsAlly && !x.IsDead && x.Distance(Player.Position) < 1800))
                    {
                        follow =
                            ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly && !x.IsDead && x.Distance(Player.Position) < 1800);
                    }
                    else
                    {
                        follow =
                            ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly && !x.IsDead);
                    }
                }
                #endregion
            }
            #endregion

        }
    }
}
