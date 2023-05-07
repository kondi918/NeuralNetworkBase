using NeuralNetworkBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkBase
{
    internal class NeuralNetwork
    {
        public enum WhatActivationFunction
        {
            relu,
            sigmoid
        }
        public WhatActivationFunction whatActivationFunction = WhatActivationFunction.relu;
        public List<Layer> mLayers = new List<Layer>();

        //
        //
        //  Tworzenie sieci
        //
        //z
        public void ReadFromFile(string path)
        {
            mLayers.Clear();
            List<Neuron> neuronSet = new List<Neuron>();
            //
            //WAZNE!! W PLIKU TEKSTOWYM PIERWSZA WAGA KTORA PODAMY (w0) odpowiada za BIAS
            //

            StreamReader sr = new StreamReader(path);           //Obsluga odczytywania struktury sieci z tekstu
            string line = "";
            List<double> weights = new List<double>();
            while (line != null)
            {
                line = sr.ReadLine();
                if (line != null)
                {
                    if (line.Contains("/Layer"))               //Znajduje koniec Layeru i dodaje nowy set Neuronow do Listy oraz zwieksza sie numberofLayers
                    {
                        mLayers.Add(new Layer(neuronSet));
                        neuronSet.Clear();
                    }
                    else if (line.Contains("/Neuron"))              //Znajduje koniec neurona i dodaje go do aktualnego setu
                    {
                        neuronSet.Add(new Neuron(weights));
                    }
                    else if (line.Contains("Neuron"))
                    {
                        weights.Clear();
                    }
                    else if (line.Contains(';'))                //Zczytywanie wag do aktualnego neurona
                    {
                        line = line.Trim();
                        string[] weightsString = line.Split(';');
                        for (int i = 0; i < weightsString.Length; i++)
                        {
                            if (weightsString[i].Contains("."))
                            {
                                weightsString[i] = weightsString[i].Replace(".", ",");
                            }
                            weights.Add(double.Parse(weightsString[i]));
                        }
                    }
                }

            }

            sr.Close();
        }
        public void RemoveWeightsFromNeuron(int layerNumber, int neuronNumber)
        {
            mLayers[layerNumber].mNeurons[neuronNumber].RemoveWeights();
        }
        public void SetNeuronWeights(int layerNumber, int neuronNumber, double[] weights)
        {
            mLayers[layerNumber].mNeurons[neuronNumber].setWeights(weights);
        }
        public void RemoveNeuron(int layerNumber, int neuronNumber)
        {
            mLayers[layerNumber].RemoveNeuron(neuronNumber);
        }

        //Dodawanie pojedycznego neuronu do Layera
        public void AddNeuron(List<double> weights, int layerNumber)
        {
            mLayers[layerNumber].AddNeuron(new Neuron(weights));
        }
        public void AddNeuron(double[] weights, int layerNumber)
        {
            mLayers[layerNumber].AddNeuron(new Neuron(weights));
        }
        public void AddNeuron(int layerNumber)
        {
            mLayers[layerNumber].AddNeuron(new Neuron());
        }
        public void RemoveLayer(int layerNumber)
        {
            mLayers.RemoveAt(layerNumber);
        }
        //Umozliwia rozszerzenie sieci o 1 layer
        public void AddLayer()
        {
            mLayers.Add(new Layer());
        }
        public void AddLayer(Neuron[] neurons)
        {
            mLayers.Add(new Layer(neurons));
        }
        public void AddLayer(List<Neuron> neurons)
        {
            mLayers.Add(new Layer(neurons));
        }
        // Tutaj przekazujemy neurony oraz podajemy numer layeru (0 to pierwszy numer) 
        // neurons to tablica klasy Neuron
        public void SetLayer(Neuron[] neurons, int number_of_layer)
        {
            mLayers[number_of_layer] = new Layer(neurons);
        }
        // Tutaj przekazujemy neurony oraz podajemy numer layeru (0 to pierwszy numer)
        // neurons to lista klasy Neuron  
        public void SetLayer(List<Neuron> neurons, int number_of_layer)
        {
            mLayers[number_of_layer] = new Layer(neurons);
        }

        //
        //
        //  Dzialania na sieci
        //
        //

        private double ActivationFunctionRelu(double x)
        {
            return Math.Max(0, x);
        }
        private double ActivationFunctionSigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp((float)-x));
        }
        private void SetInputs(double[] inputData)
        {
            foreach (var neuron in mLayers[0].mNeurons)
            {
                neuron.setInputData(inputData);
            }
        }
        private double CalculateSingleNeuronResult(Neuron neuron)
        {
            double result = neuron.weights[0];      //Przypisuje liczbe bias
            for (int i = 1; i < neuron.weights.Count; i++)
            {
                result += neuron.weights[i] * neuron.inputData[i - 1];  // input data ma indeks o 1 mniejszy, poniewaz w wagach waga o indeksie 0 to bias
            }
            neuron.neuronResult = result;
            if(whatActivationFunction == WhatActivationFunction.relu)
            {
                return ActivationFunctionRelu(result);
            }
            return ActivationFunctionSigmoid(result);
        }
        private void AddNextLayerInputs(List<double> neuronResults, int index)
        {
            foreach (var neuron in mLayers[index].mNeurons)
            {
                neuron.setInputData(neuronResults);
            }
        }
        private List<double> CalculateNeuronsResults(Layer layer, int nextLayerIndex)
        {
            List<double> neuronsResults = new List<double>();
            foreach (var neuron in layer.mNeurons)
            {
                neuronsResults.Add(CalculateSingleNeuronResult(neuron));
            }
            if (nextLayerIndex != mLayers.Count)
            {
                AddNextLayerInputs(neuronsResults, nextLayerIndex);
            }
            return neuronsResults;
        }
        private double getNetworkResult()
        {
            if (mLayers[mLayers.Count - 1].mNeurons.Count == 1)
            {
                return mLayers[mLayers.Count - 1].mNeurons[0].neuronResult;
            }
            else
            {
                int index = 0;
                for (int i = 0; i < mLayers[mLayers.Count - 1].mNeurons.Count - 1; i++)
                {
                    if (mLayers[mLayers.Count - 1].mNeurons[i].neuronResult < mLayers[mLayers.Count - 1].mNeurons[i + 1].neuronResult)
                    {
                        index = i + 1;
                    }
                }
                return index;
            }
        }
        public double CalculateSmallNetworkResult(double[] inputData)
        {
            SetInputs(inputData);
            int nextLayerIndex = 1;
            foreach (var layer in mLayers)
            {
                CalculateNeuronsResults(layer, nextLayerIndex);
                nextLayerIndex++;
            }
            return getNetworkResult();
        }

        //                          UCZENIE SIECI                             // 



        private double getZeroOneLoss(double neuronResult, int predictedResult)
        {
            double threshold = 0.5;
            if (neuronResult >= threshold)
            {
                return predictedResult == 1 ? 0 : 1;
            }
            else
            {
                return predictedResult == 0 ? 0 : 1;
            }
        }
        private void IncreaseWeights(Neuron neuron, double loss, double learningSpeed)
        {
            for (int i = 1; i < neuron.weights.Count; i++)
            {
                double input = neuron.inputData[i - 1];
                double weight = Math.Abs(neuron.weights[i]);
                double gradient = learningSpeed * loss * input;
                double delta = weight * gradient;
                neuron.weights[i] += Math.Sign(neuron.weights[i]) * delta;
            }
            neuron.weights[0] += loss * learningSpeed;
        }
        private void ReduceWeights(Neuron neuron, double loss, double learningSpeed)
        {
            for (int i = 1; i < neuron.weights.Count; i++)
            {
                double input = neuron.inputData[i - 1];
                double weight = Math.Abs(neuron.weights[i]);
                double gradient = learningSpeed * loss * input;
                double delta = weight * gradient;
                neuron.weights[i] -= Math.Sign(neuron.weights[i]) * delta;
            }
            neuron.weights[0] -= loss * learningSpeed;
        }
        private void Backpropagation(Neuron neuron, int predictedResult, double learningSpeed)
        {
            double loss = getZeroOneLoss(neuron.neuronResult, predictedResult);
            if (predictedResult == 1)
            {
                IncreaseWeights(neuron, loss, learningSpeed);
            }
            else
            {
                ReduceWeights(neuron, loss, learningSpeed);
            }
        }
        public bool NetworkTraining(double[] inputData, int predictedResult, double learningSpeed)
        {
            double networkResult = CalculateSmallNetworkResult(inputData);
            if (mLayers[mLayers.Count - 1].mNeurons.Count == 1)
            {
                if (predictedResult == 1 && networkResult > 0.5 || predictedResult == 0 && networkResult <= 0.5)
                {
                    return true;
                }
            }
            else
            {
                if (predictedResult == networkResult)
                {
                    return true;
                }
            }
            for (int i = mLayers.Count - 1; i >= 0; i--)
            {
                foreach (var neuron in mLayers[i].mNeurons)
                {
                    Backpropagation(neuron, predictedResult, learningSpeed);
                }
            }
            return false;
        }

        //
        //
        //      Konstruktory
        //
        //
        public NeuralNetwork()
        {
        }
        public NeuralNetwork(string path)
        {
            ReadFromFile(path);
        }
        public NeuralNetwork(int number_of_layers)
        {
            for (int i = 0; i < number_of_layers; i++)
            {
                mLayers.Add(new Layer());
            }
        }
        public NeuralNetwork(int number_of_layers, List<Neuron[]> neurons)
        {
            for (int i = 0; i < number_of_layers; i++)
            {
                mLayers.Add(new Layer(neurons[i]));
            }
        }
        //Funkcja, ktora umozliwia wyswietlenie struktury calej sieci neuronowej
        public string GetWholeStructure()
        {
            string result = string.Empty;
            foreach (var element in mLayers)
            {
                result += "<Layer>\n";
                foreach (var neuron in element.mNeurons)
                {
                    result += "<Neuron>\n";
                    foreach (var weight in neuron.weights)
                    {
                        result += weight + ";";
                    }
                    result += "\n</Neuron>\n";
                }
                result += "</Layer>\n";
            }
            return result;
        }
    }
}
