using ConsoleApp1;

Console.WriteLine("Tworzymy Siec Neuronowa");
int number_of_layers = 3;
NeuralNetwork siec_Testowa = new NeuralNetwork(number_of_layers);
List<Neuron> mNeurons = new();  //Tu bedzie zczytywane np. z pliku
double[] weights = new double[3];

for(int i =0; i<number_of_layers; i++)
{
    for (int j =0;j < 10; j++)
    {
        weights = new double[3];
        for(int k =0;k < weights.Length; k++)
        {
            Random rnd = new Random();
            weights[k] = rnd.Next(10);
        }
        mNeurons.Add(new Neuron(weights));
    }
    siec_Testowa.setLayer(mNeurons, i);
    mNeurons.Clear();
}
siec_Testowa.getWholeStructure();