#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion

namespace JeonComboScriptor
{
    class Program:CaptureLib
    {
        /*
        ComboOrder=1
        Range=200
        Damagetype=1
        IsMissile=0
        IsBlockable=0
        IsIgnorePrediction=0
        DmgLv1=50
        DmgPer=30
        +totalAD(%)=0.25
        +addAD(%)=0.25
        +AP(%)=0.45
        =SpeacialList((:D))=
        +EnemyAP(%)=0
        +MaxMana(%)=0
        +EnemyMaxHP(%)=0
        +EnemyCurHP(%)=0
        +EnemyMissHP(%)=0
        +AP(%)Per100AP=0
        IsNeedCalculate=0
         */

        public class SpellStatus
        {
            //read info in ini file
            public int Range, ComboOrder, Damagetype;
            public double DmgLv1, DmgPer, totalAD, addAD, totalAP, EnemyAP, MaxMana, EnemyMaxHP, EnemyCurHP, EnemyMissHP, Per100AP;
            public bool IsMissile, IsBlockable, IsIgnorePrediction,IsNeedCalculate;

            //read info in Client
            public int level;
            public float manacost;
            public SpellSlot slot;
        }

        public static String cName = "TwistedFate";//Player.baseSkinName
        public static DirectoryInfo dir = new DirectoryInfo(LeagueSharp.Common.Config.LeagueSharpDirectory.ToString() + "/JeonScriptor");
        public static FileInfo setFile = new FileInfo(dir + "/" + cName + ".ini");

        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static SpellStatus Q = new SpellStatus(), W = new SpellStatus(), E = new SpellStatus(), R = new SpellStatus();

        static void Main(string[] args)
        {
            Console.WriteLine("dir : " + dir.FullName);

            if (!dir.Exists)
                dir.Create();

            if (!setFile.Exists)
            {
                Readini.SetSpellstatus("Q");
                Readini.SetSpellstatus("W");
                Readini.SetSpellstatus("E");
                Readini.SetSpellstatus("R");
            }

            Readini.GetSpellstatus(Q, "Q");
            Readini.GetSpellstatus(W, "W");
            Readini.GetSpellstatus(E, "E");
            Readini.GetSpellstatus(R, "R");

            Console.ReadLine();
        }
    }
}
