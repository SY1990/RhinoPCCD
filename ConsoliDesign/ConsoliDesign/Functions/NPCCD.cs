using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino;
using Rhino.FileIO;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Commands;
using Rhino.Geometry;
using ConsoliDesign.Forms;
using ConsoliDesign.Data;

namespace ConsoliDesign.Functions
{
    /// <summary>
    /// NPCCD function
    /// </summary>
    public class NPCCD
    {
       // private List<RhinoObject> PartList{get; set;}
        
        // Verify all rules including R3
        public static void NPCCDAllRules(List<List<int>> LinkToCut, double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, double[] SizeLimit, int[] MaterialList, int[,] RelativeMotionMatrix, int[] CFTypeCode,
            out double[,] WMNode4, out List<List<int>> TroubleNodeList, out List<int> TroubleCode, out  List<List<int>> NodeGroups)
        {
            double[,] WMNode;
            double[,] WMNode2, WMNode3;
            TroubleNodeList = new List<List<int>>();
            List<List<int>> TroubleNodeList2 = new List<List<int>>();
            List<List<int>> TroubleNodeList3 = new List<List<int>>();
            List<List<int>> TroubleNodeList4 = new List<List<int>>();
            TroubleCode = new List<int>();
            List<int> TroubleCode2 = new List<int>();
            List<int> TroubleCode3 = new List<int>();
            List<int> TroubleCode4 = new List<int>();
            WMNode4 = new double[WeightMatrix.GetLength(0), WeightMatrix.GetLength(0)];
            NodeGroups = new List<List<int>>();

            PCCDNode(WeightMatrix, VerticeMatrix, SizeLimit, MaterialList, CFTypeCode, out WMNode, out TroubleNodeList, out TroubleCode);
            PCCDNodePair( WMNode, VerticeMatrix, RelativeMotionMatrix, SizeLimit, CFTypeCode, out WMNode2, out TroubleNodeList2, out TroubleCode2);
            PCCDNodeGroup(WMNode2, VerticeMatrix, SizeLimit, CFTypeCode, out WMNode3, out TroubleNodeList3, out TroubleCode3);
            PCCDR3(LinkToCut, WMNode2, VerticeMatrix, SizeLimit, CFTypeCode, out WMNode4, out TroubleNodeList4, out TroubleCode4);
            PCCDChain(WMNode4, out NodeGroups);
            TroubleNodeList3.AddRange(TroubleNodeList4.ToList());
            TroubleNodeList2.AddRange(TroubleNodeList3.ToList());
            TroubleNodeList.AddRange(TroubleNodeList2.ToList());
            TroubleCode3.AddRange(TroubleCode4.ToList());
            TroubleCode2.AddRange(TroubleCode3.ToList());
            TroubleCode.AddRange(TroubleCode2.ToList());
        }


        // Verify all rules R1-7 EXCEPT R3
        public static void NPCCDAllRules_R3(double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, double[] SizeLimit, int[] MaterialList, int[,] RelativeMotionMatrix, int[] CFTypeCode,
            out double[,] WMNode3, out List<List<int>> TroubleNodeList, out List<int> TroubleCode, out List<List<int>> NodeGroups)
        {
            double[,] WMNode, WMNode2;
            WMNode3 = new double[WeightMatrix.GetLength(0), WeightMatrix.GetLength(0)];
            TroubleNodeList = new List<List<int>>();
            List<List<int>> TroubleNodeList2 = new List<List<int>>();
            List<List<int>> TroubleNodeList3 = new List<List<int>>();
            TroubleCode = new List<int>();
            List<int> TroubleCode2 = new List<int>();
            List<int> TroubleCode3 = new List<int>();
            NodeGroups = new List<List<int>>();

            PCCDNode(WeightMatrix, VerticeMatrix, SizeLimit, MaterialList, CFTypeCode, out WMNode, out TroubleNodeList, out TroubleCode);
            PCCDNodePair(WMNode, VerticeMatrix, RelativeMotionMatrix, SizeLimit, CFTypeCode, out WMNode2, out TroubleNodeList2, out TroubleCode2);
            PCCDNodeGroup(WMNode2, VerticeMatrix, SizeLimit, CFTypeCode, out WMNode3, out TroubleNodeList3, out TroubleCode3);
            PCCDChain(WMNode3, out NodeGroups);

            TroubleNodeList2.AddRange(TroubleNodeList3.ToList());
            TroubleNodeList.AddRange(TroubleNodeList2.ToList());
            TroubleCode2.AddRange(TroubleCode3.ToList());
            TroubleCode.AddRange(TroubleCode2.ToList());
        }

