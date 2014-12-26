using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace JeonZilean
{
    class Program
    {
        public static String ChampionName = "Zilean";//ObjectManager.Player.BaseSkinName;
        public static Menu baseMenu;
        public static List<Spell> ComboSpells;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }
        static void OnGameLoad(EventArgs args) // 게임이 로드될 때 딱 한번
        {
            if (ChampionName == "Zilean")
                Game.PrintChat("Zilean Script loaded!");
            else
            {
                Game.PrintChat("You are not Zilean, this scripts unloaded");
                return;
            }

            //메뉴,플레이어 캐릭터 체크
            baseMenu = new Menu("JeonZilean", "JeonZilean",true);
            baseMenu.AddToMainMenu();

            var menu_Qskill = new Menu("Q-Bomb", "Q-Bomb");
            baseMenu.AddSubMenu(menu_Qskill);
            var menu_Wskill = new Menu("W-Cooldown", "W-Cooldown");
            baseMenu.AddSubMenu(menu_Wskill);
            var menu_Eskill = new Menu("E-TimeWarp", "E-TimeWarp");
            baseMenu.AddSubMenu(menu_Eskill);
            var menu_Rskill = new Menu("R-ChronoShift", "R-ChronoShift");
            baseMenu.AddSubMenu(menu_Rskill);

            menu_Qskill.AddItem(new MenuItem("menu_q_isincombo", "Combo:").SetValue<bool>(true));//ON - OFF <bool> 
            menu_Wskill.AddItem(new MenuItem("menu_w_isincombo", "Combo:").SetValue<bool>(true));
            menu_Eskill.AddItem(new MenuItem("menu_e_isincombo", "Combo:").SetValue<bool>(true));


            baseMenu.AddItem(new MenuItem("menu_combokey", "ComboKey :").SetValue(new KeyBind('T', KeyBindType.Press)));
            //-- 메뉴 만들기 끝--//
            Game.OnGameUpdate += Update;
            
        }
        static void Update(EventArgs args) // 1FPS 당 출력, ex)61FPS -> 1초에 61번 호출된다는것,
        {
            if(baseMenu.Item("menu_q_isincombo").GetValue<bool>())
            {
                //내용
                //combo 넣어버려
                ComboSpells.Add(new Spell(SpellSlot.Q, 700f));
            }
            if (baseMenu.Item("menu_w_isincombo").GetValue<bool>())
            {
                //내용
                //콤보스펠에 넣어버려

                ComboSpells.Add(new Spell(SpellSlot.W));
            }
            if (baseMenu.Item("menu_e_isincombo").GetValue<bool>())
            {
                //내용
                //콤보스펠에 넣어버려
                ComboSpells.Add(new Spell(SpellSlot.E, 700f));
            }

            if(baseMenu.Item("menu_combokey").GetValue<bool>())
            {
                foreach(var spell in ComboSpells)
                {

                    if (spell.Slot == SpellSlot.Q)
                    {
                        foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && !hero.IsDead && hero.IsVisible &&
                        Vector3.Distance(ObjectManager.Player.Position, hero.Position) <= spell.Range))
                        {
                            spell.CastOnUnit(target);
                        }
                    }
                    if (spell.Slot == SpellSlot.W)
                        spell.Cast();
                    if (spell.Slot == SpellSlot.E)
                        spell.Cast();
                }
            }
        }
    }
}
