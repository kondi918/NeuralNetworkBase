using ConsoleApp1;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics.Tracing;
using System.IO;

CancellationTokenSource cancelTokenTraining = new CancellationTokenSource();
StreamWriter logFile = new("plikiTekstowe/rozpoznawanieZdan/logiNauczania.txt", true);    // Tworzymy plik do logowania
NeuralNetwork myNetwork = new NeuralNetwork("plikiTekstowe/BINARY/siecPoczatkowa.txt");    //pobieram dane sieci z pliku
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
void setWaiting(int seconds)
{
    Thread.Sleep(seconds * 1000);
}
void Timer(int seconds)
{
    while(seconds != 0 )
    {
        Thread.Sleep(1000);
        seconds--;
    }
}
void TrainNetwork()
{
    int seconds = 60; // TUTAJ DODAC POBIERANIE OKRESLONEGO CZASU Z OKIENKA
    Task setTimer = new Task(() =>
    {
        Timer(seconds);
    });
    setTimer.Start();
    int mistakes = 0;
    Task ShowErrors = null;
    while (!cancelTokenTraining.IsCancellationRequested)
    {
        mistakes = 0;
        for (int i = 0; i < trainingData.Count; i++)
        {
            if (!myNetwork.NetworkTraining(trainingData[i], trainingResults[i], 0.1))
            {
                mistakes++;
            }
        }
        if (ShowErrors == null || ShowErrors.IsCompleted)
        {
            ShowErrors = Task.Run(() =>
            {
                setWaiting(1);           
            });
            Console.WriteLine("Ilosc bledow:" + mistakes);
            if(mistakes == 0)
            {
                cancelTokenTraining.Cancel();
            }
        }
    }
    setTimer.Wait();
    Console.WriteLine("Zakonczono");
}



/*GetTrainingData("plikiTekstowe/BINARY/inputData.txt");
Task trainingNetworkTask = new Task(() =>
{
    TrainNetwork();
});
trainingNetworkTask.Start();
*/
while (1==1)
{
    Console.WriteLine("Podaj X");
    double[] dane = new double[2];
    dane[0] = double.Parse(Console.ReadLine());
    Console.WriteLine("Podaj Y");
    dane[1] = double.Parse(Console.ReadLine());
    if(myNetwork.CalculateSmallNetworkResult(dane) > 0.5)
    {
        Console.WriteLine("TO 1");
    }
    else
    {
        Console.WriteLine("TO 0");
    }
}
