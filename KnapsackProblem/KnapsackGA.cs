namespace KnapsackProblem;

public class KnapsackGA
{
    private static Random random = new Random();

    public class Item
    {
        public int Weight { get; set; }
        public int Value { get; set; }
    }

    public class Chromosome
    {
        public bool[] Genes { get; set; }
        public int Fitness { get; set; }
    }

    public class Knapsack
    {
        public int Capacity { get; set; }
        public List<Item> Items { get; set; }
    }

    public class GeneticAlgorithm
    {
        private int populationSize;
        private double crossoverRate;
        private double mutationRate;
        private int generations;
        private Knapsack knapsack;

        public GeneticAlgorithm(int populationSize, double crossoverRate, double mutationRate, int generations,
            Knapsack knapsack)
        {
            this.populationSize = populationSize;
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.generations = generations;
            this.knapsack = knapsack;
        }

        public Chromosome Run()
        {
            List<Chromosome> population = InitializePopulation();
            EvaluatePopulation(population);

            for (int generation = 0; generation < generations; generation++)
            {
                List<Chromosome> newPopulation = new List<Chromosome>();

                while (newPopulation.Count < populationSize)
                {
                    Chromosome parent1 = SelectParent(population);
                    Chromosome parent2 = SelectParent(population);

                    Chromosome offspring1, offspring2;
                    Crossover(parent1, parent2, out offspring1, out offspring2);

                    Mutate(offspring1);
                    Mutate(offspring2);

                    EvaluateChromosome(offspring1);
                    EvaluateChromosome(offspring2);

                    newPopulation.Add(offspring1);
                    newPopulation.Add(offspring2);
                }

                population = newPopulation.OrderByDescending(c => c.Fitness).Take(populationSize).ToList();
            }

            return population.OrderByDescending(c => c.Fitness).First();
        }

        private List<Chromosome> InitializePopulation()
        {
            List<Chromosome> population = new List<Chromosome>();

            for (int i = 0; i < populationSize; i++)
            {
                Chromosome chromosome = new Chromosome();
                chromosome.Genes = new bool[knapsack.Items.Count];

                for (int j = 0; j < knapsack.Items.Count; j++)
                {
                    chromosome.Genes[j] = random.NextDouble() < 0.5;
                }

                population.Add(chromosome);
            }

            return population;
        }

        private void EvaluatePopulation(List<Chromosome> population)
        {
            foreach (var chromosome in population)
            {
                EvaluateChromosome(chromosome);
            }
        }

        private void EvaluateChromosome(Chromosome chromosome)
        {
            int totalWeight = 0;
            int totalValue = 0;

            for (int i = 0; i < chromosome.Genes.Length; i++)
            {
                if (chromosome.Genes[i])
                {
                    totalWeight += knapsack.Items[i].Weight;
                    totalValue += knapsack.Items[i].Value;
                }
            }

            if (totalWeight <= knapsack.Capacity)
            {
                chromosome.Fitness = totalValue;
            }
            else
            {
                chromosome.Fitness = 0;
            }
        }

        private Chromosome SelectParent(List<Chromosome> population)
        {
            int tournamentSize = 3;
            List<Chromosome> tournament = new List<Chromosome>();

            for (int i = 0; i < tournamentSize; i++)
            {
                int randomIndex = random.Next(population.Count);
                tournament.Add(population[randomIndex]);
            }

            return tournament.OrderByDescending(c => c.Fitness).First();
        }

        private void Crossover(Chromosome parent1, Chromosome parent2, out Chromosome offspring1,
            out Chromosome offspring2)
        {
            offspring1 = new Chromosome { Genes = new bool[parent1.Genes.Length] };
            offspring2 = new Chromosome { Genes = new bool[parent2.Genes.Length] };

            if (random.NextDouble() < crossoverRate)
            {
                int crossoverPoint = random.Next(parent1.Genes.Length);

                for (int i = 0; i < crossoverPoint; i++)
                {
                    offspring1.Genes[i] = parent1.Genes[i];
                    offspring2.Genes[i] = parent2.Genes[i];
                }

                for (int i = crossoverPoint; i < parent1.Genes.Length; i++)
                {
                    offspring1.Genes[i] = parent2.Genes[i];
                    offspring2.Genes[i] = parent1.Genes[i];
                }
            }
            else
            {
                Array.Copy(parent1.Genes, offspring1.Genes, parent1.Genes.Length);
                Array.Copy(parent2.Genes, offspring2.Genes, parent2.Genes.Length);
            }
        }

        private void Mutate(Chromosome chromosome)
        {
            for (int i = 0; i < chromosome.Genes.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    chromosome.Genes[i] = !chromosome.Genes[i];
                }
            }
        }
    }
}