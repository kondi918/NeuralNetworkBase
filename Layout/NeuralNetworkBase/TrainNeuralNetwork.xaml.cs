using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace NeuralNetworkBase
{
    /// <summary>
    /// Interaction logic for TrainNeuralNetwork.xaml
    /// </summary>
    public partial class TrainNeuralNetwork : Window
    {
        CancellationTokenSource cancelTokenTraining = new CancellationTokenSource();                          
        StreamWriter logFile = null;    // Tworzymy plik do logowania
        string savingFilePath = "";  // Tworzymy plik do zapisu
        NeuralNetwork myNetwork = null;             //NULL BO WYBIERAM SAM PLIK Z SIECIĄ POCZĄTKOWĄ  // new NeuralNetwork("plikiTekstowe/dlugopisObraczka/siecPoczatkowa.txt");    //pobieram dane sieci z pliku
        FileManager fileManager = new FileManager();
        NeuralNetworkInputData trainingData;
        bool isTrainingCompleted = true;
        bool isFolderPicker = true;
        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);
        private readonly object mLayersLock = new object();
        private List<double[]> trainingDataSet = new List<double[]>();
        private List<double[]> trainingTestDataSet = new List<double[]>();
        private List<int> trainingResultSet = new List<int>();
        private List<int> trainingTestResultSet = new List<int>();
        private int epoch;


        private enum WhatEndingCondition
        {
            timer,
            mistakes,
            epoch
        }
        WhatEndingCondition endingCondition = WhatEndingCondition.timer;
        public TrainNeuralNetwork()
        {
            InitializeComponent();
            Closing += TrainNeuralNetwork_Closing;
        }
        private void TrainNeuralNetwork_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (logFile != null)
            {
                logFile.Close();
            }
        }      
        private string SelectTxtFile(string typeOfSelectingFile)
        {
            cancelTokenTraining.Cancel();
            string path = null;
            try
            {
                OpenFileDialog selectingTxtFile = new OpenFileDialog();
                selectingTxtFile.InitialDirectory = Directory.GetCurrentDirectory();
                selectingTxtFile.Filter = "Text files (*.txt)|*.txt|JSON files (*.json)|*.json|All Files (*)|*.*";
                selectingTxtFile.Title = typeOfSelectingFile;

                if (selectingTxtFile.ShowDialog() == true)
                {
                    path = selectingTxtFile.FileName;
                }
            } 
            catch(Exception ex){
                MessageBox.Show(ex.ToString());
            }
            return path;
        }
        private string SelectTxtFileOrFolder(string typeOfSelectingFile)
        {
            cancelTokenTraining.Cancel();
            string path = null;
            isFolderPicker = true;
            if (MessageBox.Show("Chcesz zaznaczyc pojedynczy plik?", "Rodzaj Danych", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                isFolderPicker = false;
            }
            try
            {
                CommonOpenFileDialog selectingTxtFile = new CommonOpenFileDialog();
                selectingTxtFile.InitialDirectory = Directory.GetCurrentDirectory();
                selectingTxtFile.IsFolderPicker = isFolderPicker;
                selectingTxtFile.Title = typeOfSelectingFile;

                if (selectingTxtFile.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    path = selectingTxtFile.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return path;
        }


        private async void ChooseTrainingData_Click(object sender, RoutedEventArgs e)  
        {
            try
            {
                string path = SelectTxtFileOrFolder("Dane treningowe");
                if(!isFolderPicker)
                {
                    trainingData = fileManager.GetInputData(path);
                }
                else
                {
                    fileManager.isReadingComplete = false;
                    Task readingTask = Task.Run(async () => await fileManager.GetInputDataFromFiles(path));
                    while (!fileManager.isReadingComplete)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SetTextOnTextBlock(InformationStatus, "Wczytywanie katalogow: " + fileManager.numberOfDirectiory + " / " + fileManager.totalNumberOfDirectories + " katalogów");
                        });
                        await Task.Delay(1000);
                    }

                    trainingData = fileManager.neuralInputData;
                    SetTextOnTextBlock(InformationStatus, "Wczytywanie zakończone");

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,ex.GetType().ToString());
            }
        }

        private async void SelectTrainedNeuralNetwork_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectTxtFile("Plik z siecią początkową");
            try
            {
                if (Path.GetExtension(path) == ".txt")
                {
                    myNetwork = new NeuralNetwork(path);
                }
                else if(Path.GetExtension(path) == ".json")
                {
                    StreamReader sr = new StreamReader(path);
                    string json = sr.ReadToEnd();
                    myNetwork = JsonConvert.DeserializeObject<NeuralNetwork>(json);
                    sr.Close();
                }
                else
                {
                    throw new Exception("Cannot read Network from file");
                }
                NeuralNetworkStructure.Dispatcher.Invoke(() => DrawNeuralNetwork());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectFileToSaveTrainedNeuralNetwork_Click(object sender, RoutedEventArgs e)
        {
            savingFilePath = SelectTxtFile("Plik zapisu do sieci");
        }
        private void EndingConditionClick(object sender, RoutedEventArgs e)
        {
            if(TimerCondition.IsChecked == true)
            {
                TBCondition.Text = "Oczekiwany czas treningu (s)";
            }
            else if(MistakesCondition.IsChecked == true)
            {
                TBCondition.Text = "Oczekiwany prog bledow";
            }
            else if(EpochCondition.IsChecked == true)
            {
                TBCondition.Text = "Liczba epoch";
            }
        }
        private void DrawNeuralNetwork()
        {
            if (myNetwork.mLayers.Count > 0)
            {
                bool isNetworkEmpty = false;
                foreach (var layer in myNetwork.mLayers)
                {
                    if (layer.mNeurons.Count == 0)
                    {
                        isNetworkEmpty = true;
                    }
                }
                if (!isNetworkEmpty)
                {
                    NeuralNetworkDrawer.GenerateSchemaStructure(myNetwork);
                    NeuralNetworkDrawer.CreateNeuronsDrawings(NeuralNetworkStructure);
                    NeuralNetworkDrawer.DrawNeuralNetworkSchema(NeuralNetworkStructure);
                }
                else
                {
                    MessageBox.Show("Nie mozna wczytac sieci. Upewnij sie czy plik ktory wybrales zawiera poprawna strukture");
                }
            }
            else
            {
                MessageBox.Show("Nie mozna wczytac sieci. Upewnij sie czy plik ktory wybrales zawiera poprawna strukture");
            }
        }

        // Wstrzymaj zadanie
        private void Pause()
        {
            pauseEvent.Reset();
            StopBtn.Content = "Wznów";
        }
        private void Resume()
        {
            pauseEvent.Set();
            StopBtn.Content = "Stop";
        }
        private bool IsSigmoidChecked()
        {
            if (Sigmoid.IsChecked == true)
            {
                return true;
            }
            return false;
        }

        private bool IsReluChecked()
        {
            if (Relu.IsChecked == true)
            {
                return true;
            }
            return false;
        }
        private bool IsZeroOneChecked()
        {
            if(ZeroOne.IsChecked == true)
            {
                return true;
            }
            return false;
        }
        void Timer(int totalSeconds,int actualSeconds)
        {
                TrainingProgressBar.Dispatcher.Invoke(() =>
                {
                    TrainingProgressBar.Value = (double)actualSeconds / (double)totalSeconds * 100;
                });
        }
        private void StartTimer()
        {
            int timerSeconds = 0;
            int totalSeconds = 0;
            ExpectedTrainingTime.Dispatcher.Invoke(() => { int.TryParse(ExpectedTrainingTime.Text, out totalSeconds); });
            Task setTimer = Task.Run(async () =>
            {
                while (timerSeconds < totalSeconds && !cancelTokenTraining.IsCancellationRequested)
                {
                    pauseEvent.Wait();
                    Timer(totalSeconds, timerSeconds);
                    timerSeconds++;
                    await Task.Delay(1000);
                }
                Timer(1, 1);
                cancelTokenTraining.Cancel();
            });
        }
        private void StartTimerEpoch()
        {
            int totalEpoch = 0;
            ExpectedTrainingTime.Dispatcher.Invoke(() => { int.TryParse(ExpectedTrainingTime.Text, out totalEpoch); });
            Task setTimer = Task.Run(async () =>
            {
                while (epoch < totalEpoch && !cancelTokenTraining.IsCancellationRequested)
                {
                    pauseEvent.Wait();
                    Timer(totalEpoch, epoch);
                    await Task.Delay(1000);
                }
                Timer(1, 1);
                cancelTokenTraining.Cancel();
            });
        }

        private void SetActivationFunction()
        {
            IsSigmoidChecked();
            if (IsSigmoidChecked())
            {
                myNetwork.whatActivationFunction = NeuralNetwork.WhatActivationFunction.sigmoid;
            }
            else if (IsReluChecked())
            {
                myNetwork.whatActivationFunction = NeuralNetwork.WhatActivationFunction.relu;
            }
            else if(IsZeroOneChecked())
            {
                myNetwork.whatActivationFunction = NeuralNetwork.WhatActivationFunction.zeroOne;
            }
        }
        private void GetDataReadyForTraining(NeuralNetworkInputData trainingData)
        {
            trainingTestDataSet.Clear();
            trainingTestResultSet.Clear();
            Random rnd = new Random();
            for (int x = 0; x < 10; x++)
            {
                for (int i = 0; i < trainingData.inputData.Count; i++)
                {
                    int index = rnd.Next(0, trainingData.inputData.Count);
                    var holderInputData = trainingData.inputData[i];
                    var holderResult = trainingData.trainingResults[i];
                    trainingData.inputData[i] = trainingData.inputData[index];
                    trainingData.trainingResults[i] = trainingData.trainingResults[index];
                    trainingData.inputData[index] = holderInputData;
                    trainingData.trainingResults[index] = holderResult;
                }
            }
        }
        private int Training()
        {
            if (trainingDataSet.Count > 0)
            {
                int mistakes = 0;
                for (int i = 0; i < trainingDataSet.Count; i++)
                {
                    myNetwork.NetworkTraining(trainingDataSet[i], trainingResultSet[i], 0.1);
                }
                for(int i =0; i < trainingTestDataSet.Count; i++)
                {
                    NeuralNetworkResult result = myNetwork.CalculateSmallNetworkResult(trainingTestDataSet[i]);
                    if (result.result != trainingTestResultSet[i])
                    {
                        mistakes++;
                    }
                }
                return mistakes;
            }
            else
            {
                int mistakes = 0;
                for (int i = 0; i < trainingData.inputData.Count; i++)
                {
                    if (!myNetwork.NetworkTraining(trainingData.inputData[i], trainingData.trainingResults[i], 0.001))
                    {
                        mistakes++;
                    }
                }
                return mistakes;
            }
        }
        private void SetTextOnTextBlock(TextBlock textBlock, string text)
        {
            textBlock.Dispatcher.Invoke(() =>
            {
                textBlock.Text = text;
            });
        }
        private void SetInformation()
        {
            if (cancelTokenTraining.IsCancellationRequested)
            {
                SetTextOnTextBlock(InformationStatus, "Przerwano nauczanie");
            }
            else
            {
                SetTextOnTextBlock(InformationStatus, "Zakończono nauczanie");
                cancelTokenTraining.Cancel();
            }
            isTrainingCompleted = true;
        }
        private void StartProgressBar()
        {
            if(endingCondition == WhatEndingCondition.timer)
            {
                StartTimer();
            }
            else if(endingCondition == WhatEndingCondition.epoch)
            {
                StartTimerEpoch();
            }
        }
        private void TrainNetwork(int totalSeconds)
        {
            /*
            if (trainingData.inputData.Count > 50)      // Dziele na dane treningowe i testowe dla duzego zbioru
            {
                SetTrainingData();
            }
            */
            int mistakes = 1;
            Task ShowErrors = null;
            if(endingCondition == WhatEndingCondition.timer)
            {
                StartProgressBar();
                while (mistakes != 0 && !cancelTokenTraining.IsCancellationRequested)
                {
                    pauseEvent.Wait();
                    lock (mLayersLock)
                    {
                        mistakes = Training();
                    }
                    if (ShowErrors == null || ShowErrors.IsCompleted)
                    {
                        ShowErrors = Task.Run(async () =>
                        {
                            SetTextOnTextBlock(MistakesNumber, mistakes.ToString());
                            await Task.Delay(1000);
                        });
                    }
                }
            }
            else if (endingCondition == WhatEndingCondition.epoch)
            {
                epoch = 0;
                int maxEpochNumer = Int32.Parse(ExpectedTrainingTime.Text);
                while(epoch < maxEpochNumer)
                {
                    pauseEvent.Wait();
                    lock (mLayersLock)
                    {
                        mistakes = Training();
                        epoch++;
                    }
                    if (ShowErrors == null || ShowErrors.IsCompleted)
                    {
                        ShowErrors = Task.Run(async () =>
                        {
                            SetTextOnTextBlock(MistakesNumber, mistakes.ToString());
                            await Task.Delay(1000);
                        });
                    }
                }
            }
            ShowErrors.Wait();
            SetTextOnTextBlock(MistakesNumber, mistakes.ToString());
            SetInformation();
        }

        private void SetSettings()
        {
            isTrainingCompleted = false;
            Resume();
            cancelTokenTraining = new CancellationTokenSource();
            InformationStatus.Text = "Rozpoczęto Nauczanie";
            SetActivationFunction();
            TrainingProgressBar.Value = 0;
        }
        private bool IsSetCorrectly()
        {
            if (myNetwork != null && trainingData!=null)
            {
                if (trainingData.inputData != null && trainingData.trainingResults!= null && trainingData.inputData.Count > 0 && myNetwork.mLayers[0].mNeurons[0].getWeights().Count - 1 == trainingData.inputData[0].Length && trainingData.inputData.Count == trainingData.trainingResults.Count)
                {
                    return true;
                }
            }

            return false;
        }
        private int GetSecondTimer()
        {
            int totalSeconds = 0;
            int.TryParse(ExpectedTrainingTime.Text, out totalSeconds);
            return totalSeconds;
        }
        private void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSetCorrectly())
            {
                GetDataReadyForTraining(trainingData);
                int totalSeconds = GetSecondTimer();
                Task trainingNetworkTask = new Task(() =>
                    {
                        TrainNetwork(totalSeconds);
                    });
                if(isTrainingCompleted)
                {
                    SetSettings();
                    try
                    {
                        trainingNetworkTask.Start();
                    }
                    catch(AggregateException ae)
                    {
                        foreach(var element in ae.InnerExceptions)
                        {
                            MessageBox.Show(element.ToString());
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Najpierw musisz wybrać odpowiednie pliki z danymi nauczania i strukturą sieci");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelTokenTraining.Cancel();
            Resume();
        }
        private void StopResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if(pauseEvent.IsSet)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lock (mLayersLock)
                {
                    if (savingFilePath != "")
                    {
                        StreamWriter savingFile = new StreamWriter(savingFilePath);
                        if (Path.GetExtension(savingFilePath) == ".txt")
                        {
                            savingFile.Write(myNetwork.GetWholeStructure());
                        }
                        else if(Path.GetExtension(savingFilePath) == ".json")
                        {
                            string json = JsonConvert.SerializeObject(myNetwork, Newtonsoft.Json.Formatting.Indented);
                            savingFile.Write(json);
                        }
                        else
                        {
                            throw new Exception("Bledne rozszerzenie pliku");
                        }
                        savingFile.Close();
                    }
                    else
                    {
                        throw new Exception("Aby moc zapisac siec musisz najpierw wybrac plik w ktorym ma zostac zapisana");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }      
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (myNetwork != null)
            {
                DrawNeuralNetwork();
            }
        }

    }
}