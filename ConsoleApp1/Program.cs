using ConsoleApp1;

int numberOfLayers = 0;
List<Neuron[]> neuronList = new List<Neuron[]>();   //Tutaj tablica w ktorej kazdy element to lista neuronow z poszczegolnego Layera
List<Neuron> neuronSet = new List<Neuron>(); // Tutaj lista do zapisywania pojedynczych neuronikow z layera
void pobierzStrukture()
{
    StreamReader sr = new("plikiTekstowe/przykladowaSiec.txt");             //Obsluga odczytywania struktury sieci z tekstu
    string linia = "";
    List<double> weights = new List<double>();
    while(linia != null)
    {       
        linia = sr.ReadLine();                  
        if (linia != null)
        {
            if (linia.Contains("/Layer"))               //Znajduje koniec Layeru i dodaje nowy set Neuronow do Listy oraz zwieksza sie numberofLayers
            {
                neuronList.Add(neuronSet.ToArray());
                numberOfLayers++;
                neuronSet.Clear();
            }
            else if(linia.Contains("/Neuron"))              //Znajduje koniec neurona i dodaje go do aktualnego setu
            {
                neuronSet.Add(new Neuron(weights));
            }
            else if(linia.Contains("Neuron"))
            {
                weights.Clear();
            }
            else if(linia.Contains(";"))                //Zczytywanie wag do aktualnego neurona
            {
               string[] weightsString = linia.Split(';');
               for(int i = 0; i < weightsString.Length-1; i++)
                {
                    weights.Add(double.Parse(weightsString[i]));
                }
            }
        }

    }
}
Console.WriteLine("Pobieramy strukture sieci z tekstu \n");
pobierzStrukture();
NeuralNetwork nowaSiec = new NeuralNetwork(numberOfLayers, neuronList);
nowaSiec.getWholeStructure();

