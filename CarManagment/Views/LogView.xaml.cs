using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarManagment.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
            Initialize();      
        }

        public void Initialize()
        {
            TextRange textRange = new TextRange(TextBox.Document.ContentStart, TextBox.Document.ContentEnd);
            using (FileStream fileStream = new FileStream("Log.txt", FileMode.OpenOrCreate))
                textRange.Load(fileStream, System.Windows.DataFormats.Text);
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if ((pd.ShowDialog() == true))
            {
                //use either one of the below      
                pd.PrintVisual(TextBox as Visual, "printing as visual");
                pd.PrintDocument((((IDocumentPaginatorSource)TextBox.Document).DocumentPaginator), "printing as paginator");
            }
        }
    }
}
