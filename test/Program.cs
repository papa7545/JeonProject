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


            Spell Q = new Spell(SpellSlot.Q, 1000);
            Q.StartCharging();
               
        }
        private static void OnGameUpdate(EventArgs args)
        {

        }
        private static void OnDraw_EndScene(EventArgs args)
        {
            
        }
        private static void OnDraw(EventArgs args)
        {

        }


        private static void OnCreate(GameObject sender, EventArgs args)
        {

        }


        private static void OnDelete(GameObject sender, EventArgs args)
        {

        }
    }
}
