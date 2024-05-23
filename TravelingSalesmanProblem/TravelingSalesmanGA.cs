namespace TravelingSalesmanProblem;

public class TravelingSalesmanGA
{
       public class City
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public City(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public double DistanceTo(City other)
        {
            double deltaX = X - other.X;
            double deltaY = Y - other.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }

    public class Chromosome
    {
        public List<int> Gene { get; set; }
        public double Fitness { get; set; }

        public Chromosome(int numberOfCities)
        {
            Gene = new List<int>(new int[numberOfCities]);
            Fitness = double.MaxValue;
        }
    }

    public static class DataGenerator
    {
        public static List<City> GenerateCities(int numCities)
        {
            Random rand = new Random();
            List<City> cities = new List<City>();

            for (int i = 0; i < numCities; i++)
            {
                cities.Add(new City(i, rand.NextDouble() * 100, rand.NextDouble() * 100));
            }

            return cities;
        }
    }

    public static class EvolutionaryAlgorithm
    {
        public static Chromosome Run(List<City> cities, int populationSize, int generations, double mutationRate)
        {
            Random rand = new Random();
            List<Chromosome> population = InitializePopulation(populationSize, cities.Count, rand);

            for (int generation = 0; generation < generations; generation++)
            {
                EvaluatePopulation(population, cities);
                List<Chromosome> selectedChromosomes = SelectChromosomes(population);

                // Upewnij się, że liczba wybranych chromosomów jest parzysta
                if (selectedChromosomes.Count % 2 != 0)
                {
                    selectedChromosomes.Add(selectedChromosomes[0]);
                }

                population = CrossoverAndMutate(selectedChromosomes, mutationRate, rand);
            }

            EvaluatePopulation(population, cities);
            return population.OrderBy(c => c.Fitness).First();
        }

        private static List<Chromosome> InitializePopulation(int populationSize, int numCities, Random rand)
        {
            List<Chromosome> population = new List<Chromosome>();

            for (int i = 0; i < populationSize; i++)
            {
                Chromosome chromosome = new Chromosome(numCities);
                chromosome.Gene = Enumerable.Range(0, numCities).OrderBy(x => rand.Next()).ToList();
                population.Add(chromosome);
            }

            return population;
        }

        private static void EvaluatePopulation(List<Chromosome> population, List<City> cities)
        {
            foreach (var chromosome in population)
            {
                chromosome.Fitness = CalculateTotalDistance(chromosome, cities);
            }
        }

        private static double CalculateTotalDistance(Chromosome chromosome, List<City> cities)
        {
            double totalDistance = 0.0;
            for (int i = 0; i < chromosome.Gene.Count - 1; i++)
            {
                totalDistance += cities[chromosome.Gene[i]].DistanceTo(cities[chromosome.Gene[i + 1]]);
            }

            // Dodajemy dystans powrotny do pierwszego miasta
            totalDistance += cities[chromosome.Gene.Last()].DistanceTo(cities[chromosome.Gene.First()]);
            return totalDistance;
        }

        private static List<Chromosome> SelectChromosomes(List<Chromosome> population)
        {
            return population.OrderBy(c => c.Fitness).Take(population.Count / 2).ToList();
        }

        private static List<Chromosome> CrossoverAndMutate(List<Chromosome> selectedChromosomes, double mutationRate, Random rand)
        {
            List<Chromosome> newPopulation = new List<Chromosome>();

            for (int i = 0; i < selectedChromosomes.Count; i += 2)
            {
                Chromosome parent1 = selectedChromosomes[i];
                Chromosome parent2 = selectedChromosomes[i + 1];
                Chromosome offspring1 = new Chromosome(parent1.Gene.Count);
                Chromosome offspring2 = new Chromosome(parent2.Gene.Count);

                int crossoverPoint = rand.Next(1, parent1.Gene.Count - 1);

                offspring1.Gene = parent1.Gene.Take(crossoverPoint).ToList();
                offspring1.Gene.AddRange(parent2.Gene.Except(offspring1.Gene));

                offspring2.Gene = parent2.Gene.Take(crossoverPoint).ToList();
                offspring2.Gene.AddRange(parent1.Gene.Except(offspring2.Gene));

                Mutate(offspring1, mutationRate, rand);
                Mutate(offspring2, mutationRate, rand);

                newPopulation.Add(offspring1);
                newPopulation.Add(offspring2);
            }

            return newPopulation;
        }

        private static void Mutate(Chromosome chromosome, double mutationRate, Random rand)
        {
            for (int i = 0; i < chromosome.Gene.Count; i++)
            {
                if (rand.NextDouble() < mutationRate)
                {
                    int swapIndex = rand.Next(chromosome.Gene.Count);
                    int temp = chromosome.Gene[i];
                    chromosome.Gene[i] = chromosome.Gene[swapIndex];
                    chromosome.Gene[swapIndex] = temp;
                }
            }
        }
    }
}
