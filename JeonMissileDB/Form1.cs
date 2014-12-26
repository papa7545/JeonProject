using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace Jeon_MissileDB
{
    public partial class Form1 : Form
    {
        public static BindingList<string> Champions = new BindingList<string>();
        public static BindingList<string> SpellSlots = new BindingList<string>();
        public static bool Isinit = false;

        public Form1()
        {
            InitializeComponent();
            foreach(var t in Program.Infos)
            {
                if(!Champions.Contains(t.cName))
                    Champions.Add(t.cName);

                if(t.cName == "Aatrox")
                SpellSlots.Add(t.SlotName);
            }
            comboBox1.DataSource = Champions;
            comboBox2.DataSource = SpellSlots;
            Isinit = true;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Isinit)
            {
                SpellSlots.Clear();
                foreach (var t in Program.Infos)
                {
                    if (t.cName == comboBox1.SelectedValue.ToString())
                    {
                        SpellSlots.Add(t.SlotName);
                    }
                }
                foreach (var t in Program.Infos)
                {
                    if (t.cName == comboBox1.SelectedValue.ToString()
                        && t.SlotName == comboBox2.SelectedValue.ToString())
                    {
                        textBox1.Text = t.Type.ToString();
                        textBox2.Text = t.Delay.ToString();
                        textBox3.Text = t.Range.ToString();
                        textBox4.Text = t.Width.ToString();
                        textBox5.Text = t.Speed.ToString();
                    }
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach(var t in Program.Infos)
           {
               if(t.cName == comboBox1.SelectedValue.ToString()
                   && t.SlotName == comboBox2.SelectedValue.ToString())
               {
                   textBox1.Text = t.Type.ToString();
                   textBox2.Text = t.Delay.ToString();
                   textBox3.Text = t.Range.ToString();
                   textBox4.Text = t.Width.ToString();
                   textBox5.Text = t.Speed.ToString();
               }
           }
        }
    }
}
