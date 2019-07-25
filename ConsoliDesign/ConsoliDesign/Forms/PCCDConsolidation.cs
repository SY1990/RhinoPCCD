using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rhino;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Commands;
using Rhino.Geometry;
using ConsoliDesign.Data;
using ConsoliDesign.Functions;

namespace ConsoliDesign.Forms
{
    public partial class PCCDConsolidation : Form
    {
        private RhinoDoc doc;
        private PCCDMainEntrance form;
        private List<List<int>> nodeGroups;
        private List<RhinoObject> RhObjList;
        private List<int> NodeMark;
        private Product currProduct;
        private List<int> Nodes;
        private PCCDAddFS AddFSForm;
        private List<FunctionalInterface> ListofFunctionalInterfaceofPart = new List<FunctionalInterface> ();
        private List<List<FunctionalInterface>> ListofAllFunctionalInterface;
       // private FunctionalVolPart FVSingle = new FunctionalVolPart ();
       // private FunctionalVolGroup FVGroup;
       // private FunctionalVolProduct FVProduct;
        int NumberOfFIs;
        int flag = 0;
        int flag1 = 0;
        int flag2 = 0;
        int flag3 = 0;
        int flag4 = 0;
        int flag5 = 0;
        int GroupID = 1;
        int FlagAdd = 0;
        public string fileName;
        public PCCDConsolidation(RhinoDoc doc)
        {
            InitializeComponent();
            this.doc = RhinoDoc.ActiveDoc;
        }

        private void PCCDConsolidation_Activate(object sender, System.EventArgs e)
        {
            currProduct = PCCDMainEntrance.result;
            nodeGroups= PCCDSolve.NodeGroups;
            RhObjList = PCCDSolve.rhObjList;
            NodeMark = new List<int>();
            //FVGroup = new FunctionalVolGroup();
            int mark = 1;
            for (int i = 0; i < nodeGroups.Count; i++)
            {
                string str ="";
                if (nodeGroups[i].Count > 1)
                {
                    NodeMark.Add(i);
                    
                    for (int j=0; j< nodeGroups[i].Count; j++)
                    {
                        str += nodeGroups[i].ElementAt(j).ToString() +" ";  
                    }
                    comboBox1.Items.Add(string.Format("G{0}: ({1})", mark,str));
                    mark++;
                }
            }
            // initialize ListofAllFunctionalInterface
          
           
        }
        // exit
        private void button6_Click(object sender, EventArgs e)
        {
            this.Dispose();
            form = new PCCDMainEntrance(doc);
            form.Show(RhinoApp.MainWindow());
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            richTextBox1.Clear();
            Nodes = new List<int>();
            Nodes = nodeGroups[NodeMark[comboBox1.SelectedIndex]].ToList();
            for (int i = 0; i < Nodes.Count; i++)
            {
                string str = "";
                str = Nodes[i].ToString() + "-" + currProduct.parts[Nodes[i]-1].Name;
                richTextBox1.AppendText(str);
                richTextBox1.AppendText("\n");
            }
            flag = 0;
        }

