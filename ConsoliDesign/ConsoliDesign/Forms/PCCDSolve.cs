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
using System.IO;
using ConsoliDesign.Functions;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ConsoliDesign.Forms
{
    public partial class PCCDSolve : Form
    {
        private RhinoDoc doc;
        Product currProduct;
        private PCCDPrinter form { get; set; }
        private PCCDMainEntrance form2 { get; set; }
        ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings();
        List<double> vlm;
        List<string> Materials;
        double[,] WeightMatrix ;
        List<ComponentStruct> VerticeMatrix;
        int[,] RelativeMotionMatrix ;
        int[] CFTypeCode = new int[] { 1, 2, 3, 4, 5, 6, 7 };
        double[] SizeLimit = new double[3];
        int[] MaterialList;
        double[,] SymmWeightMatrix;
        int NumOfPopulation ;
        int NumOfGeneration ;
        double CrossOverRatio = 0.4;
        double MutationRatio = 0.05;
        Printers pnts = new Printers();
        int Flag = 0;
        double Alfa;
        List<List<int>> Modules = new List<List<int>>();
        double[,] WMNode;
        List<List<int>> TroubleNodeList = new List<List<int>>();
        List<int> TroubleCode ;
        public static List<List<int>> NodeGroups = new List<List<int>>();
        List<double[,]> SubGraphs = new List<double[,]>();
        List<List<ComponentStruct>> SubVerticeMs = new List<List<ComponentStruct>>();
        List<int[,]> SubRelativeMs = new List<int[,]>();
        List<List<List<int>>> Communities;
        List<double> Modularity;
        public static List<RhinoObject> rhObjList;
        int FlagColor;
        Layer Original_Layer = new Layer();
        PCCDResult result = new PCCDResult();

        public PCCDSolve(RhinoDoc doc )
        {
            InitializeComponent();
            this.doc = RhinoDoc.ActiveDoc;
            
           
        }

        private void PCCDSolve_Activate(object sender, System.EventArgs e)
        {
            currProduct = PCCDMainEntrance.result;
            string xx = Directory.GetCurrentDirectory();
            var currPath = Path.Combine(Environment.CurrentDirectory, "\\PrinterInfo.xml");
            pnts = XmlHelper.FromXmlFile<Printers>(currPath);
            if (pnts.printer.Count != 0)
            {
                for (int i = 0; i < pnts.printer.Count; i++)
                {
                    comboBox1.Items.Add(pnts.printer[i].Name);
                }
            }   
            else
            {
                MessageBox.Show("None printer information exists! Please add printers.");
            }
            comboBox4.Items.Clear();
            // comboBox4 & 5
            for (int i = 0; i < currProduct.parts.Count; i++)
            {
                comboBox4.Items.Add(i+1);
            }
           // IEnumerable<RhinoObject> rhObjs = doc.Objects.GetObjectList(settings);
           // rhObjList = rhObjs.ToList();
            //Original_Layer.LayerIndex = doc.Layers.FindByFullPath("Default", true);
            Original_Layer = doc.Layers.CurrentLayer;
            txtBox_outputPath.Focus();
        }

        private void comboBox4_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            comboBox5.Items.Clear();
            comboBox5.Text = "";
            Part part = currProduct.parts[comboBox4.SelectedIndex];
            List<int> JointComponents = part.JointComponent.ToList();
            foreach (int x in JointComponents)
            {
                comboBox5.Items.Add(x);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox5.Items.Clear();
            int i = comboBox1.SelectedIndex;
            Materials = new List<string>();
            for (int j = 0; j < pnts.printer[i].Materials.Count; j++)
               {
                   listBox1.Items.Add(pnts.printer[i].Materials[j]);
                   Materials.Add(pnts.printer[i].Materials[j]);
               }

            vlm = new List<double>();
            vlm = pnts.printer[i].Volume;
            vlm.Sort();
            listBox2.Items.Add("L" + vlm[2].ToString());
            listBox2.Items.Add("W" + vlm[1].ToString());
            listBox2.Items.Add("H" + vlm[0].ToString());
            listBox5.Items.Add("Type:" + pnts.printer[i].BasicInfo.AMType);
            listBox5.Items.Add("Min Feature:" + pnts.printer[i].BasicInfo.MinFeatureSize.ToString()+" um");
            listBox5.Items.Add("Min Layer:" + pnts.printer[i].BasicInfo.MinLayerThickness.ToString()+" um");
        }

        // add new printers
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (null == form)
            {
                form = new PCCDPrinter();
                form.FormClosed += OnFormClosed;
                
                form.Show(RhinoApp.MainWindow());
                //Form.Show();
            }

        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            form.Dispose();
            form = null;

            // update the printer information instantly
        }

        private void btn_outputPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialgure = new FolderBrowserDialog();
            if (folderDialgure.ShowDialog() == DialogResult.OK)
            {
                txtBox_outputPath.Text = folderDialgure.SelectedPath;
            }
        }

       
        // calculate button
        private void button3_Click(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            richTextBox1.Text = "";
            string str = comboBox2.SelectedItem.ToString();
            int NumOfNode = currProduct.parts.Count;
            WeightMatrix = new double[ NumOfNode,NumOfNode];
            VerticeMatrix = new List<ComponentStruct>();
            
            RelativeMotionMatrix = new int [NumOfNode,NumOfNode ];
 
            NumOfPopulation = (int)(NumOfNode/2) +3;
            NumOfGeneration = 10*NumOfNode;
            double[] BestFitness = new double[NumOfGeneration];
            Alfa =Convert.ToDouble(textBox1.Text);
            Modules = new List<List<int>>();
            FlagColor = 0;
            
            // assemble xml info into a weight matrix
            for (int i = 0; i < NumOfNode; i++)
            {
                Part part = currProduct.parts[i];
                WeightMatrix[i,i]=1;
                if (part.JointComponent.Count!=0)
                {
                    for (int j=0; j< part.JointComponent.Count; j++)
                    {
                        WeightMatrix[i,part.JointComponent[j]-1] = part.Weight[j];
                        // Get relative motion matrix
                        RelativeMotionMatrix[i, part.JointComponent[j] - 1] = part.RelativeMotion[j];
                    }
                }    
             }
            // retrieve vertice matrix
            for (int i = 0; i < NumOfNode; i++)
            {
                Part part = currProduct.parts[i];
                ComponentStruct CMPS = new ComponentStruct();
                switch (part.material)
                {
                    case "Steel":
                         CMPS.material =1;break;
                    case "Plastic":
                         CMPS.material =2;break;
                    case "Rubber":
                         CMPS.material =3;break;
                    case "Aluminium":
                         CMPS.material =4;break;
                    case "Ceramic":
                         CMPS.material =5;break;     
                }
                CMPS.bdBox = part.bdBox;
                switch (part.IsStandard)
                {
                    case "Y":
                       CMPS.IsStandard =1;break;
                    case "N":
                       CMPS.IsStandard = 0; break;
                }
                switch (part.MaintenanceFreq)
                {
                    case "High":
                        CMPS.MaintenanceFreq = 1; break;
                    case "Low":
                        CMPS.MaintenanceFreq = 0; break;
                }
                VerticeMatrix.Add(CMPS);
            }
                // get size limit
            SizeLimit[0] = vlm[0];
            SizeLimit[1] = vlm[1];
            SizeLimit[2] = vlm[2];
            // get material list
            MaterialList = new int[Materials.Count];
            for (int i = 0; i < Materials.Count; i++)
            {
                switch (Materials[i])
                {
                    case "Steel":
                        MaterialList[i] = 1; break;
                    case "Plastic":
                        MaterialList[i] = 2;; break;
                    case "Rubber":
                        MaterialList[i] = 3;; break;
                    case "Aluminum":
                        MaterialList[i] = 4;; break;
                    case "Ceramics":
                        MaterialList[i] = 5;; break; 
                }
            }
            GAModuleDiv.TransferMatrixToSym(WeightMatrix, out SymmWeightMatrix);

             if (str == "Product")
             {
                 // module division
                 GAModuleDiv.FindBestSolution_Multiple(SymmWeightMatrix, NumOfPopulation, NumOfGeneration, CrossOverRatio, MutationRatio, out Modules, out BestFitness);
                 CPCCD.FindCommunity(Alfa, Modules, SymmWeightMatrix, VerticeMatrix, RelativeMotionMatrix, out Communities, out Modularity, out SubGraphs, out SubVerticeMs, out SubRelativeMs);
                 CPCCD.CPCCDAllRules_R3(Alfa, Modules, Communities, Modularity, SubGraphs, SubVerticeMs, SubRelativeMs, SizeLimit, CFTypeCode, MaterialList, out WMNode, out TroubleNodeList, out TroubleCode, out NodeGroups);
                 richTextBox1.AppendText("\nModule Info:\n");

                 foreach (List<int> x in Modules)
                 {
                     x.Sort();
                     foreach (int xx in x)
                     {
                         richTextBox1.AppendText(" ");
                         richTextBox1.AppendText(string.Join(" ", xx.ToString()));
                     }
                     // Console.WriteLine("\n");
                     richTextBox1.AppendText("\n");
                 }
                 richTextBox1.AppendText(string.Format("{0:0.000}", BestFitness.Max()) +"\n");
               /*foreach (List<List<int>> X0 in Communities)
                 {
                     foreach (List<int> x in X0)
                     {
                         foreach (int xx in x)
                         {
                             richTextBox1.AppendText(" ");
                             richTextBox1.AppendText(string.Join(" ", xx.ToString()));
                         }
                         richTextBox1.AppendText("\n");
                     }
                     richTextBox1.AppendText("\n\n");
                 }*/
                /* List<List<double>> StackUpSize;
                 // List<int> Module = new List<int>();
                 // MPCCD.MPCCDGenerateModule(SymmWeightMatrix.GetLength(0), out Module);

                 NPCCD.PCCDStackUpSize(NodeGroups, VerticeMatrix, out  StackUpSize);
                 foreach (List<double> x in StackUpSize)
                 {
                     foreach (double xx in x)
                     {
                         richTextBox1.AppendText(" ");
                         richTextBox1.AppendText(string.Join(" ", string.Format("{0:0.0}", xx)));
                     }
                     richTextBox1.AppendText("\n");
                 }
                 */
                 Flag = 1;
                 textBox2.Text = "CPCCD";
             }
             else
             {
                 
                 // module division
                 GAModuleDiv.FindBestSolution_Multiple(SymmWeightMatrix, NumOfPopulation, NumOfGeneration, CrossOverRatio, MutationRatio, out Modules, out BestFitness);
                 if (BestFitness.Max() > Alfa)
                 {
                     Flag = 2;
                     MPCCD.MPCCDAllRules_R3(Modules, SymmWeightMatrix, VerticeMatrix, RelativeMotionMatrix, SizeLimit, CFTypeCode, MaterialList, out WMNode, out TroubleNodeList, out TroubleCode, out NodeGroups);
                    // List<List<double>> StackUpSize;
                    // List<int> Module = new List<int>();
                    // MPCCD.MPCCDGenerateModule(SymmWeightMatrix.GetLength(0), out Module);
                    
                    /* NPCCD.PCCDStackUpSize(NodeGroups, VerticeMatrix, out  StackUpSize);
                     foreach (List<double> x in StackUpSize)
                     {
                         foreach (double xx in x)
                         {
                             richTextBox1.AppendText(" ");
                             richTextBox1.AppendText(string.Join(" ", string.Format("{0:0.0}", xx)));
                         }
                         richTextBox1.AppendText("\n");
                     }*/
                     richTextBox1.AppendText("\nModule Info:\n");
                     foreach (List<int> x in Modules)
                     {
                         x.Sort();
                         foreach (int xx in x)
                         {
                             richTextBox1.AppendText(" ");
                             richTextBox1.AppendText(string.Join(" ", xx.ToString()));
                         }
                         // Console.WriteLine("\n");
                         richTextBox1.AppendText("\n");
                     }
                     richTextBox1.AppendText(string.Format("{0:0.000}", BestFitness.Max()));
                     textBox2.Text = "CPCCD";
                 }
                 else
                 {
                     Flag = 3;
                    // List<int> Module = new List<int>();
                   //  MPCCD.MPCCDGenerateModule(SymmWeightMatrix.GetLength(0), out Module);
                     NPCCD.NPCCDAllRules_R3(SymmWeightMatrix, VerticeMatrix, SizeLimit, MaterialList, RelativeMotionMatrix, CFTypeCode, out WMNode, out TroubleNodeList, out TroubleCode, out NodeGroups);
                    /* List<List<double>> StackUpSize;
                     NPCCD.PCCDStackUpSize(NodeGroups, VerticeMatrix, out  StackUpSize);
                     
                     foreach (List<double> x in StackUpSize)
                     {
                         foreach (double xx in x)
                         {
                             richTextBox1.AppendText(" ");
                             richTextBox1.AppendText(string.Join(" ", string.Format("{0:0.0}", xx)));
                         }
                         richTextBox1.AppendText("\n");
                     }*/
                     textBox2.Text = "NPCCD";
                 } 
             }
             richTextBox1.AppendText("\nNode Groups:\n");
             foreach (List<int> x in NodeGroups)
             {
                 x.Sort();
                 foreach (int xx in x)
                 {
                     richTextBox1.AppendText(" ");
                     richTextBox1.AppendText(string.Join(" ", xx.ToString()));
                 }
                 richTextBox1.AppendText("\n");
             }
             richTextBox2.Clear();
             richTextBox2.AppendText("\nConflict Info:\n");
             int Index = 0;
             foreach (List<int> x in TroubleNodeList)
             {
                 foreach (int xx in x)
                 {
                     richTextBox2.AppendText(" ");
                     richTextBox2.AppendText(string.Join(" ", xx.ToString()));
                 }
                 richTextBox2.AppendText("\t");
                 richTextBox2.AppendText(TroubleCode[Index].ToString());
                 Index++;
                 richTextBox2.AppendText("\n");
             }
            
             watch.Stop();
             var elapsedMs = watch.ElapsedMilliseconds;
             label14.Show();
             label14.Text = elapsedMs.ToString();
        }

        // add links to be cut
        private void button5_Click(object sender, EventArgs e)
        {
            string str = comboBox4.Text + "-" + comboBox5.Text;
            listBox3.Items.Add(str);
        }

        // delete links to be cut
        private void button6_Click(object sender, EventArgs e)
        {
            listBox3.Items.Remove(listBox3.SelectedItem);
        }

        // button recalculate
        private void button7_Click(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<List<int>> LinkToCut = new List<List<int>>();
            List<int> sublist = new List<int>();
            string str1, str2, str3;
            char [] chr ;
            FlagColor = 0;
            for (int i = 0; i < listBox3.Items.Count; i++)
            {
                str1 = listBox3.Items[i].ToString().Substring(0, listBox3.Items[i].ToString().IndexOf("-")).Replace(" ", ""); ;
                str2= Regex.Replace(listBox3.Items[i].ToString(), "-", string.Empty);
                chr =str1.ToCharArray();
                str3= str2.Remove(0, chr.Length);
                sublist.Add(Convert.ToInt32(str1));
                sublist.Add(Convert.ToInt32(str3));
                LinkToCut.Add(sublist.ToList());
                sublist.Clear();
            }
           GAModuleDiv.TransferMatrixToSym(WeightMatrix, out SymmWeightMatrix);
           if (Flag == 1)
           {

               CPCCD.CPCCDAllRules(LinkToCut,Alfa, Modules, Communities, Modularity, SubGraphs, SubVerticeMs, SubRelativeMs, SizeLimit, CFTypeCode, MaterialList, out WMNode, out TroubleNodeList, out TroubleCode, out NodeGroups);
           }
           else if (Flag == 2)
           {
               MPCCD.MPCCDAllRules(LinkToCut,Modules, SymmWeightMatrix, VerticeMatrix, RelativeMotionMatrix, SizeLimit, CFTypeCode, MaterialList, out WMNode, out TroubleNodeList, out TroubleCode, out NodeGroups);

           }
           else
           {
              // List<int> Module = new List<int>();
               //MPCCD.MPCCDGenerateModule(SymmWeightMatrix.GetLength(0), out Module);
               NPCCD.NPCCDAllRules(LinkToCut,SymmWeightMatrix, VerticeMatrix, SizeLimit, MaterialList, RelativeMotionMatrix, CFTypeCode, out WMNode, out TroubleNodeList, out TroubleCode, out NodeGroups);
           }
           richTextBox1.Clear();
           richTextBox1.AppendText("Node Groups:\n");
           foreach (List<int> x in NodeGroups)
           {
               x.Sort();
               foreach (int xx in x)
               {
                   richTextBox1.AppendText(" ");
                   richTextBox1.AppendText(string.Join(" ", xx.ToString()));
               }
               richTextBox1.AppendText("\n");
           }
           richTextBox2.Clear();
           richTextBox2.AppendText("\nConflict Info:\n");
           int Index = 0;
           foreach (List<int> x in TroubleNodeList)
           {
               foreach (int xx in x)
               {
                   richTextBox2.AppendText(" ");
                   richTextBox2.AppendText(string.Join(" ", xx.ToString()));
               }
               richTextBox2.AppendText("\t");
               richTextBox2.AppendText(TroubleCode[Index].ToString());
               Index++;
               richTextBox2.AppendText("\n");
           }
           watch.Stop();
           var elapsedMs = watch.ElapsedMilliseconds;
           label14.Show();
           label14.Text = elapsedMs.ToString();
        }

        // EXIT Button
        private void button1_Click(object sender, EventArgs e)
        {
            
                 form2 = new PCCDMainEntrance(doc);
                //form2.FormClosed += OnFormClosed2;
                this.Close();

                 form2.Show(RhinoApp.MainWindow());
                //Form.Show();
            
        }

        private void OnFormClosed2(object sender, FormClosedEventArgs e)
        {
            form2.Dispose();
            form2 = null;

            // update the printer information instantly
        }

        // Display results without considering R3
        private void button4_Click(object sender, EventArgs e)
        {
           
           int Mode;
           List<Color> colorSet = new List<Color>();
           Layer New_Layer, New_Layer2;
           //List<RhinoObject> rhObjList;
           Layer Old_Layer= Original_Layer;
            if (Flag==1|| Flag == 2)
            {
                Mode =0; // CPCCD or MPCCD
            }
            else
            {
                Mode = 2; //NPCCD
            }

            if (FlagColor == 0)
            {
                if (Mode == 0)
                {
                    //CPCCD or MPCCD
                    doc.Layers.SetCurrentLayerIndex(Old_Layer.LayerIndex, true);
                    Old_Layer.IsVisible = true;
                    Old_Layer.CommitChanges();
                    if (doc.Layers.Count > 1)
                    {
                        // delete all layers except "Default"
                        foreach (Layer layer in doc.Layers)
                        {
                            if (layer.LayerIndex != Old_Layer.LayerIndex)
                            {
                                doc.Layers.Delete(layer.LayerIndex, false);
                                layer.CommitChanges();
                            }
                        }
                    }
                    //MessageBox.Show(doc.Layers.Count.ToString());
                    doc.Views.Redraw();
                    DisplayNodeGroups(doc, Old_Layer, Modules, Mode, out rhObjList, out colorSet, out New_Layer);
                    Mode = 1;
                   // Old_Layer = Original_Layer;
                    New_Layer = doc.Layers.CurrentLayer;
                    LabelNode(doc, rhObjList);
                    DisplayNodeGroups(doc, Old_Layer, NodeGroups, Mode, out rhObjList, out colorSet, out New_Layer2);
                    New_Layer.IsVisible = false;
                   // Old_Layer.IsVisible = false;
                   // Old_Layer.CommitChanges();
                    New_Layer.CommitChanges();
                    LabelNode(doc, rhObjList);
                }
             
                else 
                {
                    //NPCCD
                    doc.Layers.SetCurrentLayerIndex(Old_Layer.LayerIndex, true);
                    Old_Layer.IsVisible = true;
                    Old_Layer.CommitChanges();
                    if (doc.Layers.Count > 1)
                    {
                        // delete all layers except "Default"
                        foreach (Layer layer in doc.Layers)
                        {
                            if (layer.LayerIndex!=Old_Layer.LayerIndex)
                            {
                                doc.Layers.Delete(layer.LayerIndex,false);
                                layer.CommitChanges();
                            }
                        }
                    }
                    //MessageBox.Show(doc.Layers.Count.ToString());
                    doc.Views.Redraw();
                    DisplayNodeGroups(doc, Old_Layer, NodeGroups, Mode, out rhObjList, out colorSet, out New_Layer);
                    doc.Layers.SetCurrentLayerIndex(New_Layer.LayerIndex, true);
                    New_Layer.CommitChanges();
                    Old_Layer.IsVisible = false;
                    Old_Layer.CommitChanges();
                    LabelNode(doc, rhObjList);
                }
   
                doc.Views.Redraw();
                FlagColor = 1;
            }
        }

        // Display recalculate results
        private void button9_Click(object sender, EventArgs e)
        {
            int Mode;
            List<Color> colorSet = new List<Color>();
            Layer New_Layer, New_Layer2;
            //List<RhinoObject> rhObjList, rhObjList2;
            Layer Old_Layer = Original_Layer;
            if (Flag == 1 || Flag == 2)
            {
                Mode = 0; // CPCCD or MPCCD
            }
            else
            {
                Mode = 2; //NPCCD
            }

            if (FlagColor == 0)
            {
                if (Mode == 0)
                {
                    //CPCCD or MPCCD
                    doc.Layers.SetCurrentLayerIndex(Old_Layer.LayerIndex, true);
                    Old_Layer.IsVisible = true;
                    Old_Layer.CommitChanges();
                    if (doc.Layers.Count > 1)
                    {
                        // delete all layers except "Default"
                        foreach (Layer layer in doc.Layers)
                        {
                            if (layer.LayerIndex != Old_Layer.LayerIndex)
                            {
                                doc.Layers.Delete(layer.LayerIndex, false);
                                layer.CommitChanges();
                            }
                        }
                    }
                    //MessageBox.Show(doc.Layers.Count.ToString());
                    doc.Views.Redraw();
                    DisplayNodeGroups(doc, Old_Layer, Modules, Mode, out rhObjList, out colorSet, out New_Layer);
                    Mode = 1;
                    // Old_Layer = Original_Layer;
                    New_Layer = doc.Layers.CurrentLayer;
                    LabelNode(doc, rhObjList);
                    DisplayNodeGroups(doc, Old_Layer, NodeGroups, Mode, out rhObjList, out colorSet, out New_Layer2);
                    New_Layer.IsVisible = false;
                    // Old_Layer.IsVisible = false;
                    // Old_Layer.CommitChanges();
                    New_Layer.CommitChanges();
                    LabelNode(doc, rhObjList);
                }

                else
                {
                    //NPCCD
                    doc.Layers.SetCurrentLayerIndex(Old_Layer.LayerIndex, true);
                    Old_Layer.IsVisible = true;
                    Old_Layer.CommitChanges();
                    if (doc.Layers.Count > 1)
                    {
                        // delete all layers except "Default"
                        foreach (Layer layer in doc.Layers)
                        {
                            if (layer.LayerIndex != Old_Layer.LayerIndex)
                            {
                                doc.Layers.Delete(layer.LayerIndex, false);
                                layer.CommitChanges();
                            }
                        }
                    }
                    //MessageBox.Show(doc.Layers.Count.ToString());
                    doc.Views.Redraw();
                    DisplayNodeGroups(doc, Old_Layer, NodeGroups, Mode, out rhObjList, out colorSet, out New_Layer);
                    doc.Layers.SetCurrentLayerIndex(New_Layer.LayerIndex, true);
                    New_Layer.CommitChanges();
                    Old_Layer.IsVisible = false;
                    Old_Layer.CommitChanges();
                    LabelNode(doc, rhObjList);
                }

                doc.Views.Redraw();
                FlagColor = 1;
            }
        }

        public static void DisplayNodeGroups(RhinoDoc doc, Layer Old_Layer, List<List<int>> NodeGroups, int Mode, out List<RhinoObject> NewRhObjs, out List<Color> colorSet, out Layer New_Layer)
        {
            Random rnd = new Random();
            ObjectAttributes attributes;
            RhinoObject obj;
             NewRhObjs = new List<RhinoObject>();
            List<Vector3d> vectors = new List<Vector3d>();
            New_Layer = new Layer();
            colorSet = new List<Color>();
            int NumOfGroups = NodeGroups.Count;
            Color color;
           
            //List<RhinoObject> rhObjList = new List<RhinoObject>();
            // find object list by layer
            //rhObjList = doc.Objects.FindByLayer(Old_Layer).ToList();
           
            // add a new layer
            if (Mode == 0) // display modules
            {
                New_Layer.Name = "Module info";

            }
            else if (Mode == 1 || Mode == 2) // dispaly node group result without R3
            {
                New_Layer.Name = "Result";

            }
            else
            {
                // display R3 result
                New_Layer.Name = "Result_R3";
            }

            doc.Layers.Add(New_Layer);
            New_Layer.LayerIndex = doc.Layers.FindByFullPath(New_Layer.Name, true);
            doc.Layers.SetCurrentLayerIndex(New_Layer.LayerIndex, true);
           // PCCDInfoSetup_New.CopyObjects(doc, Old_Layer, New_Layer, out rhObjList);
            PCCDInfoSetup_New.ExplodeView(doc, Old_Layer, New_Layer, out  NewRhObjs); ;
            New_Layer.CommitChanges();

            //rhObjList = doc.Objects.FindByLayer(New_Layer).ToList();

            for (int i = 0; i < NumOfGroups; i++)
            {
                
                int x = 0;
                if (NodeGroups[i].Count > 1)
                {
                    // make sure color does not exist
                    while (true)
                    {
                        color = Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                        for (int j = 0; j < colorSet.Count; j++)
                        {
                            if (color == colorSet[j])
                            {
                                x = 1;
                            }
                        }
                        if (x == 0)
                        {
                            break;
                        }
                    }
                    for (int j = 0; j < NodeGroups[i].Count; j++)
                    {

                        obj = NewRhObjs[NodeGroups[i].ElementAt(j) - 1];
                       // attributes = obj.Attributes;
                        //attributes.ColorSource = (ObjectColorSource)1;
                        obj.Attributes.ObjectColor = color;
                        obj.CommitChanges();

                    }
                    colorSet.Add(color);
                }
                else
                {
                    obj = NewRhObjs[NodeGroups[i].ElementAt(0) - 1];
                    colorSet.Add(obj.Attributes.ObjectColor);

                }
            }

            Old_Layer.IsVisible = false;
            Old_Layer.CommitChanges();
           // doc.Views.Redraw();
        }

        public static void LabelNode(RhinoDoc doc, List<RhinoObject> rhObjList)
        {
            for (int i = 0; i < rhObjList.Count(); i++)
            {
                RhinoObject rhObj = rhObjList[i];
                Point3d point = rhObj.Geometry.GetBoundingBox(true).Center;
                Guid dotId = doc.Objects.AddTextDot((i + 1).ToString(), point);
            }
        }

        ///write results to PDF and XML
        ///PDF for user
        ///xml for passing results to "consolidate design" & "conflict info hub"
        private void button8_Click(object sender, EventArgs e)
        {
            string str = txtBox_outputPath.Text;
            result.nodeGroups = NodeGroups;
            result.troubleNodeList = TroubleNodeList;
            result.troubleCode = TroubleCode;
            string filePath = @txtBox_outputPath.Text + "\\" + currProduct.Name +"_PCCDResult" + ".xml";
            XmlHelper.ToXmlFile(result, filePath);
            MessageBox.Show(String.Format("PCCD result is saved {0}", txtBox_outputPath.Text));
            // save information as readable pdf

            System.IO.FileStream fs = new FileStream(@txtBox_outputPath.Text + "\\" + currProduct.Name + "_PCCDResult.pdf", FileMode.Create);
           Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
           document.AddAuthor("ADML");
           document.AddTitle(String.Format("{0} PCCD result", currProduct.Name));
           document.Open();
         // Add a simple and wellknown phrase to the document in a flow layout manner
           Chunk chunk = new Chunk("This is from chunk. ");
           document.Add(chunk);

           Phrase phrase = new Phrase("This is from Phrase.");
           document.Add(phrase);

           Paragraph para = new Paragraph("This is from paragraph.");
           document.Add(para);

           string text = @"you are successfully created PDF file.";
           Paragraph paragraph = new Paragraph();
           paragraph.SpacingBefore = 10;
           paragraph.SpacingAfter = 10;
           paragraph.Alignment = Element.ALIGN_LEFT;
           paragraph.Font = FontFactory.GetFont(FontFactory.HELVETICA, 12f, BaseColor.GREEN);
           paragraph.Add(text);
           document.Add(paragraph);
          // Close the document
           document.Close();
          // Close the writer instance
           writer.Close();
          // Always close open filehandles explicity
            fs.Close();


        }


     





    }
}
