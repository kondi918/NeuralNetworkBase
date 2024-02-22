using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeuralNetworkBase
{
    internal class NeuralNetwork
    {
        public enum WhatActivationFunction
        {
            relu,
            sigmoid,
            zeroOne
        }
        public WhatActivationFunction whatActivationFunction = WhatActivationFunction.relu;
        public List<Layer> mLayers = new List<Layer>();
        
        //
        //
        //  Tworzenie sieci
        //
        //
        private void ReadFromTxt(string path)
        {
            List<Neuron> neuronSet = new List<Neuron>();
            if (path != null)
            {
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
        }
        public void ReadFromFile(string path)
        {
            mLayers.Clear();
            //
            //WAZNE!! W PLIKU TEKSTOWYM PIERWSZA WAGA KTORA PODAMY (w0) odpowiada za BIAS
            //
            ReadFromTxt(path);
        }
        public void RemoveWeightsFromNeuron(int layerNumber, int neuronNumber)
        {
            mLayers[layerNumber].mNeurons[neuronNumber].RemoveWeights();
        }
        public void SetNeuronWeights(int layerNumber, int neuronNumber, List<double> weights)
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
            return 1 / (1 + Math.Exp(-x));
        }
        private double ActivationFunctionZeroOne(double x)
        {
            if(x > 0)
            {
                return 1;
            }
            return 0;
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
            double result = -neuron.weights[0];      //Przypisuje liczbe bias
            for (int i = 1; i < neuron.weights.Count; i++)
            {
                result += neuron.weights[i] * neuron.inputData[i - 1];  // input data ma indeks o 1 mniejszy, poniewaz w wagach waga o indeksie 0 to bias
            }
            if (whatActivationFunction == WhatActivationFunction.relu)
            {
                neuron.neuronResult = ActivationFunctionRelu(result);
            }
            else if (whatActivationFunction == WhatActivationFunction.sigmoid)
            {
                neuron.neuronResult = ActivationFunctionSigmoid(result);
            }
            else if(whatActivationFunction == WhatActivationFunction.zeroOne)
            {
                neuron.neuronResult = ActivationFunctionZeroOne(result);
            }
            return neuron.neuronResult;
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
            if (nextLayerIndex < mLayers.Count)
            {
                AddNextLayerInputs(neuronsResults, nextLayerIndex);
            }
            return neuronsResults;
        }
        private double GetNetworkResult()
        {
            if (mLayers[mLayers.Count - 1].mNeurons.Count == 1)
            {
                return mLayers[mLayers.Count - 1].mNeurons[0].neuronResult;
            }
            else
            {
                int index = 0;
                double max = double.Parse(mLayers[mLayers.Count - 1].mNeurons[0].neuronResult.ToString("N0"));
                for (int i = 0; i < mLayers[mLayers.Count - 1].mNeurons.Count; i++)
                {
                    if (max < double.Parse(mLayers[mLayers.Count - 1].mNeurons[i].neuronResult.ToString("N0")))
                    {
                        index = i;
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
            return GetNetworkResult();
        }


        //                          UCZENIE SIECI                             // 
        private double GetDerivativeSigmoid(double sum, double output)
        {
            return sum * output * (1 - output);
        }
        private void SetOutputMistakes(List<Neuron> neurons)
        {
            foreach(var neuron in neurons)
            {
               neuron.mistake = neuron.neuronResult - neuron.predictedResult;
            }
        }
        private void SetHiddenLayersMistakes()
        {
            for (int layers = mLayers.Count - 1; layers > 0; layers --)
            {
                for(int i =0; i < mLayers[layers-1].mNeurons.Count; i++)
                {
                    double sum = 0;
                    foreach(var neuron in mLayers[layers].mNeurons)
                    {
                        sum += neuron.mistake * neuron.weights[i+1];        // +1 bo bias ma index 0
                    }
                    mLayers[layers - 1].mNeurons[i].mistake = GetDerivativeSigmoid(sum, mLayers[layers - 1].mNeurons[i].neuronResult);
                }
            }
        }
        private void SetMistakes()
        {
            SetOutputMistakes(mLayers[mLayers.Count - 1].mNeurons);
            SetHiddenLayersMistakes();
        }
        private void ChangeWeightsLargeNetwork(Neuron neuron, double learningSpeed)
        {
            for (int i = 1; i < neuron.weights.Count; i++)
            {
                double newWeight = neuron.weights[i] - learningSpeed * neuron.inputData[i - 1] * neuron.mistake;
                if (!double.IsNaN(newWeight) && !double.IsInfinity(newWeight))
                {
                    neuron.weights[i] = newWeight;
                }
            }
            neuron.weights[0] += neuron.mistake * learningSpeed;
        }
        private void BackpropagationMultiNeuronOutput(double learningSpeed)
        {
            SetMistakes();
            for(int i= mLayers.Count-1; i >= 0; i--)
            {
                foreach (var neuron in mLayers[i].mNeurons)
                {
                    ChangeWeightsLargeNetwork(neuron, learningSpeed);
                }
            }
        }
        private void ChangeWeight(Neuron neuron, double mistake, double learningSpeed)
        {
            for (int i = 1; i < neuron.weights.Count; i++)
            {
                double newWeight = neuron.weights[i] - learningSpeed * neuron.inputData[i - 1] * mistake;
                if (!double.IsNaN(newWeight) && !double.IsInfinity(newWeight))
                {
                    neuron.weights[i] = newWeight;
                }
            }
            neuron.weights[0] += mistake * learningSpeed;
        }
        private void BackpropagationSingleOutput(Neuron neuron, int predictedResult, double learningSpeed)
        {
            double mistake = neuron.neuronResult - predictedResult;
            ChangeWeight(neuron, mistake, learningSpeed);
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
                for (int i = mLayers.Count - 1; i >= 0; i--)
                {
                    foreach (var neuron in mLayers[i].mNeurons)
                    {
                        BackpropagationSingleOutput(neuron, predictedResult, learningSpeed);
                    }
                }
            }
            else
            {
                foreach (var neuron in mLayers[mLayers.Count - 1].mNeurons)          //przekazywany parametr predicted result to index tego neuronu na ostatniej warstwie, ktory powinien wyjsc
                {
                    neuron.predictedResult = 0;
                }
                mLayers[mLayers.Count-1].mNeurons[predictedResult].predictedResult = 1;
                BackpropagationMultiNeuronOutput(learningSpeed);
                if(predictedResult == networkResult)
                {
                    return true;
                }
            }
            return false;
        }

        //
        //
        //      Konstruktory
        //
        //
        public NeuralNetwork(){}
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
                    result = result.Remove(result.Length - 1);
                    result += "\n</Neuron>\n";
                }
                result += "</Layer>\n";
            }
            return result;
        }
    }
}
