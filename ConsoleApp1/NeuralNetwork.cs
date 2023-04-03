using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class NeuralNetwork
    {
        public List<List<double>> layersValues = new List<List<double>>();
        public List<Layer> mLayers = new List<Layer>();

        //
        //
        //  Tworzenie sieci
        //
        //
        public void readFromFile(string path)
        {
            mLayers.Clear();
            List<Neuron> neuronSet = new List<Neuron>();
            //
            //WAZNE!! W PLIKU TEKSTOWYM PIERWSZA WAGA KTORA PODAMY (w0) odpowiada za BIAS
            //

            StreamReader sr = new(path);             //Obsluga odczytywania struktury sieci z tekstu
            string linia = "";
            List<double> weights = new List<double>();
            while (linia != null)
            {
                linia = sr.ReadLine();
                if (linia != null)
                {
                    if (linia.Contains("/Layer"))               //Znajduje koniec Layeru i dodaje nowy set Neuronow do Listy oraz zwieksza sie numberofLayers
                    {
                        mLayers.Add(new Layer(neuronSet));
                        neuronSet.Clear();
                    }
                    else if (linia.Contains("/Neuron"))              //Znajduje koniec neurona i dodaje go do aktualnego setu
                    {
                        neuronSet.Add(new Neuron(weights));
                    }
                    else if (linia.Contains("Neuron"))
                    {
                        weights.Clear();
                    }
                    else if (linia.Contains(';'))                //Zczytywanie wag do aktualnego neurona
                    {
                        string[] weightsString = linia.Split(';');
                        for (int i = 0; i < weightsString.Length - 1; i++)
                        {
                            weights.Add(double.Parse(weightsString[i]));
                        }
                    }
                }

            }
            sr.Close();
        }
        public void RemoveNeuron(int layerNumber, int neuronNumber)
        {
            mLayers[layerNumber].removeNeuron(neuronNumber);
        }

        //Dodawanie pojedycznego neuronu do Layera
        public void AddNeuron(List<double> weights, int layerNumber)
        {
            mLayers[layerNumber].addNeuron(new Neuron(weights));
        }
        public void AddNeuron(double[] weights, int layerNumber)
        {
            mLayers[layerNumber].addNeuron(new Neuron(weights));
        }
        public void RemoveLayer(int layerNumber)
        {
            mLayers.RemoveAt(layerNumber);
        }
        //Umozliwia rozszerzenie sieci o 1 layer
        public void AddLayer()
        {
            mLayers.Add(new Layer());
        }
        public void AddLayer(Neuron[] neurons)
        {
            mLayers.Add(new Layer(neurons));
        }
        public void AddLayer(List<Neuron> neurons)
        {
            mLayers.Add(new Layer(neurons));
        }
        // Tutaj przekazujemy neurony oraz podajemy numer layeru (0 to pierwszy numer) 
        // neurons to tablica klasy Neuron
        public void SetLayer(Neuron[] neurons, int number_of_layer) 
        {
            mLayers[number_of_layer] = new Layer(neurons);
        }
        // Tutaj przekazujemy neurony oraz podajemy numer layeru (0 to pierwszy numer)
        // neurons to lista klasy Neuron  
        public void SetLayer(List<Neuron> neurons, int number_of_layer)
        {
            mLayers[number_of_layer] = new Layer(neurons);
        }

        //
        //
        //  Dzialania na sieci
        //
        //
        private double FinalNeuronNumber()
        {
            List<double> values = new List<double>();
            for(int i=0; i< mLayers[mLayers.Count-1].mNeurons.Count; i++)
            {
                double resultValue = mLayers[mLayers.Count - 1].mNeurons[i].weights[0];
                for (int j=1; j< mLayers[mLayers.Count - 1].mNeurons[i].weights.Count; j++)
                {
                    resultValue += mLayers[mLayers.Count - 1].mNeurons[i].weights[j] * mLayers[mLayers.Count - 1].mNeurons[i].inputData[j-1];
                }
                resultValue = ActivationFunctionSigmoid(scalling(resultValue.ToString()));
                values.Add(resultValue);
            }
            layersValues.Add(new List<double>(values));
            if (mLayers[mLayers.Count - 1].mNeurons.Count > 1)
            {
                return values.IndexOf(values.Max());
            }
            return values[0];
        }
        double scalling(string number)
        {
            string divider = string.Empty;
            for(int i=0; i< number.Length; i++)
            {
                if (number[i] == ',')
                {
                    break;
                }
                else
                {
                    divider += number[i];
                }
            }
            double result = double.Parse(number);
            if (result > 1 ||result < -1)
            {
                return double.Parse(number) / Math.Pow(10, divider.Length);
            }
            return double.Parse(number);
        }
        private double ActivationFunctionRelu(double x)
        {
            return Math.Max(0, x);
        }
        private double ActivationFunctionSigmoid(double x)
        {
            x = scalling(x.ToString());
            return 1.0 / (1.0 + Math.Exp((float)-x));
        }
        private double GetNeuronResult(int layerIndex, int neuronIndex)
        {
            double neuronResult = mLayers[layerIndex].mNeurons[neuronIndex].weights[0]; //przypisujemy na starcie liczbie bias
            for (int i = 1; i < mLayers[layerIndex].mNeurons[neuronIndex].weights.Count; i++)
            {
                neuronResult += mLayers[layerIndex].mNeurons[neuronIndex].weights[i] * mLayers[layerIndex].mNeurons[neuronIndex].inputData[i - 1];
            }
            return ActivationFunctionSigmoid(scalling(neuronResult.ToString()));
        }
        private void SetInputs(double[] inputData)
        {
            foreach(var neuron in mLayers[0].mNeurons)
            {
                neuron.setInputData(inputData);
            }
        }
        public double CalculateNetworkResult(double[] inputData)
        {
            layersValues.Clear();
            SetInputs(inputData);
            List<double> neuronsResults = new List<double>();
            for (int i = 0; i < mLayers.Count-1; i++)
            {
                neuronsResults.Clear();
                for(int j=0; j < mLayers[i].mNeurons.Count; j++)
                {
                    neuronsResults.Add(GetNeuronResult(i,j));
                }
                for (int j=0; j< mLayers[i+1].mNeurons.Count; j++)
                {
                    mLayers[i + 1].mNeurons[j].setInputData(neuronsResults);
                }
                layersValues.Add(new List<double>(neuronsResults));
            }
            return FinalNeuronNumber();
        }






        private double CalculateSingleNeuronResult(Neuron neuron)
        {
            double result = neuron.weights[0];      //Przypisuje liczbe bias
            for(int i=1; i<neuron.weights.Count; i++)
            {
                //Console.WriteLine("Waga: " + neuron.weights[i]);
                //Console.WriteLine("Input: " + neuron.inputData[i - 1]);
               // Console.ReadLine();
                result += neuron.weights[i] * neuron.inputData[i - 1];  // input data ma indeks o 1 mniejszy, poniewaz w wagach waga o indeksie 0 to bias
            }
            return ActivationFunctionSigmoid(result);
        }
        private void AddNextLayerInputs(List<double> neuronResults, int index)
        {
            foreach(var neuron in mLayers[index].mNeurons)
            {
                neuron.setInputData(neuronResults);
            }
        }
        private List<double> CalculateNeuronsResults(Layer layer, int nextLayerIndex)
        {
            List<double> neuronsResults = new List<double>();
            foreach (var neuron in layer.mNeurons)
            {
                neuronsResults.Add(CalculateSingleNeuronResult(neuron));
            }
            if(nextLayerIndex != mLayers.Count)
            {
                AddNextLayerInputs(neuronsResults, nextLayerIndex);
            }
            return neuronsResults;
        }
        private double getNetworkResult()
        {
            if (mLayers[mLayers.Count-1].mNeurons.Count == 1)
            {
                return layersValues[layersValues.Count-1][0];
            }
            else
            {
                return layersValues[layersValues.Count-1].IndexOf(layersValues[layersValues.Count - 1].Max());
            }
        }
        public double CalculateSmallNetworkResult(double[] inputData)
        {
            layersValues.Clear();
            SetInputs(inputData);
            int nextLayerIndex = 1;
            foreach(var layer in mLayers)
            {
                layersValues.Add(new List<double>(CalculateNeuronsResults(layer, nextLayerIndex)));
                nextLayerIndex++;
            }
            /*
            for(int i =0; i< layersValues.Count; i++)
            {
                Console.WriteLine("Layer nr: " + i);
                for(int j=0; j< layersValues[i].Count; j++)
                {
                    Console.WriteLine("Result: " + layersValues[i][j]);
                }
            }
            Console.ReadLine();
            */
            return getNetworkResult();
        }

//                          UCZENIE SIECI                             // 

        private double ZeroOneLoss(int whatShouldBe, double neuronResult)
        {
            return whatShouldBe - neuronResult;
        }
        private double GetNewWeight(double input,double loss,double learningSpeed)
        {
            return input * learningSpeed * loss;
        }
        private void CorrectWeights(Neuron neuron,double learningSpeed,double loss, int whatShouldBe)
        {
            for(int i=1; i<neuron.weights.Count; i++)
            {
                if (neuron.weights[i] >= 0)
                {
                    neuron.weights[i] += GetNewWeight(neuron.inputData[i - 1], loss, learningSpeed);    //inputData musi miec index-1, wynika to z tego, ze weights[0] to bias, a co za tym idzie wagi maja 1 element wiecej niz inputy
                }
                else
                {
                    neuron.weights[i] -= GetNewWeight(neuron.inputData[i - 1], loss, learningSpeed);
                }
            }
            if (whatShouldBe == 0)
            {
                neuron.weights[0] += -1 * learningSpeed;    //bias
            }
            else
            {
                neuron.weights[0] += 1 * learningSpeed;     //bias
            }
        }
        private void Backpropagation(double learningSpeed, int whatShouldBe) // whatShouldBe jest to zmienna potrzebna później do kalkulacji liczby bias.
        {
            for(int i= mLayers.Count-2; i>=0; i--)
            {
                for(int j=0; j < mLayers[i].mNeurons.Count; j++)
                {
                    double loss = ZeroOneLoss(whatShouldBe, layersValues[i][j]);   //Obliczam błąd na podstawie wyniku neuronu
                    CorrectWeights(mLayers[i].mNeurons[j],learningSpeed,loss,whatShouldBe);
                }
            }
        }
        public bool TrainingNetwork(double[] inputDate, int numberOfCorrectNeuron, double learningSpeed)
        {
            if(CalculateSmallNetworkResult(inputDate) == numberOfCorrectNeuron)
            {
                return true;
            }
            else
            {
                for(int i = mLayers[mLayers.Count -1].mNeurons.Count-1; i>=0; i--)  //Zaczynamy od ostatniego layeru
                {
                    if(i == numberOfCorrectNeuron)
                    {
                        double loss = ZeroOneLoss(1, layersValues[layersValues.Count - 1][i]);
                        for(int j=1; j < mLayers[mLayers.Count - 1].mNeurons[i].weights.Count; j++) //Poprawiam wagi ostatniego layeru, musze to zrobic osobno aby potem latwiej poprawic cala reszte sieci
                        {
                            mLayers[mLayers.Count - 1].mNeurons[i].weights[j] += GetNewWeight(mLayers[mLayers.Count - 1].mNeurons[i].inputData[j-1],loss,learningSpeed);
                        }
                        mLayers[mLayers.Count - 1].mNeurons[i].weights[0] += learningSpeed * 1; //bias ustawiam oddzielnie, ponieważ delikatnie różni się wzór
                        Backpropagation(learningSpeed,1);
                    }
                    else
                    {                      
                        double loss = ZeroOneLoss(0, layersValues[layersValues.Count - 1][i]);
                        for (int j = 1; j < mLayers[mLayers.Count - 1].mNeurons[i].weights.Count; j++) // Poprawiam wagi ostatniego layeru, musze to zrobic osobno aby potem latwiej poprawic cala reszte sieci
                        {
                            mLayers[mLayers.Count - 1].mNeurons[i].weights[j] += GetNewWeight(mLayers[mLayers.Count - 1].mNeurons[i].inputData[j - 1], loss, learningSpeed);
                        }
                        mLayers[mLayers.Count - 1].mNeurons[i].weights[0] += learningSpeed * -1; //bias ustawiam oddzielnie, ponieważ delikatnie różni się wzór
                        Backpropagation(learningSpeed,0);
                    }
                }
            }
            return false;
        }

        public bool TrainingTableTennisNetwork(double[] inputData, int numberOfCorrectNeuron, double learningSpeed)
        {
            if(CalculateNetworkResult(inputData) == numberOfCorrectNeuron)
            {
                return true;
            }
            else
            {
                foreach(var neuron in mLayers[mLayers.Count-1].mNeurons)
                {
                    double neuronResult = scalling((neuron.inputData[0] * neuron.weights[1] + neuron.weights[0]).ToString());
                    Console.WriteLine(neuronResult);
                    Console.ReadLine();
                    neuronResult = ActivationFunctionSigmoid(neuronResult);
                    Console.WriteLine(neuronResult);
                }
            }
            return false;
        }

        //
        //
        //      Konstruktory
        //
        //
        public NeuralNetwork()
        {
        }
        public NeuralNetwork(string path)
        {
            readFromFile(path);
        }
        public NeuralNetwork(int number_of_layers)
        {
            for(int i=0; i<number_of_layers; i++)
            {
                mLayers.Add(new Layer());
            }
        }
        public NeuralNetwork(int number_of_layers, List<Neuron[]> neurons)
        {
            for (int i = 0; i < number_of_layers; i++)
            {
                mLayers.Add(new Layer(neurons[i]));
            }
        }
        //Funkcja, ktora umozliwia wyswietlenie struktury calej sieci neuronowej
        public string GetWholeStructure()
        {
            string result = string.Empty;
            foreach (var element in mLayers)
            {
                result += "<Layer>\n";
                foreach (var neuron in element.mNeurons)
                {
                    result += "<Neuron>\n";
                    foreach (var weight in neuron.weights)
                    {
                        result += weight +";";
                    }
                    result += "\n</Neuron>\n";
                }
                result += "</Layer>\n";
            }
            return result;
        }
    }
}
