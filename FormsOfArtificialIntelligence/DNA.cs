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
        public int Fitness;

        public void CalculateFitness()
        {
            int numberOfRounds = NumberWins + NumberLooses+ NumberDraws;
            double winRatio = (double)NumberWins / numberOfRounds;
            winRatio *= 100;
            //Fitness = (int) (winRatio * winRatio);
            Fitness = winRatio.Equals(0) ? 0 : (int) Math.Pow(2, winRatio);

            NumberWins = 0;
            NumberDraws = 0;
            NumberLooses = 0;
        }

        public void mutate(double mutationRate, Random random)
        {
            for (int i = 0; i < Genes.Count; i++)
            {
                if (random.NextDouble() < mutationRate)
                    Genes[i] = random.NextDouble();
            }
        }

        public DNA Crossover(DNA partner, Random random)
        {
            DNA child = new DNA();
            for (int i = 0; i < Genes.Count; i++)//coin flip decides if gene comes from parentA or parentB
            {
                int choice = random.Next(2);

                Genes[i] = choice == 0 ? Genes[i] : partner.Genes[i];
            }
            child.Genes = Genes;
            return child;
        }
    }
}
