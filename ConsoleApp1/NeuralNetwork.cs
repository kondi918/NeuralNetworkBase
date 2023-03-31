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
        // Tutaj przekazujemy neurony oraz podajemy numer layeru (0 to pierwszy numer) 
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
        private double getNeuronResult(int layerIndex, int neuronIndex)
        {
            double neuronResult = mLayers[layerIndex].mNeurons[neuronIndex].weights[0]; //przypisujemy na starcie liczbie bias
            for (int i = 1; i < mLayers[layerIndex].mNeurons[neuronIndex].weights.Count; i++)
            {
                neuronResult += mLayers[layerIndex].mNeurons[neuronIndex].weights[i] * mLayers[layerIndex].mNeurons[neuronIndex].inputData[i - 1];
            }
            return activationFunctionSigmoid(neuronResult);
        }
        public int calculateNetworkResult()
        {
            List<double> neuronsResults = new List<double>();
            for (int i = 0; i < mLayers.Count-1; i++)
            {
                neuronsResults.Clear();
                for(int j=0; j < mLayers[i].mNeurons.Count; j++)
                {
                    neuronsResults.Add(getNeuronResult(i,j));
                }
                for(int j=0; j< mLayers[i+1].mNeurons.Count; j++)
                {
                    mLayers[i + 1].mNeurons[j].setInputData(neuronsResults);
                }
                layersValues.Add(neuronsResults);
            }
            return finalNeuronNumber();
        }


//                          UCZENIE SIECI                             // 

        private void zeroOneLoss()
        { 
        }

        public void teachingNetwork(int numberOfCorrectNeuron)
        {

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
                    mLayers[i].mNeurons[j].readWeights();
                }
            }
        }
    }
}
