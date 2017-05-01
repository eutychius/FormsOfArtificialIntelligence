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
        private const Int32 NUMBEROFROUNDS = 1000;
        private static int numberDraws = 0;
        private static Dictionary<BaseTicTacToeAI, int> playerWins = new Dictionary<BaseTicTacToeAI, int>();
        private static List<double> winningWeights;
        private static double winningScore = -1.0;
        private static double lowest = double.MaxValue;
        private static double avgPercent;
        private static List<double> nnWinPercentages = new List<double>();

        static void Main()
        {
            TicTacToe game = new TicTacToe();
            List<BaseTicTacToeAI> players = new List<BaseTicTacToeAI>();
            Random random = new Random();

            //insert 2 desired algorithms to players list
            //players.Add(new RandomAlgorithm());
            players.Add(new TraditionalAlgorithm());
            players.Add(new NeuralNetworkAlgorithm(1, 0.009));
            players[0].Symbol = 'O';
            players[1].Symbol = 'X';

            playerWins.Add(players[0], 0);
            playerWins.Add(players[1], 0);

            NeuralNetworkAlgorithm nn = (NeuralNetworkAlgorithm)players[1];

            for (int epos = 0; epos < 300; epos++)
            {
                for (int i = 0; i < NUMBEROFROUNDS; i++)
                {
                    //player.MakeMove(List<char> board) gets called for every AI and turn in PlayGame
                    BaseTicTacToeAI winner = game.PlayGame(players, random.Next(2));//random.Next(2)

                    if (winner == null)
                        numberDraws++;
                    else
                    {
                        playerWins[winner]++;
                    }
                }
                PrintStats(players, game);

                if (nn.WinRatio > winningScore)
                {
                    winningScore = nn.WinRatio;
                    winningWeights = nn.GetWeights();
                }
                lowest = lowest > nn.WinRatio ? nn.WinRatio : lowest;
                avgPercent += nn.WinRatio;
                playerWins[players[0]] = 0;
                playerWins[players[1]] = 0;
                numberDraws = 0;
            }
            Console.WriteLine("Avg Score was {0} after any key: playing 1000 games with it. (lowest: {1}, best: {2})", avgPercent / 400, lowest, winningScore);
            Console.ReadKey();

            nn.FixedWeights = true;
            nn.SetWeights(winningWeights);
            for (int i = 0; i < 1000; i++)
            {
                BaseTicTacToeAI winner = game.PlayGame(players, random.Next(2), false);

                if (winner == null)
                    numberDraws++;
                else
                {
                    playerWins[winner]++;
                }
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