        // verify all rules R1-7 EXCEPT R3, R5
        public static void NPCCDAllRules_R3R5(List<int> Module, double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, double[] SizeLimit, int[] MaterialList, int[,] RelativeMotionMatrix, int[] CFTypeCode,
           out double[,] WMNode2, out List<List<int>> TroubleNodeList, out List<int> TroubleCode)
        {
            double[,] WMNode;
            WMNode2 = new double[WeightMatrix.GetLength(0), WeightMatrix.GetLength(0)];
            TroubleNodeList = new List<List<int>>();
            List<List<int>> TroubleNodeList2 = new List<List<int>>();
            TroubleCode = new List<int>();
            List<int> TroubleCode2 = new List<int>();

            PCCDNode(WeightMatrix, VerticeMatrix, SizeLimit, MaterialList, CFTypeCode, out WMNode, out TroubleNodeList, out TroubleCode);
            PCCDNodePair(WMNode, VerticeMatrix, RelativeMotionMatrix, SizeLimit, CFTypeCode, out WMNode2, out TroubleNodeList2, out TroubleCode2);

            TroubleNodeList.AddRange(TroubleNodeList2.ToList());
            TroubleCode.AddRange(TroubleCode2.ToList());
        }


        // Verify Node feasibility
        public static void PCCDNode(double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, double[] SizeLimit, int[] MaterialList, int[] CFTypeCode, out double[,] WMNode, out List<List<int>> TroubleNodeList, out List<int> TroubleCode)
        {
            // structure of VerticeM is: material, size (l,w,h), maintenance frequency, IsStandard
            TroubleNodeList = new List<List<int>>();
            List<int> sublist = new List<int>();
            TroubleCode = new List<int>();
            int NumOfNode = WeightMatrix.GetLength(0);
            WMNode = new double[NumOfNode, NumOfNode];
            Array.Copy(WeightMatrix, WMNode, NumOfNode * NumOfNode);
            for (int i = 0; i < NumOfNode; i++)
            {
                // check material R6
                int Flag = 1;
                for (int j = 0; j < MaterialList.Length; j++)
                {
                    if ( VerticeMatrix[i].material == MaterialList[j])
                    {
                        Flag = 0;

                        break;
                    }
                }
                if (Flag == 1)
                {
                    sublist.Add(i + 1);
                    TroubleNodeList.Add(sublist.ToList());
                    TroubleCode.Add(CFTypeCode[5]);
                    sublist.Clear();
                    for (int j = 0; j < NumOfNode; j++)
                    {
                        if (j != i)
                        {
                            WMNode[i, j] = 0;
                            WMNode[j, i] = 0;
                        }
                    }
                }
                // check R4 Isstandard
                if (VerticeMatrix[i].IsStandard == 1)
                {
                    sublist.Add(i + 1);
                    TroubleNodeList.Add(sublist.ToList());
                    TroubleCode.Add(CFTypeCode[3]);
                    sublist.Clear();
                    for (int j = 0; j < NumOfNode; j++)
                    {
                        if (j != i)
                        {
                            WMNode[i, j] = 0;
                            WMNode[j, i] = 0;
                        }
                    }
                }

                // check R5 size
                double[] Temp = new double[3];
                Temp[0] = VerticeMatrix[i].bdBox.Diagonal.X;
                Temp[1] = VerticeMatrix[i].bdBox.Diagonal.Y;
                Temp[2] = VerticeMatrix[i].bdBox.Diagonal.Z;
                Array.Sort(Temp);
                if (Temp.Max() >= SizeLimit.Max() || Temp[1] >= SizeLimit[1] || Temp.Min() >= SizeLimit.Min())
                {
                    sublist.Add(i + 1);
                    TroubleNodeList.Add(sublist.ToList());
                    TroubleCode.Add(CFTypeCode[4]);
                    sublist.Clear();
                    for (int j = 0; j < NumOfNode; j++)
                    {
                        if (j != i)
                        {
                            WMNode[i, j] = 0;
                            WMNode[j, i] = 0;
                        }
                    }
                }
            }
        }

