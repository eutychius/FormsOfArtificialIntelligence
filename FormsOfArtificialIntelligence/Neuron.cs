using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
    class Connection
    {
        public double Weight;
        public Neuron NeuronLeft;
        //public Neuron NeuronRight;
    }

    class Layer
    {
        public List<Neuron> Neurons;

        public List<double> ProcessInput(List<double> inputs)
        {
            List<double> outputs = new List<double>();
            foreach (var neuron in Neurons)
            {
                outputs.Add(neuron.FeedForward(inputs));
            }
            return outputs;
        }
    }

    class Neuron
    {
        public double LatestOutput;

        public double Bias = 0;
        public List<double> ConnectionWeights = new List<double>();

        public double FeedForward(List<double> board)
        {
            double sum = 0;
            for (int i = 0; i < board.Count; i++)
            {
                sum += ConnectionWeights[i] * board[i] + Bias;
            }
            LatestOutput = Transfer(sum);

            return LatestOutput;
        }

        //Sigmoid Function
        public double Transfer(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

    }
}
