using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoliDesign.Data
{
    /// <summary>
    /// This defines the data structure of the overall data in this project
    /// </summary>
    [XmlRoot]
    public class Product
    {
        public string Name { get; set; }
        public Guid guid {get;set;}
        
       // public string Function { get; set; }
        public string filepath { get; set; }
        public List<Part> parts { get; set; }
    }

    
    public class Part
    {
        public Guid guid { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string material { get; set; }
        public string MaintenanceFreq { get; set; }
        public string IsStandard { get; set; }
        public List<double> BoundingBox{ get; set; }
        //public int[] Color = new int[3];
        public string Function { get; set; }
        public List<int> JointComponent { get; set; }
        public List<string> ConnectionInfo { get; set; }
        public List<double> Weight { get; set; }
        public List<int> RelativeMotion { get; set; }
        public BoundingBox bdBox { get; set; }
        //public Brep GeoBrep { get; set; }
    }


   /* public class ComponentsInfo
    {
        public List<ComponentStruct> Parts { get; set; }
    }*/
    public class ComponentStruct
    {
        public int material { get; set; }
        public BoundingBox bdBox { get; set; }
        public int IsStandard { get; set; }
        public int MaintenanceFreq { get; set; }
    }
 

    public class Printers
    {
        public List<Printerinfo> printer { get; set; }
    }

    public class Printerinfo
    {
        public string Name { get; set; }
        public List<string> Materials { get; set; }
        public List<double> Volume { get; set; }
        public MachineParameters BasicInfo { get; set; }

    }
    public class MachineParameters
    {
        //public string LongName { get; set; }
        public string AMType { get; set; }
        public double MinFeatureSize { get; set; }
        public double MinLayerThickness { get; set; }

    }

    public class PCCDResult
    {
       public List<List<int>> nodeGroups { get; set; }
       public  List<List<int>> troubleNodeList { get; set; }
       public  List<int>  troubleCode{ get; set; }
    }

    public class InitialDesignSpace
    {
        public int id { get; set; }
        public List<FunctionalInterface> IDS_functionalInterfaceList{ get; set; }
    }

    public class FunctionalVolume
    {
        public int id { get; set; }
        public List<FunctionalInterface> FV_FunctionalInterfaceList { get; set; }
    }

     public class FunctionalInterface
    {
        public int id  { get; set; }
        public string name { get; set; }
        public List<Brep> surfaceList { get; set; }
        public List<string> functionList { get; set; }
        public FIType type { get; set; }
        // Mark the affilication with the origianl part
        public int partId { get; set; }
    }

    public enum FIType
    {
        part_Environment =0,
        part_User =1,
        part_Part =2,
        system_Boundary =3
    }

}
