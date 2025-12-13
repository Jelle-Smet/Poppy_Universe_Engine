using System;
using System.Collections.Generic;
using System.Linq;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Result container for GA optimization, containing the final ranked lists 
    /// per object type and the optimal weights found for each list.
    /// </summary>
    public class Layer5_GA_Result
    {
        // Final combined and ranked lists for each object type
        public List<Layer5_Poppys_GA_Object> Stars { get; set; }
        public List<Layer5_Poppys_GA_Object> Planets { get; set; }
        public List<Layer5_Poppys_GA_Object> Moons { get; set; }

        // Store the optimal weights found by the GA for inspection/persistence
        public Layer5_GA_Chromosome BestStarWeights { get; set; }
        public Layer5_GA_Chromosome BestPlanetWeights { get; set; }
        public Layer5_GA_Chromosome BestMoonWeights { get; set; }
    }

    /// <summary>
    /// Genetic Algorithm Engine for optimizing Layer 5 rank fusion.
    /// It searches for the set of weights (W1-W4) that results in a final ranking 
    /// that best represents the consensus of all input layers.
    /// </summary>
    public class Layer5_Poppys_GA_Engine
    {
        #region Configuration

        // GA Hyperparameters (Control the evolution process)
        private readonly int populationSize = 150;     // Number of candidate solutions per generation
        private readonly int maxGenerations = 100;     // Number of iterations the GA will run
        private readonly double elitePercentage = 0.15; // Percentage of top individuals preserved to the next generation
        private readonly double crossoverRate = 0.75;   // Probability that two parents will cross over
        private readonly double mutationRate = 0.20;    // Probability that an offspring will mutate
        private readonly double mutationStrength = 0.08; // Magnitude of change during mutation

        private readonly Random random;

        #endregion

        #region Constructor

        public Layer5_Poppys_GA_Engine(int? seed = null)
        {
            // Initialize random number generator, optionally with a seed for reproducible results
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Main entry point: Runs GA optimization for all three object types (Stars, Planets, Moons).
        /// Optimization is performed independently for each object type's set of rankings.
        /// </summary>
        /// <param name="inputObjects">A single combined list containing all objects with their L1-L4 ranks.</param>
        /// <returns>Three optimally ranked lists (Stars, Planets, Moons) and the optimal weights used.</returns>
        public Layer5_GA_Result OptimizeRankFusion(List<Layer5_Poppys_GA_Object> inputObjects)
        {
            if (inputObjects == null || inputObjects.Count == 0)
                throw new ArgumentException("Input objects cannot be null or empty");

            // Separate the objects by type for independent optimization
            var stars = inputObjects.Where(o => o.Object_Type.Equals("Star", StringComparison.OrdinalIgnoreCase)).ToList();
            var planets = inputObjects.Where(o => o.Object_Type.Equals("Planet", StringComparison.OrdinalIgnoreCase)).ToList();
            var moons = inputObjects.Where(o => o.Object_Type.Equals("Moon", StringComparison.OrdinalIgnoreCase)).ToList();

            Console.WriteLine($"=== Layer 5 GA Optimization Started ===");
            Console.WriteLine($"Stars: {stars.Count}, Planets: {planets.Count}, Moons: {moons.Count}");

            var result = new Layer5_GA_Result();

            // Run GA and apply optimal weights for Stars
            if (stars.Count > 0)
            {
                Console.WriteLine("\n--- Optimizing Stars ---");
                result.BestStarWeights = RunGA(stars);
                result.Stars = ApplyWeightsAndRank(stars, result.BestStarWeights);
            }

            // Run GA and apply optimal weights for Planets
            if (planets.Count > 0)
            {
                Console.WriteLine("\n--- Optimizing Planets ---");
                result.BestPlanetWeights = RunGA(planets);
                result.Planets = ApplyWeightsAndRank(planets, result.BestPlanetWeights);
            }

            // Run GA and apply optimal weights for Moons
            if (moons.Count > 0)
            {
                Console.WriteLine("\n--- Optimizing Moons ---");
                result.BestMoonWeights = RunGA(moons);
                result.Moons = ApplyWeightsAndRank(moons, result.BestMoonWeights);
            }

            Console.WriteLine("\n=== GA Optimization Complete ===");
            return result;
        }

        #endregion

        #region Private GA Core Methods

        /// <summary>
        /// Runs the genetic algorithm's main loop for a single object type (e.g., all Star objects).
        /// </summary>
        /// <returns>The Layer5_GA_Chromosome with the highest fitness found.</returns>
        private Layer5_GA_Chromosome RunGA(List<Layer5_Poppys_GA_Object> objects)
        {
            // 1. Initialize population randomly
            List<Layer5_GA_Chromosome> population = InitializePopulation();

            // 2. Evaluate initial fitness for all chromosomes
            foreach (var chromosome in population)
            {
                chromosome.Fitness = EvaluateFitness(chromosome, objects);
            }

            // 3. Track the best chromosome found so far (bestEver)
            Layer5_GA_Chromosome bestEver = population.OrderByDescending(c => c.Fitness).First().Clone();

            // 4. Evolution loop
            for (int generation = 0; generation < maxGenerations; generation++)
            {
                // Sort population by fitness (highest first)
                population = population.OrderByDescending(c => c.Fitness).ToList();

                // Update bestEver
                if (population[0].Fitness > bestEver.Fitness)
                {
                    bestEver = population[0].Clone();
                }

                // Print progress
                if (generation % 20 == 0 || generation == maxGenerations - 1)
                {
                    Console.WriteLine($"Gen {generation}: Best Fitness = {population[0].Fitness:F6} | {population[0]}");
                }

                // Create the next generation via selection, crossover, and mutation
                population = CreateNextGeneration(population, objects);
            }

            Console.WriteLine($"Final Best: {bestEver}");
            return bestEver;
        }

        /// <summary>
        /// Initializes a random population of chromosomes.
        /// </summary>
        private List<Layer5_GA_Chromosome> InitializePopulation()
        {
            var population = new List<Layer5_GA_Chromosome>(populationSize);

            for (int i = 0; i < populationSize; i++)
            {
                // Layer5_GA_Chromosome constructor generates random, normalized weights
                population.Add(new Layer5_GA_Chromosome(random));
            }

            return population;
        }

        /// <summary>
        /// Creates the next generation using elitism, crossover, and mutation operations.
        /// </summary>
        private List<Layer5_GA_Chromosome> CreateNextGeneration(
            List<Layer5_GA_Chromosome> currentPopulation,
            List<Layer5_Poppys_GA_Object> objects)
        {
            var nextGeneration = new List<Layer5_GA_Chromosome>();

            // 1. Elitism: Directly carry forward the top performers (cloned)
            int eliteCount = (int)(populationSize * elitePercentage);
            for (int i = 0; i < eliteCount; i++)
            {
                nextGeneration.Add(currentPopulation[i].Clone());
            }

            // 2. Fill the remainder of the population
            while (nextGeneration.Count < populationSize)
            {
                // Select two parents using Tournament Selection
                var parent1 = TournamentSelection(currentPopulation);
                var parent2 = TournamentSelection(currentPopulation);

                Layer5_GA_Chromosome offspring1, offspring2;

                // Crossover: Create two new chromosomes based on parents' weights
                if (random.NextDouble() < crossoverRate)
                {
                    (offspring1, offspring2) = Layer5_GA_Chromosome.Crossover(parent1, parent2, random);
                }
                else
                {
                    // No crossover: Children are clones of the parents
                    offspring1 = parent1.Clone();
                    offspring2 = parent2.Clone();
                }

                // Mutation: Introduce small random changes to maintain diversity
                if (random.NextDouble() < mutationRate)
                {
                    offspring1.Mutate(random, mutationStrength);
                }
                if (random.NextDouble() < mutationRate)
                {
                    offspring2.Mutate(random, mutationStrength);
                }

                // Evaluate fitness of new offspring
                offspring1.Fitness = EvaluateFitness(offspring1, objects);
                offspring2.Fitness = EvaluateFitness(offspring2, objects);

                // Add offspring to the next generation list
                nextGeneration.Add(offspring1);
                if (nextGeneration.Count < populationSize)
                {
                    nextGeneration.Add(offspring2);
                }
            }

            return nextGeneration;
        }

        /// <summary>
        /// Tournament selection: Selects the fittest chromosome from a small, random subset (tournament).
        /// This method is used to choose parents for the next generation.
        /// </summary>
        private Layer5_GA_Chromosome TournamentSelection(List<Layer5_GA_Chromosome> population, int tournamentSize = 5)
        {
            Layer5_GA_Chromosome best = null;

            for (int i = 0; i < tournamentSize; i++)
            {
                // Pick a random candidate from the population
                var candidate = population[random.Next(population.Count)];
                // Keep the one with the highest fitness
                if (best == null || candidate.Fitness > best.Fitness)
                {
                    best = candidate;
                }
            }

            return best;
        }

        #endregion

        #region Fitness Evaluation

        /// <summary>
        /// Fitness function: Calculates how well a given set of weights (chromosome) 
        /// creates a consensus ranking by minimizing disagreement across the four input layers.
        /// </summary>
        /// <param name="chromosome">The weights (W1, W2, W3, W4) to evaluate.</param>
        /// <param name="objects">The list of objects with their L1-L4 rank positions.</param>
        /// <returns>A fitness score (Higher is better, ranging from 0.0 to 1.0).</returns>
        private double EvaluateFitness(Layer5_GA_Chromosome chromosome, List<Layer5_Poppys_GA_Object> objects)
        {
            if (objects.Count == 0) return 0.0;

            double totalDisagreement = 0.0;

            foreach (var obj in objects)
            {
                // Get the input rank positions (lower rank is better)
                double rank1 = obj.Layer1_Rank;
                double rank2 = obj.Layer2_Rank;
                double rank3 = obj.Layer3_Rank;
                double rank4 = obj.Layer4_Rank;

                // 1. Compute the weighted average rank (finalScore) for this object
                double finalScore = chromosome.W1 * rank1 +
                                    chromosome.W2 * rank2 +
                                    chromosome.W3 * rank3 +
                                    chromosome.W4 * rank4;

                // 2. Calculate disagreement: Sum of Squared Differences (SSD) 
                // between the weighted final score and the rank position from each individual layer.
                double disagreement = Math.Pow(finalScore - rank1, 2) +
                                      Math.Pow(finalScore - rank2, 2) +
                                      Math.Pow(finalScore - rank3, 2) +
                                      Math.Pow(finalScore - rank4, 2);

                totalDisagreement += disagreement;
            }

            // Calculate the average disagreement across all objects
            double avgDisagreement = totalDisagreement / objects.Count;

            // 3. Convert disagreement (which should be minimized) to fitness (which should be maximized)
            // Function: f(d) = 1 / (1 + d), ensures fitness is between 0 and 1, where 1 is perfect consensus (d=0).
            double fitness = 1.0 / (1.0 + avgDisagreement);

            return fitness;
        }

        #endregion

        #region Final Ranking Application

        /// <summary>
        /// Applies the optimal weights found by the GA to the objects and generates the final ranked list.
        /// </summary>
        /// <param name="objects">The list of objects with their L1-L4 ranks.</param>
        /// <param name="bestChromosome">The optimal set of weights found by the GA.</param>
        /// <returns>The final list of objects, sorted by their L5 consensus rank.</returns>
        private List<Layer5_Poppys_GA_Object> ApplyWeightsAndRank(
            List<Layer5_Poppys_GA_Object> objects,
            Layer5_GA_Chromosome bestChromosome)
        {
            // 1. Calculate final scores using the optimal weights
            foreach (var obj in objects)
            {
                double finalScore = bestChromosome.W1 * obj.Layer1_Rank +
                                    bestChromosome.W2 * obj.Layer2_Rank +
                                    bestChromosome.W3 * obj.Layer3_Rank +
                                    bestChromosome.W4 * obj.Layer4_Rank;

                // Store the final weighted score
                obj.Layer5_FinalScore = finalScore;
            }

            // 2. Sort by final score (ascending order, since lower score = better rank)
            var rankedList = objects.OrderBy(o => o.Layer5_FinalScore).ToList();

            // 3. Assign the final rank position (0-based)
            for (int i = 0; i < rankedList.Count; i++)
            {
                rankedList[i].Layer5_FinalRank = i;
            }

            return rankedList;
        }

        #endregion
    }
}