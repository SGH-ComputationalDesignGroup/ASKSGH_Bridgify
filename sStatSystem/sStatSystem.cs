using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SawapanStatica;
using sDataObject.sElement;
using sDataObject.sGeometry;
using Newtonsoft.Json;
using sDataObject.IElement;
using sDataObject.ElementBase;
using sDataObject.sSteelElement;

namespace sNStatSystem
{
    public class sStatSystem : SystemBase
    {
        public Type oriSystemType { get; set; }
        public StatSystem FEsystem { get; set; }

        public sStatSystem()
        {

        }

        public sStatSystem(ISystem isys)
        {
            this.BuildSystem(isys);
        }
        
        private void BuildSystem(ISystem ssys)
        {
            this.DuplicateFromsSystem(ssys);
            BuildFESystem();
        }

        private void BuildFESystem()
        {
            this.FEsystem = new StatSystem();
            this.FEsystem.PointMergeTolerance = this.systemSettings.mergeTolerance_m;

            this.FEsystem.MergeNewNodes = true;

            sStatConverter conv = new sStatConverter();

            foreach(IFrameSet bs in this.frameSets)
            {
                foreach(sFrame b in bs.frames)
                {
                    StatCrossSection cs = conv.ToStatCrossSection(b, bs.AsMinuteDensity);

                    StatNode n0 = this.FEsystem.AddNode(conv.ToCVector(b.node0));
                    StatNode n1 = this.FEsystem.AddNode(conv.ToCVector(b.node1));

                    C_vector uv = conv.ToCVector(b.upVector);

                    StatBeam sb = this.FEsystem.AddBeam(n0, n1, cs, uv);

                    b.extraData = sb;
                    sb.ExtraData = b;
                }
            }

            foreach(sNode sn in this.nodes)
            {
                StatNode n = null;
                if(sn.boundaryCondition != null)
                {
                    //n = this.FEsystem.AddNode(sn.location.X, sn.location.Y, sn.location.Z);
                    n = FindStatNode(sn, this.systemSettings.mergeTolerance_m);
                    if (sn.boundaryCondition.supportType == eSupportType.FIXED)
                    {
                        n.SupportType = BOUNDARYCONDITIONS.ALL;
                    }
                    else if (sn.boundaryCondition.supportType == eSupportType.PINNED)
                    {
                        n.SupportType = BOUNDARYCONDITIONS.TRANSLATIONS;
                    }
                    else if (sn.boundaryCondition.supportType == eSupportType.CUSTOM)
                    {

                    }
                }

                if(sn.pointLoads != null && sn.pointLoads.Count > 0)
                {
                    n = FindStatNode(sn, this.systemSettings.mergeTolerance_m);
                }

                if (n != null)
                {
                    sn.extraData = n;
                    n.ExtraData = sn;
                }
            }


        }

        public ISystem ToEmptyISystem()
        {
            ISystem ssys = this.DuplicatesSystem();
            return ssys ;
        }

        public static ISystem GetCalculatedSystem(ISystem ori, string comboName)
        {
            sStatSystem stSys = new sStatSystem(ori);
            stSys.SolveSystemByCaseName(comboName);
            return stSys;//??.TosSystem();
        }

        public void AwareFESystemResult(double du = 0.0)
        {
            this.FEsystem.RecoverNodeReactions();

            //reaction returns wrong values...
            UpdateBeamResults(this.systemSettings.meshDensity_m, du);
            UpdateNodeResults();

            this.AwaresSystemResult();

            this.estimatedMaxD = this.FEsystem.MaximumDisplacement;
            this.estimatedWeight = this.FEsystem.TotalWeight;
        }
        
        public void SolveSystem(double deadLoadFactor)
        {
            this.FEsystem.DeadLoadFactor = deadLoadFactor;
            this.FEsystem.SolveSystem();

            this.AwareFESystemResult();
        }

        public void SolveSystemByCaseName(string caseName)
        {
            int caseType = AwarePattern_Combo(caseName);
            if(caseType == 0)
            {
                SolveSystemByPattern(caseName);
                this.AwareFESystemResult();
            }
            else if(caseType == 1)
            {
                SolveSystemByCombo(caseName);
                this.AwareFESystemResult();
            }
            else
            {
                //none
            }
        }

