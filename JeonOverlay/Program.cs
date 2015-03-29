#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using System.Windows.Forms;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

namespace JeonOverlay
{
    class Program
    {

        public static LeagueSharp.Common.Menu baseMenu;
        public static List<LeagueSharp.Common.Menu> menus = new List<LeagueSharp.Common.Menu>();
        public static bool a = true;
        public static List<J_Image> Images = new List<J_Image>();
        public static string image;
        public static int layoutnum,maxlayout = 5;


        public static DirectoryInfo dir = new DirectoryInfo(Environment.GetEnvironmentVariable("appdata")+@"/LeagueSharp/JeonOverLay");
        public static FileInfo setFile = new FileInfo(dir + "/Setting.ini");

        public class J_Image
        {
            public int Layout;
            public Render.Sprite Image;

        }



        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
        public static void OnLoad(EventArgs args)
        {
            
            
            baseMenu = new LeagueSharp.Common.Menu("JeonOverlay", "JeonOverlay", true);
            baseMenu.AddToMainMenu();

            if (!dir.Exists)
                dir.Create();

            for(var i = 0 ; i<=maxlayout ; i++)
            {
                addlayout();
            }
            for (var i = 0; i <= maxlayout; i++)
            {
                checkINI(i);
            }

            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Game.PrintChat("<font color ='#33FFFF'>JeonOverlay v" + version + " </font>Loaded!");

            Game.OnUpdate += Game_OnGameUpdate;
        }
        public static void Game_OnGameUpdate(EventArgs args)
        {
            foreach (var menu in menus)
            {
                foreach (var item in menu.Items.Where(v => v.Name.StartsWith("file") && v.GetValue<bool>()))
                {
                    #region image load
                    OpenFileDialog pFileDlg = new OpenFileDialog();
                    pFileDlg.Filter = "Image File(*.png)|*.png";
                    pFileDlg.Title = "이미지 파일을 선택해주세요.(Choose your Image)";

                    if (pFileDlg.ShowDialog() == DialogResult.OK)
                    {
                        image = pFileDlg.FileName;
                        item.DisplayName
                            = "File : " + image.Split('\\').Last();

                        if (!Images.Any(t => t.Layout == (int)Char.GetNumericValue(item.Name.Last())))
                        {
                            var Img = new J_Image()
                            {
                                Layout = (int)Char.GetNumericValue(item.Name.Last()),
                                Image = new Render.Sprite(image, new Vector2(0, 0))
                                {
                                    VisibleCondition =
                                    c => menu.Item("visible" + (int)Char.GetNumericValue(item.Name.Last())).GetValue<bool>()
                                }
                            };
                            Img.Image.Add();
                            Images.Add(Img);
                            SetInI(menu.Name, image);
                        }
                        else
                        {
                            var temp = Images.First(t => t.Layout == (int)Char.GetNumericValue(item.Name.Last()));
                            temp.Image.UpdateTextureBitmap(new Bitmap(image));
                            SetInI(menu.Name, image);
                        }
                    }
                    item.SetValue(false);
                    #endregion
                }
                foreach (var item in menu.Items.Where(v => v.Name.StartsWith("reset") && v.GetValue<bool>()))
                {
                    #region reset
                    menu.Item("X" + (int)Char.GetNumericValue(item.Name.Last())).SetValue<Slider>(new Slider(200, 1, 2100));
                    menu.Item("Y" + (int)Char.GetNumericValue(item.Name.Last())).SetValue<Slider>(new Slider(200, 0, 2100));
                    menu.Item("Scale_X" + (int)Char.GetNumericValue(item.Name.Last())).SetValue<Slider>(new Slider(1000, 0, 3000));
                    menu.Item("Scale_Y" + (int)Char.GetNumericValue(item.Name.Last())).SetValue<Slider>(new Slider(1000, 0, 3000));
                    
                    item.SetValue(false);
                    #endregion
                }
            }
            
            foreach (var T in Images)
            {
                var x = (float)baseMenu.Item("X" + T.Layout).GetValue<Slider>().Value+Convert.ToSingle(baseMenu.Item("X_smooth" + T.Layout).GetValue<StringList>().SelectedValue);
                var y = (float)baseMenu.Item("Y" + T.Layout).GetValue<Slider>().Value+Convert.ToSingle(baseMenu.Item("Y_smooth" + T.Layout).GetValue<StringList>().SelectedValue);
                var scale_x = (float)baseMenu.Item("Scale_X" + T.Layout).GetValue<Slider>().Value
                    + Convert.ToSingle(baseMenu.Item("Scale_X_smooth" + T.Layout).GetValue<StringList>().SelectedValue);
                var scale_y = (float)baseMenu.Item("Scale_Y" + T.Layout).GetValue<Slider>().Value
                    + Convert.ToSingle(baseMenu.Item("Scale_Y_smooth" + T.Layout).GetValue<StringList>().SelectedValue);


                T.Image.Position = new Vector2(x, y);
                T.Image.Scale = new Vector2(scale_x / 1000, scale_y / 1000);
            }
            
        }


