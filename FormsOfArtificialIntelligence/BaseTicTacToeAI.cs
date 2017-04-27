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
            bool valid = false;
            int choice = 0;
            while (!valid)
            {
                choice = random.Next(9) + 1;
                if (board[choice] != 'X' && board[choice] != 'O')
                    valid = true;
            }
            return choice;
        }
    }
}
