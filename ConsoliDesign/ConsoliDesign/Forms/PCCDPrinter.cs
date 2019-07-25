using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConsoliDesign.Data;
using ConsoliDesign.Functions;
using System.IO;

namespace ConsoliDesign.Forms
{
    public partial class PCCDPrinter : Form
    {
        Printers pnts_new = new Printers();
        Printers pnts_old = new Printers();
       
         
        public PCCDPrinter()
        {
            InitializeComponent();
            pnts_new.printer = new List<Printerinfo>();
        }

        // update printer info
        private void button2_Click(object sender, EventArgs e)
        {
            
                string xx = Directory.GetCurrentDirectory();
                var currPath = Path.Combine(Environment.CurrentDirectory, "\\PrinterInfo.xml");

                if (File.Exists(currPath))
                {
                    // read file to get the current items
                    
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult diaResult = MessageBox.Show(this, "The file exists, do you want to update the data?", "Update reminder", buttons);
                    if (diaResult == DialogResult.Yes)
                    {

                        pnts_old = XmlHelper.FromXmlFile<Printers>(currPath);
                        foreach (Printerinfo pnt1 in pnts_new.printer)
                        {
                            pnts_old.printer.Add(pnt1);
                        }
                        pnts_new.printer.Clear();
                        XmlHelper.ToXmlFile(pnts_old, currPath);
                        MessageBox.Show(String.Format("Information of printer {0} is added", textBox1.Text));
                    }

                }
                else
                {
                    // create a new xml file called "PrinterInfo.xml"
                    // update printer info to the end

                    XmlHelper.ToXmlFile(pnts_new, currPath);
                    MessageBox.Show(String.Format("Information of printer {0} is saved", textBox1.Text));
                    pnts_new.printer.Clear();

                }

                textBox1.Text = "";
                textBox2.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                comboBox1.Text = "";
                comboBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                listBox1.Items.Clear();
                textBox1.Focus();
            
        }


        // ADD MATERIAL
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(comboBox1.SelectedItem.ToString());

        }

        // exit button
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // next button
        private void button3_Click(object sender, EventArgs e)
        {
            Printerinfo pnt = new Printerinfo();
            MachineParameters MPS;
             pnt.Name = textBox1.Text;
             if (listBox1.Items.Count > 0)
             {
                 pnt.Materials = new List<string>(listBox1.Items.Count);
                 for (int i = 0; i < listBox1.Items.Count; i++)
                 {
                     pnt.Materials.Add(listBox1.Items[i].ToString());
                 }
                 double[] volume = new double[3];
                 volume[0] = Convert.ToInt32(textBox2.Text);
                 volume[1] = Convert.ToInt32(textBox5.Text);
                 volume[2] = Convert.ToInt32(textBox6.Text);
                 pnt.Volume = volume.ToList<double>();
                 MPS = new MachineParameters 
                 { 
                     AMType = comboBox2.Text,
                     MinFeatureSize = Convert.ToDouble(textBox3.Text),
                     MinLayerThickness = Convert.ToDouble(textBox4.Text)
                 };
                 pnt.BasicInfo = MPS;
                 pnts_new.printer.Add(pnt);
                 textBox1.Text = "";
                 textBox2.Text = "";
                 textBox5.Text = "";
                 textBox6.Text = "";
                 comboBox1.Text = "";
                 comboBox2.Text = "";
                 textBox3.Text = "";
                 textBox4.Text = "";
                 listBox1.Items.Clear();
                 textBox1.Focus();

             }
             else
             {
                 MessageBox.Show("please fill the empty space");
             }
        }
    }
}
