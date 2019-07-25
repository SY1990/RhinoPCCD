using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConsoliDesign.Forms
{
    public partial class PCCDAddFunction : Form
    {
        public static List<string> functionList;
        public PCCDAddFunction(List<string> ifunction)
        {
            InitializeComponent();
            functionList = ifunction;
            this.txtBox_ID.Text = (ifunction.Count+1).ToString();
            
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            functionList.Add(txtBox_discription.Text);
            this.Close();
        }
    }
}
