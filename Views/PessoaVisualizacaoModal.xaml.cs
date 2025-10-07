using System.Windows;
using WpfApp.Models;

namespace WpfApp.Views
{
    public partial class PessoaVisualizacaoModal : Window
    {
        public PessoaVisualizacaoModal(Pessoa pessoa)
        {
            InitializeComponent();
            DataContext = pessoa;
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