        // Confirm
        private void button1_Click(object sender, EventArgs e)
        {
            Layer NewLayer = new Layer();
            Layer OldLayer = new Layer();
            RhinoObject obj;
            string str;
            Nodes = nodeGroups[NodeMark[comboBox1.SelectedIndex]].ToList();
            OldLayer = doc.Layers.CurrentLayer;
            if (flag == 0)
            {
               
                NewLayer.Name = "Function-FI";
                doc.Layers.Add(NewLayer);
                NewLayer.LayerIndex = doc.Layers.FindByFullPath(NewLayer.Name, true);
                doc.Layers.SetCurrentLayerIndex(NewLayer.LayerIndex, true);
                NewLayer.IsVisible = true;
                NewLayer.CommitChanges();

                for (int i = 0; i < Nodes.Count; i++)
                {
                    obj =RhObjList[Nodes[i]-1];
                    Guid Duplicates = doc.Objects.Transform(obj.Id, Transform.Identity, false);
                    RhinoObject rhobj = doc.Objects.Find(Duplicates);
                    if (rhobj != null)
                    {
                        rhobj.Attributes.LayerIndex = NewLayer.LayerIndex;
                        rhobj.CommitChanges();
                    }
                }
               
                OldLayer.IsVisible = false;
                NewLayer.CommitChanges();
                
                flag = 1;
                // initialize FVGroup info
                /*FVGroup.ID = GroupID;
                FVGroup.GroupName = string.Format("Group{0}", GroupID);
                FVGroup.FVSingle = new List<FunctionalVolPart>();
                */
            }
            OldLayer.CommitChanges();
            doc.Views.Redraw();
            string treev = "xxx";
            treeView1.Nodes.Add(treev, string.Format("Group{0}", comboBox1.SelectedIndex+1));
            for (int i = 0; i < Nodes.Count; i++)
            {
                str = Nodes[i].ToString() + "-" + currProduct.parts[Nodes[i] - 1].Name;
                TreeNode node = treeView1.Nodes[treev].Nodes.Add(str);
            }
            treeView1.ExpandAll();

            // Initialize 
            ListofAllFunctionalInterface = new List<List<FunctionalInterface>>(Nodes.Count);
            for (int i=0; i<Nodes.Count; i++)
            {
                ListofAllFunctionalInterface.Add(new List<FunctionalInterface>());
            }
            NumberOfFIs = 0;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView2.Nodes.Clear();
            int selectedIndex = treeView1.SelectedNode.Index;
            if (ListofAllFunctionalInterface[selectedIndex].Count == 0)
            {
                MessageBox.Show("There exist no FI information identified!");
            }
            else
            {

                UpdateTreeview2(ListofAllFunctionalInterface.ElementAt(selectedIndex));
            }
  

        }
    


        // identify FI of a selected part
        private void button2_Click(object sender, EventArgs e)
        {
            
            int selectedId = treeView1.SelectedNode.Index;
            int partId = Nodes[selectedId];
            
            // Open the FI dialogue editor
            for (int i = 0; i < selectedId; i++)
            {
                NumberOfFIs += ListofAllFunctionalInterface[i].Count;
            }
            ListofFunctionalInterfaceofPart.Clear();
            AddFSForm = new PCCDAddFS(NumberOfFIs, partId, ListofFunctionalInterfaceofPart);
            
            AddFSForm.FormClosed += OnAddFSFormClosed;
            AddFSForm.Show();
            //ListofAllFunctionalInterface.Add(ListofFunctionalInterfaceofPart.ToList());
            
            
           

        }

     

        // Delete all FI of a selected node in FI list
        private void button3_Click(object sender, EventArgs e)
        {
            int selectedIndex = treeView2.SelectedNode.Index;

            
            /*if (selectedIndex < 0 || selectedIndex >= listOfFunctions.Count)
            {
                MessageBox.Show("Please select item for delete", "Warning");
                return;
            }
            listOfFunctions.RemoveAt(selectedIndex);
            UpdateFunctionList();
             * */
        }

  
        private void OnAddFSFormClosed(object sender, FormClosedEventArgs e)
        {
          
            AddFSForm.Dispose();
            AddFSForm = null;
            // update treeview2
            int selectedId = treeView1.SelectedNode.Index;
            ListofAllFunctionalInterface[selectedId] = ListofFunctionalInterfaceofPart.ToList();
            UpdateTreeview2(ListofFunctionalInterfaceofPart);
           
        }

        private void UpdateTreeview2(List<FunctionalInterface> FSList)
        {
            treeView2.Nodes.Clear();
            string treev = "xxx2";
          
            treeView2.Nodes.Add(treev, "Part"+FSList.ElementAt(0).partId.ToString());
            for (int i = 0; i < FSList.Count; i++)
            {
               TreeNode node = treeView2.Nodes[treev].Nodes.Add(FSList.ElementAt(i).name+"_"+FSList.ElementAt(i).type.ToString());
            }
             
        }
     
        // next button
        private void button7_Click(object sender, EventArgs e)
        {

            if (GroupID <= NodeMark.Count)
            {
                // save FVGroup info
               /* FVProduct.FVMultiple.Add(FVGroup);
                FVGroup = new FunctionalVolGroup();
                //* */
                GroupID++;
            }
            else
            {
                MessageBox.Show("No more part groups to be redesigned! Please press exit.");
            }

        }

