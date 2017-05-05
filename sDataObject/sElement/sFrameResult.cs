using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using sDataObject.sGeometry;
using sDataObject.IElement;

namespace sDataObject.sElement
{
    public class sFrameResult
    {
        public double parameterAt { get; set; }

        public sXYZ force { get; set; }
        public sXYZ moment { get; set; }
        public sXYZ deflection_mm { get; set; }
        public sXYZ deflectionLocal_mm { get; set; }

        public List<sFrameSectionResult> sectionResults { get; set; }

        public sFrameResult()
        {
            this.sectionResults = new List<sFrameSectionResult>();
        }

        public sFrameResult DuplicatesFrameResult()
        {
            sFrameResult newre = new sFrameResult();
            newre.parameterAt = this.parameterAt;
            if (this.force != null) newre.force = this.force.DuplicatesXYZ();
            if (this.moment != null) newre.moment = this.moment.DuplicatesXYZ();
            if (this.deflection_mm != null) newre.deflection_mm = this.deflection_mm.DuplicatesXYZ();
            if (this.deflectionLocal_mm != null) newre.deflectionLocal_mm = this.deflectionLocal_mm.DuplicatesXYZ();
            if (this.sectionResults != null)
            {
                newre.sectionResults = new List<sFrameSectionResult>();
                foreach(sFrameSectionResult vr in this.sectionResults)
                {
                    newre.sectionResults.Add(vr.DuplicatesFrameSectionResult());
                }
            }

            return newre;
        }

        public double GetForceDataByMode(eColorMode colorMode)
        {
            double data = 0.0;
            if (colorMode == eColorMode.Force_X)
            {
                data = this.force.X;
            }
            else if (colorMode == eColorMode.Force_Y)
            {
                data = this.force.Y;
            }
            else if (colorMode == eColorMode.Force_Z)
            {
                data = this.force.Z;
            }
            else if (colorMode == eColorMode.Moment_X)
            {
                data = this.moment.X;
            }
            else if (colorMode == eColorMode.Moment_Y)
            {
                data = this.moment.Y;
            }
            else if (colorMode == eColorMode.Moment_Z)
            {
                data = this.moment.Z;
            }
            return data;
        }
    }

    public class sFrameSectionResult
    {
        public int ID { get; set; }
        public sXYZ location { get; set; }
        public double stress_Axial_X { get; set; }
        public double stress_Axial_Y { get; set; }
        public double stress_Axial_Z { get; set; }
        public double stress_Moment_X { get; set; }
        public double stress_Moment_Y { get; set; }
        public double stress_Moment_Z { get; set; }
        public double stress_Combined { get; set; }
        public sXYZ deflection_mm { get; set; }

        public sFrameSectionResult DuplicatesFrameSectionResult()
        {
            sFrameSectionResult newvr = new sFrameSectionResult();
            newvr.ID = this.ID;
            newvr.location = this.location.DuplicatesXYZ();

            newvr.stress_Axial_X    = this.stress_Axial_X     ;
            newvr.stress_Axial_Y    = this.stress_Axial_Y     ;
            newvr.stress_Axial_Z    = this.stress_Axial_Z     ;
            newvr.stress_Moment_X   = this.stress_Moment_X    ;
            newvr.stress_Moment_Y   = this.stress_Moment_Y    ;
            newvr.stress_Moment_Z   = this.stress_Moment_Z    ;
            newvr.stress_Combined = this.stress_Combined;

            if(this.deflection_mm != null)
            {
                newvr.deflection_mm = this.deflection_mm.DuplicatesXYZ();
            }
            return newvr;
        }

        public double GetStressDataByMode(eColorMode colorMode)
        {
            double data = 0.0;

            if (colorMode == eColorMode.Stress_Combined_Absolute)
            {
                data = Math.Abs(this.stress_Combined);
            }
            else if (colorMode == eColorMode.Stress_Axial_X)
            {
                data = this.stress_Axial_X;
            }
            else if (colorMode == eColorMode.Stress_Moment_Y)
            {
                data = this.stress_Moment_Y;
            }
            else if (colorMode == eColorMode.Stress_Moment_Z)
            {
                data = this.stress_Moment_Z;
            }
            else if (colorMode == eColorMode.Deflection)
            {
                data = this.deflection_mm.GetLength();
            }
            return data;
        }
    }
}
