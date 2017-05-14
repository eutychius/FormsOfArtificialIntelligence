using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
    class Program
    {
        private const Int32 NUMBEROFROUNDS = 200;
        private const Int32 NUMBEROFGENERATIONS = 5000;
        private static int numberDraws = 0;
        private static Dictionary<BaseTicTacToeAI, int> playerWins = new Dictionary<BaseTicTacToeAI, int>();
        private static List<double> bestWeights;
        private static double bestScore = -1.0;

        private static int populationNr = 1000;
        private static List<DNA> Population = new List<DNA>();
        private const int PopulationSeed = 2;
        private static DNA overallBest = new DNA(0);
        private static double overallBestWins;

        static void Main()
        {
            List<BaseTicTacToeAI> players = new List<BaseTicTacToeAI>();
            Random random = new Random(PopulationSeed);

            //insert 2 desired algorithms to players list
            players.Add(new TraditionalAlgorithm());
            players.Add(new NeuralNetworkAlgorithm(random.Next()));
            players[0].Symbol = 'O';
            players[1].Symbol = 'X';

            playerWins.Add(players[0], 0);
            playerWins.Add(players[1], 0);

            NeuralNetworkAlgorithm nn = (NeuralNetworkAlgorithm) players[1];

            //create population randomly
            for (int i = 0; i < populationNr; i++)
            {
                nn = new NeuralNetworkAlgorithm(random.Next());
                Population.Add(new DNA(random.Next()) { Genes = nn.GetWeights() });
            }

            for (int genNr = 0; genNr < NUMBEROFGENERATIONS; genNr++)
            {
                Parallel.ForEach(Population, (individual) =>
                {
                    var game = new TicTacToe();
                    BaseTicTacToeAI[] playersParallel =
                    {
                        new TraditionalAlgorithm() {Symbol = 'O'},
                        new NeuralNetworkAlgorithm(99) {Symbol = 'X'},
                    };

                    ((NeuralNetworkAlgorithm)playersParallel[1]).SetWeights(individual.Genes);

                    for (int i = 0; i < NUMBEROFROUNDS; i++)
                    {
                        BaseTicTacToeAI winner = game.PlayGame(playersParallel.ToList(), 1);// individual.Random.Next(2));

                        if (winner == null)
                            individual.NumberDraws++;
                        else
                        {
                            if (winner is NeuralNetworkAlgorithm)
                                individual.NumberWins++;
                            else
                                individual.NumberLooses++;
                        }
                    }
                });

                IfKeyPressedPlayGamesWithBest(players, random);
                
                Population = Population.OrderByDescending(x => x.NumberWins).ToList();
                DNA best = Population.First();
                double bestWins = best.NumberWins;

                if (bestWins > overallBestWins)
                {
                    overallBest = best.Clone();
                    overallBestWins = bestWins;
                }

                foreach (var dna in Population)
                    dna.CalculateFitness();

                Console.WriteLine("Generation {0}: Best Score was {1} \t\t Overall best: {2}", genNr, bestWins / NUMBEROFROUNDS, overallBestWins /NUMBEROFROUNDS);


                //make mating pool
                var matingPool = CreateMatingPool(Population);


                //mate & Mutate
                var nextGeneration = CreateNextGeneration(matingPool, random);
                Population = nextGeneration;
            }
            //End of genetic algorithm


            Console.WriteLine("best player had {0} wins, now playing", overallBest.NumberWins);
            nn.SetWeights(overallBest.Genes);
            players[1] = nn;
            for (int i = 0; i < 10000; i++)
            {
                BaseTicTacToeAI winner = new TicTacToe().PlayGame(players, random.Next(2));
                if (winner == null)
                {
                    numberDraws++;
                }
                else
                {
                    playerWins[winner]++;
                }
            }

            PrintStats(players);
            Console.ReadKey();
        }

        private static List<DNA> CreateNextGeneration(List<DNA> matingPool, Random random)
        {
            int matingPoolCount = matingPool.Count;
            var nextGeneration = new List<DNA>();
            for (int popNr = 0; popNr < populationNr; popNr++)
            {
                int a = random.Next(matingPoolCount);
                int b = random.Next(matingPoolCount);

                DNA parentA = matingPool[a];
                DNA parentB = matingPool[b];
                if (parentA == parentB)//skip same parents
                {
                    popNr--;
                    continue;
                }

                DNA child = parentA.Crossover(parentB);
                child.Mutate(0.01);
                nextGeneration.Add(child);
            }
            return nextGeneration;
        }

        private static List<DNA> CreateMatingPool(List<DNA> population)
        {
            List<DNA> matingPool = new List<DNA>();
            for (int i = 0; i < populationNr; i++)
            {
                double fitness = population[i].Fitness;
                for (int fitnessNr = 0; fitnessNr < fitness; fitnessNr++) //add times fitness
                {
                    matingPool.Add(population[i]);
                }
            }
            return matingPool;
        }

        private static void IfKeyPressedPlayGamesWithBest(List<BaseTicTacToeAI> players, Random random)
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey();
                ((NeuralNetworkAlgorithm) players[1]).SetWeights(overallBest.Genes);
                for (int i = 0; i < 10000; i++)
                {
                    BaseTicTacToeAI winner = new TicTacToe().PlayGame(players, random.Next(2));
                    if (winner == null)
                    {
                        numberDraws++;
                    }
                    else
                    {
                        playerWins[winner]++;
                    }
                }

                PrintStats(players);
                playerWins[players[0]] = 0;
                playerWins[players[1]] = 0;
                numberDraws = 0;
            }
        }

        private static void PrintStats(List<BaseTicTacToeAI> players)
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