        public static void addlayout()
        {
            var temp = new LeagueSharp.Common.Menu("Layout" + layoutnum, "Layout" + layoutnum);
            baseMenu.AddSubMenu(temp);

            temp.AddItem(new LeagueSharp.Common.MenuItem("visible" + layoutnum, "Visible").SetValue(true));

            var fileitem = new LeagueSharp.Common.MenuItem("file" + layoutnum, "File:").SetValue(false);

            if(setFile.Exists)
            {
                var path = CaptureLib.GetSettingValue_String("Layout" + layoutnum, "path", setFile.FullName);
                fileitem.DisplayName
                            = "File : " + path.Split('\\').Last();
            }

            temp.AddItem(fileitem);

            var smoothvalue = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" 
            , "-10", "-9", "-8", "-7", "-6", "-5", "-4", "-3", "-2", "-1"};
            var smoothvalue2 = new[] { "0", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" 
            , "-100", "-90", "-80", "-70", "-60", "-50", "-40", "-30", "-20", "-10"};

            temp.AddItem(new LeagueSharp.Common.MenuItem("reset" + layoutnum, "Reset").SetValue(false));
            temp.AddItem(new LeagueSharp.Common.MenuItem("line" + layoutnum, "Coordinate--------------------------------------------------------"));
            temp.AddItem(new LeagueSharp.Common.MenuItem("X" + layoutnum, "X").SetValue<Slider>(new Slider(200, 1, 2100)));
            temp.AddItem(new LeagueSharp.Common.MenuItem("X_smooth" + layoutnum, "SmoothX").SetValue(new StringList(smoothvalue)));
            temp.AddItem(new LeagueSharp.Common.MenuItem("Y" + layoutnum, "Y").SetValue<Slider>(new Slider(200, 0, 2100)));
            temp.AddItem(new LeagueSharp.Common.MenuItem("Y_smooth" + layoutnum, "SmoothY").SetValue(new StringList(smoothvalue)));
            temp.AddItem(new LeagueSharp.Common.MenuItem("line_scale" + layoutnum, "Scale--------------------------------------------------------"));
            temp.AddItem(new LeagueSharp.Common.MenuItem("Scale_X" + layoutnum, "Scale_X").SetValue<Slider>(new Slider(1000, 0, 3000)));
            temp.AddItem(new LeagueSharp.Common.MenuItem("Scale_X_smooth" + layoutnum, "SmoothX").SetValue(new StringList(smoothvalue2)));
            temp.AddItem(new LeagueSharp.Common.MenuItem("Scale_Y" + layoutnum, "Scale_Y").SetValue<Slider>(new Slider(1000, 0, 3000)));
            temp.AddItem(new LeagueSharp.Common.MenuItem("Scale_Y_smooth" + layoutnum, "SmoothY").SetValue(new StringList(smoothvalue2)));
            menus.Add(temp);
            layoutnum += 1;
        }

        public static void checkINI(int layout)
        {
            var path = CaptureLib.GetSettingValue_String("Layout" + layout, "path", setFile.FullName);
            if(path != "None")
            {
                var Img = new J_Image()
                {
                    Layout = layout,
                    Image = new Render.Sprite(path, new Vector2(0, 0))
                    {
                        VisibleCondition =
                        c => baseMenu.Item("visible" + layout).GetValue<bool>()
                    }
                };
                Img.Image.Add();
                Images.Add(Img);
            }
        }
        public static void SetInI(string menu,string path)
        {
            CaptureLib.SetSettingValue(menu, "path", path, setFile.FullName);
        }
    }
}
