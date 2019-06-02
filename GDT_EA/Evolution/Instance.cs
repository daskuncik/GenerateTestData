using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT_EA.Evolutions
{
    //сделать шаблонным??
    public class Instance
    {
        int Value { get; set; }
        List<int> Difference { get; set; }
        public double Fitness { get; set; }
        List<bool> Conformity { get; set; } //соответствие

        public Instance()
        {
            Random r = new Random();
            Value = r.Next();
            Difference = new List<int>();
            Conformity = new List<bool>();
        }

        public Instance(int v, int d, bool f)
        {
            Value = v;
            Difference = new List<int>();
            Conformity = new List<bool>();
        }

        public int createRandom(int max = Int32.MaxValue, int min=Int32.MinValue)
        {
            Random r = new Random();
            if (max == Int32.MaxValue)
                Value = r.Next();
            else
            if (min != Int32.MinValue)
                Value = r.Next(min, max);
            else
                Value = r.Next(max);
            return Value;
        }

        public int getValue() { return Value; }
        public void setValue(int value) { Value = value; }

        public void addDiff(int d) { Difference.Add(d); }
        public int getDiff(int i) { return Difference[i]; }

        public void addConformity(bool c) { Conformity.Add(c); }
        public bool getConformity(int i) { return Conformity[i]; }

        public void calculateFitness()
        {
            double sum = 0.0, mul = 1.0;
            for (int i=0; i<Difference.Count; i++)
            {
                sum += Difference[i];
                mul *= Math.Abs(1.0 / Difference[i]);
            }
            mul *= Math.Abs((double)sum);
            if (!Double.IsInfinity(mul))
                Fitness = mul;
            else
                Fitness = -1.0;
        }

        public bool allTrue()
        {
            int count = 0;
            foreach (bool b in Conformity)
                if (b) count++;
            if (count == Conformity.Count)
                return true;
            return false;
        }

        
    }
    public class InstanceComparer : IEqualityComparer<Instance>
    {
        public bool Equals(Instance a, Instance b)
        {
            return a.getValue() == b.getValue();
        }

        public int GetHashCode(Instance obj)
        {
            return obj.GetHashCode();
        }
    }
}
