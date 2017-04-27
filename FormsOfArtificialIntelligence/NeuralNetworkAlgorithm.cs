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
        private List<double> inputs;
        private List<Layer> neuralLayers;
        private double learningConstant;
        private double[] inputBoard = new double[27];
        private bool innerCall = false;
        public bool FixedWeights = false;

        public NeuralNetworkAlgorithm(int seed, double learningConstant)
        {
            random = new Random(seed);
            neuralLayers = CreateLayers(2, 27);
            this.learningConstant = learningConstant;

            InitializeWeightsOnNeurons(neuralLayers, false);
        }

        public override int MakeMove(List<char> board)
        {
            if (!innerCall && !FixedWeights)
                Reweight(board);
            innerCall = false;

            inputs = GetOutputValues(board);

            return GetIndexForHighestPossibleValue(inputs, board);
        }

        private List<double> GetOutputValues(List<char> board)
        {
            inputs = ConvertBoard(board);

            foreach (var layer in neuralLayers)
            {
                inputs = layer.ProcessInput(inputs);
            }
            return inputs;
        }

        private int GetIndexForHighestPossibleValue(List<double> results, List<char> board)
        {
            List<double> orderedResults = results.OrderByDescending(x => x).ToList();
            for (int i = 0; i < orderedResults.Count; i++)
            {
                if (board[i + 1] != opponentSymbol && board[i + 1] != symbol)
                    return i + 1;
            }
            return -1;
        }

        private List<double> ConvertBoard(List<char> board)
        {
            var inputs = new List<double>();
            //double[] result = new double[27];

            board.RemoveAt(0);

            //for (int i = 0; i < 9; i++)
            //{
            //    if (board[i] == symbol)
            //    {
            //        result[i] = 1;
            //    }
            //    else if (board[i] == opponentSymbol)
            //    {
            //        result[i + 9] = 1;
            //    }
            //    else
            //    {
            //        result[i + 18] = 1;
            //    }
            //}
            //result.ToList().CopyTo(inputBoard);
            //return result.ToList();
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
            for (int i = 1; i < numberLayers; i++)
            {
                List<Neuron> neurons = new List<Neuron>();
                for (int j = 0; j < numberHiddenLayerNeurons; j++)
                {
                    neurons.Add(new Neuron());
                }
                layers.Add(new Layer(){Neurons = neurons});
            }

            var outputNeurons = new List<Neuron>();
            for (int i = 1; i < 28; i++)
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
            MakeMove(board);

            traditionalAi.Symbol = symbol;
            int choice = traditionalAi.MakeMoveNotRandom(board);
            if(choice==-1)
                return;

            board[choice] = symbol;
            List<double> expectedInts = ConvertBoard(board);
            board[choice] = choice.ToString()[0];

            BackPropagate(neuralLayers, expectedInts);
        }


        //https://www.youtube.com/watch?v=zpykfC4VnpM
        private void BackPropagate(List<Layer> layers, List<double> expectedInts)
        {
            int layerCount = 0;
            foreach (var layer in layers)
            {
                int neuronCount = 0;
                foreach (var neuron in layer.Neurons)
                {
                    double neuronLayerBeforeOutput = GetNeuronLayerBeforeOutput(layerCount, neuronCount);

                    neuron.Bias += learningConstant * neuronLayerBeforeOutput * (1 - neuronLayerBeforeOutput) * GetSumOfOutputLayer(layers[layerCount], neuronCount, layer.Neurons.Select(x => x.LatestOutput).ToList(), expectedInts);
                    for (int i = 0; i < neuron.ConnectionWeights.Count; i++)
                    {
                        double weightmodifier;

                        if (layerCount == layers.Count-1)//output nodes get reweighted differently
                        {
                            weightmodifier = neuron.LatestOutput * (1 - neuron.LatestOutput) * (neuron.LatestOutput - expectedInts[i % 9]);
                        }
                        else
                        {
                            weightmodifier = neuron.LatestOutput * (1 - neuron.LatestOutput) * GetSumOfOutputLayer(layers[layerCount], neuronCount, layer.Neurons.Select(x => x.LatestOutput).ToList(), expectedInts);
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

        private double GetSumOfOutputLayer(Layer layer, int neuronNr, List<double> kOut, List<double> expected)
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
                int numberConnections = layerCount != 0 ? Math.Max(layer.Neurons.Count, layers[layerCount-1].Neurons.Count) : layer.Neurons.Count;
                foreach (var neuron in layer.Neurons)
                {
                    //neuron.Bias = random.NextDouble() * 2;

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
