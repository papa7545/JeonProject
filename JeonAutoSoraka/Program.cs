using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace JeonAutoSoraka
{
    public class Program
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static Vector3 spawn;
        private static bool recalling = false,StartBuy = true;
        private static Menu menu;
        private static double count;
        private static bool stopdoingshit = false;
        private static double foundturret;
        private static Obj_AI_Turret turret;
        private static double gamestart;
        private static ItemToShop nextItem;
        private static List<ItemToShop> buyThings;
        private static List<Obj_AI_Hero> allies;
        private static int i = 0;
        private static bool stopfollowingshittarget = false;



        private static readonly string[] ad =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "KogMaw",
            "MissFortune", "Quinn", "Sivir", "Tristana", "Twitch", "Varus", "Vayne", "Jinx", "Lucian"
        };

        private static readonly string[] ap =
        {
            "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana",
            "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus", "Kassadin", "Katarina", "Kayle", "Kennen",
            "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna", "Ryze", "Sion",
            "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra",
            "Velkoz"
        };

        private static readonly string[] bruiser =
        {
            "Darius", "Elise", "Evelynn", "Fiora", "Gangplank", "Gnar", "Jayce",
            "Pantheon", "Irelia", "JarvanIV", "Jax", "Khazix", "LeeSin", "Nocturne", "Olaf", "Poppy", "Renekton",
            "Rengar", "Riven", "Shyvana", "Trundle", "Tryndamere", "Udyr", "Vi", "MonkeyKing", "XinZhao", "Aatrox",
            "Rumble", "Shaco", "MasterYi","Shen"
        };

        private static Vector3 followpos;
        public static bool canBuyItems = true;
        private static Obj_AI_Hero follow;
        private static double followtime;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Soraka") return;
            allies = new List<Obj_AI_Hero>();
            Q = new Spell(SpellSlot.Q, 970);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R);

            menu = new Menu("Soraka Slack", "slack", true);
            menu.AddItem(new MenuItem("on", "Start Slacking!").SetValue(new KeyBind(32, KeyBindType.Toggle)));
            menu.AddItem(new MenuItem("user", "Use R?").SetValue(true));
            menu.AddItem(new MenuItem("usew", "Use W?").SetValue(true));
            menu.AddItem(new MenuItem("allyhpw", "Ally % HP for W").SetValue(new Slider(50, 0, 93)));
            menu.AddItem(new MenuItem("wabovehp", "Use W when my hp > x%").SetValue(new Slider(20, 0, 99)));
            menu.AddItem(new MenuItem("allyhpr", "Ally % HP for R").SetValue(new Slider(15, 0, 50)));
            menu.AddItem(new MenuItem("hpb", "B if hp < %").SetValue(new Slider(15, 0, 80)));

            menu.AddSubMenu(new Menu("Follow:", "follower"));
            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && !x.IsMe))
            {
                allies.Add(ally);
                if (ad.Contains(ally.ChampionName))
                    menu.SubMenu("follower").AddItem(new MenuItem(ally.ChampionName, ally.ChampionName).SetValue(true));
                else
                {
                    menu.SubMenu("follower").AddItem(new MenuItem(ally.ChampionName, ally.ChampionName).SetValue(false));
                }
            }

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
                    goldReach = 360,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(3096)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(1004),JeonItem.GetItemIdbyInt(1004)}
                },
                new ItemToShop()
                {
                    goldReach = 500,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(1004),JeonItem.GetItemIdbyInt(1004)},
                    itemIds = new List<ItemId>{JeonItem.GetItemIdbyInt(1033)}
                },
                new ItemToShop()
                {
                    goldReach = 180,
                    itemsMustHave = new List<ItemId>{JeonItem.GetItemIdbyInt(1033),JeonItem.GetItemIdbyInt(1004),JeonItem.GetItemIdbyInt(1004)},
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

            var myAutoLevel = new AutoLevel(new[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 3, 4, 3, 3, 1, 1, 4, 1, 3 });
            
            gamestart = Game.Time;
            menu.AddToMainMenu();
            nextItem = buyThings[0];
            followtime = Game.Time;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Game.OnGameUpdate += Game_OnGameUpdate;

        }


        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            GamePacket p = new GamePacket(args.PacketData);
            if (p.Header != Packet.S2C.TowerAggro.Header) return;
            if (Packet.S2C.TowerAggro.Decoded(args.PacketData).TargetNetworkId != ObjectManager.Player.NetworkId)
                return;
            if (Game.Time - foundturret > 20 && !recalling)
            {
                var turret2 =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Where(x => x.Distance(ObjectManager.Player.Position) < 3500 && x.IsAlly);

                if (turret2.Any())
                {
                    stopdoingshit = true;
                    turret = turret2.First();
                    foundturret = Game.Time;
                }
            }


            if (!stopdoingshit || recalling) return;
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, turret);
            if (!(ObjectManager.Player.Distance(turret.Position) <= 350) || !(Game.Time - count > 15)) return;
            ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall);

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
                for (int i = 0; i < ObjectManager.Player.InventoryItems.Count(); i++)
                {
                    if (usedItems[i])
                        continue;
                    if (t != (decimal)ObjectManager.Player.InventoryItems[i].Id) continue;
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


            if (!(Items.HasItem(3301) || Items.HasItem(3096) || Items.HasItem(3069)))
            {
                ObjectManager.Player.BuyItem(JeonItem.GetItemIdbyInt(3301));
                ObjectManager.Player.BuyItem(JeonItem.GetItemIdbyInt(3340));
                ObjectManager.Player.BuyItem(JeonItem.GetItemIdbyInt(2003));
                ObjectManager.Player.BuyItem(JeonItem.GetItemIdbyInt(2003));
                ObjectManager.Player.BuyItem(JeonItem.GetItemIdbyInt(2003));
            }



            if (!stopfollowingshittarget)
            {

                var hero = ObjectManager.Get<Obj_AI_Hero>().Where(x => !x.IsMe && x.IsAlly && ad.Contains(x.ChampionName));
                foreach(var t in hero)
                {
                    //Game.PrintChat("cast1");
                    follow = t;
                    break;
                }

                if(follow == null)
                {
                    hero = ObjectManager.Get<Obj_AI_Hero>().Where(x => !x.IsMe && x.IsAlly && ap.Contains(x.ChampionName));
                    foreach (var t in hero)
                    {
                        //Game.PrintChat("cast2");
                        follow = t;
                        break;
                    }
                }

                if(follow == null)
                {
                    hero = ObjectManager.Get<Obj_AI_Hero>().Where(x => !x.IsMe && x.IsAlly && bruiser.Contains(x.ChampionName));
                    foreach (var t in hero)
                    {
                        //Game.PrintChat("cast3");
                        follow = t;
                        break;
                    }
                }
                if (follow == null)
                {
                    follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly);
                }

            }




            if (Game.Time - gamestart > 480)
            {
                follow = allies[i];
                i++;
                gamestart = Game.Time;
            }

            if (Game.Time - followtime > 40 && followpos.Distance(follow.Position) <= 100)
            {
                follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly && ap.Contains(x.ChampionName)) ?? ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.IsAlly && bruiser.Contains(x.ChampionName));
                followpos = follow.Position;
                followtime = Game.Time;
                stopfollowingshittarget = true;
            }
            if (Utility.InShopRange())
            {
                foreach (var item in nextItem.itemIds)
                {
                    if (!Items.HasItem(Convert.ToInt32(item)))
                    {
                        ObjectManager.Player.BuyItem(item);
                    }
                }
                checkItemInventory();
            }

            Console.WriteLine("Recalling = " + recalling);

            Console.WriteLine("stop: " + stopdoingshit);
            if (Game.Time - foundturret > 25)
                stopdoingshit = false;


            if (follow.IsDead)
            {
                follow = ObjectManager.Get<Obj_AI_Hero>().First(x => !x.IsMe && x.Distance(ObjectManager.Player.Position) < 1300);
            }

            Console.WriteLine(follow.IsDead);
            if ((follow.IsDead ||
                 (follow.Distance(ObjectManager.Player.Position) > 5000 && !Utility.InShopRange() &&
                  spawn.Distance(follow.Position) < 1500) ||
                 ObjectManager.Player.Health / ObjectManager.Player.MaxHealth * 100 <
                 menu.Item("hpb").GetValue<Slider>().Value))
            {

                if (Game.Time - foundturret > 20 && !recalling)
                {
                    var turret2 =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(x => x.Distance(ObjectManager.Player.Position) < 3500 && x.IsAlly);

                    if (turret2.Any())
                    {
                        stopdoingshit = true;
                        turret = turret2.First();
                        foundturret = Game.Time;
                    }
                }


                if (stopdoingshit && !recalling)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, turret);
                    if (ObjectManager.Player.Distance(turret.Position) <= 350 && Game.Time - count > 15)
                    {
                        ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Recall);

                        recalling = true;
                        count = Game.Time;
                    }
                }
            }

            if ((Game.Time - count > 15 && Game.Time - count < 17))
            {
                stopdoingshit = false;
                recalling = false;
            }

            if (!recalling && !stopdoingshit && W.IsReady())
            {
                var allies2 =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            x =>
                                x.IsAlly && x.Health / x.MaxHealth * 100 < menu.Item("allyhpw").GetValue<Slider>().Value &&
                                !x.IsDead && x.Distance(ObjectManager.Player.Position) < 550);
                var objAiHeroes = allies2 as Obj_AI_Hero[] ?? allies2.ToArray();
                if (objAiHeroes.Any() &&
                    ObjectManager.Player.Health / ObjectManager.Player.MaxHealth * 100 >
                    menu.Item("wabovehp").GetValue<Slider>().Value)
                    W.Cast(objAiHeroes.First());
            }

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

            if (!recalling && !stopdoingshit)
            {
                if (follow.Distance(ObjectManager.Player.Position) > 500)
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, follow);
                if (!follow.IsDead)
                {
                    if (W.IsReady() && menu.Item("usew").GetValue<bool>() &&
                        ObjectManager.Player.Health / ObjectManager.Player.MaxHealth * 100 >
                        menu.Item("wabovehp").GetValue<Slider>().Value)
                    {
                        if (follow.Health / follow.MaxHealth * 100 < menu.Item("allyhpw").GetValue<Slider>().Value &&
                            follow.Distance(ObjectManager.Player.Position) < 550 &&
                            ObjectManager.Player.Health / ObjectManager.Player.MaxHealth * 100 >
                            menu.Item("wabovehp").GetValue<Slider>().Value)
                        {
                            W.Cast(follow);
                        }
                        else if (follow.Health / follow.MaxHealth * 100 < menu.Item("allyhpw").GetValue<Slider>().Value &&
                                 follow.Distance(ObjectManager.Player.Position) > 550)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, follow.Position);
                        }
                    }

                    if (Q.IsReady() && !Utility.UnderTurret(ObjectManager.Player, true))
                    {
                        var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                        Q.Cast(target);
                    }

                    if (E.IsReady() && !Utility.UnderTurret(ObjectManager.Player, true))
                    {

                        var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                        E.Cast(target);
                    }

                    if (!(follow.Distance(ObjectManager.Player.Position) > 350)) return;
                    Random x = new Random();
                    var xPos = ((spawn.X - follow.Position.X) / Vector3.Distance(follow.Position, spawn)) * 300 +
                               follow.Position.X -
                               x.Next(25, 150);
                    var yPos = ((spawn.Y - follow.Position.Y) / Vector3.Distance(follow.Position, spawn)) * 300 +
                               follow.Position.Y -
                               x.Next(25, 150);
                    var vec = new Vector3(xPos, yPos, follow.Position.Z);
                    if (
                        NavMesh.GetCollisionFlags(
                            vec.To2D().Extend(ObjectManager.Player.Position.To2D(), 150).To3D())
                            .HasFlag(CollisionFlags.None))
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, vec);
                    //Game.PrintChat("following");
                }

                else
                {
                    Random y = new Random();
                    var turret =
                        ObjectManager.Get<Obj_AI_Turret>()
                            .First(x => x.Distance(ObjectManager.Player.Position) < 3500 && x.IsAlly);
                    var xPos = ((spawn.X - turret.Position.X) / Vector3.Distance(turret.Position, spawn)) * 300 +
                               turret.Position.X -
                               y.Next(25, 150);
                    var yPos = ((spawn.Y - turret.Position.Y) / Vector3.Distance(turret.Position, spawn)) * 300 +
                               turret.Position.Y -
                               y.Next(25, 150);

                    var vec = new Vector3(xPos, yPos, follow.Position.Z);
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, vec);
                }
            }

        }
    }
}