        // Review all functional interfaces
        private void button8_Click(object sender, EventArgs e)
        {
            // Display all FIs in treeview2 and in the graphic window
            treeView2.Nodes.Clear();
            string treev = "xxx3";
            treeView2.Nodes.Add(treev, string.Format("Group{0}", comboBox1.SelectedIndex + 1));
            List<FunctionalInterface> FSList = new List<FunctionalInterface>();
            for (int i = 0; i < ListofAllFunctionalInterface.Count; i++)
            { 
                FSList = ListofAllFunctionalInterface[i].ToList();
                if (FSList.Count != 0)
                {
                    for (int j = 0; j < FSList.Count; j++)
                    {
                        TreeNode node = treeView2.Nodes[treev].Nodes.Add(FSList.ElementAt(j).name);
                        // insert values to step 3
                        comboBox3.Items.Add(FSList.ElementAt(j).name);
                        comboBox4.Items.Add(FSList.ElementAt(j).name);
                        comboBox5.Items.Add(FSList.ElementAt(j).name);
                        comboBox6.Items.Add(FSList.ElementAt(j).name);
                    }
                }
            }
            // create another layer

            Layer NewLayer = new Layer();
            Layer OldLayer = new Layer();
            OldLayer = doc.Layers.CurrentLayer;
            if (flag1 == 0)
            {

                NewLayer.Name = "Exst. FI";
                doc.Layers.Add(NewLayer);
                NewLayer.LayerIndex = doc.Layers.FindByFullPath(NewLayer.Name, true);
                doc.Layers.SetCurrentLayerIndex(NewLayer.LayerIndex, true);
                NewLayer.IsVisible = true;
                NewLayer.CommitChanges();
                // import the extracted FIs
                fileName = "F:\\CodeSource\\Test\\RhinoPCCD\\CAD models\\OldFI.3dm";
                //RhinoDoc.OpenFile(fileName);
                string script = string.Format("_-Import \"{0}\" _Enter", fileName);
                Rhino.RhinoApp.RunScript(script,false);
           
                OldLayer.IsVisible = false;
                NewLayer.CommitChanges();

                flag1 = 1;
                // initialize FVGroup info
                /*FVGroup.ID = GroupID;
                FVGroup.GroupName = string.Format("Group{0}", GroupID);
                FVGroup.FVSingle = new List<FunctionalVolPart>();
                */
            }
            OldLayer.CommitChanges();
            doc.Views.Redraw();


        }

       // Synthesize button
        private void button4_Click_1(object sender, EventArgs e)
        {
            // Import new version of FIs
            Layer NewLayer = new Layer();
            Layer OldLayer = new Layer();
            
            OldLayer = doc.Layers.CurrentLayer;
            if (flag2 == 0)
            {

                NewLayer.Name = "New FI";
                doc.Layers.Add(NewLayer);
                NewLayer.LayerIndex = doc.Layers.FindByFullPath(NewLayer.Name, true);
                doc.Layers.SetCurrentLayerIndex(NewLayer.LayerIndex, true);
                NewLayer.IsVisible = true;
                NewLayer.CommitChanges();
                // import the extracted FIs
                fileName = "F:\\CodeSource\\Test\\RhinoPCCD\\CAD models\\NewFI.3dm";
                //RhinoDoc.OpenFile(fileName);
                string script = string.Format("_-Import \"{0}\" _Enter", fileName);
                Rhino.RhinoApp.RunScript(script, false);

                OldLayer.IsVisible = false;
                NewLayer.CommitChanges();

                flag2 = 1;
                // initialize FVGroup info
                /*FVGroup.ID = GroupID;
                FVGroup.GroupName = string.Format("Group{0}", GroupID);
                FVGroup.FVSingle = new List<FunctionalVolPart>();
                */
            }
            OldLayer.CommitChanges();
            doc.Views.Redraw();
        }

