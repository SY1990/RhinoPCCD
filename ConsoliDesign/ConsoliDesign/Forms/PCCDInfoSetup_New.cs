using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rhino;
using Rhino.FileIO;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Commands;
using Rhino.Geometry;
using System.IO;
using ConsoliDesign.Functions;
using ConsoliDesign.Data;
using System.Text.RegularExpressions;

namespace ConsoliDesign.Forms
{
    public partial class PCCDInfoSetup_New : Form
    {
        private RhinoDoc doc;
        private PCCDMainEntrance form {get; set;}
        ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
        System.Collections.Generic.List<Guid> ids = new List<Guid>();
        int flag_numberOfClick = 0;
        int flag_numberofAdd = 0;
        int flag_Explode = 0;
        Product pd = new Product();
        public int currentID;
        public List<RhinoObject> rhObjList = new List<RhinoObject>();
        int OriginalLayerIndex;
       // List<Point3d> BdBoxCenters;
       // Point3d Center;
        List<Vector3d> vectors;

        public PCCDInfoSetup_New(RhinoDoc doc)
        {
            
            InitializeComponent();
            this.doc = RhinoDoc.ActiveDoc;
            OriginalLayerIndex = doc.Layers.CurrentLayerIndex;
            
        }
        
        private void btn_outputPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialgure = new FolderBrowserDialog();
            if (folderDialgure.ShowDialog() == DialogResult.OK)
            {
                txtBox_outputPath.Text = folderDialgure.SelectedPath;
            }
        }

        // get the list of components, auto label them, and show list in tree view
        private void btn_TraverseAssembly_Click(object sender, EventArgs e)
        {
            if (flag_numberOfClick == 0)
            {
                // retrieve the name of the assembly
                string str = doc.DocumentId.ToString();
                CompTreeView.Nodes.Add(str, System.IO.Path.GetFileNameWithoutExtension(PCCDMainEntrance.fileName));

                IEnumerable<RhinoObject> rhObjs = doc.Objects.GetObjectList(settings);
                rhObjList = rhObjs.ToList();
                RhinoObject rhObj;
                int i = 0;
                for (i = 0; i < rhObjList.Count(); i++)
                {
                    //doc.Objects.GetObjectList(settings).ElementAt(0);

                    rhObj = rhObjList.ElementAt(i);
                    ids.Add(rhObj.Id);
                    if (i < rhObjList.Count())
                    {
                        TreeNode node = CompTreeView.Nodes[str].Nodes.Add((i + 1).ToString() + "_" + rhObj.Name);
 
                    }

                }
                //CompTreeView.Nodes[str].Nodes[i - 1].Remove();
                CompTreeView.ExpandAll();
                flag_numberOfClick = 1;
                pd.filepath = PCCDMainEntrance.fileName;
                pd.guid = rhObjList[0].Id;
                pd.Name = System.IO.Path.GetFileNameWithoutExtension(PCCDMainEntrance.fileName);
                pd.parts = new List<Part>(rhObjList.Count());
                for ( i = 0; i < pd.parts.Capacity; i++)
                {
                    pd.parts.Add(default(Part));
                }
                txtBox_outputPath.Focus();
                // auto label each part
               
                for(i=0; i < rhObjList.Count(); i++)
                {
                    rhObj = rhObjList.ElementAt(i);
                    Point3d point = rhObj.Geometry.GetBoundingBox(true).Center;
                    Guid dotId = doc.Objects.AddTextDot((i+1).ToString(), point);
                    
              
                }
                doc.Views.Redraw();
          



                // MessageBox.Show(ids.Count.ToString());
                /* ObjRef obj_ref;
                var rc = RhinoGet.GetOneObject("Select object", false, ObjectType.AnyObject, out obj_ref);
                if (rc != Result.Success)
                {
                    MessageBox.Show("fail to select objects");
                }
            

                var uuid = obj_ref.ObjectId;
                var current_object = obj_ref.Object();  // return the object of obj_ref


                int matIndex = current_object.Attributes.MaterialIndex;
                //Material mat = doc.Materials[matIndex];
                string matName = doc.Materials[matIndex].Name;
                MessageBox.Show(uuid.ToString() + matName);
                //RhinoApp.WriteLine("The object's unique id is {0}", uuid.ToString());

               // MessageBox.Show("it works");
                * */
            }
            else
            {
                MessageBox.Show("You have already pressed this button once!");
            }
        }




