using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuralNetworkBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateNewNeuralNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            var CreateNeuralNetworkWindow = new CreateNeuralNetwork();
            CreateNeuralNetworkWindow.Show();
        }

        private void TrainNeuralNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            var TrainNeuralNetworkWindow = new TrainNeuralNetwork();
            TrainNeuralNetworkWindow.Show();
        }

        private void ExitApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
