using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sElement
{
    public class sLoadCombination
    {
        public string combinationName  { get; set; }
        public eCombinationType combinationType { get; set; }
        public List<string> patterns { get; set; }
        public List<double> factors { get; set; }

        public sLoadCombination()
        {

        }

        public sLoadCombination(string comboName, eCombinationType type, List<string> patternNames, List<double> patternFactors)
        {
            this.combinationName = comboName;
            this.combinationType = type;
            this.patterns = patternNames.ToList();
            this.factors = patternFactors.ToList();
        }

        public sLoadCombination DuplicatesLoadCombination()
        {
            sLoadCombination nc = new sLoadCombination(this.combinationName, this.combinationType, this.patterns.ToList(), this.factors.ToList());
            return nc;
        }

        public List<sPointLoad> GetCorrespondingFactoredPointLoads(List<sPointLoad> loadAll)
        {
            List<sPointLoad> factored = new List<sPointLoad>();

            foreach(sPointLoad pl in loadAll)
            {
                for(int i = 0; i < this.patterns.Count; ++i)
                {
                    if(this.patterns[i] == pl.loadPatternName)
                    {
                        sPointLoad factoredLoad = pl.DuplicatePointLoad();
                        if (factoredLoad.forceVector != null)
                        {
                            factoredLoad.forceVector *= this.factors[i];
                        }
                        factored.Add(factoredLoad);
                        break;
                    }
                }
            }

            return factored;
        }


    }

    public enum eCombinationType
    {
        LinearAdditive,
        Envelope,
        AbsoluteAdditive,
        SRSS,
        RangeAdditive
    }
}
