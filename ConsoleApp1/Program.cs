using ConsoleApp1;





NeuralNetwork nowaSiec = new NeuralNetwork("plikiTekstowe/przykladowaSiec.txt");
nowaSiec.GetWholeStructure();
double[] data = { 1, 2, 3 };
while(!nowaSiec.TeachingNetwork(data, 0, 0.1))
{
}
Console.WriteLine("Wyniki");
nowaSiec.GetWholeStructure();
Console.WriteLine(nowaSiec.CalculateNetworkResult(data).ToString());
Console.ReadLine();

