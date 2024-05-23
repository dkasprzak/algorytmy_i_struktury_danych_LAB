using static KnapsackProblem.KnapsackGA;

Knapsack knapsack = new Knapsack
{
    Capacity = 50,
    Items = new List<Item>
    {
        new Item { Weight = 10, Value = 60 },
        new Item { Weight = 20, Value = 100 },
        new Item { Weight = 30, Value = 120 }
    }
};

GeneticAlgorithm ga = new GeneticAlgorithm(10, 0.8, 0.05, 100, knapsack);
Chromosome bestSolution = ga.Run();

Console.WriteLine("Najlepsze rozwiązanie:");
for (int i = 0; i < bestSolution.Genes.Length; i++)
{
    if (bestSolution.Genes[i])
    {
        Console.WriteLine($"Przedmiot {i + 1}: Waga = {knapsack.Items[i].Weight}, Wartość = {knapsack.Items[i].Value}");
    }
}
Console.WriteLine($"Całkowita wartość: {bestSolution.Fitness}");






