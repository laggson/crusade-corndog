using LPlay.Logic;
using System;

namespace LPlay.Gui
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        Class1 _con;

        public MainWindow()
        {
            InitializeComponent();
            _con = new Class1();
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Title = "LPlay v" + _con.GetVersion();
        }
    }
}
