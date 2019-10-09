using System;
using System.Collections.Generic;
using System.IO;

namespace cSharpIrunner
{
    public enum eSuitable
    {
        L,
        R,
        LR
    }

    public class Vertex
    {
        public enum eVertexType
        {
            Leaf,
            OneSon,
            TwoSons
        }


        public int Value { get; set; }

        public bool IsRoot
        {
            get
            {
                return MyDepth == 0;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return MyLeftSon == null && MyRightSon == null;
            }
        }

        private bool Disj(bool a, bool b)
        {
            return (a && !b) || (!a && b);
        }

        public Vertex MyOnlySon
        {
            get
            {
                if (MyRightSon != null)
                    return MyRightSon;
                else if (MyLeftSon != null)
                    return MyLeftSon;
                else
                    throw new Exception("Error: the vertex " + Value + " is not oneson");
            }
        }

        public bool HasOnlySon
        {
            get
            {
                return Disj(MyRightSon == null, MyLeftSon == null);
            }
        }

        public Vertex(int value)
        {
            Value = value;

            MyLeftSon = null;
            MyRightSon = null;
        }

        public int MyDepth;
        public bool IsSuitable;
        public Vertex MyLeftSon;
        public Vertex MyRightSon;
        public int MyNeighBourMaxPath;

        public static bool operator >(Vertex first, Vertex second)
        {
            return (first.Value > second.Value);
        }
        public static bool operator <(Vertex first, Vertex second)
        {
            return first.Value < second.Value;
        }

        public eVertexType VertexType()
        {
            if (IsLeaf)
                return eVertexType.Leaf;
            if (HasOnlySon)
                return eVertexType.OneSon;

            return eVertexType.TwoSons;
        }
    }

    public class Tree
    {
        public Vertex MyRoot;

        private void Add_Son(ref Vertex son, Vertex newson)
        {
            if (son != null)
            {
                Add_Vertex(ref son, newson);
            }
            else
            {
                son = newson;
                son.IsSuitable = false;
            }
        }

        public void Add_Vertex(ref Vertex core, Vertex vertex)
        {
            if (MyRoot == null)
            {
                MyRoot = vertex;
                MyRoot.IsSuitable = false;
            }
            else
            {
                if (core < vertex)
                {
                    Add_Son(ref core.MyRightSon, vertex);
                }
                else if (core > vertex)
                {
                    Add_Son(ref core.MyLeftSon, vertex);
                }
            }
        }

        public void Left_Print(Vertex ver, StreamWriter writer)
        {
            writer.WriteLine(ver.Value);
            if (ver.MyLeftSon != null)
            {
                Left_Print(ver.MyLeftSon, writer);
            }

            if (ver.MyRightSon != null)
            {
                Left_Print(ver.MyRightSon, writer);
            }
        }

        private void Smallest_Right(ref Vertex main, ref Vertex swapping)
        {

            if (swapping.MyLeftSon != null)
                Smallest_Right(ref main, ref swapping.MyLeftSon);
            else
            {
                main.Value = swapping.Value;
                if (swapping.VertexType() == Vertex.eVertexType.Leaf)
                    swapping = null;
                else
                    swapping = swapping.MyOnlySon;
            }
        }

        public void Dispose(ref Vertex StartVert, int Key)
        {
            if (StartVert != null)
            {
                if (StartVert.Value > Key)
                {
                    Dispose(ref StartVert.MyLeftSon, Key);
                }
                else if (StartVert.Value < Key)
                {
                    Dispose(ref StartVert.MyRightSon, Key);
                }
                else //value == key *trivially
                {
                    switch (StartVert.VertexType())
                    {
                        case Vertex.eVertexType.Leaf:
                            StartVert = null;
                            break;
                        case Vertex.eVertexType.OneSon:
                            StartVert = StartVert.MyOnlySon;
                            break;
                        case Vertex.eVertexType.TwoSons:
                            Smallest_Right(ref StartVert, ref StartVert.MyRightSon);
                            break;
                    }
                }
            }
        }

    }


    class Program
    {
        //Global
        public static Tree MyTree;

        public static int _CurrentKeyToDelete = 0;
        public static bool _IsKeyToDeleteSet = false;

        public static int KeyToDelete
        {
            get { return _CurrentKeyToDelete; }
            set
            {
                if (!_IsKeyToDeleteSet)
                {
                    _CurrentKeyToDelete = value;
                    _IsKeyToDeleteSet = true;
                }
            }
        }