        // verify node pair feasibility
        public static void PCCDNodePair(double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, int[,] RelativeMotionMatrix, double[] SizeLimit, int[] CFTypeCode, out double[,] WMNode, out List<List<int>> TroubleNodeList, out List<int> TroubleCode)
        {
            int NumOfNode = WeightMatrix.GetLength(0);
            TroubleNodeList = new List<List<int>>();
            List<int> sublist = new List<int>();
            TroubleCode = new List<int>();
            WMNode = new double[NumOfNode, NumOfNode];
            Array.Copy(WeightMatrix, WMNode, NumOfNode * NumOfNode);
            List<List<int>> NodeGroups = new List<List<int>>();
            List<List<double>> StackUpSize = new List<List<double>>();
            for (int i = 0; i < NumOfNode; i++)
            {
                for (int j = i + 1; j < NumOfNode; j++)
                {
                    if (WMNode[i, j] != 0)
                    {
                        // relative motion
                        if (RelativeMotionMatrix[i, j] != 0)
                        {
                            sublist.Add(i + 1);
                            sublist.Add(j + 1);
                            TroubleNodeList.Add(sublist.ToList());
                            TroubleCode.Add(CFTypeCode[0]);
                            sublist.Clear();
                            WMNode[i, j] = 0;
                            WMNode[j, i] = 0;
                        }
                    }
                    if (WMNode[i, j] != 0)
                    {
                        // material difference
                        if (VerticeMatrix[i].material != VerticeMatrix[j].material)
                        {
                            sublist.Add(i + 1);
                            sublist.Add(j + 1);
                            TroubleNodeList.Add(sublist.ToList());
                            TroubleCode.Add(CFTypeCode[1]);
                            sublist.Clear();
                            WMNode[i, j] = 0;
                            WMNode[j, i] = 0;

                        }
                    }
                    if (WMNode[i, j] != 0)
                    {
                        // maintenance difference
                        if (VerticeMatrix[i].MaintenanceFreq != VerticeMatrix[j].MaintenanceFreq)
                        {
                            sublist.Add(i + 1);
                            sublist.Add(j + 1);
                            TroubleNodeList.Add(sublist.ToList());
                            TroubleCode.Add(CFTypeCode[6]);
                            sublist.Clear();
                            WMNode[i, j] = 0;
                            WMNode[j, i] = 0;
                        }
                    }
                    if (WMNode[i, j] != 0)
                    {
                        // size limitation R5
                        sublist.Add(i + 1);
                        sublist.Add(j + 1);
                        NodeGroups.Add(sublist.ToList());
                        sublist.Clear();
                        PCCDStackUpSize(NodeGroups, VerticeMatrix, out StackUpSize);
                        StackUpSize[0].Sort();
                        Array.Sort(SizeLimit);
                        if (StackUpSize[0].Max() >= SizeLimit.Max() || StackUpSize[0].ElementAt(1) >= SizeLimit[1] || StackUpSize[0].Min() >= SizeLimit.Min())
                        {
                            sublist.Add(i + 1);
                            sublist.Add(j + 1);
                            TroubleNodeList.Add(sublist.ToList());
                            TroubleCode.Add(CFTypeCode[4]);
                            sublist.Clear();
                            WMNode[i, j] = 0;
                            WMNode[j, i] = 0;
                        }
                        NodeGroups.Clear();
                        StackUpSize.Clear();
                    }
                }

            }
        }


