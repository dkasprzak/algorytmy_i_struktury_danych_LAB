﻿namespace TaskAllocationProblem;

public class TaskAlocationGA 
{
        public class Task
        {
            public int Id { get; set; }
            public int ComputationCost { get; set; }

            public Task(int id, int computationCost)
            {
                Id = id;
                ComputationCost = computationCost;
            }
        }

        public class Processor
        {
            public int Id { get; set; }
            public int Capacity { get; set; }
            public List<Task> AssignedTasks { get; set; }
            public int CurrentLoad => AssignedTasks.Sum(t => t.ComputationCost);

            public Processor(int id, int capacity)
            {
                Id = id;
                Capacity = capacity;
                AssignedTasks = new List<Task>();
            }

            public bool CanAssign(Task task)
            {
                return CurrentLoad + task.ComputationCost <= Capacity;
            }

            public void AssignTask(Task task)
            {
                if (CanAssign(task))
                {
                    AssignedTasks.Add(task);
                }
            }

            public void ClearTasks()
            {
                AssignedTasks.Clear();
            }
        }

        public class Chromosome
        {
            public List<int> Gene { get; set; }
            public int Fitness { get; set; }

            public Chromosome(int numberOfTasks)
            {
                Gene = new List<int>(new int[numberOfTasks]);
            }
        }

        public static class DataGenerator
        {
            public static (List<Task>, List<Processor>) GenerateData(int numTasks, int numProcessors)
            {
                Random rand = new Random();
                List<Task> tasks = new List<Task>();
                List<Processor> processors = new List<Processor>();

                for (int i = 0; i < numTasks; i++)
                {
                    tasks.Add(new Task(i, rand.Next(1, 100)));
                }

                for (int i = 0; i < numProcessors; i++)
                {
                    processors.Add(new Processor(i, rand.Next(100, 300)));
                }

                return (tasks, processors);
            }
        }

        public static class EvolutionaryAlgorithm
        {
            public static Chromosome Run(List<Task> tasks, List<Processor> processors, int populationSize, int generations, double mutationRate)
            {
                Random rand = new Random();
                List<Chromosome> population = InitializePopulation(populationSize, tasks.Count, processors.Count, rand);

                for (int generation = 0; generation < generations; generation++)
                {
                    EvaluatePopulation(population, tasks, processors);
                    List<Chromosome> selectedChromosomes = SelectChromosomes(population);

                    // Upewnij się, że liczba wybranych chromosomów jest parzysta
                    if (selectedChromosomes.Count % 2 != 0)
                    {
                        selectedChromosomes.Add(selectedChromosomes[0]);
                    }

                    population = CrossoverAndMutate(selectedChromosomes, processors.Count, mutationRate, rand);
                }

                EvaluatePopulation(population, tasks, processors);
                return population.OrderBy(c => c.Fitness).First();
            }

            private static List<Chromosome> InitializePopulation(int populationSize, int numTasks, int numProcessors, Random rand)
            {
                List<Chromosome> population = new List<Chromosome>();

                for (int i = 0; i < populationSize; i++)
                {
                    Chromosome chromosome = new Chromosome(numTasks);
                    for (int j = 0; j < numTasks; j++)
                    {
                        chromosome.Gene[j] = rand.Next(numProcessors);
                    }

                    population.Add(chromosome);
                }

                return population;
            }

            private static void EvaluatePopulation(List<Chromosome> population, List<Task> tasks, List<Processor> processors)
            {
                foreach (var chromosome in population)
                {
                    foreach (var processor in processors)
                    {
                        processor.ClearTasks();
                    }

                    for (int i = 0; i < tasks.Count; i++)
                    {
                        int processorId = chromosome.Gene[i];
                        processors[processorId].AssignTask(tasks[i]);
                    }

                    chromosome.Fitness = processors.Sum(p => Math.Abs(p.CurrentLoad - p.Capacity));
                }
            }

            private static List<Chromosome> SelectChromosomes(List<Chromosome> population)
            {
                return population.OrderBy(c => c.Fitness).Take(population.Count / 2).ToList();
            }

            private static List<Chromosome> CrossoverAndMutate(List<Chromosome> selectedChromosomes, int numProcessors, double mutationRate, Random rand)
            {
                List<Chromosome> newPopulation = new List<Chromosome>();

                for (int i = 0; i < selectedChromosomes.Count; i += 2)
                {
                    Chromosome parent1 = selectedChromosomes[i];
                    Chromosome parent2 = selectedChromosomes[i + 1];
                    Chromosome offspring1 = new Chromosome(parent1.Gene.Count);
                    Chromosome offspring2 = new Chromosome(parent2.Gene.Count);

                    for (int j = 0; j < parent1.Gene.Count; j++)
                    {
                        if (rand.NextDouble() < 0.5)
                        {
                            offspring1.Gene[j] = parent1.Gene[j];
                            offspring2.Gene[j] = parent2.Gene[j];
                        }
                        else
                        {
                            offspring1.Gene[j] = parent2.Gene[j];
                            offspring2.Gene[j] = parent1.Gene[j];
                        }
                    }

                    Mutate(offspring1, numProcessors, mutationRate, rand);
                    Mutate(offspring2, numProcessors, mutationRate, rand);

                    newPopulation.Add(offspring1);
                    newPopulation.Add(offspring2);
                }

                return newPopulation;
            }

            private static void Mutate(Chromosome chromosome, int numProcessors, double mutationRate, Random rand)
            {
                for (int i = 0; i < chromosome.Gene.Count; i++)
                {
                    if (rand.NextDouble() < mutationRate)
                    {
                        chromosome.Gene[i] = rand.Next(numProcessors);
                    }
                }
            }
        }

        public static class TaskAllocator
        {
            public static void AssignTasks(List<Task> tasks, List<Processor> processors, Chromosome chromosome)
            {
                foreach (var processor in processors)
                {
                    processor.ClearTasks();
                }

                for (int i = 0; i < tasks.Count; i++)
                {
                    int processorId = chromosome.Gene[i];
                    processors[processorId].AssignTask(tasks[i]);
                }
            }

            public static void DisplayResults(List<Processor> processors)
            {
                foreach (var processor in processors)
                {
                    Console.WriteLine($"Processor {processor.Id} (Pojemność: {processor.Capacity}):");
                    foreach (var task in processor.AssignedTasks)
                    {
                        Console.WriteLine($"\tTask {task.Id} (Nakład: {task.ComputationCost})");
                    }
                    Console.WriteLine($"Aktualne obciążenie: {processor.CurrentLoad}\n");
                }
            }
        }
    }