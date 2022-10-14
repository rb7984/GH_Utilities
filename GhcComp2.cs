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

namespace Gin
{
    public class GhcComp2 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GhcComp2()
          : base("ToExcel", "ToEx",
              "Gives you a string you can copy as a table in Excel",
              "Gin", "Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("In", "I", "The tree exiting from the main component", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Out", "O", "A list to be pasted in Excel", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetDataTree(0, out GH_Structure<GH_Number> a);
            DA.SetDataList(0, Solve(a));
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Properties.Resources.Icon2;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ee634c8b-1904-4128-89e1-7b2a98c02a6d"); }
        }

        public List<string> Solve(GH_Structure<GH_Number> x)
        {
            List<string> ret = new List<string>();

            for (int i = 0; i < x.Branches.Count; i++)
            {
                ret.Add(x.Branches[i][0].ToString());

                for (int j = 1; j < x.Branches[i].Count; j++)
                {
                    ret[ret.Count - 1] = ret[ret.Count - 1] + "," + x.Branches[i][j].ToString();
                }
            }
            return ret;
        }
    }
}