        // verify node group deasibility
        public static void PCCDNodeGroup(double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, double[] SizeLimit, int[] CFTypeCode, out double[,] WMNode, out List<List<int>> TroubleNodeList, out List<int> TroubleCode)
        {
            int NumOfNode = WeightMatrix.GetLength(0);
            TroubleNodeList = new List<List<int>>();
            List<int> sublist = new List<int>();
            TroubleCode = new List<int>();
            WMNode = new double[NumOfNode, NumOfNode];
            Array.Copy(WeightMatrix, WMNode, NumOfNode * NumOfNode);
            List<List<int>> NodeGroups_Old = new List<List<int>>();
            List<List<int>> NodeGroups_New = new List<List<int>>();
            List<List<double>> StackUpSize = new List<List<double>>();
            PCCDChain(WMNode, out  NodeGroups_Old); // for keeping a record purpose
            NodeGroups_New = NodeGroups_Old.ToList();
            List<List<int>> EdgeRank = new List<List<int>>();
            List<List<int>> LinkToBeCut = new List<List<int>>();
            List<int> TempTroubleNodeGroup = new List<int>();
            List<int> MarkUnfixedNodes = new List<int>();
            List<List<int>> MarkLinkcutGlobal = new List<List<int>>();
            int NumOfLinksCut = 0;
            int M = NodeGroups_New[0].Count;
            int i = 0;
            int Temp_Flag = 0;
            int Flag = 1;
            int GroupNum = NodeGroups_New.Count;
            while (true)
            {
                if (M >= 2)
                {
                    PCCDStackUpSize(NodeGroups_New, VerticeMatrix, out StackUpSize);
                    StackUpSize[i].Sort();
                    Array.Sort(SizeLimit);
                    if (StackUpSize[i].Max() >= SizeLimit.Max() || StackUpSize[i].ElementAt(1) >= SizeLimit[1] || StackUpSize[i].Min() >= SizeLimit.Min())
                    {
                        TroubleNodeList.Add(NodeGroups_New[i].ToList());
                        TroubleCode.Add(CFTypeCode[4]);
                        // find the weakest link in NodeGroups_New[i], and break it
                        PCCDSort(WMNode, NodeGroups_New[i], out  EdgeRank);
                        LinkToBeCut.Add(EdgeRank[0].ToList());
                        NumOfLinksCut++;
                        // find the row where the trouble pair is located
                        TempTroubleNodeGroup = NodeGroups_Old[PCCDFindRow(NodeGroups_Old, LinkToBeCut[NumOfLinksCut - 1])].ToList();
                        // break the edges in WMNode
                        WMNode[EdgeRank[0].ElementAt(0) - 1, EdgeRank[0].ElementAt(1) - 1] = 0;
                        WMNode[EdgeRank[0].ElementAt(1) - 1, EdgeRank[0].ElementAt(0) - 1] = 0;
                        PCCDChain(WMNode, out NodeGroups_New);
                        GroupNum = NodeGroups_New.Count;
                        i = 0;
                        M = NodeGroups_New[i].Count;
                    }
                    else
                    {
                        // check if current node group is from an old trouble group
                        for (int j = 0; j < TempTroubleNodeGroup.Count; j++)
                        {
                            if (TempTroubleNodeGroup[j] == NodeGroups_New[i].ElementAt(0))
                            {
                                Temp_Flag = 1;
                                break;

                            }
                        }
                        if (Temp_Flag == 1)
                        {
                            if (Flag == 1)
                            {//if this is the first time

                                PCCDSetMinus(TempTroubleNodeGroup.ToList(), NodeGroups_New[i], out MarkUnfixedNodes);
                                Flag = 0;
                            }
                            else { PCCDSetMinus(MarkUnfixedNodes, NodeGroups_New[i], out MarkUnfixedNodes); }
                            if (MarkUnfixedNodes.Count == 0 && NumOfLinksCut != 0)
                            {
                                // restore the broken edges in the previous steps except the last very cut
                                for (int j = 0; j < NumOfLinksCut - 1; j++)
                                {
                                    WMNode[LinkToBeCut[j].ElementAt(0) - 1, LinkToBeCut[j].ElementAt(1) - 1] = WeightMatrix[LinkToBeCut[j].ElementAt(0) - 1, LinkToBeCut[j].ElementAt(1) - 1];
                                    WMNode[LinkToBeCut[j].ElementAt(1) - 1, LinkToBeCut[j].ElementAt(0) - 1] = WeightMatrix[LinkToBeCut[j].ElementAt(1) - 1, LinkToBeCut[j].ElementAt(0) - 1];
                                }
                                if (NumOfLinksCut > 1)
                                {
                                    MarkLinkcutGlobal.Add(LinkToBeCut[NumOfLinksCut - 1].ToList());
                                }
                                NumOfLinksCut = 0;
                                LinkToBeCut.Clear();
                                Flag = 1;
                                PCCDChain(WMNode, out NodeGroups_New);
                                GroupNum = NodeGroups_New.Count;
                                i = 0;
                                M = NodeGroups_New[i].Count;
                            }
                            else
                            {
                                if (i < GroupNum-1)
                                { i++; M = NodeGroups_New[i].Count; }

                                else
                                { break; }
                            }
                        }
                      else
                      {
                        if (i < GroupNum-1)
                        { i++; M = NodeGroups_New[i].Count; }
                        else { break; }
                      }

                    }
                }
                else // m=1
                {
                    // check if current node group is from an old trouble group
                    for (int j = 0; j < TempTroubleNodeGroup.Count; j++)
                    {
                        if (TempTroubleNodeGroup[j] == NodeGroups_New[i - 1].ElementAt(0))
                        {
                            Temp_Flag = 1;
                            break;

                        }
                    }
                    if (Temp_Flag == 1)
                    {
                        if (Flag == 1)
                        {//if this is the first time

                            PCCDSetMinus(TempTroubleNodeGroup.ToList(), NodeGroups_New[i - 1], out MarkUnfixedNodes);
                            Flag = 0;
                        }
                        else { PCCDSetMinus(MarkUnfixedNodes, NodeGroups_New[i - 1], out MarkUnfixedNodes); }
                        if (MarkUnfixedNodes.Count == 0 && NumOfLinksCut != 0)
                        {
                            // restore the broken edges in the previous steps except the last very cut
                            for (int j = 0; j < NumOfLinksCut - 2; j++)
                            {
                                WMNode[LinkToBeCut[j].ElementAt(0) - 1, LinkToBeCut[j].ElementAt(1) - 1] = WeightMatrix[LinkToBeCut[j].ElementAt(0) - 1, LinkToBeCut[j].ElementAt(1) - 1];
                                WMNode[LinkToBeCut[j].ElementAt(1) - 1, LinkToBeCut[j].ElementAt(0) - 1] = WeightMatrix[LinkToBeCut[j].ElementAt(1) - 1, LinkToBeCut[j].ElementAt(0) - 1];
                            }
                            if (NumOfLinksCut > 1)
                            {
                                MarkLinkcutGlobal.Add(LinkToBeCut[NumOfLinksCut - 1].ToList());
                            }
                            NumOfLinksCut = 0;
                            LinkToBeCut.Clear();
                            Flag = 1;
                            PCCDChain(WMNode, out NodeGroups_New);
                            GroupNum = NodeGroups_New.Count;
                            i = 0;
                            M = NodeGroups_New[i].Count;
                        }
                        else
                        {
                            if (i < GroupNum-1)
                            { i++; M = NodeGroups_New[i].Count; }

                            else
                            { break; }
                        }
                    }
                   else
                    {
                        if (i < GroupNum-1 )
                        { i++; M = NodeGroups_New[i].Count; }

                        else
                        { break; }
                    }

                }

            }

        }

