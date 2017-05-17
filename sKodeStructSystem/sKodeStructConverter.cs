using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kodestruct.Common.Entities;
using Kodestruct.Common.Section.Interfaces;
using Kodestruct.Steel.AISC.SteelEntities.Materials;
using Kodestruct.Steel.AISC.AISC360v10.Flexure;
using Kodestruct.Steel.AISC.SteelEntities;
using Kodestruct.Steel.AISC.Interfaces;
using Kodestruct.Steel.AISC.Steel.Entities;
using Kodestruct.Analysis.Section;

using sDataObject.sElement;
using sDataObject.sGeometry;
using Kodestruct.Steel.AISC.SteelEntities.Sections;
using Kodestruct.Common.Section.Predefined;
using sDataObject.IElement;

namespace sKodeStructSystem
{
    public class sKodeStructConverter
    {
        public string baseUnit { get; set; }
        public string targetUnit { get; set; }

        public sKodeStructConverter()
        {
            this.baseUnit = "Feets"; //kips-in
            this.targetUnit = "";
        }
        public sKodeStructConverter(string tarUnit)
        {
            this.baseUnit = "Feets"; //kips-in
            this.targetUnit = tarUnit;
        }

        public MomentAxis ToKodeStructMomentAxis(eColorMode dataType)
        {
            string ax = "";
            if (dataType.ToString().Contains("_Y"))
            {
                ax = "XAxis";
            }
            else if (dataType.ToString().Contains("_Z"))
            {
                ax = "YAxis";
            }

            MomentAxis Axis;
            bool IsValidStringAxis = Enum.TryParse(ax, true, out Axis);
            if (IsValidStringAxis == false)
            {
                throw new Exception("Axis selection not recognized. Check input string.");
            }

            return Axis;
        }

        public SteelMaterial ToKodeStructMaterial_Steel(sCrossSection section, double Fy = -1)
        {
            double fy = 50;
            if (section.sectionType == eSectionType.AISC_I_BEAM)
            {
                fy = 50;
            }
            else if (section.sectionType == eSectionType.HSS_REC)
            {
                fy = 46;
            }
            else if (section.sectionType == eSectionType.HSS_ROUND)
            {
                fy = 42;
            }
            else if (section.sectionType == eSectionType.RECTANGLAR)
            {
                fy = 36;
            }
            else if (section.sectionType == eSectionType.ROUND)
            {
                fy = 36;
            }
            else if (section.sectionType == eSectionType.SQUARE)
            {
                fy = 36;
            }

            if(Fy > 0)
            {
                fy = Fy;
            }
            SteelMaterial mat = new SteelMaterial(fy, 29000);
            return mat;
        }

        //there are few shapes KodeStruct doesn't recognize. i.g. W8X10
        public ISection ToKodeStructCrossSection(sCrossSection sCs)
        {
            ISection shape = null;
            AiscShapeFactory shapeFac = new AiscShapeFactory();

            if (sCs.sectionType == eSectionType.AISC_I_BEAM)
            {
                shape = shapeFac.GetShape(sCs.shapeName);   
            }
            else if(sCs.sectionType == eSectionType.HSS_REC)
            {

            }
            else if (sCs.sectionType == eSectionType.HSS_ROUND)
            {

            }
            else if (sCs.sectionType == eSectionType.RECTANGLAR)
            {

            }
            else if (sCs.sectionType == eSectionType.ROUND)
            {

            }
            else if (sCs.sectionType == eSectionType.SQUARE)
            {

            }
            return shape;
        }
        

    }
}
