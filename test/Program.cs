#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.IO;
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
            Game.OnUpdate += OnGameUpdate;

            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Drawing.OnEndScene += OnDraw_EndScene;
            Drawing.OnDraw += OnDraw;
            Game.PrintChat("load test");

            OpenFileDialog pFileDlg = new OpenFileDialog();
            pFileDlg.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";
            pFileDlg.Title = "편집할 파일을 선택하여 주세요.";
            if (pFileDlg.ShowDialog() == DialogResult.OK)
            {
                String strFullPathFile = pFileDlg.FileName;
                // ToDo
            }

               
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
