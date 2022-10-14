using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Gin
{
    public class GinInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Gin";
            }
        }
        public override Bitmap Icon
        {
            get
            {

                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "A try";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("01f1bc7c-708a-4e43-ba49-fe131476fd54");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "RB";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "333";
            }
        }
    }
}
