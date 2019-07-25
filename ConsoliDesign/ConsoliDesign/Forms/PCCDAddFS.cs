using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConsoliDesign.Data;

namespace ConsoliDesign.Forms
{
    public partial class PCCDAddFS : Form
    {
        private List<string> listOfFunctions;
        private PCCDAddFunction AddFunctionform;
        public List<FunctionalInterface> FIList;
        private int PartID;
        public PCCDAddFS(int iFIid, int iPartID, List<FunctionalInterface> iFIList)
        {
            InitializeComponent();
            FIList = iFIList;
            PartID = iPartID;
            listOfFunctions = new List<string>();
           // create a new surface 
            txtbox_ID.Text = (iFIid+ 1).ToString();
            this.TopMost = true;

        }
       /* public PCCDAddFS()
        {
            InitializeComponent();
            txtbox_ID.Text = "1";
            listOfFunctions = new List<string>();
        }*/



        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // select multiple surfaces
        private void Add_surface_Click(object sender, EventArgs e)
        {
            txtBox_flag.Text = "True";
            txtBox_flag.BackColor = Color.Green;
            // hide the current window

            // enable filter of multiple surfaces


        }

        
        /// <summary>
        ///  Add & delete functions related 
        /// </summary>

        private void btn_addfunction_Click(object sender, EventArgs e)
        {
            AddFunctionform = new PCCDAddFunction(listOfFunctions);
            AddFunctionform.FormClosed += OnfunctionFormClosed;
            AddFunctionform.Show();

        }
        private void OnfunctionFormClosed(object sender, FormClosedEventArgs e)
        {
            AddFunctionform.Dispose();
            AddFunctionform = null;
            UpdateFunctionList();
            // Update current form
        }

        private void UpdateFunctionList()
        {
            listBox_Functions.Items.Clear();
            for (int i = 0; i < listOfFunctions.Count; i++)
            {
                listBox_Functions.Items.Add(listOfFunctions[i]);
            }
            return;
        }

        private void btn_DeletFSs_Click(object sender, EventArgs e)
        {
            int selectedIndex = this.listBox_Functions.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex >= listOfFunctions.Count)
            {
                MessageBox.Show("Please select item for delete", "Warning");
                return;
            }
            listOfFunctions.RemoveAt(selectedIndex);
            UpdateFunctionList();
        }

        private void DisplayFS(FunctionalInterface fs)
        {
            txtbox_ID.Text = fs.id.ToString();
            txtbox_Name.Text = fs.name;
            if (fs.surfaceList.Count != 0)
            {
                txtBox_flag.Text = "True";
            }
            else
            {
                txtBox_flag.Text = "False";
            }
            listBox_Functions.Items.Clear();
            combo_FSType.SelectedIndex = (int)fs.type;
            for (int i = 0; i < fs.functionList.Count; i++)
            {
                listBox_Functions.Items.Add(fs.functionList[i]);
            }
            return;
        }

        // save functional FI info
        private void btn_save_Click(object sender, EventArgs e)
        {
            FunctionalInterface fs = new FunctionalInterface();
            fs.id =Convert.ToInt32(txtbox_ID.Text);
            fs.name = txtbox_Name.Text;
            fs.type =  (FIType)combo_FSType.SelectedIndex;
            /*for (int i = 0; i < listBox_Functions.Items.Count; i++)
            {
                fs.functionList.Add(listBox_Functions.Items[i].ToString());
            }*/
            fs.functionList = listBox_Functions.Items.OfType<string>().ToList();
            fs.partId = PartID;
            // Add fs.surfaceList
            FIList.Add(fs);

            // 

        }

        private void Next_Click(object sender, EventArgs e)
        {
            txtbox_ID.Text = (FIList.Count + 1).ToString();
            txtbox_Name.Text = "";
            listBox_Functions.Items.Clear();
            txtBox_flag.Text = "";
            txtBox_flag.BackColor = Color.White;
            combo_FSType.SelectedItem = null;
            listOfFunctions.Clear();
        }



    }
}
