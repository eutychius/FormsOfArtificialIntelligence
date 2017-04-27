using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
    class RandomAlgorithm : BaseTicTacToeAI
    {
        public override string AIType => "Random";

        public override int MakeMove(List<char> board)
        {
            return MakeRandomMove(board);
        }
    }
}
