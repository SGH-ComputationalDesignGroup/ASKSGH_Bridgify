using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sElement
{
    public class sMaterial
    {
        public string materialName { get; set; }
        
        public eMaterialType materialType { get; set; }

        public double weightPerVol { get; set; } 
        public double massPerVol { get; set; } 

        public double materialAngle { get; set; }

        public double E { get; set; } 
        public double U { get; set; } 
        public double A { get; set; } 

        public double G { get; set; } 
        public double T { get; set; }

        public sMaterial()
        {

        }

        public sMaterial(double e, double u, double a, double g, double t)
        {

        }

        public sMaterial(string matName, eMaterialType type)
        {
            this.materialName = matName;
            this.materialType = type;
        }

        public sMaterial DuplicatesMaterial()
        {
            sMaterial sm = new sMaterial();

            sm.materialName = this.materialName;
            sm.materialType = this.materialType;
            sm.weightPerVol = this.weightPerVol;
            sm.massPerVol = this.massPerVol;
            sm.materialAngle = this.materialAngle;
            sm.E = this.E;
            sm.U = this.U;
            sm.A = this.A;
            sm.T = this.T;

            return sm;
        }

        

    }

    public enum eMaterialType
    {
        STEEL_A36,
        STEEL_A53GrB,
        STEEL_A500GrB_Fy42,
        STEEL_A500GrB_Fy46,
        STEEL_A572Gr50,
        STEEL_A913Gr50,
        STEEL_A992_Fy50,

        CONCRETE_FC3000_NORMALWEIGHT,
        CONCRETE_FC4000_NORMALWEIGHT,
        CONCRETE_FC5000_NORMALWEIGHT,
        CONCRETE_FC6000_NORMALWEIGHT,
        CONCRETE_FC3000_LIGHTWEIGHT,
        CONCRETE_FC4000_LIGHTWEIGHT,
        CONCRETE_FC5000_LIGHTWEIGHT,
        CONCRETE_FC6000_LIGHTWEIGHT,

        ALUMINUM_6061_T6,
        ALUMINUM_6063_T6,
        ALUMINUM_5052_H34,

        COLDFORMED_Grade_33,
        COLDFORMED_Grade_50,

        OAK_TYP,
        CARBONFRP_TYP,
        STAINLESSSTEEL_TYP,

        Custom_Isotropic,
    }
}