        // Verify assembly access
        public static void PCCDR3(List<List<int>> LinkToCut, double[,] WeightMatrix, List<ComponentStruct> VerticeMatrix, double[] SizeLimit, int[] CFTypeCode, out double[,] WMNode, out List<List<int>> TroubleNodeList, out List<int> TroubleCode)
        {
            int NumOfLinks = LinkToCut.Count;
            int NumOfNode = WeightMatrix.GetLength(0);
            List<List<int>> TroubleNodeList0 = new List<List<int>>();
            TroubleNodeList = new List<List<int>>();
            List<int> sublist = new List<int>();
            List<int> TroubleCode0 = new List<int>();
            double[,] WMNode0 = new double[NumOfNode, NumOfNode];
            WMNode0 = new double[NumOfNode, NumOfNode];
            Array.Copy(WeightMatrix, WMNode0, NumOfNode * NumOfNode);
            for (int i = 0; i < NumOfLinks; i++)
            {
                TroubleNodeList0.Add(LinkToCut[i].ToList());
                TroubleCode0.Add(CFTypeCode[2]);
                WMNode0[LinkToCut[i].ElementAt(0) - 1, LinkToCut[i].ElementAt(1) - 1] = 0;
                WMNode0[LinkToCut[i].ElementAt(1) - 1, LinkToCut[i].ElementAt(0) - 1] = 0;
            }
            PCCDNodeGroup(WMNode0, VerticeMatrix, SizeLimit, CFTypeCode, out WMNode, out TroubleNodeList, out TroubleCode);
            TroubleNodeList.AddRange(TroubleNodeList0.ToList());
            TroubleCode.AddRange(TroubleCode0.ToList());

        }


        // Find the difference between two sets of data
        public static void PCCDSetMinus(List<int> A, List<int> B, out List<int> A_B)
        {
            A_B = new List<int>();
            for (int i = 0; i < A.Count; i++)
            {
                for (int j = 0; j < B.Count; j++)
                {
                    if (A[i] == B[j])
                    {
                        A[i] = 0;
                    }
                }
            }
            for (int i = 0; i < A.Count; i++)
            {
                if (A[i] != 0)
                { A_B.Add(A[i]); }
            }
        }

