using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
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

        //class neuron
        public double FeedForward(List<double> inputs)
        {
            double sum = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                sum += ConnectionWeights[i] * inputs[i];
            }

            LatestOutput = Transfer(sum + Bias);

            return LatestOutput;
        }

        //Sigmoid Function
        public double Transfer(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

    }
}
