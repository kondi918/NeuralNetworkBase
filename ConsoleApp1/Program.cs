using ConsoleApp1;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics.Tracing;


StreamWriter logFile = new("plikiTekstowe/rozpoznawanieZdan/logiNauczania.txt", true);    // Tworzymy plik do logowania
NeuralNetwork myNetwork = new NeuralNetwork("plikiTekstowe/dlugopisObraczka/siecPoczatkowa.txt");    //pobieram dane sieci z pliku
List<double[]> data = new List<double[]>();
List<int> dataResult = new List<int>();
/*
void getData()
{
    StreamReader sr = new("plikiTekstowe/TableTennis/daneNauczania.txt");
    string line = sr.ReadLine();
    line = sr.ReadLine();
    while(line != null)
    {
        string[] lineSplit = line.Split(';');
        double[] lineData = new double[3];
        for(int i =0; i < lineSplit.Length-1; i++)
        {
            lineData[i] = double.Parse(lineSplit[i]);
        }
        data.Add(lineData);
        dataResult.Add(Int32.Parse(lineSplit[3]));
        line = sr.ReadLine();
    }
}
*/
void TrainNetwork()
{
    logFile.WriteLine("\nData rozpoczęcia logow: " + DateTime.Now);
    int minBledow = 100;
    int bledy = 100;
    while (bledy >= 10)
    {
        bledy = 0;
        for(int i= 0; i < data.Count; i++)
        {
            if (!myNetwork.NetworkTraining(data[i], dataResult[i],0.1))
            {
                bledy++;
            }
        }
        if(bledy < minBledow)
        {
            minBledow = bledy;
            logFile.WriteLine("Nowa Struktura: ");
            logFile.WriteLine(myNetwork.GetWholeStructure());
            logFile.WriteLine("-----------------------------------------------------");
        }
        Console.WriteLine("Ilosc bledow: " + bledy);
        Console.WriteLine(myNetwork.layersValues[myNetwork.layersValues.Count - 1][0]);
        Console.WriteLine(myNetwork.layersValues[myNetwork.layersValues.Count - 1][1]);
    }
    foreach(var d in data) 
    {
        /*
        Console.WriteLine("result");
        Console.WriteLine(myNetwork.CalculateSmallNetworkResult(d));
        Console.ReadLine();
        */
        Console.WriteLine(myNetwork.CalculateSmallNetworkResult(d));
    }
}
//TrainNetwork();

