#region
using System;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
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

        public static void Drawtext_outlined(Font font, String text, int x, int y, Color color)
        {
            font.DrawText(null, text, x + 1, y + 1, Color.Black);
            font.DrawText(null, text, x, y + 1, Color.Black);
            font.DrawText(null, text, x - 1, y - 1, Color.Black);
            font.DrawText(null, text, x, y - 1, Color.Black);
            font.DrawText(null, text, x, y, color);
        }
    }
}
