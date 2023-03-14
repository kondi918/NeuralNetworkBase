using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Neuron
    {
        public List<double> weights = new List<double>();
        private double[] input_data;
        private double bias;
        public Neuron(double[] weights)
        {
            for(int i=0; i< weights.Length; i++)
            {
                this.weights.Add(weights[i]);
            }
        }
        public Neuron(List<double> weights)
        {
            for (int i = 0; i < weights.Count; i++)
            {
                this.weights.Add(weights[i]);
            }
        }
        public void read_weights()
        {
            Console.WriteLine("Wagi: ");
            for (int i = 0; i < weights.Count; i++)
            {
                Console.Write(weights[i] + ",");
            }
            Console.WriteLine();
        }

    }
}
