using TaskAllocationProblem;

var (tasks, processors) = TaskAlocationGA.DataGenerator.GenerateData(10, 3);

int populationSize = 100;
int generations = 100;
double mutationRate = 0.01;

var bestChromosome = TaskAlocationGA.EvolutionaryAlgorithm.Run(tasks, processors, populationSize, generations, mutationRate);

TaskAlocationGA.TaskAllocator.AssignTasks(tasks, processors, bestChromosome);

TaskAlocationGA.TaskAllocator.DisplayResults(processors);
