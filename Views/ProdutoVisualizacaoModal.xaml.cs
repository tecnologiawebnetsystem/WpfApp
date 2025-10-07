using System.Windows;
using WpfApp.Models;

namespace WpfApp.Views
{
    public partial class ProdutoVisualizacaoModal : Window
    {
        public ProdutoVisualizacaoModal(Produto produto)
        {
            InitializeComponent();
            
            if (produto != null)
            {
                TxtId.Text = produto.Id.ToString();
                TxtNome.Text = produto.Nome;
                TxtCodigo.Text = produto.Codigo;
                TxtValor.Text = produto.Valor.ToString("C2");
            }
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
