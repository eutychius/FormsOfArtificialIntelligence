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

        private static int populationNr = 500;
        private static List<DNA> population = new List<DNA>();
        private const int PopulationSeed = 1;
        private static DNA overallBest = new DNA();

        static void Main()
        {
            List<BaseTicTacToeAI> players = new List<BaseTicTacToeAI>();
            Random random = new Random(PopulationSeed);

            //insert 2 desired algorithms to players list
            //players.Add(new RandomAlgorithm());
            players.Add(new RandomAlgorithm());
            players.Add(new NeuralNetworkAlgorithm(random.Next()));
            players[0].Symbol = 'O';
            players[1].Symbol = 'X';

            playerWins.Add(players[0], 0);
            playerWins.Add(players[1], 0);

            NeuralNetworkAlgorithm nn = (NeuralNetworkAlgorithm) players[1];
            population.Add(new DNA() {Genes = nn.GetWeights()});
            //create population randomly
            for (int i = 1; i < populationNr; i++)
            {
                nn = new NeuralNetworkAlgorithm(random.Next());
                population.Add(new DNA() { Genes = nn.GetWeights() });
            }

            for (int genNr = 0; genNr < NUMBEROFGENERATIONS; genNr++)
            {
                Parallel.ForEach(population, (individual) =>
                {
                    var game = new TicTacToe();
                    var randomParallel = new Random();
                    BaseTicTacToeAI[] playersParallel = new BaseTicTacToeAI[]
                    {
                        new RandomAlgorithm() {Symbol = 'O'}, new NeuralNetworkAlgorithm(1) {Symbol = 'X'}, 
                    };

                    ((NeuralNetworkAlgorithm)playersParallel[1]).SetWeights(individual.Genes);

                    for (int i = 0; i < NUMBEROFROUNDS; i++)
                    {
                        BaseTicTacToeAI winner = game.PlayGame(playersParallel.ToList(), randomParallel.Next(2));

                        if (winner == null)
                        {
                            individual.NumberDraws++;
                        }
                        else
                        {
                            if (winner is NeuralNetworkAlgorithm)
                                individual.NumberWins++;
                            else
                                individual.NumberLooses++;
                        }
                    }
                });

              

                DNA best = population.OrderByDescending(x => x.NumberWins).First();
                double bestWins = best.NumberWins;

                if (bestWins > overallBest.NumberWins)
                    overallBest = best;

                foreach (var dna in population)
                {
                    dna.CalculateFitness();
                }

                Console.WriteLine("Generation {0}: Best Score was {1}"/*".. Playing 1000 games with it now:"*/, genNr, bestWins / NUMBEROFROUNDS);
                //nn.SetWeights(best.Genes);

                //for (int i = 0; i < 1000; i++)
                //{
                //    BaseTicTacToeAI winner = new TicTacToe().PlayGame(players, random.Next(2));
                //    if (winner == null)
                //    {
                //        numberDraws++;
                //    }
                //    else
                //    {
                //        playerWins[winner]++;
                //    }
                //}
                //PrintStats(players);

                //playerWins[players[0]] = 0;
                //playerWins[players[1]] = 0;
                //numberDraws = 0;

                //make mating pool
                List<DNA> matingPool = new List<DNA>();

                foreach (var dna in population)
                {
                    for (int fitnessNr = 0; fitnessNr < dna.Fitness; fitnessNr++)//add times fitness.. more variety needed? take rank instead
                    {
                        matingPool.Add(dna);
                    }
                }

                //mate & mutate
                int matingPoolCount = matingPool.Count;
                var nextGeneration = new List<DNA>();
                for (int popNr = 0; popNr < population.Count; popNr++)
                {
                    int a = random.Next(matingPoolCount);
                    int b = random.Next(matingPoolCount);

                    DNA parentA = matingPool[a];
                    DNA parentB = matingPool[b];

                    DNA child = parentA.Crossover(parentB, random);
                    child.mutate(0.015, random);
                    nextGeneration.Add(child);
                }
                population.Clear();
                population = nextGeneration;
            }

            //nn.SetWeights(best.Genes);

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
