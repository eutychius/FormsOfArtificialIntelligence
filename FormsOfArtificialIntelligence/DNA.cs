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
            Fitness = (int) (winRatio * winRatio) + NumberDraws / 4;
            //Fitness = winRatio.Equals(0) ? 0 : (int) Math.Pow(2, winRatio);
            if (Fitness == 0) Fitness = 1;
            NumberWins = 0;
            NumberDraws = 0;
            NumberLooses = 0;
        }

        public void mutate(double mutationRate, Random random)
        {
            for (int i = 0; i < Genes.Count; i++)
            {
                if (random.NextDouble() < mutationRate)
                    Genes[i] = random.NextDouble() * 2 - 1;
            }
        }

        public DNA Crossover(DNA partner, Random random)
        {
            DNA child = new DNA();
            //for (int i = 0; i < Genes.Count; i++)//coin flip decides if gene comes from parentA or parentB
            //{
            //    int choice = random.Next(2);

            //    Genes[i] = choice == 0 ? Genes[i] : partner.Genes[i];
            //}

            //for (int i = Genes.Count / 2; i < Genes.Count; i++)// half/half genes
            //{
            //    Genes[i] = partner.Genes[i];
            //}
            for (int i = Genes.Count / 2; i < Genes.Count; i++)
            {
                Genes[i] = (partner.Genes[i] + Genes[i]) / 2;
            }
            child.Genes = Genes;
            return child;
        }
    }
}
