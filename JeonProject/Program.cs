using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;


namespace JeonProject
{
    class Program
    {
        private static Menu baseMenu;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Game.OnGameUpdate += OnGameUpdate;

        }

        private static void OnGameLoad(EventArgs args) 
        {
            

            //메인메뉴
            baseMenu = new Menu("ProjectJ", "ProjectJ", true);

            var menu_smite = new Menu("Jsmite", "Jsmite");
            baseMenu.AddSubMenu(menu_smite);

            /* 
             스마이트 세부 메뉴
             */

            menu_smite.AddItem(new MenuItem("AutoSmite", "AutoSmite").SetValue(true));


            
            baseMenu.AddToMainMenu();



            //ObjectManager.Player.BaseSkinName <ㅡ 현재 사용하고 있는 챔피언의 기본 스킨 이름
   
        }
        private static void OnGameUpdate(EventArgs args)
        {
            if (baseMenu.Item("AutoSmite").GetValue<bool>())
            {
                Game.PrintChat("on");
            }
            if (!baseMenu.Item("AutoSmite").GetValue<bool>())
            {
                Game.PrintChat("off");
            }
        }
    }
}
