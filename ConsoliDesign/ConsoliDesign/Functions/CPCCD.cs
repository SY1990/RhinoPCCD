using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoliDesign.Data;

namespace ConsoliDesign.Functions
{
    /// <summary>
    /// CPCCD function
    /// </summary>
    public class CPCCD
    {

        // Verify all rules in a general product
        public static void CPCCDAllRules(List<List<int>> LinkToCut, double Alfa, List<List<int>> Modules, List<List<List<int>>> Communities, List<double> Modularity, List<double[,]> SubGraphs, List<List<ComponentStruct>> SubVerticeMs,
            List<int[,]> SubRelativeMs, double[] SizeLimit, int[] CFTypeCode, int[] MaterialList, out double[,] WMNode, out List<List<int>> TroubleNodeList, out List<int> TroubleCode, out List<List<int>> NodeGroups)
        {
            int NumOfLinks = LinkToCut.Count;
            int NumOfModules = Communities.Count;
            int NumOfNodes = 0;
            int ModuleID = 0;
            int NodeID1, NodeID2;
            for (int i = 0; i < Modules.Count; i++)
            {
                NumOfNodes += Modules[i].Count;
            }
            double[,] WMNode1 = new double[NumOfNodes, NumOfNodes];
            //Array.Copy(WeightMatrix, WMNode1, NumOfNode * NumOfNode);
            List<List<int>> TroubleNodeList2 = new List<List<int>>();
            List<int> TroubleCode2 = new List<int>();

            for (int i = 0; i < NumOfLinks; i++)
            {
                TroubleNodeList2.Add(LinkToCut[i].ToList());
                TroubleCode2.Add(CFTypeCode[2]);
                //WMNode1[LinkToCut[i].ElementAt(0) - 1, LinkToCut[i].ElementAt(1) - 1] = 0;
                // WMNode1[LinkToCut[i].ElementAt(1) - 1, LinkToCut[i].ElementAt(0) - 1] = 0;
                /// Modify the links within the corresponding SubGraphs
                // find module ID first
                ModuleID = NPCCD.PCCDFindRow(Modules, LinkToCut[i].ToList());
                double[,] T_Matrix = new double[Modules[ModuleID].Count, Modules[ModuleID].Count];
                Array.Copy(SubGraphs[ModuleID], T_Matrix, Modules[ModuleID].Count * Modules[ModuleID].Count);
                // find the index of nodes in the specific module
                NodeID1 = FindNodeIndex(Modules[ModuleID], LinkToCut[i].ElementAt(0));
                NodeID2 = FindNodeIndex(Modules[ModuleID], LinkToCut[i].ElementAt(1));
                T_Matrix[NodeID1, NodeID2] = 0;
                T_Matrix[NodeID2, NodeID1] = 0;
                SubGraphs[ModuleID] = T_Matrix;
            }
            CPCCDAllRules_R3(Alfa, Modules, Communities, Modularity, SubGraphs, SubVerticeMs, SubRelativeMs, SizeLimit, CFTypeCode, MaterialList, out WMNode, out TroubleNodeList, out TroubleCode, out NodeGroups);
            TroubleNodeList.AddRange(TroubleNodeList2.ToList());
            TroubleCode.AddRange(TroubleCode2.ToList());
        }


