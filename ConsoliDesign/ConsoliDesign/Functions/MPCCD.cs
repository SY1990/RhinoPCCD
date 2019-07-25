using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoliDesign.Data;

namespace ConsoliDesign.Functions
{
    public class MPCCD
    {

        // Verify rule R3
        public static void MPCCDAllRules(List<List<int>> LinkToCut, List<List<int>> Modules, double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, int[,] RelativeMotionMatrix, double[] SizeLimit, int[] CFTypeCode, int[] MaterialList,
            out double[,] WMNode, out List<List<int>> TroubleNodeList, out List<int> TroubleCode, out List<List<int>> NodeGroups)
        {
            int NumOfLinks = LinkToCut.Count;
            int NumOfNode = WeightMatrix.GetLength(0);
            TroubleNodeList = new List<List<int>>();
            TroubleCode = new List<int>();
            NodeGroups = new List<List<int>>();
            List<List<int>> TroubleNodeList2 = new List<List<int>>();
            List<int> TroubleCode2 = new List<int>();
            double[,] WMNode1 = new double[WeightMatrix.GetLength(0), WeightMatrix.GetLength(0)];
            WMNode = new double[WeightMatrix.GetLength(0), WeightMatrix.GetLength(0)];
            Array.Copy(WeightMatrix, WMNode1, NumOfNode * NumOfNode);
            for (int i = 0; i < NumOfLinks; i++)
            {
                TroubleNodeList2.Add(LinkToCut[i].ToList());
                TroubleCode2.Add(CFTypeCode[2]);
                WMNode1[LinkToCut[i].ElementAt(0) - 1, LinkToCut[i].ElementAt(1) - 1] = 0;
                WMNode1[LinkToCut[i].ElementAt(1) - 1, LinkToCut[i].ElementAt(0) - 1] = 0;
            }
            MPCCDAllRules_R3(Modules, WMNode1, VerticeMatrix, RelativeMotionMatrix, SizeLimit, CFTypeCode, MaterialList, out WMNode, out TroubleNodeList, out TroubleCode, out  NodeGroups);
            // Print.PrintMatrix(WMNode);
            TroubleNodeList.AddRange(TroubleNodeList2.ToList());
            TroubleCode.AddRange(TroubleCode2.ToList());

        }


