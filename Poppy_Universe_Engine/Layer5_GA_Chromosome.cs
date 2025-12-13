using System;
using System.Linq;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Represents a chromosome (solution) in the Genetic Algorithm (GA).
    /// This chromosome holds 4 weights (W1, W2, W3, W4) used for combining the rankings 
    /// of Layer 1 (Base), Layer 2 (Trending), Layer 3 (Matrix), and Layer 4 (NN).
    /// The primary constraint is that all weights must be non-negative and sum to 1.0.
    /// </summary>
    public class Layer5_GA_Chromosome : IComparable<Layer5_GA_Chromosome>
    {
        #region Properties

        /// <summary>
        /// The array containing the four weights for layers 1-4. 
        /// Guaranteed to be normalized (sum to 1.0).
        /// </summary>
        public double[] Weights { get; private set; }

        /// <summary>
        /// Weight for Layer 1 rankings (Base Relevance)
        /// </summary>
        public double W1 => Weights[0];

        /// <summary>
        /// Weight for Layer 2 rankings (Trending Boost)
        /// </summary>
        public double W2 => Weights[1];

        /// <summary>
        /// Weight for Layer 3 rankings (Matrix Preferences)
        /// </summary>
        public double W3 => Weights[2];

        /// <summary>
        /// Weight for Layer 4 rankings (Neural Network Prediction)
        /// </summary>
        public double W4 => Weights[3];

        /// <summary>
        /// Fitness score of this chromosome. 
        /// This value is determined by an external function (the GA's objective function)
        /// and indicates how well this set of weights performs in combining the layers.
        /// </summary>
        public double Fitness { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new chromosome with specified weights (W1-W4).
        /// Weights are immediately clamped to non-negative values and normalized to sum to 1.0.
        /// </summary>
        public Layer5_GA_Chromosome(double w1, double w2, double w3, double w4)
        {
            Weights = new double[4] { w1, w2, w3, w4 };
            Normalize();
            Fitness = 0.0;
        }

        /// <summary>
        /// Creates a new chromosome with a weight array.
        /// </summary>
        /// <param name="weights">Array of 4 weights. Must contain exactly 4 elements.</param>
        public Layer5_GA_Chromosome(double[] weights)
        {
            if (weights == null || weights.Length != 4)
                throw new ArgumentException("Weights array must contain exactly 4 elements");

            Weights = new double[4];
            Array.Copy(weights, Weights, 4);
            Normalize(); // Enforce constraints
            Fitness = 0.0;
        }

        /// <summary>
        /// Creates a new chromosome with random initial weights (used for the initial GA population).
        /// </summary>
        /// <param name="random">Random number generator.</param>
        public Layer5_GA_Chromosome(Random random)
        {
            Weights = new double[4];
            for (int i = 0; i < 4; i++)
            {
                // Assign a random, non-negative value between 0 and 1
                Weights[i] = random.NextDouble();
            }
            Normalize(); // Normalize random values to sum to 1.0
            Fitness = 0.0;
        }

        /// <summary>
        /// Copy constructor for cloning the chromosome.
        /// </summary>
        /// <param name="other">Chromosome instance to copy.</param>
        public Layer5_GA_Chromosome(Layer5_GA_Chromosome other)
        {
            Weights = new double[4];
            // Perform a shallow copy of the weight array contents
            Array.Copy(other.Weights, Weights, 4);
            Fitness = other.Fitness;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Normalizes the Weights array to sum to 1.0 and ensures all weights are non-negative.
        /// This is the primary constraint enforcement for the chromosome.
        /// </summary>
        private void Normalize()
        {
            // 1. Clamp negative values to 0 (non-negativity constraint)
            for (int i = 0; i < 4; i++)
            {
                if (Weights[i] < 0)
                    Weights[i] = 0;
            }

            // 2. Calculate sum
            double sum = Weights.Sum();

            // 3. Normalize to sum to 1
            if (sum < 1e-10) // Check if the sum is effectively zero
            {
                // Fallback: If all weights are zero, assign equal weights (0.25)
                for (int i = 0; i < 4; i++)
                    Weights[i] = 0.25;
            }
            else
            {
                // Normalize by dividing each weight by the total sum
                for (int i = 0; i < 4; i++)
                    Weights[i] /= sum;
            }
        }

        /// <summary>
        /// Mutates this chromosome by adding Gaussian (normal distribution) noise to its weights.
        /// </summary>
        /// <param name="random">Random number generator.</param>
        /// <param name="mutationStrength">Standard deviation of the Gaussian perturbation (controls magnitude of change).</param>
        public void Mutate(Random random, double mutationStrength = 0.05)
        {
            // Apply a perturbation to each gene (weight)
            for (int i = 0; i < 4; i++)
            {
                // Use the Box-Muller transform to generate a pseudo-random number 
                // with a standard normal distribution (mean 0, std dev 1).
                double u1 = random.NextDouble();
                double u2 = random.NextDouble();
                double gaussianNoise = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

                // Apply the noise scaled by the mutation strength
                Weights[i] += gaussianNoise * mutationStrength;
            }

            // The weights might now be negative or sum to more/less than 1.0, so re-normalize
            Normalize();
        }

        /// <summary>
        /// Creates a deep copy of this chromosome (calls copy constructor).
        /// </summary>
        /// <returns>A new, independent instance of the chromosome.</returns>
        public Layer5_GA_Chromosome Clone()
        {
            return new Layer5_GA_Chromosome(this);
        }

        /// <summary>
        /// Compares chromosomes based on their Fitness score (enables sorting the population).
        /// </summary>
        /// <returns>A value indicating the relative order of the objects.</returns>
        public int CompareTo(Layer5_GA_Chromosome other)
        {
            if (other == null) return 1;
            // Compares fitness values. Standard C# comparison is used (ascending order for fitness).
            return this.Fitness.CompareTo(other.Fitness);
        }

        /// <summary>
        /// Returns a formatted string representation of the chromosome's weights and fitness.
        /// </summary>
        public override string ToString()
        {
            return $"Weights: [W1={W1:F4}, W2={W2:F4}, W3={W3:F4}, W4={W4:F4}] | Fitness: {Fitness:F6}";
        }

        #endregion

        #region Static Factory Methods

        /// <summary>
        /// Performs an average (BLX-alpha style) crossover operation between two parents 
        /// to generate two new offspring chromosomes.
        /// </summary>
        /// <param name="parent1">First parent chromosome.</param>
        /// <param name="parent2">Second parent chromosome.</param>
        /// <param name="random">Random number generator.</param>
        /// <returns>A tuple containing the two resulting offspring chromosomes.</returns>
        public static (Layer5_GA_Chromosome, Layer5_GA_Chromosome) Crossover(
            Layer5_GA_Chromosome parent1,
            Layer5_GA_Chromosome parent2,
            Random random)
        {
            // Alpha is the blending factor (0.0 to 1.0)
            double alpha = random.NextDouble();

            double[] offspring1Weights = new double[4];
            double[] offspring2Weights = new double[4];

            for (int i = 0; i < 4; i++)
            {
                // Offspring 1: alpha blending (mixes parent 1 strongly with parent 2 weakly)
                offspring1Weights[i] = alpha * parent1.Weights[i] + (1 - alpha) * parent2.Weights[i];
                // Offspring 2: inverted alpha blending (mixes parent 1 weakly with parent 2 strongly)
                offspring2Weights[i] = (1 - alpha) * parent1.Weights[i] + alpha * parent2.Weights[i];
            }

            // Create new chromosomes; normalization occurs within the constructor
            return (new Layer5_GA_Chromosome(offspring1Weights),
                    new Layer5_GA_Chromosome(offspring2Weights));
        }

        #endregion
    }
}