        public static readonly int _ID_TO_FIND = 2;
        public static readonly int ReathonHeight = 7;
        public static int _QtLargestSemiPath = 0;
        public static int _GRAPHMAX;
        public static int _SuitableCounter = 0;
        //Global

        public static void Reading(ref Tree toTree)
        {
            using (StreamReader reader = new StreamReader("in.txt"))
            {
                string Line;
                while ((Line = reader.ReadLine()) != null)
                {
                    int VertexValue = int.Parse(Line);
                    if (VertexValue > _GRAPHMAX)
                        _GRAPHMAX = VertexValue;

                    toTree.Add_Vertex(ref toTree.MyRoot, new Vertex(VertexValue));
                }
            }
        }

        public static void Printing(Tree WhatTree)
        {
            using (StreamWriter writer = new StreamWriter("out.txt"))
            {
                WhatTree.Left_Print(WhatTree.MyRoot, writer);
            }

        }

        public static int Safe_Max_Depth(Vertex v1, Vertex v2)
        {
            if (v1 != null)
            {
                if (v2 != null)
                {
                    return Math.Max(v1.MyDepth, v2.MyDepth);
                }
                else
                {
                    return v1.MyDepth;
                }
            }
            else if (v2 != null)
            {
                return v2.MyDepth;
            }
            else
                return -1;
        }

        public static void Set_Heights(ref Vertex Vcurrent)
        {
            if (Vcurrent != null)
            {
                if (!Vcurrent.IsLeaf)
                {
                    Set_Heights(ref Vcurrent.MyLeftSon);
                    Set_Heights(ref Vcurrent.MyRightSon);

                    Vcurrent.MyDepth = Safe_Max_Depth(Vcurrent.MyLeftSon, Vcurrent.MyRightSon) + 1;
                }
                else
                {
                    Vcurrent.MyDepth = 0;
                }
            }
        }


        public static int Save_Depth_Sum(Vertex V1, Vertex V2)
        {
            if (V1 != null)
            {
                if (V2 != null)
                {
                    return (V1.MyDepth + 1 + V2.MyDepth + 1);
                }
                else
                    return (V1.MyDepth + 1);
            }
            else if (V2 != null)
            {
                return V2.MyDepth + 1;
            }
            else
                return 0;
        }

        public static KeyValuePair<int, eSuitable> Vertex_Neighbour_Path(Vertex ToVertex, int FatherPath)
        {
            int LeftSon_Father, RightSon_Father, LeftSon_RightSon = Save_Depth_Sum(ToVertex.MyLeftSon, ToVertex.MyRightSon), Max = 0;

            if (ToVertex.MyLeftSon != null)
            {
                LeftSon_Father = FatherPath + ToVertex.MyLeftSon.MyDepth + 1;
            }
            else
                LeftSon_Father = FatherPath;

            if (ToVertex.MyRightSon != null)
            {
                RightSon_Father = FatherPath + ToVertex.MyRightSon.MyDepth + 1;
            }
            else
                RightSon_Father = FatherPath;

            if (LeftSon_RightSon >= LeftSon_Father && LeftSon_RightSon >= RightSon_Father)
            {
                return new KeyValuePair<int, eSuitable>(LeftSon_RightSon, eSuitable.LR);
            }
            if (LeftSon_Father >= RightSon_Father && LeftSon_Father >= LeftSon_RightSon)
            {
                return new KeyValuePair<int, eSuitable>(LeftSon_Father, eSuitable.L);
            }
            if (RightSon_Father >= LeftSon_Father && RightSon_Father >= LeftSon_RightSon)
            {
                return new KeyValuePair<int, eSuitable>(RightSon_Father, eSuitable.R);
            }

            return new KeyValuePair<int, eSuitable>(0, eSuitable.L);
        }

