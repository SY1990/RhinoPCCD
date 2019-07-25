using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rhino;
using ConsoliDesign.Data;
using ConsoliDesign.Functions;

namespace ConsoliDesign.Forms
{
    public partial class PCCDMainEntrance : Form
    {
        private RhinoDoc doc;
        public static string fileName;
        public static string xmlfileName;
        public static Product result;
        public PCCDMainEntrance(RhinoDoc doc)
        {
            InitializeComponent();
            this.doc = doc;
        }
        private PCCDInfoSetup_New Form { get; set; }
        private PCCDInfoSetup_Edit Form2 { get; set; }
        private PCCDSolve Form3 { get; set; }
        private PCCDMainEntrance Form4 { get; set; }
        private PCCDConsolidation Form5 { get; set; }
  
        // edit existing xml file      
        private void editAnExistingSessionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
          
            if (null == Form2)
            {
                Form2 = new PCCDInfoSetup_Edit();
                Form2.FormClosed += OnFormClosed;
                this.Close();
                Form2.Show(RhinoApp.MainWindow());
                //Form.Show();
            }
        }
        // extract all information from the beginning
        private void startANewSessionToolStripMenuItem_Click(object sender1, EventArgs e1)
        {
            if (null == Form)
            {
                Form = new PCCDInfoSetup_New(doc);
                Form.FormClosed += OnFormClosed2;
                this.Close();
                Form.Show(RhinoApp.MainWindow());
                //Form.Show();
            }

        }
        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Form2.Dispose();
            Form2 = null;
        }

        private void OnFormClosed2(object sender1, FormClosedEventArgs e1)
        {
            Form.Dispose();
            Form = null;
        }
       
        private void openCADFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            var fd = new Rhino.UI.OpenFileDialog { Filter = "CAD Files (*.3dm;*.step)|*.3dm;*.step"};
            fileName = fd.FileName;

            if (fd.ShowOpenDialog() == true)
            {
                fileName = fd.FileName;
               // MessageBox.Show(fileName, "File directory");

            }
            else
            {
                MessageBox.Show("no file is selected");
            }
            if (fileName != null)
            {
                RhinoDoc.OpenFile(fileName);
                doc.Views.Redraw();
            }
            
            //doc.Views.Redraw();
            
            
           
            
        }

        private void openXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fdXml = new Rhino.UI.OpenFileDialog { Filter = "XML Files (*.xml)|*.xml"};
            
            if (fdXml.ShowOpenDialog() == true)
            {
                 xmlfileName = fdXml.FileName;
                 result = XmlHelper.FromXmlFile<Product>(xmlfileName);
                 if (result.filepath != null)
                 {
                     RhinoDoc.OpenFile(result.filepath);
                     doc.Views.Redraw();

                 }
                 else
                 {
                     MessageBox.Show("The given file cannot be successfully open");
                 }
            }
            else
            {
                MessageBox.Show("no file is selected");
            }

            
        }

        private void findFeasibleGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == Form3)
            {
                Form3 = new PCCDSolve(doc);
                Form3.FormClosed += OnFormClosed3;
                this.Close();
                Form3.Show(RhinoApp.MainWindow());
                //Form.Show();
            }
        }

        private void OnFormClosed3(object sender, FormClosedEventArgs e)
        {
            Form3.Dispose();
            Form3 = null;
           // Form4 = new PCCDMainEntrance(doc);
          //  Form4.Show(RhinoApp.MainWindow());
        }
        private void OnFormClosed4(object sender, FormClosedEventArgs e)
        {
            Form4.Dispose();
            Form4 = null;
           
        }

        private void redesignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == Form5)
            {
                Form5 = new PCCDConsolidation(doc);
                Form5.FormClosed += OnFormClosed5;
                this.Close();
                Form5.Show(RhinoApp.MainWindow());
                //Form.Show();
            }
        }
        private void OnFormClosed5(object sender, FormClosedEventArgs e)
        {
            Form5.Dispose();
            Form5 = null;
           // Form4 = new PCCDMainEntrance(doc);
          //  Form4.Show(RhinoApp.MainWindow());
        }
      
    }
}
