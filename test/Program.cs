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


namespace test
{
    class Program
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }
        private static void OnGameLoad(EventArgs args)
        {
            Game.OnGameUpdate += OnGameUpdate;

            Game.OnGameUpdate += OnGameUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Drawing.OnEndScene += OnDraw_EndScene;
            Drawing.OnDraw += OnDraw;
            Game.PrintChat("load test");
        }
        private static void OnGameUpdate(EventArgs args)
        {

        }
        private static void OnDraw_EndScene(EventArgs args)
        {

        }
        private static void OnDraw(EventArgs args)
        {
            Obj_AI_Hero target;
            if (ObjectManager.Get<Obj_AI_Hero>().Any(t => t.Distance(Game.CursorPos) <= 200))
                target = ObjectManager.Get<Obj_AI_Hero>().First(t => t.Distance(Game.CursorPos) <= 200);


            var s = Player.Position.Normalized();

            Game.PrintChat((s * Player.Position).ToString());

            Utility.DrawCircle(Player.Position*s, 10, Color.Red);

            Utility.DrawCircle(JVector.Front(Player, 100), 10, Color.Red);
            Utility.DrawCircle(JVector.Back(Player, 100), 10, Color.DarkRed);
            Utility.DrawCircle(JVector.Left(Player, 100), 10, Color.Blue);
            Utility.DrawCircle(JVector.Right(Player, 100), 10, Color.White);

            Utility.DrawCircle(JVector.ForCursor(Player, 300), 10, Color.White);
            
            
        }


        private static void OnCreate(GameObject sender, EventArgs args)
        {

        }


        private static void OnDelete(GameObject sender, EventArgs args)
        {

        }
    }
}