        // treeNode action
        private void CompTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            //IEnumerable<RhinoObject> rhObjs = doc.Objects.GetObjectList(settings);
           // List<RhinoObject> rhObjList = rhObjs.ToList();

         
            
            char[] str = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_' };
            try
            {
                textBox1.Text = (e.Node.Index +1).ToString();
                string str1 = e.Node.Text;
                foreach (char chr in str)
                {
                    str1 = str1.TrimStart(chr);
                }
                textBox2.Text = str1;
               // read material of the given object
                int matIndex = rhObjList.ElementAt(e.Node.Index).Attributes.MaterialIndex;
               textBox3.Text = doc.Materials[matIndex].Name;
              // read bounding box size
               BoundingBox boundbox = rhObjList.ElementAt(e.Node.Index).Geometry.GetBoundingBox(true);
               List<double> dimensions = new List<double>();
               dimensions.Add(boundbox.Diagonal.X);
               dimensions.Add(boundbox.Diagonal.Y);
               dimensions.Add(boundbox.Diagonal.Z);
               dimensions.Sort(); // from small to large
               textBox4.Text = String.Format("{0:0.##}", dimensions.ElementAt(0));
               textBox5.Text = String.Format("{0:0.##}", dimensions.ElementAt(1));
               textBox6.Text = String.Format("{0:0.##}", dimensions.ElementAt(2));
               

               Part prt = new Part
               {
                   material = textBox3.Text,
                   Id = (e.Node.Index + 1),
                   Name = str1,
                   BoundingBox = dimensions,
                   guid = rhObjList.ElementAt(e.Node.Index).Id,
                   bdBox = boundbox,
                  // IsStandard = comboBox7.Text,
                 //  MaintenanceFreq = comboBox6.Text

                   //GeoBrep = rhObjList.ElementAt(e.Node.Index).Geometry
               };
  
               pd.parts[e.Node.Index]= prt; // you cannot skip some of the elements, e.g. directly add the fifth part. How to add a member radomly
               
             
               currentID = e.Node.Index;
               RhinoObject rhObj ;
               
               if (checkedListBox1.Items.Count == 0)
               {
                   for (int i = 0; i < rhObjList.Count(); i++)
                   {
                       rhObj = rhObjList.ElementAt(i);
                       if (i < e.Node.Index)
                       {
                           checkedListBox1.Items.Insert(i, (i + 1).ToString() + rhObj.Name);
                       }
                       else if (i > e.Node.Index)
                       {
                           checkedListBox1.Items.Insert(i - 1, (i + 1).ToString() + rhObj.Name);
                       }
                       else
                       { continue; }
                   }
               }
               else
               {
                   checkedListBox1.Items.Clear();
                   for (int i = 0; i < rhObjList.Count(); i++)
                   {
                       rhObj = rhObjList.ElementAt(i);
                       if (i < e.Node.Index)
                       {
                           checkedListBox1.Items.Insert(i, (i + 1).ToString() + rhObj.Name);
                       }
                       else if (i > e.Node.Index)
                       {
                           checkedListBox1.Items.Insert(i - 1, (i + 1).ToString() + rhObj.Name);
                       }
                       else
                       { continue; }
                   }
               }
                // clear the following field
               comboBox1.Text = "";
               comboBox2.Text = "";
               comboBox3.Text = "";
               comboBox4.Text = "";
               comboBox5.Text = "";
               checkedListBox2.Items.Clear();
               label14.Text = textBox1.Text;
               label14.BackColor = Color.YellowGreen;
               comboBox7.Focus();
               comboBox7.Select(0,0);
               listBox1.Items.Clear();
               flag_numberofAdd = 0;
               
            }
            // If the file is not found, handle the exception and inform the user.
            catch (Exception ex)
            {
                throw;
            }
        }

        // trancate a string at a given length
       
        
         
        


        private void exit_Click(object sender, EventArgs e)
        {
            form = new PCCDMainEntrance(doc);
            form.Show();
            this.Close();
            
        }

        // physical info update buttom
        private void button4_Click(object sender, EventArgs e)
        {
            // check if all textbox are not empty && file path is defined
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox3.Text)
                || String.IsNullOrEmpty(textBox4.Text) || String.IsNullOrEmpty(textBox5.Text) || String.IsNullOrEmpty(textBox6.Text)
                || String.IsNullOrEmpty(comboBox6.Text) || String.IsNullOrEmpty(comboBox7.Text) || String.IsNullOrEmpty(txtBox_outputPath.Text))
            {
                MessageBox.Show("please fill the empty text box(es)");
            }
            else
            {
                // write all infomation into an XML file
                try
                {


                    if (pd.parts[currentID].IsStandard == null || pd.parts[currentID].MaintenanceFreq == null)
                    {
                        pd.parts[currentID].IsStandard = comboBox7.Text;
                        pd.parts[currentID].MaintenanceFreq = comboBox6.Text;
                        string filePath = @txtBox_outputPath.Text + "\\"+ pd.Name +".xml";
                        XmlHelper.ToXmlFile(pd, filePath);
                        MessageBox.Show(String.Format("Physical info is properly saved for component {0}", textBox1.Text));
                    }
                    else if (pd.parts[currentID].IsStandard != comboBox7.Text || pd.parts[currentID].MaintenanceFreq != comboBox6.Text)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult diaResult = MessageBox.Show(this, "do you want to override the data?", "override reminder", buttons);
                        if (diaResult == DialogResult.Yes)
                        {
                            pd.parts[currentID].IsStandard = comboBox7.Text;
                            pd.parts[currentID].MaintenanceFreq = comboBox6.Text;
                            string filePath = @txtBox_outputPath.Text + "\\" + pd.Name + ".xml";
                            XmlHelper.ToXmlFile(pd, filePath);
                            MessageBox.Show(String.Format("Physical info is override for component {0}", textBox1.Text));
                        }

                    }
                    comboBox4.Focus();
   
                }
                catch (IOException exception)
                {
                    exception.ToString();
                }
            }

        }

        // add joint components
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++ )
            {
                checkedListBox2.Items.Insert(i,checkedListBox1.CheckedItems[i]);
            }
            

        }

        // remove joint components
        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.CheckedItems.Count; i++)
            {
                checkedListBox2.Items.Remove(checkedListBox2.CheckedItems[i]);
            }
        }

        // update function information
        private void button5_Click(object sender, EventArgs e)
        {
            pd.parts[currentID].Function = comboBox4.SelectedItem.ToString() + "_"+ comboBox5.SelectedItem.ToString();
            pd.parts[currentID].JointComponent = new List<int>(checkedListBox2.Items.Count);
            pd.parts[currentID].Weight = new List<double>(checkedListBox2.Items.Count);
            pd.parts[currentID].RelativeMotion = new List<int>(checkedListBox2.Items.Count);
            // clear combobox1
            comboBox1.Items.Clear();
            int xx = 0;
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                xx = Int32.Parse(Regex.Match(checkedListBox2.Items[i].ToString(), @"\d+").Value); 
                
                pd.parts[currentID].JointComponent.Add(xx);

                comboBox1.Items.Insert(i, checkedListBox2.Items[i].ToString());
            }
            string filePath = @txtBox_outputPath.Text + "\\" + pd.Name + ".xml";
            XmlHelper.ToXmlFile(pd, filePath);
            MessageBox.Show(String.Format("Function info is updated for component {0}", textBox1.Text));



        }
        

        // specify FPI information
        private void button7_Click(object sender, EventArgs e)
        {
            string x1=null, x2=null, x4=null;
            double x3 = 0;
            x1 = comboBox2.Text;
            x2 = comboBox3.Text;
            x4 = comboBox8.Text;
            int x5=0;
           
            if (comboBox1.Text != "" && comboBox2.Text != "" && comboBox3.Text != "")
            {
                if (flag_numberofAdd <= checkedListBox2.Items.Count-1)
                {
                    
                    
                    if (x1=="Large force" && x2=="Firm")
                    {
                        x3=1.0;
                       
                    }
                    else if (x1=="Large force" && x2=="Medium")
                    {
                        x3=0.8;
                    }
                    else if (x1=="Medium force" && x2=="Firm")
                    {
                        x3=0.6;
                    }
                    else if ((x1=="Medium force"||x1=="Energy") && x2=="Medium" )
                    {
                        x3=0.5;
                    }
                    else if (x1=="Low force" && x2=="Loose")
                    {
                        x3=0.4;
                    }
                    else if ((x1=="Signal"|| x1=="Material") && x2=="No")
                    {
                        x3=0.2;
                    }
                    else
                    {
                        MessageBox.Show("impossible combinations!");

                    }
                    if (x4 == "Y")
                    {
                        x5 = 1;
                        pd.parts[currentID].RelativeMotion.Add(x5);

                    }
                    else
                    {
                        x5 = 0;
                        pd.parts[currentID].RelativeMotion.Add(x5);
                    }
                    if (x3 != 0)
                    {
                        listBox1.Items.Insert(flag_numberofAdd, (currentID + 1).ToString() + "-" + Regex.Match(comboBox1.Text, @"\d+").Value + ":" + " ( " + comboBox2.Text + "," + comboBox3.Text + "," + comboBox8.Text + ") " );
                        flag_numberofAdd++;
                        comboBox1.Text = "";
                        comboBox2.Text = "";
                        comboBox3.Text = "";
                        comboBox8.Text = "";
                        
                        pd.parts[currentID].Weight.Add(x3);
                    }
                }
                else
                {
                    flag_numberofAdd = 0;
                    MessageBox.Show("You have clicked too many times");
                    comboBox1.Text = "";
                    comboBox2.Text = "";
                    comboBox3.Text = "";
                }
                //

            }
        }

        // update functional relation information
        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == comboBox1.Items.Count)
            {
                pd.parts[currentID].ConnectionInfo = new List<string>(listBox1.Items.Count);
                
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                  pd.parts[currentID].ConnectionInfo.Add(listBox1.Items[i].ToString());

                }

                string filePath = @txtBox_outputPath.Text + "\\" + pd.Name + ".xml";
                XmlHelper.ToXmlFile(pd, filePath);
                MessageBox.Show(String.Format("Function relation info is updated for component {0}", textBox1.Text));
            }
            else
            {
                MessageBox.Show("You have missed at least one link!");
            }

        }

        // exploded view
        private void button1_Click(object sender, EventArgs e)
        {
            Rhino.DocObjects.Layer New_Layer = new Layer();
            Layer Old_Layer = new Layer();
            New_Layer.Name = "ExplodedView";
            Old_Layer = doc.Layers.CurrentLayer;
            List<RhinoObject> NewRhObjs = new List<RhinoObject>() ;

           // vectors = new List<Vector3d>();
         
            
            if (flag_Explode == 0)
            {
                doc.Layers.Add(New_Layer);
                New_Layer.LayerIndex = doc.Layers.FindByFullPath(New_Layer.Name, true);
                doc.Layers.SetCurrentLayerIndex(New_Layer.LayerIndex, true);
                New_Layer.IsVisible = true;
                Old_Layer.IsVisible = false;
                if (rhObjList.Count != 0)
                {
                   /* foreach (RhinoObject obj in rhObjList)
                    {

                        BdBoxCenters.Add(obj.Geometry.GetBoundingBox(true).Center);
                        var Duplicates = doc.Objects.Transform(obj.Id, Transform.Identity, false);

                        if (Duplicates != null)
                        {
                            obj.Attributes.LayerIndex = New_Layer.LayerIndex;
                            obj.CommitChanges();
                        }
                        _bdbox = BoundingBox.Union(obj.Geometry.GetBoundingBox(true), _bdbox);
                    }
                    // generate all vectors
                    VectorsGenerator(rhObjList, BdBoxCenters, _bdbox.Center, out vectors);
                    for (int i = 0; i < rhObjList.Count; i++)
                    {
                        if (vectors[i].Length > 0.001)
                        {
                            var xf = Transform.Translation(-vectors[i] * 1.25);
                            Guid guid = doc.Objects.Transform(rhObjList[i].Id, xf, true);
                            NewRhObjs.Add(guid);
                        }
                        else
                        {
                            NewRhObjs.Add(rhObjList[i].Id);
                        }
                    }*/
                    ExplodeView(doc, Old_Layer, New_Layer, out  NewRhObjs);

                    Old_Layer.CommitChanges();
                    New_Layer.CommitChanges();
                    flag_Explode = 1;
                    // copy all rhino objects to new layers
                    for (int i = 0; i < NewRhObjs.Count(); i++)
                    {
                        RhinoObject rhObj = NewRhObjs[i];
                        Point3d point = rhObj.Geometry.GetBoundingBox(true).Center;
                        Guid dotId = doc.Objects.AddTextDot((i + 1).ToString(), point);


                    }
                    doc.Views.Redraw();
                }
                else
                { MessageBox.Show("Please press Traverse Assembly button first!");
                  
                }
                
                
            }
            else if (flag_Explode == 2)
            {
                New_Layer.LayerIndex = doc.Layers.FindByFullPath(New_Layer.Name, true);
                doc.Layers.SetCurrentLayerIndex(New_Layer.LayerIndex, true);
                New_Layer.IsVisible = true;
                Old_Layer.IsVisible = false;
                Old_Layer.CommitChanges();
                New_Layer.CommitChanges();
                flag_Explode = 1;
                doc.Views.Redraw();
            }
            else
            { }

        }
        // collapse view
        private void button9_Click(object sender, EventArgs e)
        {
            Layer New_Layer = new Layer();
            Layer Old_Layer = new Layer();
            New_Layer.Name = "ExplodedView";

            if (flag_Explode == 1)
            {
                New_Layer = doc.Layers.CurrentLayer;
                doc.Layers.SetCurrentLayerIndex(OriginalLayerIndex,true);
                Old_Layer = doc.Layers.CurrentLayer;
                Old_Layer.IsVisible = true;
                New_Layer.IsVisible = false;
                Old_Layer.CommitChanges();
                New_Layer.CommitChanges();
               // New_Layer.LayerIndex = doc.Layers.FindByFullPath(New_Layer.Name, true);
                
                flag_Explode = 2;
                doc.Views.Redraw();
            }
            else
            {
                MessageBox.Show("No explode action is applied! Please press explode first");
            }

        }

        // genegrate transform vectors
        static void VectorsGenerator(List<RhinoObject> ObjectList, List<Point3d> points, Point3d center, out List<Vector3d> vectors_Center )
        {
            int NumOfParts = ObjectList.Count;
            vectors_Center = new List<Vector3d>();
            Vector3d Temp = new Vector3d();
            List<List<int>> Nodes = new List<List<int>>();
            List<int> sublist = new List<int>();
            List<List<Vector3d>> vectors_Mutual = new List<List<Vector3d>>();
            List<Vector3d> VM = new List<Vector3d>();
            List<int> MarkNodes = new List<int>();
            // find out whether some parts are too closed
            for (int i = 0; i < NumOfParts; i++)
            {
                for (int j = i + 1; j < NumOfParts; j++)
                {
                    Temp = points[i] - points[j];
                    if (Temp.Length < 5)
                    {
                        sublist.Add(j);
                        VM.Add(Temp);
                    }
                }
                Nodes.Add(sublist.ToList());
                vectors_Mutual.Add(VM.ToList());
                VM.Clear();
                sublist.Clear();
                
            }

                for (int i = 0; i < NumOfParts; i++)
                {
                    Vector3d vector = center - points[i];
                    vectors_Center.Add(vector);
                    MarkNodes.Add(0);
                }
                for (int i = 0; i < Nodes.Count; i++)
                {
                   
                    if (Nodes[i].Count != 0)
                    {
                        for (int j = 0; j < Nodes[i].Count; j++)
                        {
                            if (MarkNodes[Nodes[i].ElementAt(j)] != 1)
                            {
                                Vector3d Vector2 = new Vector3d();
                                Vector2 = vectors_Mutual[i].ElementAt(j) * (-20);
                                Vector2 += vectors_Center[i];
                                vectors_Center[Nodes[i].ElementAt(j)] = Vector2;
                                MarkNodes[Nodes[i].ElementAt(j)] = 1;
                            }
                        }
                    }
                }
        }

       public static void ExplodeView(RhinoDoc doc, Layer OldLayer, Layer NewLayer, out List<RhinoObject> NewRhObjs)
        {
            List<Vector3d> vectors = new List<Vector3d>();
            List<Point3d> BdBoxCenters = new List<Point3d>();
           BoundingBox _bdbox = new BoundingBox();
            NewRhObjs = new List<RhinoObject> ();
           List<RhinoObject> rhObjList;

           CopyObjects(doc, OldLayer, NewLayer, out rhObjList);
            foreach (RhinoObject obj in rhObjList)
            {
                BdBoxCenters.Add(obj.Geometry.GetBoundingBox(true).Center);
                _bdbox = BoundingBox.Union(obj.Geometry.GetBoundingBox(true), _bdbox);
            }

            // generate all vectors
            VectorsGenerator(rhObjList, BdBoxCenters, _bdbox.Center, out vectors);
            for (int i = 0; i < rhObjList.Count; i++)
            {
                if (vectors[i].Length > 0.001)
                {
                    var xf = Transform.Translation(-vectors[i] * 1.25);
                    Guid guid = doc.Objects.Transform(rhObjList[i].Id, xf, true);
                    NewRhObjs.Add(doc.Objects.Find(guid));
                    
                }
                else
                {
                    NewRhObjs.Add(rhObjList[i]);
                }
            }
        }
       public static void CopyObjects(RhinoDoc doc, Layer OldLayer, Layer NewLayer, out List<RhinoObject> rhObjList)
       {
           // copy objects from old layer to new layer
           ObjectType type = ObjectType.Brep;
           rhObjList = new List<RhinoObject>();
           List<RhinoObject>  rhObjList0 = doc.Objects.FindByLayer(OldLayer).ToList();
          // rhObjList= doc.Objects.FindByObjectType(type).ToList();
          // rhObjList = doc.Objects.GetObjectList(type).ToList();

           foreach (RhinoObject obj in rhObjList0)
           {
               if (type == obj.ObjectType)
               {
                   Guid Duplicates = doc.Objects.Transform(obj.Id, Transform.Identity, false);
                   RhinoObject rhobj = doc.Objects.Find(Duplicates);
                   if (Duplicates != null)
                   {
                       rhobj.Attributes.LayerIndex = NewLayer.LayerIndex;
                       rhobj.CommitChanges();
                   }
                   rhObjList.Add(rhobj);
               }

           }
       }


       // sort which objects are in line, plane or not
     /*   static void ObjectSort(List<RhinoObject> ObjectList, List<Point3d> points, out List<List<RhinoObject>> objInLine,out List<List<RhinoObject>> objInPlane, out List<List<RhinoObject>> objNotInPlane)
        {
            int NumOfParts = ObjectList.Count;
            objInLine = new List<List<RhinoObject>> ();
            objInPlane = new List<List<RhinoObject>>();
            objNotInPlane = new List<List<RhinoObject>>();
            List<List<Vector3d>> Vectors = new List<List<Vector3d>>();
            List<Vector3d> subVectors = new List<Vector3d>();
            int flag = 0;
            for (int i = 0; i < NumOfParts-1; i++)
            {
                for (int j = i+1; j < NumOfParts ; j++)
                {
                    // count how many vectors can have from each point
                    Vector3d vector = points[i] - points[j];
                    foreach (Vector3d vct in subVectors)
                    {
                        if (vector.Unitize().CompareTo(vct.Unitize()) == 0)
                        {
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                       subVectors.Add(vector);
                    }
                }
                Vectors.Add(subVectors.ToList());
                subVectors.Clear();
                // compare 
            }

            // decide which vectors should be keep to minimize the total number of vectors
            for (int i = 0; i < Vectors.Count; i++)
            {
                for (int j = 0; j < Vectors[i].Count; j++)
                {

                }
            }


            // line minimum distance methods
        }
        */
      


  
       
    
     
    }
}
