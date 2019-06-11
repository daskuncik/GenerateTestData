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
        const double d = 0.25;
        const double a = 0.67;
        int test_num;
        public Dictionary<int, string> OperationType = new Dictionary<int, string>()
        {
            {1, ">=" }, { 0, "<=" }, { 3, ">" }, { 2, "<" }, { 5, "!=" }, { 4, "==" }
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
                children = createNewPopulation_all(1);

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
            return Int32.MinValue;
        }

        public int ResearchSolve(int parent_way, int combination_way, int selection_way, ref int iter)
        {
            population = randPopulation(N);
            fillPopulationDetails(ref population);
            int iteration = 0;
            int allTrue = isFullTrue();
            List<Instance> children;
            do
            {
                children = null;
                switch (parent_way)
                {
                    case 1: //all parents
                        {
                            children = createNewPopulation_all(combination_way);
                            break;
                        }
                    case 2:
                        {
                            children = ParentSelection_nearest(combination_way);
                            break;
                        }
                    default: break;
                }
                if (children == null) return Int32.MinValue;
                fillPopulationDetails(ref children);
                population = Selection(population, children, selection_way);
                allTrue = isFullTrue();
            } while (allTrue < 0 && ++iteration < 100);
            iter = iteration;
            if (iteration < 100)
                return population[allTrue].getValue();
            return Int32.MinValue;
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

        public List<Instance> createNewPopulation_all(int way)
        {
            List<Instance> children = new List<Instance>();
            int a;
            for (int i=0; i < population.Count; i++)
                for (int j=i+1; j< population.Count; j++)
                {
                    int val1 = population[i].getValue();
                    int val2 = population[j].getValue();

                    List<Instance> ch = createChild(val1, val2, children, way);
                    children.AddRange(ch);
                }
            return children;
        }

        private List<Instance> createChild(int p1_val, int p2_val, List<Instance> children, int way)
        {
            List<Instance> ch_2 = new List<Instance>();
            InstanceComparer comparer = new InstanceComparer();
            Instance child = new Instance();
            int counter = 0;
            switch (way)
            {
                case 1:
                    {
                        if (Math.Abs(p1_val - p2_val) <= 3)
                            break;
                        if (p1_val < p2_val)
                        {
                            int k = p1_val;
                            p1_val = p2_val;
                            p2_val = k;
                        }
                            do
                            {
                                child.createRandom(p1_val, p2_val);
                                counter++;
                            } while (children.Contains(child, comparer) && counter < N);
                        ch_2.Add(child);
                        break;
                    }
                case 2:
                    {
                        child.setValue((p1_val + p2_val) / 2);
                        ch_2.Add(child);
                        break;
                    }
                case 3:
                    {
                        int e = (int)(p1_val + a * (p2_val - p1_val));
                        child.setValue(e);
                        ch_2.Add(child);
                        child = new Instance();
                        e = (int)(p2_val + a * (p1_val - p2_val));
                        child.setValue(e);
                        ch_2.Add(child);
                        break;
                    }
            }
            return ch_2;
        }

        private List<Instance> ParentSelection_nearest(int way)
        {
            List<Instance> children = new List<Instance>();
            InstanceComparer comparer = new InstanceComparer();
            List<int> selected = new List<int>();
            int counter = 0;
            Random r = new Random();
            for (int i=0; i < population.Count/2; i++)
            {
                int index = 0;
                do
                {
                    index = r.Next(0, population.Count - 1);
                    counter++;
                } while (selected.Contains(index) && counter < 10);
                selected.Add(index);
                int min = Math.Abs(population[0].getValue() - population[1].getValue());
                int ind2 = 0;
                for (int j=0; j<population.Count; j++)
                {
                    if (j == index) continue;
                    if (Math.Abs(population[index].getValue() - population[j].getValue()) < min)
                    {
                        min = Math.Abs(population[index].getValue() - population[j].getValue());
                        ind2 = j;
                    }
                }
                List<Instance> ch = createChild(population[index].getValue(), population[ind2].getValue(), children, way);
                children.AddRange(ch);
            }
            return children;
        }

        private List<Instance> Selection (List<Instance> parents, List<Instance> children, int way)
        {
            List<Instance> result = null ;
            switch (way)
            {
                case 1:
                    {
                        children = children.OrderByDescending(s => s.Fitness).ToList();
                        if (children.Count > N)
                        {
                            List<Instance> lst = children.GetRange(0, N);
                            children.Clear();
                            children = lst;
                        }

                        break;
                    }
                case 2:
                    {
                        children.AddRange(parents);
                        children = children.OrderByDescending(s => s.Fitness).ToList();
                        int index = GetIndexMinFitness(children);
                        children = children.GetRange(0, index);
                        break;
                    }
            }
            int g = (int)(N / 6);
            if (children.Count >= g )
                result = children;
            else
            {
                List<Instance> lst = randPopulation(N);
                fillPopulationDetails(ref lst);
                children.AddRange(lst);
                result = children;
            }
            return result;
            
        }

        private bool applyOperation(List<Instance> pop, int v_i, int p_i) //value-counter && population_counter
        {
            int operation = -1;
            switch (op[v_i].goal[test_num])
            {
                case 1: operation = op[v_i].operation; break;
                case 0: operation = opposit[op[v_i].operation]; break;
                case -1: return true;
                default: break;
            }
            //if (op[v_i].goal[test_num])
            //    operation = op[v_i].operation;
            //else
            //    operation = opposit[op[v_i].operation];
            switch ( operation)
            {
                case 4:
                    return op[v_i].value == pop[p_i].getValue();
                case 2:
                    return op[v_i].value > pop[p_i].getValue();
                case 0:
                    return op[v_i].value >= pop[p_i].getValue();
                case 3:
                    return op[v_i].value < pop[p_i].getValue();
                case 1:
                    return op[v_i].value <= pop[p_i].getValue();
                case 5:
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

        private int GetIndexMinFitness(List<Instance> pop)
        {
            double avg_f = 0;
            int result_index = 0;
            foreach (var element in pop)
            {
                avg_f += element.Fitness;
            }
            avg_f /= pop.Count;

            for (int i=0; i<pop.Count; i++)
            {
                if (pop[i].Fitness < avg_f)
                {
                    result_index = i;
                    break;
                }
            }
            return result_index;
        }
    }
}
