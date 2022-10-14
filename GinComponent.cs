using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System.Linq;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Gin
{
    public class GinComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GinComponent()
          : base("Gin",
                 "Gin",
                 "First try",
                 "Gin",
                 "Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("DataTree of Edges", "D", "Retrieve an ordered list of the various panels' dimensions", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Warnings", "W", "Is everything alright?", GH_ParamAccess.list);
            pManager.AddNumberParameter("Width", "W", "The Width of the Panels", GH_ParamAccess.list);
            pManager.AddNumberParameter("Height", "H", "The Height of the Panels", GH_ParamAccess.list);
            pManager.AddNumberParameter("Register", "R", "All the Heights Categories", GH_ParamAccess.list);
            pManager.AddNumberParameter("Table", "T", "How are this panels divided?", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ordered = new DataTree<double>();
            ordered.Add(0, new GH_Path(0));
            ordered.Add(-1);
            ordered.Add(-1);

            DA.GetDataTree(0, out GH_Structure<GH_Curve> a);
            Check(a);

            DA.SetDataList(0, CheckError(a));
            DA.SetDataList(1, panelListWidth);
            DA.SetDataList(2, panelListHeight);
            DA.SetDataList(3, FillRegister(panelListHeight));
            DA.SetDataTree(4, ordered);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                return Properties.Resources.Icon;            
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("14cc34b1-03e8-40fd-ae52-08731a525a89"); }
        }

        //fields for the Component

        public List<double> panelListWidth = new List<double>();
        public List<double> panelListHeight = new List<double>();
        public List<double> counting = new List<double>();
        public List<bool> countingBool = new List<bool>();
        public List<Panel> panelList = new List<Panel>();
        public DataTree<double> ordered;

        // Main Operation

        public void Check(GH_Structure<GH_Curve> tree)
        {
            for (int i = 0; i < tree.Branches.Count; i++)
            {
                double test = tree.Branches[i][0].Value.PointAtLength(1).Z - tree.Branches[i][0].Value.PointAtStart.Z;
                double test1 = tree.Branches[i][1].Value.PointAtLength(1).Z - tree.Branches[i][1].Value.PointAtStart.Z;
                counting.Add(test);

                if (Math.Abs(test) < 0.1)
                {
                    countingBool.Add(true);
                    panelListWidth.Add(Math.Round(tree.Branches[i][0].Value.GetLength()));

                    if (Math.Abs(test1 - 1) < 0.1)
                    {
                        panelListHeight.Add(Math.Round(tree.Branches[i][1].Value.GetLength()));
                    }
                    else
                    {
                        panelListHeight.Add(Math.Round(tree.Branches[i][2].Value.GetLength()));
                    }
                }
                else
                {
                    countingBool.Add(false);
                    panelListHeight.Add(Math.Round(tree.Branches[i][0].Value.GetLength()));

                    if (Math.Abs(test1) < 0.1)
                    {
                        panelListWidth.Add(Math.Round(tree.Branches[i][1].Value.GetLength()));
                    }
                    else
                    {
                        panelListWidth.Add(Math.Round(tree.Branches[i][2].Value.GetLength()));
                    }
                }

                Panel tmpPanel = new Panel(panelListWidth[panelListWidth.Count - 1], panelListHeight[panelListHeight.Count - 1]);

                panelList.Add(tmpPanel);

                PanelCheck(tmpPanel, ordered);
            }
        }

        //Fill the register of heights

        public IEnumerable<double> FillRegister(List<double> total)
        {
            IEnumerable<double> temp = total;

            return temp.Distinct();
        }

        // Check if every Panel is a Rectangle

        public List<double> CheckError(GH_Structure<GH_Curve> tree)
        {
            List<double> error = new List<double>();
                          
            for (int i = 0; i < tree.Branches.Count; i++)
            {                      
                if (tree.Branches[i].Count != 4)
                {
                    error.Add(i);
                }
            }

            return error;
        }

        // The Panel
        public struct Panel
        {
            public double W;
            public double H;

            public Panel(double W, double H)
            {
                this.W = W;
                this.H = H;
            }
        }

        //Count Panels

        public void PanelCheck(Panel p, DataTree<double> lp)
        {
            double index = 0;

            for (int i = 0; i < lp.BranchCount; i++)
            {
                if (p.W == lp.Branch(i)[1])
                {
                    if (p.H == lp.Branch(i)[2])
                    {
                        index = i;
                        lp.Branch(i)[0]++;
                        break;
                    }
                }
            }

            if (index == 0)
            {
                lp.Add(1, new GH_Path(lp.BranchCount));
                lp.Add(p.W);
                lp.Add(p.H);

            }
        }
    }
}