        // CalBO button
        private void button5_Click_1(object sender, EventArgs e)
        {
            //orientate the FIs in Z direction and set the origin to be on the XY plane.
                // Import new version of FIs
            Layer NewLayer = new Layer();
            Layer OldLayer = new Layer();
            
            OldLayer = doc.Layers.CurrentLayer;
            if (flag3 == 0)
            {

                NewLayer.Name = "NewFI_BO";
                doc.Layers.Add(NewLayer);
                NewLayer.LayerIndex = doc.Layers.FindByFullPath(NewLayer.Name, true);
                doc.Layers.SetCurrentLayerIndex(NewLayer.LayerIndex, true);
                NewLayer.IsVisible = true;
                NewLayer.CommitChanges();
                // import the extracted FIs
                fileName = "F:\\CodeSource\\Test\\RhinoPCCD\\CAD models\\NewFI_BO.3dm";
                //RhinoDoc.OpenFile(fileName);
                string script = string.Format("_-Import \"{0}\" _Enter", fileName);
                Rhino.RhinoApp.RunScript(script, false);

                OldLayer.IsVisible = false;
                NewLayer.CommitChanges();

                flag3 = 1;
                // initialize FVGroup info
                /*FVGroup.ID = GroupID;
                FVGroup.GroupName = string.Format("Group{0}", GroupID);
                FVGroup.FVSingle = new List<FunctionalVolPart>();
                */
            }
            OldLayer.CommitChanges();
            doc.Views.Redraw();
        

        }

        //generrate FVs
        private void button12_Click(object sender, EventArgs e)
        {
            // import the FV models
            Layer NewLayer = new Layer();
            Layer OldLayer = new Layer();

            OldLayer = doc.Layers.CurrentLayer;
            if (flag4 == 0)
            {

                NewLayer.Name = "FVs";
                doc.Layers.Add(NewLayer);
                NewLayer.LayerIndex = doc.Layers.FindByFullPath(NewLayer.Name, true);
                doc.Layers.SetCurrentLayerIndex(NewLayer.LayerIndex, true);
                NewLayer.IsVisible = true;
                NewLayer.CommitChanges();
                // import the extracted FIs
                fileName = "F:\\CodeSource\\Test\\RhinoPCCD\\CAD models\\FVs.3dm";
                //RhinoDoc.OpenFile(fileName);
                string script = string.Format("_-Import \"{0}\" _Enter", fileName);
                Rhino.RhinoApp.RunScript(script, false);

                OldLayer.IsVisible = false;
                NewLayer.CommitChanges();

                flag4 = 1;
                // initialize FVGroup info
                /*FVGroup.ID = GroupID;
                FVGroup.GroupName = string.Format("Group{0}", GroupID);
                FVGroup.FVSingle = new List<FunctionalVolPart>();
                */
            }
            OldLayer.CommitChanges();
            doc.Views.Redraw();
        }

        private void comboBox9_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (comboBox8.SelectedIndex == 0 && comboBox9.SelectedIndex == 0)
            {
                textBox1.Text = "ConnectTheDots";
            }
            else if (comboBox8.SelectedIndex == 0 && comboBox9.SelectedIndex == 1)
            {
                textBox1.Text = "MaximumBoundary";
            }
            else
            {
                textBox1.Text = "CTD or MB";

            }

        }
        // view IDS
        private void button9_Click(object sender, EventArgs e)
        {
            // Import IDS model
            Layer NewLayer = new Layer();
            Layer OldLayer = new Layer();

            OldLayer = doc.Layers.CurrentLayer;
            if (flag5 == 0)
            {

                NewLayer.Name = "IDS";
                doc.Layers.Add(NewLayer);
                NewLayer.LayerIndex = doc.Layers.FindByFullPath(NewLayer.Name, true);
                doc.Layers.SetCurrentLayerIndex(NewLayer.LayerIndex, true);
                NewLayer.IsVisible = true;
                NewLayer.CommitChanges();
                // import the extracted FIs
                fileName = "F:\\CodeSource\\Test\\RhinoPCCD\\CAD models\\IDS.3dm";
                //RhinoDoc.OpenFile(fileName);
                string script = string.Format("_-Import \"{0}\" _Enter", fileName);
                Rhino.RhinoApp.RunScript(script, false);

                OldLayer.IsVisible = false;
                NewLayer.CommitChanges();

                flag5 = 1;
                // initialize FVGroup info
                /*FVGroup.ID = GroupID;
                FVGroup.GroupName = string.Format("Group{0}", GroupID);
                FVGroup.FVSingle = new List<FunctionalVolPart>();
                */
            }
            OldLayer.CommitChanges();
            doc.Views.Redraw();

        }







    }
}