        // VERIFY all rules except R3 in a general product
        public static void CPCCDAllRules_R3(double Alfa, List<List<int>> Modules, List<List<List<int>>> Communities, List<double> Modularity, List<double[,]> SubGraphs, List<List<ComponentStruct>> SubVerticeMs, List<int[,]> SubRelativeMs,
            double[] SizeLimit, int[] CFTypeCode, int[] MaterialList, out double[,] WMNode, out List<List<int>> TroubleNodeList, out List<int> TroubleCode, out List<List<int>> NodeGroups)
        {
            int NumOfModules = Communities.Count;
            int NumOfNodes = 0;
            for (int i = 0; i < Modules.Count; i++)
            {
                NumOfNodes += Modules[i].Count;
            }

            double[,] T_WMNode;
            List<List<int>> T_TroubleNodeList;
            List<int> T_TroubleCode;
            List<List<int>> T_NodeGroups;
            List<List<int>> GlobalTroubleNodeList;
            List<int> GlobalTroubleCode;
            TroubleNodeList = new List<List<int>>();
            TroubleCode = new List<int>();
            WMNode = new double[NumOfNodes, NumOfNodes];
            NodeGroups = new List<List<int>>();


            for (int i = 0; i < NumOfModules; i++)
            {
                if (Modularity[i] >= Alfa)
                {
                    // apply MPCCD in this module
                    MPCCD.MPCCDAllRules_R3(Communities[i], SubGraphs[i], SubVerticeMs[i], SubRelativeMs[i], SizeLimit, CFTypeCode, MaterialList, out T_WMNode, out T_TroubleNodeList, out T_TroubleCode, out T_NodeGroups);

                }
                else
                {
                    // apply NPCCD in this module
                    NPCCD.NPCCDAllRules_R3(SubGraphs[i], SubVerticeMs[i], SizeLimit, MaterialList, SubRelativeMs[i], CFTypeCode, out T_WMNode, out T_TroubleNodeList, out T_TroubleCode, out T_NodeGroups);
                }

                MPCCD.MPCCDTransLocalToGlobal(Modules[i], T_TroubleNodeList, T_TroubleCode, T_NodeGroups, out GlobalTroubleNodeList, out GlobalTroubleCode);
                TroubleNodeList.AddRange(GlobalTroubleNodeList.ToList());
                TroubleCode.AddRange(GlobalTroubleCode.ToList());
                // translate the surviving submodules to a whole global groph
                for (int j = 0; j < T_WMNode.GetLength(0); j++)
                {
                    for (int k = 0; k < T_WMNode.GetLength(0); k++)
                    {
                        if (T_WMNode[j, k] != 0)
                        {
                            WMNode[Modules[i].ElementAt(j) - 1, Modules[i].ElementAt(k) - 1] = T_WMNode[j, k];
                            WMNode[Modules[i].ElementAt(k) - 1, Modules[i].ElementAt(j) - 1] = T_WMNode[k, j];
                        }
                    }
                }
            }
            NPCCD.PCCDChain(WMNode, out NodeGroups);




            // Translate solutions to its global node notation
        }

        // Find community information in each module
        public static void FindCommunity(double Alfa, List<List<int>> Modules, double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, int[,] RelativeMotionMatrix,
            out List<List<List<int>>> Communities, out List<double> Modularity, out List<double[,]> SubGraphs, out List<List<ComponentStruct>> SubVerticeMs, out List<int[,]> SubRelativeMs)
        {
            int NumOfModules = Modules.Count;
            Communities = new List<List<List<int>>>();
            Modularity = new List<double>();
            SubGraphs = new List<double[,]>();
            SubVerticeMs = new List<List<ComponentStruct>>(); ; ;
            SubRelativeMs = new List<int[,]>();
            double[,] WMIntraModules;
            int NumOfIteration = 5;
            double CrossOverRatio = 0.4;
            double MutationRatio = 0.05;
            List<int> NumOfPopulation = new List<int>();
            List<int> NumOfGeneration = new List<int>();
            List<List<int>> SubModules;
            double[] T_BestFitness;
            List<int> sublist = new List<int>();
            MPCCD.MPCCDSubGraph(Modules, WeightMatrix, VerticeMatrix, RelativeMotionMatrix, out WMIntraModules, out SubGraphs, out  SubVerticeMs, out  SubRelativeMs);
            int x = 0;
            for (int i = 0; i < NumOfModules; i++)
            {

                NumOfPopulation.Add((int)(Modules[x].Count / 2) + 1);
                NumOfGeneration.Add(5 * Modules[x].Count);
                x++;
            }
            // find the maximum modularity in each module 
            for (int i = 0; i < NumOfModules; i++)
            {
                GAModuleDiv.FindBestSolution_Multiple(SubGraphs[i], NumOfPopulation[i], NumOfGeneration[i], CrossOverRatio, MutationRatio, out SubModules, out T_BestFitness);
                double XX = T_BestFitness.Max();
                if (T_BestFitness.Max() > Alfa)
                {
                    Communities.Add(SubModules.ToList());
                    Modularity.Add(T_BestFitness.Max());
                }
                else
                {
                    SubModules.Clear();
                    for (int j = 0; j < Modules[i].Count; j++)
                    {
                        sublist.Add(j + 1);
                    }
                    SubModules.Add(sublist.ToList());
                    Communities.Add(SubModules);
                    Modularity.Add(T_BestFitness.Max());
                    sublist.Clear();
                }

            }


        }

        // find the index of a node in a module
        public static int FindNodeIndex(List<int> module, int Node)
        {
            int NodeId = 0;
            for (int i = 0; i < module.Count; i++)
            {
                if (module[i] == Node)
                {
                    NodeId = i;
                    break;
                }
            }
            return NodeId;
        }
    }
}