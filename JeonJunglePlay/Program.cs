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

namespace JeonJunglePlay
{
    public class Program
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        private static Spell Q,W,E,R;
        private static Vector3 spawn;

        private static double gamestart;
        private static ItemToShop nextItem;
        private static List<ItemToShop> buyThings;
        public static List<timer_clock> timerlist = new List<timer_clock>();


        public static bool canBuyItems = true, IsBlueTeam;


        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static Spell smite;

        public static SpellDataInst Qdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R);

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {

            setSmiteSlot();

            if (Player.Team.ToString() == "Chaos")
            {
                spawn = new Vector3(14318f, 14354, 171.97f);
                Game.PrintChat("Set PurpleTeam Spawn");
                IsBlueTeam = false;
            }
            else
            {
                spawn = new Vector3(415.33f, 453.38f, 182.66f);
                Game.PrintChat("Set BlueTeam Spawn");
                IsBlueTeam = true;
            }


            Q = new Spell(SpellSlot.Q,GetSpellRange(Qdata));
            W = new Spell(SpellSlot.W, GetSpellRange(Wdata));
            E = new Spell(SpellSlot.E, GetSpellRange(Edata));
            R = new Spell(SpellSlot.R, GetSpellRange(Rdata));

            #region 아이템
            buyThings = new List<ItemToShop>
            {
                new ItemToShop()
                {
                    goldReach = 350,
                    itemsMustHave = new List<ItemId>{ItemId.Hunters_Machete},
                    itemIds = new List<ItemId>{ItemId.Rangers_Trailblazer}
                },
                new ItemToShop()
                {
                    goldReach = 450,
                    itemsMustHave = new List<ItemId>{ItemId.Rangers_Trailblazer},
                    itemIds = new List<ItemId>{ItemId.Dagger}
                },
                new ItemToShop()
                {
                    goldReach = 1050,
                    itemsMustHave = new List<ItemId>{ItemId.Dagger},
                    itemIds = new List<ItemId>{ItemId.Rangers_Trailblazer_Enchantment_Devourer}
                },
                new ItemToShop()
                {
                    goldReach = 325,
                    itemsMustHave = new List<ItemId>{ItemId.Rangers_Trailblazer_Enchantment_Devourer},
                    itemIds = new List<ItemId>{ItemId.Boots_of_Speed}
                },
                new ItemToShop()
                {
                    goldReach = 675,
                    itemsMustHave = new List<ItemId>{ItemId.Boots_of_Speed},
                    itemIds = new List<ItemId>{ItemId.Boots_of_Swiftness}
                },
                new ItemToShop()
                {
                    goldReach = 1400,
                    itemsMustHave = new List<ItemId>{ItemId.Boots_of_Swiftness},
                    itemIds = new List<ItemId>{ItemId.Bilgewater_Cutlass}
                },
                new ItemToShop()
                {
                    goldReach = 1800,
                    itemsMustHave = new List<ItemId>{ItemId.Bilgewater_Cutlass},
                    itemIds = new List<ItemId>{ItemId.Blade_of_the_Ruined_King}
                },
                new ItemToShop()
                {
                    goldReach = 1100,
                    itemsMustHave = new List<ItemId>{ItemId.Blade_of_the_Ruined_King},
                    itemIds = new List<ItemId>{ItemId.Zeal}
                },
                new ItemToShop()
                {
                    goldReach = 1700,
                    itemsMustHave = new List<ItemId>{ItemId.Zeal},
                    itemIds = new List<ItemId>{ItemId.Phantom_Dancer}
                },
                new ItemToShop()
                {
                    goldReach = 1550,
                    itemsMustHave = new List<ItemId>{ItemId.Phantom_Dancer},
                    itemIds = new List<ItemId>{ItemId.B_F_Sword}
                },
                new ItemToShop()
                {
                    goldReach = 2250,
                    itemsMustHave = new List<ItemId>{ItemId.B_F_Sword},
                    itemIds = new List<ItemId>{ItemId.Infinity_Edge}
                },
                new ItemToShop()
                {
                    goldReach = 2900,
                    itemsMustHave = new List<ItemId>{ItemId.Infinity_Edge},
                    itemIds = new List<ItemId>{ItemId.Last_Whisper}
                }
            };
            #endregion

            var myAutoLevel = new AutoLevel(new[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 3, 4, 3, 3, 1, 1, 4, 1, 3 });
            
            gamestart = Game.Time;

            nextItem = buyThings[0];


            #region 타이머
            timer_clock Baron = new timer_clock
            {
                ID = "Baron" , Position = new Vector3(4910f, 10268f, -71.24f),
                name = "SRU_BaronSpawn",
                respawntime = 420
            };
            timer_clock Dragon = new timer_clock
            {
                ID = "Dragon", Position = new Vector3(9836f, 4408f, -71.24f),
                name = "SRU_Dragon",
                respawntime = 360
            };
            timer_clock top_crab = new timer_clock
            {
                ID = "top_crab",
                Position = new Vector3(4266f, 9634f, -67.87f),
                name = "Sru_Crab",
                respawntime = 180,
                Range = 3000
            };
            timer_clock down_crab = new timer_clock
            {
                ID = "down_crab",
                Position = new Vector3(10524f, 5116f, -62.81f),
                name = "Sru_Crab",
                respawntime = 180,
                Range = 3000
            };

            timer_clock bteam_Razorbeak = new timer_clock { ID = "bteam_Razorbeak", Position = new Vector3(6974f, 5460f, 54f), name = "SRU_Razorbeak" };
            timer_clock bteam_Red = new timer_clock
            {
                ID = "bteam_Red", Position = new Vector3(7796f, 4028f, 54f),
                name = "SRU_Red",
                respawntime = 300
            };
            timer_clock bteam_Krug = new timer_clock { ID = "bteam_Krug", Position = new Vector3(8394f, 2750f, 50f), name = "SRU_Krug" };
            timer_clock bteam_Blue = new timer_clock
            {
                ID = "bteam_Blue", Position = new Vector3(3832f, 7996f, 52f),
                name = "SRU_Blue",
                respawntime = 300
            };
            timer_clock bteam_Gromp = new timer_clock { ID = "bteam_Gromp", Position = new Vector3(2112f, 8372f, 51.7f), name = "SRU_Gromp" };
            timer_clock bteam_Wolf = new timer_clock { ID ="bteam_Wolf" , Position = new Vector3(3844f, 6474f, 52.46f), name = "SRU_Murkwolf" };

            timer_clock pteam_Razorbeak = new timer_clock { ID ="pteam_Razorbeak" , Position = new Vector3(7856f, 9492f, 52.33f), name = "SRU_Razorbeak" };
            timer_clock pteam_Red = new timer_clock
            {
                ID ="pteam_Red" , Position = new Vector3(7124f, 10856f, 56.34f),
                name = "SRU_Red",
                respawntime = 300
            };
            timer_clock pteam_Krug = new timer_clock { ID = "pteam_Krug", Position = new Vector3(6495f, 12227f, 56.47f), name = "SRU_Krug" };
            timer_clock pteam_Blue = new timer_clock
            {
                ID = "pteam_Blue", Position = new Vector3(10850f, 6938f, 51.72f),
                name = "SRU_Blue",
                respawntime = 300
            };
            timer_clock pteam_Gromp = new timer_clock { ID = "pteam_Gromp", Position = new Vector3(12766f, 6464f, 51.66f), name = "SRU_Gromp" };
            timer_clock pteam_Wolf = new timer_clock { ID = "pteam_Wolf", Position = new Vector3(10958f, 8286f, 62.46f), name = "SRU_Murkwolf" };


            #endregion
            Game.OnGameUpdate += Game_OnGameUpdate;

        }



        internal class ItemToShop
        {
            public int goldReach;
            public List<ItemId> itemIds;
            public List<ItemId> itemsMustHave;
        }
        
        public class timer_clock
        {
            public Vector3 Position;
            public string ID;
            public string name;
            public bool show = false;
            public bool minimap_show = true;
            public int respawntime = 100;
            public int spawntime = 0;
            public int Range = 1000;

            public timer_clock()
            {
                timerlist.Add(this);
            }
        }

        public static float GetSpellRange(SpellDataInst targetSpell, bool IsChargedSkill = false)
        {
            if (targetSpell.SData.CastRangeDisplayOverride[0] <= 0)
            {
                if (targetSpell.SData.CastRange[0] <= 0)
                {
                    return
                        targetSpell.SData.CastRadius[0];
                }
                else
                {
                    if (!IsChargedSkill)
                        return
                            targetSpell.SData.CastRange[0];
                    else
                        return
                            targetSpell.SData.CastRadius[0];
                }
            }
            else
                return
                    targetSpell.SData.CastRangeDisplayOverride[0];
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
            setSmiteSlot();
            if (!(Items.HasItem(Convert.ToInt32(ItemId.Hunters_Machete))||
                Items.HasItem(Convert.ToInt32(ItemId.Rangers_Trailblazer)) || 
                Items.HasItem(Convert.ToInt32(ItemId.Rangers_Trailblazer_Enchantment_Devourer))))
            {
                if(smiteSlot == SpellSlot.Unknown)
                    Player.BuyItem(ItemId.Hunters_Machete);
            }


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

            if(IsBlueTeam)
            {
                if (Game.Time - gamestart >= 0)
                {
                    var pos = timerlist.First(t => t.ID == "bteam_Krug");
                    Player.IssueOrder(GameObjectOrder.MoveTo,pos.Position);
                }
            }
        }

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
        #endregion


    }
}
