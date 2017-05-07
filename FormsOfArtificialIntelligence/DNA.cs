using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
    class DNA
    {
        public List<double> Genes = new List<double>();

        public int NumberWins;
        public int NumberLooses;
        public int NumberDraws;
        public long Fitness;
        public Random Random;

        public DNA(int seed)
        {
            Random = new Random(seed);
        }

        public void CalculateFitness()
        {
            int numberOfRounds = NumberWins + NumberLooses+ NumberDraws;
            double winRatio = (double)NumberWins / numberOfRounds;
            winRatio *= 100;
            Fitness = (int) (winRatio * winRatio);

            if (Fitness == 0) Fitness = 1;//so matingPool will never be empty
            NumberWins = 0;
            NumberDraws = 0;
            NumberLooses = 0;
        }

        public DNA Crossover(DNA partner)
        {
            DNA child = new DNA(Random.Next());
            for (int i = 0; i < Genes.Count; i++)//coin flip decides if gene comes from parentA or parentB
            {
                int choice = Random.Next(2);

                Genes[i] = choice == 0 ? Genes[i] : partner.Genes[i];
            }

            child.Genes = Genes;
            return child;
        }

        //generate new gene if mutating
        public void Mutate(double mutationRate)
        {
            for (int i = 0; i < Genes.Count; i++)
            {
                if (Random.NextDouble() < mutationRate)
                    Genes[i] = Random.NextDouble() * 3.2 - 1.6;
            }
        }

        public DNA Clone()
        {
            DNA clone = new DNA(Random.Next());
            double[] geneCopy = new double[Genes.Count];
            Genes.CopyTo(geneCopy);
            clone.Genes = geneCopy.ToList();
            clone.NumberWins = NumberWins;
            return clone;
        }
    }
}
