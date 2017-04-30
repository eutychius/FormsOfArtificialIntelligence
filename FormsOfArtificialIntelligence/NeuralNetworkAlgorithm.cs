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
            if (!innerCall && !FixedWeights)
                Reweight(board);
            innerCall = false;

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

        public void Reweight(List<char> board)
        {
            //InitializeWeightsOnNeurons(neuralLayers, true); //Random Reweighting/training
            innerCall = true;
            int ownChoice = MakeMove(board);

            traditionalAi.Symbol = symbol;
            int choice = traditionalAi.MakeMoveNotRandom(board);
            if(choice==-1 /*|| ownChoice == choice*/)
                return;

            double[] expectedDoubles = new double[9];

            //for (int i = 0; i < expectedDoubles.Length; i++)
            //{
            //    expectedDoubles[i] = 0.2;
            //}
            expectedDoubles[choice-1] = 1;

            BackPropagate(neuralLayers, expectedDoubles);
        }


        //https://www.youtube.com/watch?v=zpykfC4VnpM
        private void BackPropagate(List<Layer> layers, double[] expected)
        {
            int layerCount = 0;
            foreach (var layer in layers)
            {
                int neuronCount = 0;
                foreach (var neuron in layer.Neurons)
                {
                    double neuronLayerBeforeOutput = GetNeuronLayerBeforeOutput(layerCount, neuronCount);

                    //neuron.Bias -= learningConstant * neuronLayerBeforeOutput * (1 - neuronLayerBeforeOutput) * GetOutputSumOfLayer(layers[layerCount], neuronCount, layer.Neurons.Select(x => x.LatestOutput).ToList(), expected);
                    for (int i = 0; i < neuron.ConnectionWeights.Count; i++)
                    {
                        double weightmodifier;

                        if (layerCount == layers.Count-1)//output nodes get reweighted differently
                        {
                            weightmodifier = neuron.LatestOutput * (1 - neuron.LatestOutput) * (neuron.LatestOutput - expected[i % 9]);
                        }
                        else
                        {
                            weightmodifier = neuron.LatestOutput * (1 - neuron.LatestOutput) * GetOutputSumOfLayer(layers[layerCount], neuronCount, layer.Neurons.Select(x => x.LatestOutput).ToList(), expected);
                        }
                        neuron.ConnectionWeights[i] -= weightmodifier * learningConstant * neuronLayerBeforeOutput;
                    }
                    neuronCount++;
                }
                layerCount++;
            }
        }

        private double GetNeuronLayerBeforeOutput(int layerCount, int neuronCount)
        {
            if (layerCount == 0)
                return inputBoard[neuronCount % 9];

            return neuralLayers[layerCount - 1].Neurons[neuronCount].LatestOutput;
        }

        private double GetOutputSumOfLayer(Layer layer, int neuronNr, List<double> kOut, double[] expected)
        {
            double sum = 0;
            foreach (var neuron in layer.Neurons)
            {
                sum += kOut[neuronNr%9] * (1 - kOut[neuronNr%9]) * (kOut[neuronNr%9] - expected[neuronNr%9]) * neuron.ConnectionWeights[neuronNr];
            }
            return sum;
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
