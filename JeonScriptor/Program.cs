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
    

    class Program
    {
        public class SpellStatus
        {
            public int range,order;
            public double ADper,APper;
            public string type;
            public bool IsMissile,IsBlockable;
        }

        public static String player = "a";
        public static DirectoryInfo dir = new DirectoryInfo(LeagueSharp.Common.Config.LeagueSharpDirectory.ToString() + "/JeonScriptor");
        public static FileInfo setFile = new FileInfo(dir+"/"+player+".ini");

        static void Main(string[] args)
        {

            SpellStatus Q = new SpellStatus();
            SpellStatus W = new SpellStatus();
            SpellStatus E = new SpellStatus();
            SpellStatus R = new SpellStatus();
            
            Console.WriteLine("dir : "+dir.FullName);

            if (!dir.Exists)
                dir.Create();

            if (!setFile.Exists)
            {
                SetSpellstatus("Q");
                SetSpellstatus("W");
                SetSpellstatus("E");
                SetSpellstatus("R");
            }
                GetSpellstatus(Q, "Q");
                GetSpellstatus(W, "W");
                GetSpellstatus(E, "E");
                GetSpellstatus(R, "R");



            Console.WriteLine(Q.order);
            Console.ReadLine();
        }

        public static void SetSpellstatus(string targetSpell)
        {
            Capture.SetSettingValue(targetSpell, "ComboOrder", "1", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "Range", "200", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "Damagetype", "1", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "IsMissile", "0", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "IsBlockable", "0", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "DmgLv1", "50", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "DmgPer", "30", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+AD(%)", "25", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+AP(%)", "45", setFile.FullName);
            Capture.SetSettingValue(targetSpell, "=SpeacialList((:D))", "",setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+EnemyAP(%)", "0",setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+MaxMana(%)", "0",setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+EnemyMaxHP(%)", "0",setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+EnemyCurHP(%)", "0",setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+EnemyMissHP(%)", "0",setFile.FullName);
            Capture.SetSettingValue(targetSpell, "+AP(%p)Per100AP", "0",setFile.FullName);
            Capture.SetSettingValue(targetSpell, "BonusDamagePerStacks", "0",setFile.FullName);
        }
        public static void GetSpellstatus(SpellStatus targetSpell,String name)
        {
            targetSpell.order = Capture.GetSettingValue_Int(name, "ComboOrder",setFile.FullName);
            targetSpell.range = Capture.GetSettingValue_Int(name, "Range",setFile.FullName);
            targetSpell.type = Capture.GetSettingValue_String(name, "Damagetype",setFile.FullName);
            targetSpell.IsMissile = Capture.GetSettingValue_Bool(name, "IsMissile",setFile.FullName);
            targetSpell.IsBlockable = Capture.GetSettingValue_Bool(name, "IsBlockable",setFile.FullName);
            targetSpell.ADper = Capture.GetSettingValue_Double(name, "+AD(%)",setFile.FullName);
            targetSpell.APper = Capture.GetSettingValue_Double(name, "+AP(%)",setFile.FullName);
        }
    }
}
