using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
    class TicTacToe
    {
        private List<char> arr;

        public BaseTicTacToeAI PlayGame(List<BaseTicTacToeAI> players, int player, bool showWinBoard = false)
        {
            int choice = 0;
            int flag = 0;
            arr = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            do
            {
                //Console.Clear();
                //Console.WriteLine("Player1:X and Player2:O");
                //Console.WriteLine("\n");
                //if (player % 2 == 0)
                //{
                //    Console.WriteLine("Player 2 Chance");
                //}
                //else
                //{
                //    Console.WriteLine("Player 1 Chance");
                //}
                //Board();

                choice = players[player % 2].MakeMove(arr);

                if (arr[choice] != 'X' && arr[choice] != 'O')
                {
                    if (player % 2 == 0) //if chance is of player 2 then mark O else mark X  
                    {
                        arr[choice] = 'O';
                        player++;
                    }
                    else
                    {
                        arr[choice] = 'X';
                        player++;
                    }
                }
                else //If there is any possition where user want to run and that is already marked then show message and load board again  
                {
                    Console.WriteLine("Sorry the row {0} is already marked with {1}", choice, arr[choice]);
                }
                flag = CheckWin();
            } while (flag != 1 && flag != -1); // tie -> -1, win -> 1

            if (showWinBoard)
            {
                Console.WriteLine();
                Board();
            }

            ResetForPlaying();

            if (flag == 1)
            {
                //Console.WriteLine("Player {0} has won", (player % 2) + 1);
                return players[(player+1) % 2];
            }
            else
            {
                return null;
            }
        }

        private void ResetForPlaying()
        {
            arr = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        }

        private void Board()
        {
            Console.WriteLine("     |     |      ");
            Console.WriteLine("  {0}  |  {1}  |  {2}", arr[1], arr[2], arr[3]);
            Console.WriteLine("_____|_____|_____ ");
            Console.WriteLine("     |     |      ");
            Console.WriteLine("  {0}  |  {1}  |  {2}", arr[4], arr[5], arr[6]);
            Console.WriteLine("_____|_____|_____ ");
            Console.WriteLine("     |     |      ");
            Console.WriteLine("  {0}  |  {1}  |  {2}", arr[7], arr[8], arr[9]);
            Console.WriteLine("     |     |      ");
        }

        private int CheckWin()
        {
            #region Horzontal Winning Condtion
            //Winning Condition For First Row   
            if (arr[1] == arr[2] && arr[2] == arr[3])
            {
                return 1;
            }
            //Winning Condition For Second Row   
            else if (arr[4] == arr[5] && arr[5] == arr[6])
            {
                return 1;
            }
            //Winning Condition For Third Row   
            else if (arr[7] == arr[8] && arr[8] == arr[9])
            {
                return 1;
            }
            #endregion

            #region vertical Winning Condtion
            //Winning Condition For First Column       
            else if (arr[1] == arr[4] && arr[4] == arr[7])
            {
                return 1;
            }
            //Winning Condition For Second Column  
            else if (arr[2] == arr[5] && arr[5] == arr[8])
            {
                return 1;
            }
            //Winning Condition For Third Column  
            else if (arr[3] == arr[6] && arr[6] == arr[9])
            {
                return 1;
            }
            #endregion

            #region Diagonal Winning Condition
            else if (arr[1] == arr[5] && arr[5] == arr[9])
            {
                return 1;
            }
            else if (arr[3] == arr[5] && arr[5] == arr[7])
            {
                return 1;
            }
            #endregion

            #region Checking For Draw
            // If all the cells or values filled with X or O then any player has won the match  
            else if (arr[1] != '1' && arr[2] != '2' && arr[3] != '3' && arr[4] != '4' && arr[5] != '5' && arr[6] != '6' && arr[7] != '7' && arr[8] != '8' && arr[9] != '9')
            {
                return -1;
            }
            #endregion

            else
            {
                return 0;
            }
        }
    }
}
