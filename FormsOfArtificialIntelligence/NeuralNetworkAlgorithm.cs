using System;
using System.Collections.Generic;
using System.Linq;

namespace FormsOfArtificialIntelligence
{
    class NeuralNetworkAlgorithm : BaseTicTacToeAI
    {
        public override string AIType => "NeuralNetwork";
        private List<Layer> neuralLayers;
        private readonly double[] inputBoard = new double[18];

        public NeuralNetworkAlgorithm(int seed)
        {
            random = new Random(seed);
            neuralLayers = CreateLayers(2, 18);

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
                            neuron.ConnectionWeights[i] = random.NextDouble() * 3.2 - 1.6;
                        else
                            neuron.ConnectionWeights.Add(random.NextDouble() * 3.2 - 1.6);

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
            double[] result = new double[weights.Count];
            weights.CopyTo(result);
            return result.ToList();
        }

        public void SetWeights(List<double> weights)
        {
            double[] newWeights = new double[weights.Count];
            weights.CopyTo(newWeights);
            int weightCount = 0;
            foreach (var layer in neuralLayers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    for (int i = 0; i < neuron.ConnectionWeights.Count; i++)
                    {
                        neuron.ConnectionWeights[i] = newWeights[weightCount];
                        weightCount++;
                    }
                }
            }
        }
    }
}