        // find the row of a specific node in the NodeGroups
        public static int PCCDFindRow(List<List<int>> NodeGroups, List<int> NodePair)
        {
            int x = 0;
            for (int i = 0; i < NodeGroups.Count; i++)
            {
                for (int j = 0; j < NodeGroups[i].Count; j++)
                {
                    if (NodeGroups[i].ElementAt(j) == NodePair[0])
                    {
                        x = i;
                        break;
                    }
                }
            }
            return x;
        }


        // Sort the links according to the weight of each link
        public static void PCCDSort(double[,] WeightMatrix, List<int> SingleNodeGroup, out List<List<int>> EdgeRank)
        {

            EdgeRank = new List<List<int>>();
            List<int> sublist = new List<int>();
            List<double> Weight = new List<double>();
            double Temp = 0;
            for (int i = 0; i < SingleNodeGroup.Count; i++)
            {
                for (int j = 0; j < SingleNodeGroup.Count; j++)
                {
                    if (j != i && WeightMatrix[SingleNodeGroup[i] - 1, SingleNodeGroup[j] - 1] != 0)
                    {
                        sublist.Add(SingleNodeGroup[i]);
                        sublist.Add(SingleNodeGroup[j]);
                        EdgeRank.Add(sublist.ToList());
                        Weight.Add(WeightMatrix[SingleNodeGroup[i] - 1, SingleNodeGroup[j] - 1]);
                        sublist.Clear();
                    }
                }
            }
            for (int i = 0; i < EdgeRank.Count - 1; i++)
            {
                for (int j = 0; j < EdgeRank.Count - i - 1; j++)
                {
                    if (Weight[j + 1] < Weight[j])
                    {
                        sublist = EdgeRank[j].ToList();
                        EdgeRank[j] = EdgeRank[j + 1].ToList();
                        EdgeRank[j + 1] = sublist.ToList();
                        Temp = Weight[j];
                        Weight[j] = Weight[j + 1];
                        Weight[j + 1] = Temp;

                    }
                }
            }
        }

        // chain graph into groups

        public static void PCCDChain(double[,] WeightMatrix, out List<List<int>> NodeGroups)
        {
            int NumOfNode = WeightMatrix.GetLength(0);
            NodeGroups = new List<List<int>>();
            List<int> SubList = new List<int>();
            List<int> TempSet = new List<int>();
            int Flag = 1;
            int T1 = 0;
            int T2 = 0;
            for (int i = 0; i < NumOfNode; i++)
            {
                for (int j = i + 1; j < NumOfNode; j++)
                {
                    if (WeightMatrix[i, j] != 0)
                    {
                        if (Flag == 1)
                        {
                            TempSet.Add(i + 1);
                            TempSet.Add(j + 1);
                            SubList = TempSet.ToList();
                            NodeGroups.Add(SubList.ToList());
                            TempSet.Clear();
                        }
                        if (!PCCDChain_SubRutine(SubList, i + 1) && !PCCDChain_SubRutine(SubList, j + 1))
                        {// both i, j are not marked
                            TempSet.Add(i + 1);
                            TempSet.Add(j + 1);
                            SubList.Add(i + 1);
                            SubList.Add(j + 1);
                            NodeGroups.Add(TempSet.ToList());
                            TempSet.Clear();
                        }
                        else if (PCCDChain_SubRutine(SubList, i + 1) && PCCDChain_SubRutine(SubList, j + 1))
                        {
                            // both are marked
                            for (int k = 0; k < NodeGroups.Count; k++)
                            {
                                for (int x = 0; x < NodeGroups[k].Count; x++)
                                {
                                    if (NodeGroups[k].ElementAt(x) == i + 1)
                                    { T1 = k; }
                                    if (NodeGroups[k].ElementAt(x) == j + 1)
                                    { T2 = k; }
                                }
                            }
                            if (T1 != T2)
                            {
                                if (T1 < T2)
                                {
                                    NodeGroups[T1].AddRange(NodeGroups[T2].ToList());
                                    NodeGroups.RemoveAt(T2);
                                }
                            }
                            Flag = 0;
                        }
                        else
                        {
                            // one of i, j is marked but the other is not
                            for (int k = 0; k < NodeGroups.Count; k++)
                            {
                                for (int x = 0; x < NodeGroups[k].Count; x++)
                                {
                                    if (NodeGroups[k].ElementAt(x) == i + 1)
                                    {
                                        SubList.Add(j + 1);
                                        NodeGroups[k].Add(j + 1);
                                        break;
                                    }
                                    if (NodeGroups[k].ElementAt(x) == j + 1)
                                    {
                                        SubList.Add(i + 1);
                                        NodeGroups[k].Add(i + 1);
                                        break;
                                    }

                                }
                            }
                            Flag = 0;
                        }
                    }
                }
            }
            // Obtain the isolated nodes and combine them with the existing NodeGroups
            for (int i = 0; i < NumOfNode; i++)
            {
                int FlagRow = 1;
                int FlagCollum = 1;
                for (int j = i + 1; j < NumOfNode; j++)
                {
                    if (WeightMatrix[i, j] != 0)
                    { FlagRow = 0; }
                }
                for (int j = 0; j < i; j++)
                {
                    if (WeightMatrix[j, i] != 0)
                    { FlagCollum = 0; }
                }
                if (FlagCollum * FlagRow == 1)
                {
                    TempSet.Add(i + 1);
                    NodeGroups.Add(TempSet.ToList());
                    TempSet.Clear();
                }
            }
            for (int i = 0; i < NodeGroups.Count; i++)
            {
                NodeGroups[i].Sort();
            }

        }

