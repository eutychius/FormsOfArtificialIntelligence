using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace FormsOfArtificialIntelligence
{
    class NeuralNetworkAlgorithm : BaseTicTacToeAI
    {
        TraditionalAlgorithm traditionalAi = new TraditionalAlgorithm();

        public override string AIType => "NeuralNetwork";
        private List<Layer> neuralLayers;
        private double learningConstant;
        private double[] inputBoard = new double[18];
        private bool innerCall = false;
        public bool FixedWeights = false;

        public NeuralNetworkAlgorithm(int seed, double learningConstant)
        {
            random = new Random(seed);
            neuralLayers = CreateLayers(2, 18);
            this.learningConstant = learningConstant; //needed for reweighting

            InitializeWeightsOnNeurons(neuralLayers, false);
        }

        public override int MakeMove(List<char> board)
        {
            List<double> inputs = GetOutputValues(board);

            return GetIndexForHighestPossibleValue(inputs, board);
        }

        private List<double> GetOutputValues(List<char> board)
        {
            List<double> inputs = ConvertBoard(board);

            foreach (var layer in neuralLayers)
            {
                inputs = layer.ProcessInput(inputs);
            }
            return inputs;
        }

        private int GetIndexForHighestPossibleValue(List<double> results, List<char> board)
        {
            List<double> orderedResults = results.OrderByDescending(x => x).ToList();
            foreach (var orderedResult in orderedResults)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (orderedResult.Equals(results[i]) && board[i+1] != opponentSymbol && board[i+1] != symbol)
                        return i + 1;
                }
            }
            return -1;
        }

        private List<double> ConvertBoard(List<char> board)
        {
            var inputs = new List<double>();

            board.RemoveAt(0);//first index is unused for visualiziation purposes
            foreach (var cell in board)
            {
                if (cell == symbol)
                    inputs.Add(1);
                else if (cell == opponentSymbol)
                    inputs.Add(0);
                else
                    inputs.Add(0.5);
            }
            inputs.CopyTo(inputBoard);
            board.Insert(0, '0');
            return inputs;
        }
        
        private List<Layer> CreateLayers(int numberLayers, int numberHiddenLayerNeurons)
        {
            var layers = new List<Layer>();
            for (int i = 1; i < numberLayers; i++)//create hidden layers
            {
                List<Neuron> neurons = new List<Neuron>();
                for (int j = 0; j < numberHiddenLayerNeurons; j++)
                {
                    neurons.Add(new Neuron());
                }
                layers.Add(new Layer(){Neurons = neurons});
            }

            var outputNeurons = new List<Neuron>();
            for (int i = 0; i < 9; i++)//we want 9 outputs, 1 for every field
            {
                outputNeurons.Add(new Neuron());
            }
            layers.Add(new Layer(){Neurons = outputNeurons});
            return layers;
        }
        
        private void InitializeWeightsOnNeurons(List<Layer> layers, bool reset)
        {
            int layerCount = 0;
            foreach (var layer in layers)
            {
                int numberConnections = layerCount != 0 ? Math.Max(layer.Neurons.Count, layers[layerCount-1].Neurons.Count) : inputBoard.Length;
                foreach (var neuron in layer.Neurons)
                {
                    //neuron.Bias = random.NextDouble();

                    for (int i = 0; i < numberConnections; i++)
                    {
                        if (reset)
                            neuron.ConnectionWeights[i] = random.NextDouble();
                        else
                            neuron.ConnectionWeights.Add(random.NextDouble());

                    }
                }
                layerCount++;
            }
        }

        public List<double> GetWeights()
        {
            List<double> weights = new List<double>();

            foreach (var layer in neuralLayers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    foreach (var weight in neuron.ConnectionWeights)
                    {
                        weights.Add(weight);
                    }
                }
            }
            return weights;
        }

        public void SetWeights(List<double> weights)
        {
            int weightCount = 0;
            foreach (var layer in neuralLayers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    for (int i = 0; i < neuron.ConnectionWeights.Count; i++)
                    {
                        neuron.ConnectionWeights[i] = weights[weightCount];
                        weightCount++;
                    }
                }
            }
        }
    }
}
