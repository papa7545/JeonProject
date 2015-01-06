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
