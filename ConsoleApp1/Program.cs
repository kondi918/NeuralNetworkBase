using ConsoleApp1;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics.Tracing;
using System.IO;


StreamWriter logFile = new("plikiTekstowe/rozpoznawanieZdan/logiNauczania.txt", true);    // Tworzymy plik do logowania
NeuralNetwork myNetwork = new NeuralNetwork("plikiTekstowe/dlugopisObraczka/siecPoczatkowa.txt");    //pobieram dane sieci z pliku
List<double[]> trainingData = new List<double[]>();
List<int> trainingResults = new List<int>();

void GetTrainingData(string path)
{
    List<double> data = new List<double>();
    StreamReader sr = new(path);
    string line = sr.ReadLine();
    line = sr.ReadLine();
    while (line != null)
    {

        string[] dataString = line.Split(";");
        for(int i=0; i< dataString.Length-1; i++)
        {
            data.Add(double.Parse(dataString[i]));
        }
        trainingData.Add(data.ToArray());
        trainingResults.Add(Int32.Parse(dataString[dataString.Length - 1]));
        data.Clear();
        line = sr.ReadLine();
    }
    sr.Close();
}
void TrainNetwork()
{
    int bledy = 100;
    while (bledy != 0)
    {
        bledy = 0;
        for(int i= 0; i < trainingData.Count; i++)
        {
            if (!myNetwork.NetworkTraining(trainingData[i], trainingResults[i],0.1))
            {
                bledy++;
            }
        }
        Console.WriteLine(bledy);
    }
}
GetTrainingData("plikiTekstowe/dlugopisObraczka/daneNauczania.txt");
TrainNetwork();
while(1==1)
{
    Console.WriteLine("Podaj dlugosc przedmiotu");
    double[] dane = new double[2];
    dane[0] = double.Parse(Console.ReadLine());
    dane[1] = double.Parse(Console.ReadLine());
    if(myNetwork.CalculateSmallNetworkResult(dane) > 0.5)
    {
        Console.WriteLine("To dlugopis");
    }
    else
    {
        Console.WriteLine("To obraczka");
    }
}

