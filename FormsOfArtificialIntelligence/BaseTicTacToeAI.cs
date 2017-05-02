using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
    public abstract class BaseTicTacToeAI
    {
        public double WinRatio;
        protected Random random = new Random();
        protected char symbol;
        protected char opponentSymbol;

        public char Symbol
        {
            set
            {
                symbol = value;
                if (Symbol == 'X')
                    opponentSymbol = 'O';
                else
                    opponentSymbol = 'X';
            }
            get { return symbol; }
        }
        public virtual string AIType { get; set; }

        public abstract int MakeMove(List<char> board);

        protected int MakeRandomMove(List<char> board)
        {
            List<int> availableChoices = new List<int>();
            for (int i = 1; i < 10; i++)
            {
                if(board[i] != 'X' && board[i] != 'O')
                    availableChoices.Add(i);
            }

            return availableChoices[random.Next(availableChoices.Count)];
        }
    }
}
