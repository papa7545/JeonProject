#region
using System;
using LeagueSharp.Common;
#endregion

namespace JeonUtility
{
    class Jlib:Program
    {

        public enum test
        {
            show_inventory,
            show_enemybuff,
            show_allybuff,
            show_mebuff
        }



        public static bool getm_bool(String menu)
        {
            bool result;
            result = Program.baseMenu.Item(menu).GetValue<bool>();
            return result;    
        }
        public static int getm_value(String menu)
        {
            int result;
            result = Program.baseMenu.Item(menu).GetValue<Slider>().Value;
            return result;
        }
    }
}
