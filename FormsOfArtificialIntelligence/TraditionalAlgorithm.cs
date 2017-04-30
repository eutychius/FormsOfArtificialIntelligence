using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
    class TraditionalAlgorithm : BaseTicTacToeAI
    {
        
        public override string AIType => "Traditional";

        public override int MakeMove(List<char> board)
        {
            if (board.All(x => x != symbol && x != opponentSymbol))
                return 5;

            int choice = LastMovePossible(board, symbol, opponentSymbol);
            if (choice != -1)
                return choice;

            //block enemy if he has possible last move
            choice = LastMovePossible(board, opponentSymbol, symbol);
            if (choice != -1)
                return choice;

            choice = CheckForUsefulSecondSymbol(board);
            if (choice != -1)
                return choice;

            //for (int i = 1; i < board.Count; i++)
            //{
            //    if (board[i] != symbol && board[i] != opponentSymbol)
            //        return i;
            //}



            return MakeRandomMove(board);
        }

        public int MakeMoveNotRandom(List<char> board)
        {
            int choice = LastMovePossible(board, symbol, opponentSymbol);
            if (choice != -1)
                return choice;

            //block enemy if he has possible last move
            choice = LastMovePossible(board, opponentSymbol, symbol);
            if (choice != -1)
                return choice;

            choice = CheckForUsefulSecondSymbol(board);
            if (choice != -1)
                return choice;

            return -1;
        }

        private int CheckForUsefulSecondSymbol(List<char> board)
        {
            for (int i = 1; i <= 3; i++)//iterate rows
            {
                int asd = i * 3;
                if (board[asd] == symbol && board[asd - 1] != opponentSymbol && board[asd - 2] != opponentSymbol)
                    return asd - 2;
                if (board[asd] == symbol && board[asd - 2] != opponentSymbol && board[asd - 1] != opponentSymbol)
                    return asd - 2;
                if (board[asd - 1] == symbol && board[asd - 2] != opponentSymbol && board[asd] != opponentSymbol)
                    return asd;
            }
            for (int colNr = 1; colNr <= 3; colNr++)//iterate columns
            {
                if (board[colNr] == symbol && board[colNr + 3] != opponentSymbol && board[colNr + 6] != opponentSymbol)
                    return colNr + 6;
                if (board[colNr] != opponentSymbol && board[colNr + 6] == symbol && board[colNr + 3] != opponentSymbol)
                    return colNr + 3;
                if (board[colNr + 3] == symbol && board[colNr + 6] != opponentSymbol && board[colNr] != opponentSymbol)
                    return colNr;
            }
            //diagonal checks
            if (board[1] == symbol && board[5] != opponentSymbol && board[9] != opponentSymbol)
                return 5;
            if (board[5] == symbol && board[9] != opponentSymbol && board[1] != opponentSymbol)
                return 1;
            if (board[1] == symbol && board[9] != opponentSymbol && board[5] != opponentSymbol)
                return 5;

            if (board[3] == symbol && board[5] != opponentSymbol && board[7] != opponentSymbol)
                return 5;
            if (board[5] == symbol && board[7] != opponentSymbol && board[3] != opponentSymbol)
                return 3;
            if (board[7] == symbol && board[3] != opponentSymbol && board[5] != opponentSymbol)
                return 5;

            return -1;
        }

        private int LastMovePossible(List<char> board, char own, char other)
        {
            for (int i = 1; i <= 3; i++)//iterate rows
            {
                int cellNr = i * 3;
                if (board[cellNr] == own && board[cellNr - 1] == own && board[cellNr - 2] != other)
                    return cellNr - 2;
                if (board[cellNr] == own && board[cellNr - 2] == own && board[cellNr - 1] != other)
                    return cellNr - 1;
                if (board[cellNr - 1] == own && board[cellNr - 2] == own && board[cellNr] != other)
                    return cellNr;
            }
            for (int colNr = 1; colNr <= 3; colNr++)//iterate columns
            {
                if (board[colNr] == own && board[colNr + 3] == own && board[colNr + 6] != other)
                    return colNr + 6;
                if (board[colNr] == own && board[colNr + 6] == own && board[colNr + 3] != other)
                    return colNr + 3;
                if (board[colNr + 3] == own && board[colNr + 6] == own && board[colNr] != other)
                    return colNr;
            }
            //diagonal checks
            if (board[1] == own && board[5] == own && board[9] != other)
                return 9;
            if (board[5] == own && board[9] == own && board[1] != other)
                return 1;
            if (board[1] == own && board[9] == own && board[5] != other)
                return 5;

            if (board[3] == own && board[5] == own && board[7] != other)
                return 7;
            if (board[5] == own && board[7] == own && board[3] != other)
                return 3;
            if (board[3] == own && board[7] == own && board[5] != other)
                return 5;

            return -1;
        }
    }
}
