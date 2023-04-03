using ConsoleApp1;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics.Tracing;


StreamWriter logFile = new("plikiTekstowe/rozpoznawanieZdan/logiNauczania.txt", true);    // Tworzymy plik do logowania
NeuralNetwork myNetwork = new NeuralNetwork("plikiTekstowe//rozpoznawanieZdan/siecPoczatkowa.txt");    //pobieram dane sieci z pliku
NeuralNetwork goodNetwork = new NeuralNetwork("plikiTekstowe//rozpoznawanieZdan/dobraSiec.txt");
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
double[] getSingleData(string text)
{
    double[] result = new double[7];
    result[0] = getWordsCount(text);
    result[1] = text.Length;
    result[2] = getSumOfCharValues(text);
    result[3] = getUpperCaseCount(text);
    result[4] = getAvgWordLength(text);
    result[5] = getPunctuationMarksCount(text);
    result[6] = getDotCount(text);
    return result;
}

int getPunctuationMarksCount(string text)
{
    return text.Count(char.IsPunctuation);
}
double getAvgWordLength(string text)
{
    string[] words = text.Split(' ');
    double avg = 0;
    foreach(var word in words)
    {
        avg += word.Length;
    }
    return avg/words.Length;
}
int getUpperCaseCount(string text)
{
    return text.Count(char.IsUpper);
}
int getWordsCount(string text)
{
    return text.Count(char.IsWhiteSpace)+1;
}
int getSumOfCharValues(string text)
{
    int result = 0;
    for(int i=0; i< text.Length; i++)
    {
        result+= text[i];
    }
    return result;
}
int getDotCount(string text)
{
    int result = 0;
    foreach(var letter in text)
    {
        if (letter == '.')
        {
            result++;
        }
    }
    return result;
}
void getData()
{
    StreamReader sr = new StreamReader("plikiTekstowe//rozpoznawanieZdan/daneNauczania.txt");
    string line = sr.ReadLine();
    while(line != null)
    {
        if(line == "1" || line == "0")
        {
            dataResult.Add(Int32.Parse(line));
        }
        else
        {
            double[] newData = new double[7];
            newData[0] = getWordsCount(line);
            newData[1] = line.Length;
            newData[2] = getSumOfCharValues(line);
            newData[3] = getUpperCaseCount(line);
            newData[4] = getAvgWordLength(line);
            newData[5] = getPunctuationMarksCount(line);
            newData[6] = getDotCount(line);
            data.Add(newData);
        }
        line = sr.ReadLine();
    }
}

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
            if (!myNetwork.TrainingNetwork(data[i], dataResult[i],0.1))
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
getData();
//TrainNetwork();
logFile.WriteLine("Nowa Struktura: ");
logFile.WriteLine(goodNetwork.GetWholeStructure());
logFile.WriteLine("-----------------------------------------------------");
logFile.Close();