        public static bool PCCDChain_SubRutine(List<int> Sublist, int x)
        {
            // find out if x is in the list
            bool result = false;

            for (int i = 0; i < Sublist.Count; i++)
            {
                if (Sublist[i] - x == 0)
                {
                    result = true;
                }
            }
            return result;

        }

        // Calculate the bounding box of any node combinations
       /* public static void PCCDStackUpSize(List<List<int>> NodeGroups, double[,] VerticeMatrix, out List<List<double>> StackUpSize)
        {
            // for test purpose only
            int NumOfGroup = NodeGroups.Count;
            StackUpSize = new List<List<double>>(NumOfGroup);
            List<double> SubList = new List<double>();
            double[] SizeSum = new double[3];
            for (int i = 0; i < NumOfGroup; i++)
            {
                for (int j = 0; j < NodeGroups[i].Count; j++)
                {
                    SizeSum[0] = SizeSum[0] + VerticeMatrix[NodeGroups[i].ElementAt(j) - 1, 1];
                    SizeSum[1] = SizeSum[1] + VerticeMatrix[NodeGroups[i].ElementAt(j) - 1, 2];
                    SizeSum[2] = SizeSum[2] + VerticeMatrix[NodeGroups[i].ElementAt(j) - 1, 3];
                }
                SubList = SizeSum.ToList();
                StackUpSize.Add(SubList.ToList());
                SubList.Clear();
                Array.Clear(SizeSum, 0, 3);
            }
        }*/
        /// <summary>
        ///  instantly communicate with rhino
        /// </summary>
        /// <param name="NodeGroups"></param>
        /// <param name="VerticeMatrix"></param>
        /// <param name="StackUpSize"></param>
        public static void PCCDStackUpSize(List<List<int>> NodeGroups, List<ComponentStruct> VerticeMatrix,  out List<List<double>> StackUpSize)
        {
            int NumOfGroup = NodeGroups.Count;
            StackUpSize = new List<List<double>>(NumOfGroup);
            List<double> SubList = new List<double>();
            double[] SizeSum = new double[3];
            //Product pd = PCCDMainEntrance.result;
            BoundingBox bx;
           
            for (int i = 0; i < NumOfGroup; i++)
            {
                bx = new BoundingBox();
                for (int j = 0; j < NodeGroups[i].Count; j++)
                {
                    bx = BoundingBox.Union(VerticeMatrix[NodeGroups[i].ElementAt(j)-1].bdBox, bx);
                   // bx = BoundingBox.Intersection(pd.parts[NodeGroups[i].ElementAt(j) - 1].bdBox, bx);

                }
                SizeSum[0] = bx.Diagonal.X;
                SizeSum[1] = bx.Diagonal.Y;
                SizeSum[2] = bx.Diagonal.Z;
                SubList = SizeSum.ToList();
                SubList.Sort();
                StackUpSize.Add(SubList.ToList());
                SubList.Clear();
                Array.Clear(SizeSum, 0, 3);
            }
            
        }


    }
}