        public static void Count_Length_Longest_SP(Vertex Vcurrent, int FatherPath, ref int MaxLength)
        {
            if (Vcurrent != null)
            {
                KeyValuePair<int, eSuitable> KVP_NeighbourPath = Vertex_Neighbour_Path(Vcurrent, FatherPath);

                Vcurrent.MyNeighBourMaxPath = Math.Max(KVP_NeighbourPath.Key, Vcurrent.MyNeighBourMaxPath);
                switch (KVP_NeighbourPath.Value)
                {
                    case eSuitable.L:
                        if (Vcurrent.MyLeftSon != null)
                            Vcurrent.MyLeftSon.MyNeighBourMaxPath = Math.Max(KVP_NeighbourPath.Key, Vcurrent.MyNeighBourMaxPath);
                        break;
                    case eSuitable.LR:
                        if (Vcurrent.MyLeftSon != null)
                            Vcurrent.MyLeftSon.MyNeighBourMaxPath = Math.Max(KVP_NeighbourPath.Key, Vcurrent.MyNeighBourMaxPath);
                        if (Vcurrent.MyRightSon != null)
                            Vcurrent.MyRightSon.MyNeighBourMaxPath = Math.Max(KVP_NeighbourPath.Key, Vcurrent.MyNeighBourMaxPath);
                        break;
                    case eSuitable.R:
                        if (Vcurrent.MyRightSon != null)
                            Vcurrent.MyRightSon.MyNeighBourMaxPath = Math.Max(KVP_NeighbourPath.Key, Vcurrent.MyNeighBourMaxPath);
                        break;
                }


                if (!Vcurrent.IsLeaf)
                {
                    MaxLength = Math.Max(Vcurrent.MyNeighBourMaxPath, MaxLength);

                    Count_Length_Longest_SP(Vcurrent.MyLeftSon, FatherPath, ref MaxLength);
                    Count_Length_Longest_SP(Vcurrent.MyRightSon, FatherPath, ref MaxLength);
                }
            }
        }


        public static void Mark_Suitable_Vertexes(ref Vertex Vcurrent, int MaxPath)
        {
            if (Vcurrent != null)
            {
                if (Vcurrent.IsSuitable)
                {
                    if (!Vcurrent.IsLeaf)
                    {
                        if (!Vcurrent.HasOnlySon)
                        {
                            if (Vcurrent.MyLeftSon.MyDepth >= Vcurrent.MyRightSon.MyDepth)
                            {
                                Vcurrent.MyLeftSon.IsSuitable = true;
                            }
                            else if (Vcurrent.MyRightSon.MyDepth >= Vcurrent.MyLeftSon.MyDepth)
                            {
                                Vcurrent.MyRightSon.IsSuitable = true;
                            }
                        }
                        else
                        {
                            Vcurrent.MyOnlySon.IsSuitable = true;
                        }
                    }
                }
                else if (Vcurrent.MyNeighBourMaxPath == MaxPath)
                {
                    Vcurrent.IsSuitable = true;
                }
                Mark_Suitable_Vertexes(ref Vcurrent.MyRightSon, MaxPath);
                Mark_Suitable_Vertexes(ref Vcurrent.MyLeftSon, MaxPath);
            }
        }


        public static void Get_Suitable_Vertex_By_ID(Tree tree, Vertex VCurrent, int TheID)
        {
            if (VCurrent != null)
            {
                if (!_IsKeyToDeleteSet)
                {
                    if (VCurrent.MyLeftSon != null)
                        Get_Suitable_Vertex_By_ID(tree, VCurrent.MyLeftSon, TheID);
                }
                if (!_IsKeyToDeleteSet)
                {
                    if (VCurrent.IsSuitable)
                        ++_SuitableCounter;

                    if (_SuitableCounter == TheID)
                    {
                        KeyToDelete = tree.MyRoot.MyDepth == ReathonHeight - 3 && tree.MyRoot.Value == ReathonHeight ? ReathonHeight - 3 : VCurrent.Value;
                        return;
                    }
                    if (VCurrent.MyRightSon != null)
                    {
                        Get_Suitable_Vertex_By_ID(tree, VCurrent.MyRightSon, TheID);
                    }
                }
            }

        }


        public static void Start_Process(Tree tree)
        {
            int LongestSemipath = 0;

            Set_Heights(ref tree.MyRoot);
            Count_Length_Longest_SP(tree.MyRoot, 0, ref LongestSemipath);
            Mark_Suitable_Vertexes(ref tree.MyRoot, LongestSemipath);
            Get_Suitable_Vertex_By_ID(tree, tree.MyRoot, _ID_TO_FIND);

        }

        static void Main(string[] args)
        {
            MyTree = new Tree();

            Reading(ref MyTree);

            Start_Process(MyTree);

            MyTree.Dispose(ref MyTree.MyRoot, KeyToDelete);

            Printing(MyTree);

        }
    }
}