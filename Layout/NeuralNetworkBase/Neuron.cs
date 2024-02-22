using System.Collections.Generic;

namespace NeuralNetworkBase
{
    internal class Neuron
    {
        public List<double> weights = new List<double>();       // w0 = bias
        public List<double> inputData = new List<double>();
        public List<double> weightsChanges = new List<double>();
        public double treshold;
        public double neuronResult { get; set; }
        public double predictedResult {  get; set; }
        public double mistake { get; set; }
        public bool shouldBeIncreased { get; set; }
        public Neuron(double[] weights)
        {
            this.weights.Clear();
            this.weights.AddRange(weights);
        }

        public Neuron(List<double> weights)
        {
            this.weights.Clear();
            this.weights.AddRange(weights);
        }

        public Neuron() { }

        public void setInputData(List<double> inputData)
        {
            this.inputData.Clear();
            this.inputData.AddRange(inputData);
        }

        public void setInputData(double[] inputData)
        {
            this.inputData.Clear();
            this.inputData.AddRange(inputData);
        }

        public List<double> getInputData() { return this.inputData; }

        public void setWeights(List<double> weights)
        {
            this.weights.Clear();
            this.weights.AddRange(weights);
        }

        public List<double> getWeights() { return this.weights; }

        public void RemoveWeights()
        {
            this.weights.Clear();
        }
    }
}