        public void SolveSystemByPattern(string loadPattern)
        {
            if(loadPattern == "DEAD")
            {
                this.FEsystem.DeadLoadFactor = 1.0;
                this.FEsystem.SolveSystem();
            }
            else
            {
                //superdead should come here
                this.ApplyPointLoadsByPattern(loadPattern);
                this.ApplyLineLoadsByPattern(loadPattern);
                //add area loads

                this.FEsystem.DeadLoadFactor = 0.0;
                this.FEsystem.SolveSystem();
            }
        }

        public void SolveSystemByCombo(string combo)
        {
            double deadFactor = 0.0;
            this.ApplyPointLoadsByCombo(combo, ref deadFactor);
            this.ApplyLineLoadsByCombo(combo, ref deadFactor);
            //add area loads

            this.FEsystem.DeadLoadFactor = deadFactor;
            this.FEsystem.SolveSystem();
        }

        public int AwarePattern_Combo(string name)
        {
            int patCount = 0;
            int comboCount = 0;
            foreach (string pat in this.loadPatterns)
            {
                if (pat == name)
                {
                    patCount++;
                    break;
                }
            }
            if (patCount == 0)
            {
                foreach (sLoadCombination co in this.loadCombinations)
                {
                    if (co.combinationName == name)
                    {
                        comboCount++;
                        break;
                    }
                }
            }

            if(patCount > 0 && comboCount == 0)
            {
                return 0;
            }
            else if(patCount == 0 && comboCount > 0)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public void Dispose()
        {
            this.FEsystem = null;
        }

        public StatNode FindStatNode(sNode jn, double tol)
        {
            StatNode clnode = null;
            
            foreach(StatNode n in this.FEsystem.Nodes)
            {
                C_vector jv = new C_vector(jn.location.X, jn.location.Y, jn.location.Z);
                if(jv.DistanceTo(n.p) < tol)
                {
                    clnode = n;
                    break;
                }
            }

            return clnode;
        }

        public void UpdateNodeResults()
        {
            sStatConverter conv = new sStatConverter();
            this.nodes.Clear();
            this.nodes = new List<sNode>();

            foreach (StatNode sn in this.FEsystem.Nodes)
            {
                int count = 0;
                sNode n = sn.ExtraData as sNode;
                if (n != null && sn.SupportType != BOUNDARYCONDITIONS.NONE)
                {
                    if (sn.ReactionForce.Length > 0.0)
                    {
                        count++;
                        n.boundaryCondition.reaction_force = conv.TosXYZ(sn.ReactionForce);
                    }
                    if (sn.ReactionMoment.Length > 0.0)
                    {
                        count++;
                        n.boundaryCondition.reaction_moment = conv.TosXYZ(sn.ReactionMoment);
                    }
                }

                if (n != null && n.pointLoads != null && n.pointLoads.Count > 0)
                {
                    count++;
                }

                if (count > 0)
                {
                    this.nodes.Add(n.DuplicatesNode());
                }
            }
        }

        public void UpdateBeamResults(double dataLenTol, double du)
        {
            
            if (this.FEsystem != null)
            {
                foreach (IFrameSet bs in this.frameSets)
                {
                    sResultRange bsRe = new sResultRange();

                    foreach (sFrame sb in bs.frames)
                    {
                        AwaresBeamResult(sb, ref bsRe, dataLenTol);
                    }

                    bs.results_Max = bsRe;
                }
            }    
        }

        private void AwaresBeamResult(sFrame sb, ref sResultRange bsRe, double dataLenTol)
        {
            StatBeamResults br = new StatBeamResults();
            sStatConverter conv = new sStatConverter();

            StatBeam b = sb.extraData as StatBeam;

            b.RecoverForces();

            sb.beamWeight = b.Weight;
            sb.results = new List<sFrameResult>();

            double len = sb.axis.length;
            int count = (int)(len / dataLenTol);
            if (count < 1) count = 1;

            double step = 1.0 / (double)(count);
            for (int i = 0; i < count + 1; ++i)
            {
                double tNow = step * i;
                br.t = tNow;
                b.GetInterpolatedResultsAt(new C_vector(0, 0, -1), this.FEsystem.DeadLoadFactor, br);

                sFrameResult bre = new sFrameResult();
                bre.moment = new sXYZ(br.MomentL.x, br.MomentL.y, br.MomentL.z);
                bre.force = new sXYZ(-br.ForceL.x, br.ForceL.y, br.ForceL.z); /// force X negate?????????
                bre.deflection_mm = conv.TosXYZ(b.Csys.LocalToGlobalVector(br.DeflL * 1000));
                bre.deflectionLocal_mm = conv.TosXYZ(br.DeflL * 1000);

                bre.parameterAt = tNow;
                

                sPlane secPlane = new sPlane(sb.axis.PointAt(tNow), sb.localPlane.Xaxis, sb.localPlane.Yaxis);

                List<sXYZ> secVertices = new List<sXYZ>();
                if (sb.crossSection.sectionType == eSectionType.AISC_I_BEAM)
                {
                    secVertices = sb.crossSection.GetWbeamFaceVertices(secPlane).ToList();
                }
                else if (sb.crossSection.sectionType == eSectionType.HSS_REC)
                {
                    secVertices = sb.crossSection.GetHSSRecFaceVertices_Simple(secPlane).ToList();
                }
                else if (sb.crossSection.sectionType == eSectionType.HSS_ROUND)
                {
                    secVertices = sb.crossSection.GetHSSRoundFaceVertices_Simple(secPlane).ToList();
                }
                else if (sb.crossSection.sectionType == eSectionType.SQUARE)
                {
                    secVertices = sb.crossSection.GetSquareFaceVertices_Simple(secPlane).ToList();
                }
                else if (sb.crossSection.sectionType == eSectionType.RECTANGLAR)
                {
                    secVertices = sb.crossSection.GetRecFaceVertices_Simple(secPlane).ToList();
                }
                else if (sb.crossSection.sectionType == eSectionType.ROUND)
                {
                    secVertices = sb.crossSection.GetRoundFaceVertices_Simple(secPlane).ToList();
                }

                if (secVertices != null)
                {
                    for (int j = 0; j < secVertices.Count; ++j)
                    {
                        sXYZ svp = secVertices[j];
                        sXYZ localDir = svp - sb.axis.PointAt(tNow);

                        sXYZ ToLocalZ = localDir.ProjectTo(secPlane.Zaxis);
                        sXYZ ToLocalY = localDir.ProjectTo(secPlane.Yaxis);

                        double len_ToLocalZ = ToLocalZ.Z; // vertical like
                        double len_ToLocalY = ToLocalY.Y; // horizontal like

                        br.zL = len_ToLocalZ;// vertical like
                        br.yL = len_ToLocalY;// horizontal like

                        double axialStress_X = (br.ForceL.x / b.CrossSection.Area);
                        //double axialStress_Y = ??;
                        //double axialStress_Z = ??;

                        double MyStress = ((br.MomentL.y * br.zL) / b.CrossSection.Iyy);
                        double MzStress = ((br.MomentL.z * br.yL) / b.CrossSection.Izz);
                        //double MxStress = ??

                        double stressTest = axialStress_X + MyStress - MzStress; //why negate?...(StatBeamResult does this)

                        b.GetSectionPointResult(br);

                        sFrameSectionResult secRe = new sFrameSectionResult();
                        secRe.location = svp;
                        secRe.deflection_mm = conv.TosXYZ(b.Csys.LocalToGlobalVector(br.DeflL * 1000));

                        secRe.stress_Combined = Math.Abs(stressTest);
                        secRe.stress_Axial_X = axialStress_X;
                        secRe.stress_Moment_Y = MyStress;
                        secRe.stress_Moment_Z = MzStress;

                        bre.sectionResults.Add(secRe);
                    }
                }

                bsRe.UpdateMaxValues(bre);
                sb.results.Add(bre);
            }
        }      

        public sLoadCombination GetLoadComboByName(string comboName)
        {
            sLoadCombination combo = null;
            foreach(sLoadCombination co in this.loadCombinations)
            {
                if(co.combinationName == comboName)
                {
                    combo = co;
                    break;
                }
            }
            return combo;
        }

        public void ApplyPointLoadsByCombo(string comboName, ref double deadFactor)
        {
            sLoadCombination combo = GetLoadComboByName(comboName);
            //double Dfactor = 0.0;

            foreach (sNode sn in this.nodes)
            {
                StatNode stn = sn.extraData as StatNode;
                if (stn != null)
                {
                    if (sn.pointLoads != null && sn.pointLoads.Count > 0)
                    {
                        sPointLoad comboLoad = new sPointLoad();
                        if (combo.combinationType == eCombinationType.LinearAdditive)
                        {
                            for (int i = 0; i < combo.patterns.Count; ++i)
                            {
                                string pattern = combo.patterns[i];
                                double factor = combo.factors[i];

                                if(pattern != "DEAD")
                                {
                                   sn.UpdatePointLoadByPatternFactor_LinearAdditive(pattern, factor, ref comboLoad);
                                }
                                else
                                {
                                    deadFactor = factor;
                                }
                                
                            }
                        }
                        //else what?

                        if (comboLoad.forceVector != null || comboLoad.momentVector != null)
                        {
                            //need to reset node load????????????????
                            comboLoad.location = sn.location;
                            if(comboLoad.forceVector != null)
                            {
                                stn.AddLoad(comboLoad.forceVector.X, comboLoad.forceVector.Y, comboLoad.forceVector.Z);
                               
                                object t = stn;
                            }
                            if(comboLoad.momentVector != null)
                            {
                                stn.AddLoad(comboLoad.momentVector.X, comboLoad.momentVector.Y, comboLoad.momentVector.Z);
                            }
                        }
                    }
                }
            }
        }

        public void ApplyPointLoadsByPattern(string patternName)
        {
            foreach(sNode sn in this.nodes)
            {
                StatNode stn = sn.extraData as StatNode;
                if (stn != null)
                {
                    if (sn.pointLoads != null && sn.pointLoads.Count > 0)
                    {
                        sXYZ load = sXYZ.Zero();
                        //sXYZ moment = sXYZ.Zero();
                        foreach (sPointLoad pl in sn.pointLoads)
                        {
                            if (pl.loadPatternName == patternName)
                            {
                                load += pl.forceVector;
                                //moment += pl.momentVector;
                                //currently StatSystem cannot do moment load
                            }
                        }
                        if (load.GetLength() > 0.0)
                        {
                            stn.AddLoad(load.X, load.Y, load.Z);
                        }
                    }
                }
            }
        }

        public void ApplyLineLoadsByCombo(string comboName, ref double deadFactor)
        {
            sLoadCombination combo = GetLoadComboByName(comboName);

            foreach (StatBeam stb in this.FEsystem.Beams)
            {
                sFrame sb = stb.ExtraData as sFrame;
                if (sb != null)
                {
                    if (sb.lineLoads != null && sb.lineLoads.Count > 0)
                    {
                        sLineLoad comboLoad = new sLineLoad();
                        if (combo.combinationType == eCombinationType.LinearAdditive)
                        {
                            for (int i = 0; i < combo.patterns.Count; ++i)
                            {
                                string pattern = combo.patterns[i];
                                double factor = combo.factors[i];

                                if (pattern != "DEAD")
                                {
                                    sb.UpdateLineLoadByPatternFactor_LinearAdditive(pattern, factor, ref comboLoad);
                                }
                                else
                                {
                                    deadFactor = factor;
                                }
                            }
                        }
                        //else what?

                        if (comboLoad.load_Force != null || comboLoad.load_Moment != null)
                        {
                            if (comboLoad.load_Force != null)
                            {
                                stb.AppliedLinearLoad = new C_vector(comboLoad.load_Force.X, comboLoad.load_Force.Y, comboLoad.load_Force.Z);
                            }
                            if (comboLoad.load_Moment != null)
                            {
                                stb.AppliedLinearLoad = new C_vector(comboLoad.load_Moment.X, comboLoad.load_Moment.Y, comboLoad.load_Moment.Z);
                            }
                        }
                        if(comboLoad.load_Scalar > 0.0)
                        {

                        }
                    }
                }
            }
        }

        public void ApplyLineLoadsByPattern(string patternName)
        {
            foreach(StatBeam stb in this.FEsystem.Beams)
            {
                sFrame sb = stb.ExtraData as sFrame;
                if(sb != null)
                {
                    if(sb.lineLoads != null && sb.lineLoads.Count > 0)
                    {
                        sXYZ load = sXYZ.Zero();
                        //sXYZ moment = sXYZ.Zero();
                        //double scalar = 0.0;
                        foreach(sLineLoad ll in sb.lineLoads)
                        {
                            if(ll.loadPatternName == patternName)
                            {
                                load += ll.load_Force;
                                //moment += ll.load_Moment;
                                //scalar += ll.load_Scalar;
                            }
                        }

                        if(load.GetLength() > 0.0)
                        {
                            stb.AppliedLinearLoad = new C_vector(load.X, load.Y, load.Z);
                        }
                    }
                }
            }
        }


        //abstract override
        public override ISystem DuplicatesSystem()
        {
            sStatSystem nsys = new sStatSystem();

            nsys.oriSystemType = this.oriSystemType;
            
            nsys.TransfersSystemBasis(this);
            nsys.TransfersSystemIFrameSetElements(this);
            //any other things only for stat system??
            //how about FE system?... duplicate???

            return nsys;
        }

        private void toggleMinuteDensityStatus(eSteelFrameSetType type, bool toggle)
        {
            foreach (sSteelFrameSet fs in this.frameSets.Cast<sSteelFrameSet>().Where(f => f.frameStructureType == type))
            {
                fs.AsMinuteDensity = toggle;
            }
        }

        private int applyDesignedCrossSections(eSteelFrameSetType type, int index = 0)
        {
            int count = 0;
            foreach (sSteelFrameSet fs in this.frameSets.Cast<sSteelFrameSet>().Where(f => f.frameStructureType == type))
            {
                if (fs.designedCrossSections != null && fs.designedCrossSections.Count > 0)
                {
                    fs.crossSection = null;
                    if (index > fs.designedCrossSections.Count - 1) index = fs.designedCrossSections.Count - 1;
                    fs.crossSection = fs.designedCrossSections[index].DuplicatesCrosssection();

                    fs.UpdatesFrameCrossSections();

                    count++;
                }
            }
            return count;
        }

        public override void ToggleMinuteDensityStatus(object frameSetFilter, bool toggle)
        {
            if (frameSetFilter is string)
            {
                foreach (IFrameSet fs in this.frameSets.Where(f => f.frameSetName.Equals((string)frameSetFilter)))
                {
                    fs.AsMinuteDensity = toggle;
                }
            }
            else if (frameSetFilter is eSteelFrameSetType)
            {
                eSteelFrameSetType type = (eSteelFrameSetType)frameSetFilter;
                this.toggleMinuteDensityStatus(type, toggle);
            }
        }

        public override int ApplyDesignedCrossSections(object frameSetFilter, int index = 0)
        {
            int count = 0;

            if (frameSetFilter is eSteelFrameSetType)
            {
                eSteelFrameSetType type = (eSteelFrameSetType)frameSetFilter;
                count = this.applyDesignedCrossSections(type, index);
            }
            else if (frameSetFilter is string)
            {
                foreach (IFrameSet fs in this.frameSets.Where(f => f.frameSetName.Equals((string)frameSetFilter)))
                {
                    if (fs.designedCrossSections != null && fs.designedCrossSections.Count > 0)
                    {
                        fs.crossSection = null;
                        if (index > fs.designedCrossSections.Count - 1) index = fs.designedCrossSections.Count - 1;
                        fs.crossSection = fs.designedCrossSections[index].DuplicatesCrosssection();

                        fs.UpdatesFrameCrossSections();

                        count++;
                    }
                }
            }
            return count;
        }
    }
}
