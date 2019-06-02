using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDT_EA.Classes;


namespace GDT_EA.Evolutions
{
    
    public class Evolution
    {
        List<Instance> population;
        const double minFitness = 0.85;
        int test_num;
        public Dictionary<int, string> OperationType = new Dictionary<int, string>()
        {
            {0, ">=" }, { 1, "<=" }, { 2, ">" }, { 3, "<" }, { 4, "!=" }, { 5, "==" }
        };
        private Dictionary<int, int> opposit = new Dictionary<int, int>() { {0, 3 }, { 3, 0}, { 1, 2 }, {2, 1 }, { 4, 5}, {5, 4 } };
        const int N = 10;
        //List<int> operations;
       // List<int> values;
        //operations. fields: operationId, value, globalId
        List<OP> op;
        string var_name;
        //int fitness;

        public Evolution(string varname)
        {
            var_name = varname;
            test_num = 0;
            //population = randPopulation(N, -30, 30);
            //population = randPopulation(N);
            population = new List<Instance>();
            //create();
            //operations = new List<int>();
            //values = new List<int>();
            op = new List<OP>();
        }

        public double Fitness(int a)
        {
            return Math.Abs(1 / a);
        }

        
        public void addConditions(List<OP> dic)
        {
            op = dic;
            //foreach (var dic_el in dic)
            //{
            //    operations.Add(dic_el.Value);
            //    values.Add(dic_el.Key);
            //}
        }
        public int Solve()
        {
            double fitness = -1;
            // Generate initial population.
            population = randPopulation(N);

            fillPopulationDetails(ref population);

            population.Sort((emp1, emp2) => emp1.Fitness.CompareTo(emp2.Fitness));
            population = population.OrderByDescending(s => s.Fitness).ToList();
            fitness = population[0].Fitness;
            int allTrue = isFullTrue();
            if (allTrue >= 0)
                return population[allTrue].getValue();
            int iteration = 0;
            List<Instance> children; //= createNewPopulation();
            do
            {
                children = createNewPopulation();

                fillPopulationDetails(ref children);

                children = children.OrderByDescending(s => s.Fitness).ToList();
                if (children.Count > 0)
                    fitness = children[0].Fitness;
                else
                    fitness = population[0].Fitness;
                if (children.Count > N)
                {
                    List<Instance> lst = children.GetRange(0, N);
                    children.Clear();
                    children = lst;
                }
                if (children.Count > 2)
                    population = children;
                else
                {
                    List<Instance> lst = randPopulation(N + N);
                    population.AddRange(lst);
                    fillPopulationDetails(ref population);
                }
                allTrue = isFullTrue();
            } while ( allTrue < 0 && ++iteration < 100);
            if (iteration < 100)
                return population[allTrue].getValue();
            return -10;
        }

        public List<Instance> randPopulation(int count, int min = Int32.MinValue, int max = Int32.MaxValue)
        {
            List<Instance> _population = new List<Instance>();
            if (op.Count > 0)
            {
                min = op[0].value;
                max = op[0].value;
                for (int i=1; i<op.Count; i++)
                {
                    if (min > op[i].value)
                        min = op[i].value ;
                    if (op[i].value > max)
                        max = op[i].value;
                }
            }
            min -= 1000;
            max += 1000;
            for (int i = 0; i < count; i++)
            {
                Instance inst = new Instance();
                //if (max != 0)
                InstanceComparer compar = new InstanceComparer();
                do
                {
                    inst.createRandom(max, min);
                    
                } while (_population.Contains(inst, compar));
                _population.Add(inst);
            }
            return _population;
        }

        public List<Instance> createNewPopulation()
        {
            List<Instance> children = new List<Instance>();
            InstanceComparer comparer = new InstanceComparer();
            int counter = 0;
            int a;
            for (int i=0; i < population.Count; i++)
                for (int j=i+1; j< population.Count; j++)
                {
                    Instance child = new Instance();
                    int val1 = population[i].getValue();
                    int val2 = population[j].getValue();
                    if (Math.Abs(val1 - val2) <= 3)
                        continue;
                    if (val1 < val2)
                        do
                        {
                            child.createRandom(val2, val1);
                            counter++;
                        } while (children.Contains(child, comparer) && counter < N);
                    else
                        do {
                            child.createRandom(val1, val2);
                            counter++;
                        } while (children.Contains(child, comparer) && counter < N);
                    if (counter < N)
                        children.Add(child);
                    else
                        a = 10;
                    counter = 0;
                }
            return children;
        }

        private bool applyOperation(List<Instance> pop, int v_i, int p_i) //value-counter && population_counter
        {
            int operation = -1;
            switch (op[v_i].goal[test_num])
            {
                case 1: operation = op[v_i].operation; break;
                case 0: operation = opposit[op[v_i].operation]; break;
                default: break;
            }
            //if (op[v_i].goal[test_num])
            //    operation = op[v_i].operation;
            //else
            //    operation = opposit[op[v_i].operation];
            switch ( op[v_i].operation)
            {
                case 5:
                    return op[v_i].value == pop[p_i].getValue();
                case 3:
                    return op[v_i].value > pop[p_i].getValue();
                case 1:
                    return op[v_i].value >= pop[p_i].getValue();
                case 2:
                    return op[v_i].value < pop[p_i].getValue();
                case 0:
                    return op[v_i].value <= pop[p_i].getValue();
                case 4:
                    return op[v_i].value != pop[p_i].getValue();
            }
            return false;
        }

        private int isFullTrue()
        {
            for (int i=0; i<population.Count; i++)
            //foreach (Instance ins in population)
            {
                if (population[i].allTrue())
                    return i;
            }
            return -1;
        }

        //public void addCondition(int _operation, int _value)
        //{
        //    operations.Add(_operation);
        //    values.Add(_value);
        //}

        private void fillPopulationDetails(ref List<Instance> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                for (int j = 0; j < op.Count; j++)
                {
                    int diff = lst[i].getValue() - op[j].value;
                    lst[i].addDiff(diff);
                    lst[i].addConformity(applyOperation(lst, j, i));
                }
                lst[i].calculateFitness();
            }
        }

        public void setTestNum(int n)
        {
            test_num = n;
        }

        public void incTestNum() { if (test_num + 1 >= op[0].goal.Count) test_num = 0; else test_num++; }

        private void create()
        {
            Instance ins = new Instance();
            ins.setValue(11);
            population.Add(ins);

            ins = new Instance();
            ins.setValue(-20);
            population.Add(ins);

            ins = new Instance();
            ins.setValue(18);
            population.Add(ins);

            ins = new Instance();
            ins.setValue(2);
            population.Add(ins);

            ins = new Instance();
            ins.setValue(11);
            population.Add(ins);

        }
    }
}
