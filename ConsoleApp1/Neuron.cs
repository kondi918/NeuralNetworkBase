using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Neuron
    {
        public double[] weights;
        private double[] input_data;
        private double bias;
        public Neuron(double[] weights)
        {
            this.weights = weights;
        }
        public void read_weights()
        {
            Console.WriteLine("Wagi: ");
            for (int i = 0; i < weights.Length; i++)
            {
                Console.Write(weights[i] + ",");
            }
            Console.WriteLine();
        }

    }
}