        // verify all rules except R3 in each modules
        public static void MPCCDAllRules_R3(List<List<int>> Modules, double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, int[,] RelativeMotionMatrix, double[] SizeLimit, int[] CFTypeCode, int[] MaterialList,
            out double[,] WMNode, out List<List<int>> TroubleNodeList, out List<int> TroubleCode, out List<List<int>> NodeGroups)
        {
            int NumOfModules = Modules.Count;
            int NumOfNode = WeightMatrix.GetLength(0);
            double[,] WMIntraModules = new double[NumOfNode, NumOfNode];
            List<double[,]> SubGraphs = new List<double[,]>();
            List<List<ComponentStruct>> SubVerticeMs = new List<List<ComponentStruct>>(); 
            List<int[,]> SubRelativeMs = new List<int[,]>();
            WMNode = new double[NumOfNode, NumOfNode];

            TroubleNodeList = new List<List<int>>();
            TroubleCode = new List<int>();
            NodeGroups = new List<List<int>>();
            double[,] WMNode1 = new double[NumOfNode, NumOfNode];

            double[,] WMNode2, WMNode3;
            List<int> TroubleCode1, TroubleCode2;
            List<List<int>> TroubleNodeList1, TroubleNodeList2;
            List<List<int>> T_TroubleNodeList = new List<List<int>>();
            List<int> T_TroubleCode = new List<int>();
            List<List<int>> T_NodeGroups = new List<List<int>>();
            List<List<int>> GlobalTroubleNodeList;
            List<int> GlobalTroubleCode;
            double[,] WMInterModule;
           // List<int> Module = new List<int>();

          //  MPCCDGenerateModule(NumOfNode, out Module);

            MPCCDSubGraph(Modules, WeightMatrix, VerticeMatrix, RelativeMotionMatrix, out WMIntraModules, out SubGraphs, out SubVerticeMs, out SubRelativeMs);
            MPCCDExtractInterM(WeightMatrix, Modules, out WMInterModule);  // extract inter module graph

            for (int i = 0; i < NumOfModules; i++)
            {
                double[,] T_WMNode = new double[Modules[i].Count, Modules[i].Count];
                NPCCD.NPCCDAllRules_R3(SubGraphs[i], SubVerticeMs[i], SizeLimit, MaterialList, SubRelativeMs[i], CFTypeCode, out T_WMNode, out T_TroubleNodeList, out T_TroubleCode, out T_NodeGroups);
                MPCCDTransLocalToGlobal(Modules[i], T_TroubleNodeList, T_TroubleCode, T_NodeGroups, out GlobalTroubleNodeList, out GlobalTroubleCode);
                TroubleNodeList.AddRange(GlobalTroubleNodeList.ToList());
                TroubleCode.AddRange(GlobalTroubleCode.ToList());
                // translate the surviving submodules to a whole global groph
                for (int j = 0; j < T_WMNode.GetLength(0); j++)
                {
                    for (int k = 0; k < T_WMNode.GetLength(0); k++)
                    {
                        if (T_WMNode[j, k] != 0)
                        {
                            WMNode1[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1] = T_WMNode[j, k];
                            WMNode1[Modules[i].ElementAt(k) - 1, Modules[i].ElementAt(j) - 1] = T_WMNode[k, j];
                        }
                    }
                }
            }
            // Print.PrintList(TroubleNodeList);
            // Print.PrintMatrix(WMNode1);
            // verify R1-7 EXCEPT R5, R3 on inter-module graph
            // NPCCD.NPCCDAllRules_R3R5(WMInterModule, VerticeMatrix, SizeLimit, MaterialList, RelativeMotionMatrix, CFTypeCode, out WMNode2, out TroubleNodeList1, out TroubleCode1);

            NPCCD.PCCDNodePair(WMInterModule, VerticeMatrix, RelativeMotionMatrix, SizeLimit, CFTypeCode, out WMNode2, out TroubleNodeList1, out TroubleCode1);
            //Print.PrintList(TroubleNodeList1);
            // Print.PrintMatrix(WMNode2);
            MPCCDCombineGraph(WMNode1, WMNode2, out WMNode3);

            //NPCCD.NPCCDAllRules_R3(WMNode3, VerticeMatrix, SizeLimit, MaterialList, RelativeMotionMatrix, CFTypeCode, out WMNode, out TroubleNodeList2, out TroubleCode2, out NodeGroups);
            NPCCD.PCCDNodeGroup(WMNode3, VerticeMatrix, SizeLimit, CFTypeCode, out WMNode, out TroubleNodeList2, out TroubleCode2);
            NPCCD.PCCDChain(WMNode, out NodeGroups);
            TroubleNodeList.AddRange(TroubleNodeList1.ToList());
            TroubleNodeList.AddRange(TroubleNodeList2.ToList());
            TroubleCode.AddRange(TroubleCode1.ToList());
            TroubleCode.AddRange(TroubleCode2.ToList());

        }

        // Extract the module network from the whole network
        public static void MPCCDSubGraph(List<List<int>> Modules, double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, int[,] RelativeMotionMatrix,
            out double[,] WMIntraModules, out List<double[,]> SubGraphs, out List<List<ComponentStruct>> SubVerticeMs, out List<int[,]> SubRelativeMs)
        {
            int NumOfModules = Modules.Count;
            int NumOfNode = WeightMatrix.GetLength(0);
            WMIntraModules = new double[NumOfNode, NumOfNode];
            SubGraphs = new List<double[,]>();
            SubVerticeMs = new List<List<ComponentStruct>>();
           
           
            SubRelativeMs = new List<int[,]>();
            for (int i = 0; i < NumOfModules; i++)
            {
                      
                List<ComponentStruct> partList  = new List<ComponentStruct>();
                ComponentStruct part = new ComponentStruct();
                int NumOfElements = Modules[i].Count;
                double[,] SubGraph = new double[NumOfElements, NumOfElements];
                ComponentStruct SubVerticeM = new ComponentStruct ();
                int[,] SubRelativeM = new int[NumOfElements, NumOfElements];
                for (int j = 0; j < NumOfElements; j++)
                {
                    for (int k = 0; k < NumOfElements; k++)
                    {
                        if (j != k)
                        {
                            if (WeightMatrix[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1] != 0)
                            {
                                SubGraph[j, k] = WeightMatrix[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1];
                                SubGraph[k, j] = WeightMatrix[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1];
                                WMIntraModules[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1] = WeightMatrix[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1];
                                WMIntraModules[Modules[i].ElementAt(k) - 1, Modules[i].ElementAt(j) - 1] = WeightMatrix[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1];
                            }
                            if (RelativeMotionMatrix[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1] != 0)
                            {
                                SubRelativeM[j, k] = 1;

                            }
                        }
                        else
                        {
                            SubGraph[j, k] = 1;
                            WMIntraModules[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1] = 1;
                            WMIntraModules[Modules[i].ElementAt(k) - 1, Modules[i].ElementAt(j) - 1] = 1;
                        }
                    }
                    SubVerticeM = new ComponentStruct 
                    {
                        material = VerticeMatrix[Modules[i].ElementAt(j) - 1].material,
                        MaintenanceFreq = VerticeMatrix[Modules[i].ElementAt(j) - 1].MaintenanceFreq,
                        bdBox =VerticeMatrix[Modules[i].ElementAt(j) - 1].bdBox,
                        IsStandard = VerticeMatrix[Modules[i].ElementAt(j) - 1].IsStandard
  
                    };
                    
                    partList.Add(SubVerticeM);
                    
                }
                SubGraphs.Add(SubGraph);
                SubRelativeMs.Add(SubRelativeM);
                SubVerticeMs.Add(partList.ToList());

            }
            
        }

