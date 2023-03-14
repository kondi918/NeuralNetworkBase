using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class NeuralNetwork
    {
        private List<Neuron> neuronList = new List<Neuron>();
        private Layer[] mLayers;
        private int get_neurons_count(int index)
        {
            Console.WriteLine("Podaj liczbe neuronow w warstwie nr: " + (index + 1));
            int number = 0;
            int.TryParse(Console.ReadLine(), out number);
            return number;
        }
        private int get_weights_count(int index)
        {
            int weights_count = 0;
            Console.WriteLine("Podaj ilosc wag w neuronie nr. " + (index+1));
            int.TryParse(Console.ReadLine(), out weights_count);
            return weights_count;
        }
        public void setLayer(Neuron[] neurons, int number_of_layers)
        {
            mLayers[number_of_layers] = new Layer(neurons);
        }
        public NeuralNetwork(string path)   //TRZEBA ZROBIC OBSLUGE UZYWANIA GOTOWEJ SIECI, ZAMIAST TWORZENIA NOWEJ
        {

        }
        public NeuralNetwork(int number_of_layers)
        {
            mLayers = new Layer[number_of_layers];
        }
        public void get_structure()
        {
            for(int i =0; i<mLayers.Length; i++)
            {
                Console.WriteLine(" Layer nr: " + (i + 1) + "\n");
                for(int j = 0; j < mLayers[i].mNeurons.Length; j++)
                {
                    Console.WriteLine("Neuron nr: " + (j + 1));
                    mLayers[i].mNeurons[j].read_weights();
                }
            }
        }
    }
}
