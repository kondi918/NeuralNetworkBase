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
        private List<Neuron> neuronList = new List<Neuron>();
        private List<Layer> mLayers = new List<Layer>();
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
