using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System.Windows.Forms;
using ConsoliDesign.Forms;

namespace ConsoliDesign
{
    [System.Runtime.InteropServices.Guid("0df1021c-5630-476f-b540-82ff69f5c218")]
    public class ConsoliDesignCommand : Command
    {
        public ConsoliDesignCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static ConsoliDesignCommand Instance
        {
            get;
            private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "ConsoliDesignCommand"; }
        }

        private PCCDMainEntrance Form {get;set;}

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (null == Form)
            {
                Form = new PCCDMainEntrance(doc);
                Form.FormClosed += OnFormClosed;
                Form.Show(RhinoApp.MainWindow());
            }

            return Result.Success;
        }
        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Form.Dispose();
            Form = null;
        }
    }
}
