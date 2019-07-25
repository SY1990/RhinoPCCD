using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoliDesign.Functions
{
    /// <summary>
    /// module division function
    /// GA Girven-Newman method
    /// </summary>
    public class GAModuleDiv
    {
        public GAModuleDiv()
        {
        }

        // Find the best module division result through multiple iterations
        public static void FindBestSolution_Multiple(double[,] SymmWeightMatrix, int NumOfPopulation, int NumOfGeneration, double CrossOverRatio, double MutationRatio,
            out List<List<int>> Modules, out double[] BestFitness)
        {
            int NumOfIteration = 5;
            BestFitness = new double[NumOfGeneration];
            Modules = new List<List<int>>();

            for (int i = 0; i < NumOfIteration; i++)
            {
                double[] Fitness = new double[NumOfGeneration];
                List<List<int>> Temp_Modules = new List<List<int>>();
                GAModuleDiv.FindModule(NumOfPopulation, NumOfGeneration, CrossOverRatio, MutationRatio, SymmWeightMatrix, out Temp_Modules, out Fitness);
                if (i > 0)
                {
                    if (BestFitness.Max() < Fitness.Max())
                    {
                        Modules = Temp_Modules.ToList();
                        Array.Copy(Fitness, BestFitness, Fitness.Length);
                    }

                }
                else
                {
                    Modules = Temp_Modules.ToList();
                    Array.Copy(Fitness, BestFitness, Fitness.Length);
                }

                /* foreach (List<int> x in Modules)
                 {
                     foreach (int xx in x)
                     {
                         Console.Write(" ");
                         Console.Write(string.Join(" ", xx.ToString()));
                     }
                     Console.WriteLine("\n");
                 }

                 Console.WriteLine(BestFitness.Max().ToString());
                 Console.WriteLine("\n");
                 
                */
            }
        }

        // find the optimal module division result
        public static void FindModule(int NumOfPopulation, int NumOfGeneration, double CrossOverRatio, double MutationRatio, double[,] WeightMatrix, out List<List<int>> Modules, out double[] BestFitness)
        {
            int NumOfNode = WeightMatrix.GetLength(0);
            Modules = new List<List<int>>();
            BestFitness = new double[NumOfGeneration];
            int[] BestGene = new int[NumOfNode]; // the best gene in one generation
            double MaxFitness = 0;
            int[] MaxGene = new int[NumOfNode]; // the max gene in one generation
            int[,] InitialPop;
            int[,] OldPop = new int[NumOfPopulation, NumOfNode];
            int[,] NewPop = new int[NumOfPopulation, NumOfNode];
            int[][] AdjacentNodes;
            List<List<int>> Community = new List<List<int>>();
            double[] Fitness;
            double[] OldFitness = new double[NumOfPopulation];
            double[] NewFitness = new double[NumOfPopulation];
            double[] AveFitness = new double[NumOfGeneration];
            double[] Temp_NormalizeFitness = new double[NumOfPopulation];
            double[] Temp_Fitness = new double[NumOfPopulation];
            int SelectionFather = 0, SelectionMother = 0, CrossOverPosition = 0, MutationPosition = 0;
            double Temp_sum;

            int NumMax = 0;
            List<int> RowOfMax = new List<int>();

            // Initialize the population
            GAModuleDiv.InitializePopulation(NumOfNode, NumOfPopulation, WeightMatrix, out  InitialPop, out  AdjacentNodes);
            GAModuleDiv.FitnessCalAll(InitialPop, AdjacentNodes, WeightMatrix, out Fitness);
            Array.Copy(InitialPop, OldPop, NumOfPopulation * NumOfNode);
            Array.Copy(Fitness, OldFitness, Fitness.Length);
            Random rnd = new Random();
            //find the best solution
            for (int i = 0; i < NumOfGeneration; i++)
            {
                Temp_sum = 0;
                MaxFitness = OldFitness.Max();
                AveFitness[i] = OldFitness.Sum() / NumOfPopulation;
                RowOfMax.Clear();
                // find the row of the max
                for (int j = 0; j < NumOfPopulation; j++)
                {
                    if (MaxFitness == OldFitness[j])
                    {
                        RowOfMax.Add(j);
                    }
                }
                for (int k = 0; k < OldPop.GetLength(1); k++)
                {
                    MaxGene[k] = OldPop[RowOfMax[0], k];
                }
                // copy the best gene to the first few rows
                if (i > 0)
                {
                    if (MaxFitness > BestFitness[i - 1])
                    {
                        BestFitness[i] = MaxFitness;
                        Array.Copy(MaxGene, BestGene, NumOfNode);
                        // copy the best rows
                        /* for (int k = 0; k < RowOfMax.Count; k++)
                         {
                             for (int x = 0; x < NumOfNode; x++)
                             {
                                 NewPop[k, x] = OldPop[RowOfMax[k], x];
                             }
                         }
                         NumMax = RowOfMax.Count;
                         * */
                        for (int x = 0; x < NumOfNode; x++)
                        {
                            NewPop[0, x] = OldPop[RowOfMax[0], x];
                        }
                        NumMax = 1;
                    }
                    else
                    {
                        BestFitness[i] = BestFitness[i - 1];

                        for (int x = 0; x < NumOfNode; x++)
                        {
                            NewPop[0, x] = OldPop[RowOfMax[0], x];
                        }
                        NumMax = 1;


                    }
                }
                else
                {
                    BestFitness[i] = MaxFitness;
                    Array.Copy(MaxGene, BestGene, NumOfNode);
                    // copy the best rows
                    /*for (int k = 0; k < RowOfMax.Count; k++)
                    {
                        for (int x = 0; x < NumOfNode; x++)
                        {
                            NewPop[k, x] = OldPop[RowOfMax[k], x];
                        }
                    }
                    NumMax = RowOfMax.Count;*/
                    for (int x = 0; x < NumOfNode; x++)
                    {
                        NewPop[0, x] = OldPop[RowOfMax[0], x];
                    }
                    NumMax = 1;
                }

                // selection, normalization
                double[] Temp_NormalizeFitness1 = new double[NumOfPopulation];
                for (int k = 0; k < NumOfPopulation; k++)
                {
                    Temp_NormalizeFitness1[k] = OldFitness[k] + Math.Abs(OldFitness.Min()) + 0.01;
                }
                for (int k = 0; k < NumOfPopulation; k++)
                {
                    Temp_NormalizeFitness[k] = Temp_NormalizeFitness1[k] / Temp_NormalizeFitness1.Sum();
                }



                for (int k = 0; k < NumOfPopulation; k++)
                {
                    Temp_sum = Temp_sum + Temp_NormalizeFitness[k];
                    Temp_Fitness[k] = Temp_sum;
                }


                for (int j = NumMax; j < NumOfPopulation; j++)
                {

                    double Delta = rnd.NextDouble();
                    for (int k = 0; k < NumOfPopulation; k++)
                    {
                        if (Delta < Temp_Fitness[k])
                        {
                            for (int x = 0; x < NumOfNode; x++)
                            {
                                NewPop[j, x] = OldPop[k, x];
                            }
                            SelectionFather = k;
                            break;

                        }
                    }
                    ////////////////////////check NewPop  from above to find why it does not work////////////////

                    // crossover

                    SelectionMother = rnd.Next(0, NumOfPopulation);
                    CrossOverPosition = rnd.Next(0, NumOfNode);
                    double Delta2 = rnd.NextDouble();
                    if (Delta2 <= CrossOverRatio)
                    {
                        for (int k = 0; k < CrossOverPosition; k++)
                        {
                            NewPop[j, k] = OldPop[SelectionFather, k];
                        }

                        for (int k = CrossOverPosition; k < NumOfNode; k++)
                        {
                            NewPop[j, k] = OldPop[SelectionMother, k];
                        }
                    }
                    // mutation

                    double Delta3 = rnd.NextDouble();
                    MutationPosition = rnd.Next(0, NumOfNode);
                    if (Delta3 <= MutationRatio)
                    {
                        NewPop[j, MutationPosition] = AdjacentNodes[MutationPosition][rnd.Next(0, AdjacentNodes[MutationPosition].GetLength(0))];

                    }
                }
                GAModuleDiv.FitnessCalAll(NewPop, AdjacentNodes, WeightMatrix, out NewFitness);
                Array.Copy(NewFitness, OldFitness, NumOfPopulation);
                Array.Copy(NewPop, OldPop, NumOfPopulation * NumOfNode);
                Array.Clear(NewPop, 0, NumOfPopulation * NumOfNode);
                Array.Clear(NewFitness, 0, NumOfPopulation);
            }

            GAModuleDiv.TransToCommunity(NumOfNode, BestGene, out Modules);
        }

        // initialize population
        public static void InitializePopulation(int NumOfNode, int NumOfPopulation, double[,] WeightMatrix, out int[,] InitialPop, out int[][] AdjacentNodes)
        {
            AdjacentNodes = new int[NumOfNode][];
            InitialPop = new int[NumOfPopulation, NumOfNode];
            int Member = 0;
            Random rnd = new Random(DateTime.Now.Millisecond);



            for (int i = 0; i < NumOfNode; i++)
            {
                int size = 0;
                for (int j = 0; j < NumOfNode; j++)
                {
                    if (WeightMatrix[i, j] != 0)
                    {
                        size++;
                    }
                }
                AdjacentNodes[i] = new int[size];

            }

            for (int i = 0; i < NumOfNode; i++)
            {
                int x = 0;
                for (int j = 0; j < NumOfNode; j++)
                {
                    if (WeightMatrix[i, j] != 0)
                    {
                        AdjacentNodes[i][x] = j + 1; // Adjacent nodes have to be self-inclusive.
                        x++;
                    }
                }
            }
            for (int i = 0; i < NumOfPopulation; i++)
            {
                for (int j = 0; j < NumOfNode; j++)
                {
                    Member = AdjacentNodes[j].Length;
                    InitialPop[i, j] = AdjacentNodes[j][rnd.Next(0, Member)];

                }

            }
        }
        // calculate fitness of an array

        public static void FitnessCalAll(int[,] InitialPop, int[][] AdjacentNodes, double[,] WeightMatrix, out double[] Fitness)
        {
            int NumOfNode = InitialPop.GetLength(1);
            int NumOfPopulation = InitialPop.GetLength(0);

            double fitness;
            Fitness = new double[NumOfPopulation];
            List<List<int>> Community = new List<List<int>>();
            int[] SingleGene = new int[NumOfNode];

            for (int i = 0; i < NumOfPopulation; i++)
            {
                for (int j = 0; j < NumOfNode; j++)
                {
                    SingleGene[j] = InitialPop[i, j];
                }
                TransToCommunity(NumOfNode, SingleGene, out Community);
                FitnessCal(Community, WeightMatrix, out fitness);
                Fitness[i] = fitness;
            }
        }

        // calculate fitness
        public static void FitnessCal(List<List<int>> Community, double[,] WeightMatrix, out double Fitness)
        {
            int NumOfCommunity = Community.Count;
            int NumOfNode = WeightMatrix.GetLength(0);
            double[] IntraCommunity = new double[NumOfCommunity];
            double[] InterCommunity = new double[NumOfCommunity];
            double MValue = 0;
            double[] KDegree = new double[NumOfNode];
            Fitness = 0;

            // calculate M value
            for (int i = 0; i < NumOfNode; i++)
            {
                for (int j = 0; j < NumOfNode; j++)
                {
                    MValue = MValue + WeightMatrix[i, j];
                    KDegree[i] = KDegree[i] + WeightMatrix[i, j];
                }
                MValue = MValue - WeightMatrix[i, i];
                KDegree[i] = KDegree[i] - WeightMatrix[i, i];
            }
            MValue = 0.5 * MValue;


            // calculate intra community strength
            // calculate inter community strength
            for (int i = 0; i < NumOfCommunity; i++)
            {
                int MemberSize = Community[i].Count;
                for (int j = 0; j < MemberSize; j++)
                {
                    for (int k = 0; k < MemberSize; k++)
                    {
                        if (j != k)
                        {
                            if (WeightMatrix[Community[i].ElementAt(j) - 1, Community[i].ElementAt(k) - 1] != 0)
                            {
                                IntraCommunity[i] = IntraCommunity[i] + WeightMatrix[Community[i].ElementAt(j) - 1, Community[i].ElementAt(k) - 1];
                            }
                        }
                    }

                    InterCommunity[i] = InterCommunity[i] + KDegree[Community[i].ElementAt(j) - 1] / (2 * MValue);
                }
                IntraCommunity[i] = IntraCommunity[i] / (2 * MValue);
                InterCommunity[i] = InterCommunity[i] * InterCommunity[i];

            }
            // calculate fitness
            for (int i = 0; i < NumOfCommunity; i++)
            {
                Fitness = Fitness + IntraCommunity[i] - InterCommunity[i];
            }
        }

        // convert one gene to community
        public static void TransToCommunity(int NumOfNode, int[] SingleGene, out List<List<int>> Community)
        {
            Community = new List<List<int>>(NumOfNode);
            List<int> sublist = new List<int>();
            // initialize community

            int[] TempGene = new int[NumOfNode];
            Array.Copy(SingleGene, TempGene, NumOfNode);
            int j = 0;
            int CommunityID = 0;
            //int CommunityMemberIndex = 0;
            int[] TempCommunityID = new int[NumOfNode];
            //int Temp_ID = 0;
            for (int i = 0; i < NumOfNode; i++)
            {
                TempCommunityID[i] = 0; // for marking purpose
            }

            for (int i = 1; i < NumOfNode + 1; i++)
            {
                j = i;
                int TempVariable = 0;
                sublist.Clear();
                if (TempGene[j - 1] != j)
                {
                    if (TempGene[j - 1] != 0)
                    {
                        CommunityID++;
                        sublist.Add(j);
                        Community.Add(sublist.ToList());
                        sublist.Clear();

                        while (TempVariable == 0)
                        {
                            int Temp_j = TempGene[j - 1];
                            TempGene[j - 1] = 0;
                            TempCommunityID[j - 1] = CommunityID;
                            //Temp_ID = j;
                            j = Temp_j;
                            // Compare j with the rest members in the same community CommunityID
                            for (int k = 0; k < Community[CommunityID - 1].Count; k++)
                            {
                                if (j != Community[CommunityID - 1].ElementAt(k))
                                {
                                    TempVariable = 0;
                                }
                                else
                                {
                                    TempVariable = 1;
                                    break;
                                }
                            }

                            if (TempVariable == 0 && TempGene[j - 1] != 0)
                            {
                                sublist.Add(j);
                                Community[CommunityID - 1].AddRange(sublist.ToList());
                                sublist.Clear();

                            }
                            else if (TempVariable == 0 && TempGene[j - 1] == 0)
                            {
                                int[] Temp_Collumn = new int[Community[CommunityID - 1].Count];
                                // copy existing members to the new community
                                Community[CommunityID - 1].CopyTo(Temp_Collumn);

                                Community[CommunityID - 1].Clear();
                                Community.RemoveAt(CommunityID - 1);
                                CommunityID = TempCommunityID[j - 1];
                                for (int x = 0; x < Temp_Collumn.Length; x++)
                                {
                                    // clear all the members in current Community[CommunityID]

                                    TempCommunityID[Temp_Collumn[x] - 1] = CommunityID;
                                }


                                Community[CommunityID - 1].AddRange(Temp_Collumn.ToList());
                                //Community.Insert(CommunityID-1, Temp_Collumn.ToList());
                                CommunityID = TempCommunityID.Max();
                                break;
                            }

                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {

                    /*if (Community[CommunityID].Count != 0)
                    {
                        sublist = Community[CommunityID].ToList();
                        sublist.Add(j);
                    }
                    else
                    {
                        sublist.Add(j);
                    }*/
                    CommunityID++;
                    sublist.Add(j);
                    Community.Add(sublist.ToList());


                    sublist.Clear();


                    TempCommunityID[j - 1] = CommunityID;


                    TempGene[j - 1] = 0;
                    continue;
                }

            }
            // delete empty communities

        }

        // convert upper matrix to symmetric matrix
        public static void TransferMatrixToSym(double[,] WeightMatrix, out double[,] SymmWeightMatrix)
        {
            int NumOfNode = WeightMatrix.GetLength(0);
            SymmWeightMatrix = new double[NumOfNode, NumOfNode];
            for (int i = 0; i < NumOfNode; i++)
            {
                for (int j = 0; j < NumOfNode; j++)
                {
                    if (WeightMatrix[i, j] != 0)
                    {
                        SymmWeightMatrix[i, j] = WeightMatrix[i, j];
                        SymmWeightMatrix[j, i] = WeightMatrix[i, j];
                    }
                }
            }
        }
    }
}