        // Translate submodule information to global information
        public static void MPCCDTransLocalToGlobal(List<int> Module, List<List<int>> TroubleNodeList, List<int> TroubleCode, List<List<int>> NodeGroups,
             out List<List<int>> GlobalTroubleNodeList, out List<int> GlobalTroubleCode)
        {
            GlobalTroubleNodeList = new List<List<int>>();
            List<int> sublist = new List<int>();

            GlobalTroubleCode = new List<int>();
            //GlobalNodeGroups = new List<List<int>>();
            for (int i = 0; i < TroubleNodeList.Count; i++)
            {
                for (int j = 0; j < TroubleNodeList[i].Count; j++)
                {
                    sublist.Add(Module[TroubleNodeList[i].ElementAt(j) - 1]);
                }
                GlobalTroubleNodeList.Add(sublist.ToList());
                GlobalTroubleCode.Add(TroubleCode[i]);
                sublist.Clear();
            }



            /* for (int i = 0; i < NodeGroups.Count; i++)
             {
                 for (int j = 0; j < NodeGroups[i].Count; j++)
                 {
                     sublist.Add(Module[NodeGroups[i].ElementAt(j) - 1]);
                 }
                 GlobalNodeGroups.Add(sublist.ToList());
                 sublist.Clear();
             }
             */
        }

        // Extract inter-module graph
        public static void MPCCDExtractInterM(double[,] WeightMatrix, List<List<int>> Modules, out double[,] WMInterModule)
        {
            int NumOfNode = WeightMatrix.GetLength(0);
            WMInterModule = new double[NumOfNode, NumOfNode];
            for (int i = 0; i < NumOfNode; i++)
            {
                for (int j = 0; j < NumOfNode; j++)
                {
                    if (WeightMatrix[i, j] != 0)
                    {
                        if (MPCCDRetrieveID(i + 1, Modules) != MPCCDRetrieveID(j + 1, Modules))
                        {
                            WMInterModule[i, j] = WeightMatrix[i, j];
                            WMInterModule[j, i] = WeightMatrix[j, i];
                        }
                        if (i == j)
                        {

                            WMInterModule[i, j] = WeightMatrix[i, j];
                            WMInterModule[j, i] = WeightMatrix[j, i];
                        }
                    }
                }
            }

        }

        // Retrieve module ID of a specific node
        public static int MPCCDRetrieveID(int Node, List<List<int>> Modules)
        {
            int ID = Modules.Count + 1;
            for (int i = 0; i < Modules.Count; i++)
            {
                for (int j = 0; j < Modules[i].Count; j++)
                {
                    if (Modules[i].ElementAt(j) == Node)
                    {
                        ID = i;
                        break;
                    }
                }
            }
            return ID;
        }

        // Combine inter module and intra module graph
        public static void MPCCDCombineGraph(double[,] A, double[,] B, out double[,] C)
        {
            int NumOfNode = A.GetLength(0);
            C = new double[NumOfNode, NumOfNode];

            for (int i = 0; i < NumOfNode; i++)
            {
                for (int j = 0; j < NumOfNode; j++)
                {
                    if (A[i, j] != 0)
                    {
                        C[i, j] = A[i, j];
                    }
                    if (B[i, j] != 0)
                    {
                        C[i, j] = B[i, j];
                    }
                }
            }

        }

        public static void MPCCDGenerateModule(int NumOfNode, out List<int>Module)
        {
            Module = new List<int>();
            for (int i = 0; i < NumOfNode; i++)
            {
                Module.Add(i + 1);
            }
        }


    }
}
