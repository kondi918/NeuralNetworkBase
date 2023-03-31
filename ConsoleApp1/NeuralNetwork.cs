using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class NeuralNetwork
    {
        private List<List<double>> layersValues = new List<List<double>>();
        private List<Layer> mLayers = new List<Layer>();


        //
        //
        //  Tworzenie sieci
        //
        //



        public void removeNeuron(int layerNumber, int neuronNumber)
        {
            mLayers[layerNumber].removeNeuron(neuronNumber);
        }

        //Dodawanie pojedycznego neuronu do Layera
        public void addNeuron(List<double> weights, int layerNumber)
        {
            mLayers[layerNumber].addNeuron(new Neuron(weights));
        }
        public void addNeuron(double[] weights, int layerNumber)
        {
            mLayers[layerNumber].addNeuron(new Neuron(weights));
        }
        public void removeLayer(int layerNumber)
        {
            mLayers.RemoveAt(layerNumber);
        }
        //Umozliwia rozszerzenie sieci o 1 layer
        public void addLayer()
        {
            mLayers.Add(new Layer());
        }
        public void addLayer(Neuron[] neurons)
        {
            mLayers.Add(new Layer(neurons));
        }
        public void addLayer(List<Neuron> neurons)
        {
            mLayers.Add(new Layer(neurons));
        }
        // Tutaj przekazujemy neurony oraz podajemy numer layeru (0 to pierwszy numer) przekazujemy 
        // neurons to tablica klasy Neuron
        public void setLayer(Neuron[] neurons, int number_of_layer) 
        {
            mLayers[number_of_layer] = new Layer(neurons);
        }
        // Tutaj przekazujemy neurony oraz podajemy numer layeru (0 to pierwszy numer)
        // neurons to lista klasy Neuron  
        public void setLayer(List<Neuron> neurons, int number_of_layer)
        {
            mLayers[number_of_layer] = new Layer(neurons);
        }



        //
        //
        //  Dzialania na sieci
        //
        //
        private int finalNeuronNumber()
        {
            List<double> values = new List<double>();
            for(int i=0; i< mLayers[mLayers.Count-1].mNeurons.Count; i++)
            {
                double resultValue = mLayers[mLayers.Count - 1].mNeurons[i].weights[0];
                for (int j=1; j< mLayers[mLayers.Count - 1].mNeurons[i].weights.Count; j++)
                {
                    resultValue += mLayers[mLayers.Count - 1].mNeurons[i].weights[j] * mLayers[mLayers.Count - 1].mNeurons[i].inputData[j-1];
                }
                resultValue = activationFunctionSigmoid(resultValue);
                values.Add(resultValue);
            }
            layersValues.Add(values);
            return values.IndexOf(values.Max());
        }
        private double activationFunctionSigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp((float)-x));
        }
        public int calculateNetworkResult()
        {
            List<double> neuronsResults = new List<double>();
            for (int i = 0; i < mLayers.Count-1; i++)
            {
                neuronsResults.Clear();
                for(int j=0; j < mLayers[i].mNeurons.Count; j++)
                {
                    double neuronResult = mLayers[i].mNeurons[j].weights[0];    //Dodajemy liczbe bias
                    for (int k = 1; k < mLayers[i].mNeurons[j].weights.Count; k++)
                    {
                        neuronResult += mLayers[i].mNeurons[j].weights[k] * mLayers[i].mNeurons[j].inputData[k-1];
                    }
                    neuronResult = activationFunctionSigmoid(neuronResult);
                    neuronsResults.Add(neuronResult);
                }
                for(int j=0; j< mLayers[i+1].mNeurons.Count; j++)
                {
                    mLayers[i + 1].mNeurons[j].setInputData(neuronsResults);
                }
                layersValues.Add(neuronsResults);
            }
            return finalNeuronNumber();
        }






        //
        //
        //      Konstruktory
        //
        //




        public NeuralNetwork(int number_of_layers)
        {
            for(int i=0; i<number_of_layers; i++)
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
        //Funkcja, ktora umozliwia wyswietlenie struktury calej sieci neuronowej w konsoli
        public void getWholeStructure()
        {
            for(int i =0; i<mLayers.Count; i++)
            {
                Console.WriteLine(" Layer nr: " + (i + 1) + "\n");
                for(int j = 0; j < mLayers[i].mNeurons.Count; j++)
                {
                    Console.WriteLine("Neuron nr: " + (j + 1));
                    mLayers[i].mNeurons[j].read_weights();
                }
            }
        }
    }
}
