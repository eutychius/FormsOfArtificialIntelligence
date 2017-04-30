using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
    class Program
    {
        private const Int32 NUMBEROFROUNDS = 200;
        private static int numberDraws = 0;
        private static Dictionary<BaseTicTacToeAI, int> playerWins = new Dictionary<BaseTicTacToeAI, int>();
        private static List<double> bestWeights;
        private static double bestScore = -1.0;

        private static int populationNr = 1000;
        private static List<DNA> population = new List<DNA>();
        private const int PopulationSeed = 1;

        static void Main()
        {
            TicTacToe game = new TicTacToe();
            List<BaseTicTacToeAI> players = new List<BaseTicTacToeAI>();
            Random random = new Random(PopulationSeed);

            //insert 2 desired algorithms to players list
            //players.Add(new RandomAlgorithm());
            players.Add(new TraditionalAlgorithm());
            players.Add(new NeuralNetworkAlgorithm(random.Next(), 0.009));
            players[0].Symbol = 'O';
            players[1].Symbol = 'X';

            playerWins.Add(players[0], 0);
            playerWins.Add(players[1], 0);

            NeuralNetworkAlgorithm nn = (NeuralNetworkAlgorithm) players[1];
            population.Add(new DNA() {Genes = nn.GetWeights()});
            //create population randomly
            for (int i = 1; i < populationNr; i++)
            {
                nn = new NeuralNetworkAlgorithm(random.Next(), 0.009);
                population.Add(new DNA() { Genes = nn.GetWeights() });
            }

            for (int genNr = 0; genNr < 400; genNr++)
            {
                for (int popNr = 0; popNr < population.Count; popNr++)
                {
                    nn.SetWeights(population[popNr].Genes);

                    for (int i = 0; i < NUMBEROFROUNDS; i++)
                    {
                        BaseTicTacToeAI winner = game.PlayGame(players, random.Next(2));

                        if (winner == null)
                        {
                            numberDraws++;
                            population[popNr].NumberDraws++;
                        }
                        else
                        {
                            playerWins[winner]++;
                            if (winner is NeuralNetworkAlgorithm)
                                population[popNr].NumberWins++;
                            else
                                population[popNr].NumberLooses++;
                        }
                    }
                    //PrintStats(players, game);

                    playerWins[players[0]] = 0;
                    playerWins[players[1]] = 0;
                    numberDraws = 0;
                }

                double best = (double) population.OrderByDescending(x => x.NumberWins).First().NumberWins / NUMBEROFROUNDS;


                foreach (var dna in population)
                {
                    dna.CalculateFitness();
                }

                Console.WriteLine("Generation {0}: Best Score was {1}", genNr, best);

                //make mating pool
                List<DNA> matingPool = new List<DNA>();

                foreach (var dna in population)
                {
                    for (int fitnessNr = 0; fitnessNr < dna.Fitness; fitnessNr++)//add fitness.. more variety needed? take rank instead
                    {
                        matingPool.Add(dna);
                    }
                }

                if (matingPool.Count == 0)
                {
                    Console.WriteLine("seed was bad, reseeding..");
                    random = new Random(random.Next());
                    population.Clear();
                    for (int i = 1; i < populationNr; i++)
                    {
                        nn = new NeuralNetworkAlgorithm(random.Next(), 0.009);
                        population.Add(new DNA() { Genes = nn.GetWeights() });
                    }
                    continue;
                }

                //mate & mutate
                var nextGeneration = new List<DNA>();
                for (int popNr = 0; popNr < population.Count; popNr++)
                {
                    int a = random.Next(matingPool.Count);
                    int b = random.Next(matingPool.Count);

                    DNA parentA = matingPool[a];
                    DNA parentB = matingPool[b];

                    DNA child = parentA.Crossover(parentB, random);
                    child.mutate(0.01, random);
                    nextGeneration.Add(child);
                }
                population = nextGeneration;
            }


            PrintStats(players, game);
            Console.ReadKey();
        }

        private static void PrintStats(List<BaseTicTacToeAI> players, TicTacToe game)
        {
            int numberOfRounds = playerWins[players[0]] + playerWins[players[1]] + numberDraws;
            players[0].WinRatio = (double) playerWins[players[0]] / numberOfRounds;
            players[1].WinRatio = (double) playerWins[players[1]] / numberOfRounds;

            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("Player  \t Wins\t Ratio");
            Console.WriteLine("{0}\t {1}\t {2}", players[0].AIType, playerWins[players[0]], players[0].WinRatio);
            Console.WriteLine("{0}\t {1}\t {2}", players[1].AIType, playerWins[players[1]], players[1].WinRatio);
            Console.WriteLine();
            Console.WriteLine("Draws: {0}", numberDraws);
            Console.WriteLine();

        }
    }

    
}
