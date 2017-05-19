using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sStatSystem
{
    /*
    public class StiffnessTest
    {
        public double A = 0.0;
        public double E = 0.0;
        public double Txx = 0.0;
        public double Tzx = 0.0;
        public double Tyx = 0.0;
        public double Tzy = 0.0;
        public double Tzz = 0.0;
        public double Tyy = 0.0;
        public double Tyz = 0.0;
        
        public double L = 0.0;

        public double Iy = 0.0;
        public double Iz = 0.0;

        public void test()
        {
            double[] K = new double[144]
            {
               (A* E * Txx^2)/L + (3*E* Iy*Tzx^2)/L^3 + (3*E* Iz*Tyx^2)/L^3,
            (A* E* Txx*Txy)/L + (3*E* Iz*Tyx* Tyy)/L^3 + (3*E* Iy*Tzx* Tzy)/L^3,
            (A* E* Txx*Txz)/L + (3*E* Iz*Tyx* Tyz)/L^3 + (3*E* Iy*Tzx* Tzz)/L^3,
            0,
            0,
            0,
            - (A* E* Txx^2)/L - (3*E* Iy*Tzx^2)/L^3 - (3*E* Iz*Tyx^2)/L^3,
            - (A* E* Txx*Txy)/L - (3*E* Iz*Tyx* Tyy)/L^3 - (3*E* Iy*Tzx* Tzy)/L^3,
            - (A* E* Txx*Txz)/L - (3*E* Iz*Tyx* Tyz)/L^3 - (3*E* Iy*Tzx* Tzz)/L^3,
            (3*E* Iz*Tyx* Tzx)/L^2 - (3*E* Iy*Tyx* Tzx)/L^2,
            (3*E* Iz*Tyx* Tzy)/L^2 - (3*E* Iy*Tyy* Tzx)/L^2,
            (3*E* Iz*Tyx* Tzz)/L^2 - (3*E* Iy*Tyz* Tzx)/L^2,
            (A* E * Txx * Txy)/L + (3*E* Iz*Tyx* Tyy)/L^3 + (3*E* Iy*Tzx* Tzy)/L^3,
            (A* E* Txy^2)/L + (3*E* Iy*Tzy^2)/L^3 + (3*E* Iz*Tyy^2)/L^3,
            (A* E* Txy*Txz)/L + (3*E* Iz*Tyy* Tyz)/L^3 + (3*E* Iy*Tzy* Tzz)/L^3,
            0,
            0,
            0,
            - (A* E* Txx*Txy)/L - (3*E* Iz*Tyx* Tyy)/L^3 - (3*E* Iy*Tzx* Tzy)/L^3,
            - (A* E* Txy^2)/L - (3*E* Iy*Tzy^2)/L^3 - (3*E* Iz*Tyy^2)/L^3,
            - (A* E* Txy*Txz)/L - (3*E* Iz*Tyy* Tyz)/L^3 - (3*E* Iy*Tzy* Tzz)/L^3,
            (3*E* Iz*Tyy* Tzx)/L^2 - (3*E* Iy*Tyx* Tzy)/L^2,
            (3*E* Iz*Tyy* Tzy)/L^2 - (3*E* Iy*Tyy* Tzy)/L^2,
            (3*E* Iz*Tyy* Tzz)/L^2 - (3*E* Iy*Tyz* Tzy)/L^2,
            (A* E * Txx * Txz)/L + (3*E* Iz*Tyx* Tyz)/L^3 + (3*E* Iy*Tzx* Tzz)/L^3,
            (A* E* Txy*Txz)/L + (3*E* Iz*Tyy* Tyz)/L^3 + (3*E* Iy*Tzy* Tzz)/L^3,
            (A* E* Txz^2)/L + (3*E* Iy*Tzz^2)/L^3 + (3*E* Iz*Tyz^2)/L^3,
            0,
            0,
            0,
            - (A* E* Txx*Txz)/L - (3*E* Iz*Tyx* Tyz)/L^3 - (3*E* Iy*Tzx* Tzz)/L^3,
            - (A* E* Txy*Txz)/L - (3*E* Iz*Tyy* Tyz)/L^3 - (3*E* Iy*Tzy* Tzz)/L^3,
            - (A* E* Txz^2)/L - (3*E* Iy*Tzz^2)/L^3 - (3*E* Iz*Tyz^2)/L^3,
            (3*E* Iz*Tyz* Tzx)/L^2 - (3*E* Iy*Tyx* Tzz)/L^2,
            (3*E* Iz*Tyz* Tzy)/L^2 - (3*E* Iy*Tyy* Tzz)/L^2,
            (3*E* Iz*Tyz* Tzz)/L^2 - (3*E* Iy*Tyz* Tzz)/L^2,
            0,
            0,
            0,
            (G* Ix * Txx ^ 2)/L,
            (G* Ix * Txx * Txy)/L,
            (G* Ix * Txx * Txz)/L,
            0,
            0,
            0,
            -(G* Ix* Txx^2)/L,
            -(G* Ix* Txx*Txy)/L,
            -(G* Ix* Txx*Txz)/L,
            0,
            0,
            0,
            (G* Ix * Txx * Txy)/L,
            (G* Ix * Txy ^ 2)/L,
            (G* Ix * Txy * Txz)/L,
            0,
            0,
            0,
            -(G* Ix* Txx*Txy)/L,
            -(G* Ix* Txy^2)/L,
            -(G* Ix* Txy*Txz)/L,
            0,
            0,
            0,
            (G* Ix * Txx * Txz)/L,
            (G* Ix * Txy * Txz)/L,
            (G* Ix * Txz ^ 2)/L,
            0,
            0,
            0,
            -(G* Ix* Txx*Txz)/L,
            -(G* Ix* Txy*Txz)/L,
            -(G* Ix* Txz^2)/L,
            - (A* E* Txx^2)/L - (3*E* Iy*Tzx^2)/L^3 - (3*E* Iz*Tyx^2)/L^3,
            - (A* E* Txx*Txy)/L - (3*E* Iz*Tyx* Tyy)/L^3 - (3*E* Iy*Tzx* Tzy)/L^3,
            - (A* E* Txx*Txz)/L - (3*E* Iz*Tyx* Tyz)/L^3 - (3*E* Iy*Tzx* Tzz)/L^3,
            0,
            0,
            0,
            (A* E* Txx^2)/L + (3*E* Iy*Tzx^2)/L^3 + (3*E* Iz*Tyx^2)/L^3,
            (A* E* Txx*Txy)/L + (3*E* Iz*Tyx* Tyy)/L^3 + (3*E* Iy*Tzx* Tzy)/L^3,
            (A* E* Txx*Txz)/L + (3*E* Iz*Tyx* Tyz)/L^3 + (3*E* Iy*Tzx* Tzz)/L^3,
            (3*E* Iy*Tyx* Tzx)/L^2 - (3*E* Iz*Tyx* Tzx)/L^2,
            (3*E* Iy*Tyy* Tzx)/L^2 - (3*E* Iz*Tyx* Tzy)/L^2,
            (3*E* Iy*Tyz* Tzx)/L^2 - (3*E* Iz*Tyx* Tzz)/L^2,
            - (A* E* Txx*Txy)/L - (3*E* Iz*Tyx* Tyy)/L^3 - (3*E* Iy*Tzx* Tzy)/L^3,
            - (A* E* Txy^2)/L - (3*E* Iy*Tzy^2)/L^3 - (3*E* Iz*Tyy^2)/L^3,
            - (A* E* Txy*Txz)/L - (3*E* Iz*Tyy* Tyz)/L^3 - (3*E* Iy*Tzy* Tzz)/L^3,
            0,
            0,
            0,
            (A* E* Txx*Txy)/L + (3*E* Iz*Tyx* Tyy)/L^3 + (3*E* Iy*Tzx* Tzy)/L^3,
            (A* E* Txy^2)/L + (3*E* Iy*Tzy^2)/L^3 + (3*E* Iz*Tyy^2)/L^3,
            (A* E* Txy*Txz)/L + (3*E* Iz*Tyy* Tyz)/L^3 + (3*E* Iy*Tzy* Tzz)/L^3,
            (3*E* Iy*Tyx* Tzy)/L^2 - (3*E* Iz*Tyy* Tzx)/L^2,
            (3*E* Iy*Tyy* Tzy)/L^2 - (3*E* Iz*Tyy* Tzy)/L^2,
            (3*E* Iy*Tyz* Tzy)/L^2 - (3*E* Iz*Tyy* Tzz)/L^2,
            - (A* E* Txx*Txz)/L - (3*E* Iz*Tyx* Tyz)/L^3 - (3*E* Iy*Tzx* Tzz)/L^3,
            - (A* E* Txy*Txz)/L - (3*E* Iz*Tyy* Tyz)/L^3 - (3*E* Iy*Tzy* Tzz)/L^3,
            - (A* E* Txz^2)/L - (3*E* Iy*Tzz^2)/L^3 - (3*E* Iz*Tyz^2)/L^3,
            0,
            0,
            0,
            (A* E* Txx*Txz)/L + (3*E* Iz*Tyx* Tyz)/L^3 + (3*E* Iy*Tzx* Tzz)/L^3,
            (A* E* Txy*Txz)/L + (3*E* Iz*Tyy* Tyz)/L^3 + (3*E* Iy*Tzy* Tzz)/L^3,
            (A* E* Txz^2)/L + (3*E* Iy*Tzz^2)/L^3 + (3*E* Iz*Tyz^2)/L^3,
            (3*E* Iy*Tyx* Tzz)/L^2 - (3*E* Iz*Tyz* Tzx)/L^2,
            (3*E* Iy*Tyy* Tzz)/L^2 - (3*E* Iz*Tyz* Tzy)/L^2,
            (3*E* Iy*Tyz* Tzz)/L^2 - (3*E* Iz*Tyz* Tzz)/L^2,
            (3 * E* Iz * Tyx* Tzx)/L^2 - (3*E* Iy*Tyx* Tzx)/L^2,
            (3*E* Iz*Tyy* Tzx)/L^2 - (3*E* Iy*Tyx* Tzy)/L^2,
            (3*E* Iz*Tyz* Tzx)/L^2 - (3*E* Iy*Tyx* Tzz)/L^2,
            -(G* Ix* Txx^2)/L, -(G* Ix* Txx*Txy)/L, -(G* Ix* Txx*Txz)/L,
            (3*E* Iy*Tyx* Tzx)/L^2 - (3*E* Iz*Tyx* Tzx)/L^2,
            (3*E* Iy*Tyx* Tzy)/L^2 - (3*E* Iz*Tyy* Tzx)/L^2,
            (3*E* Iy*Tyx* Tzz)/L^2 - (3*E* Iz*Tyz* Tzx)/L^2,
            (G* Ix* Txx^2)/L + (3*E* Iy*Tyx^2)/L + (3*E* Iz*Tzx^2)/L,
            (3*E* Iy*Tyx* Tyy)/L + (3*E* Iz*Tzx* Tzy)/L + (G* Ix* Txx*Txy)/L,
            (3*E* Iy*Tyx* Tyz)/L + (3*E* Iz*Tzx* Tzz)/L + (G* Ix* Txx*Txz)/L,
            (3 * E* Iz * Tyx* Tzy)/L^2 - (3*E* Iy*Tyy* Tzx)/L^2,
            (3*E* Iz*Tyy* Tzy)/L^2 - (3*E* Iy*Tyy* Tzy)/L^2,
            (3*E* Iz*Tyz* Tzy)/L^2 - (3*E* Iy*Tyy* Tzz)/L^2, -(G* Ix* Txx*Txy)/L,
            -(G* Ix* Txy^2)/L, -(G* Ix* Txy*Txz)/L,
            (3*E* Iy*Tyy* Tzx)/L^2 - (3*E* Iz*Tyx* Tzy)/L^2,
            (3*E* Iy*Tyy* Tzy)/L^2 - (3*E* Iz*Tyy* Tzy)/L^2,
            (3*E* Iy*Tyy* Tzz)/L^2 - (3*E* Iz*Tyz* Tzy)/L^2,
            (3*E* Iy*Tyx* Tyy)/L + (3*E* Iz*Tzx* Tzy)/L + (G* Ix* Txx*Txy)/L,
            (G* Ix* Txy^2)/L + (3*E* Iy*Tyy^2)/L + (3*E* Iz*Tzy^2)/L,
            (3*E* Iy*Tyy* Tyz)/L + (3*E* Iz*Tzy* Tzz)/L + (G* Ix* Txy*Txz)/L,
            (3 * E* Iz * Tyx* Tzz)/L^2 - (3*E* Iy*Tyz* Tzx)/L^2,
            (3*E* Iz*Tyy* Tzz)/L^2 - (3*E* Iy*Tyz* Tzy)/L^2,
            (3*E* Iz*Tyz* Tzz)/L^2 - (3*E* Iy*Tyz* Tzz)/L^2,
            -(G* Ix* Txx*Txz)/L,
            -(G* Ix* Txy*Txz)/L,
            -(G* Ix* Txz^2)/L,
            (3*E* Iy*Tyz* Tzx)/L^2 - (3*E* Iz*Tyx* Tzz)/L^2,
            (3*E* Iy*Tyz* Tzy)/L^2 - (3*E* Iz*Tyy* Tzz)/L^2,
            (3*E* Iy*Tyz* Tzz)/L^2 - (3*E* Iz*Tyz* Tzz)/L^2,
            (3*E* Iy*Tyx* Tyz)/L + (3*E* Iz*Tzx* Tzz)/L + (G* Ix* Txx*Txz)/L,
            (3*E* Iy*Tyy* Tyz)/L + (3*E* Iz*Tzy* Tzz)/L + (G* Ix* Txy*Txz)/L,
            (G* Ix* Txz^2)/L + (3*E* Iy*Tyz^2)/L + (3*E* Iz*Tzz^2)/L,





            };
        }
        
    }
    */
}
