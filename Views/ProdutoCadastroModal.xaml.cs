using System.Windows;
using WpfApp.Models;

namespace WpfApp.Views
{
    public partial class ProdutoCadastroModal : Window
    {
        public Produto ProdutoEditado { get; private set; }

        public ProdutoCadastroModal(Produto produto)
        {
            InitializeComponent();
            
            if (produto != null)
            {
                TxtTitulo.Text = "Editar Produto";
                TxtNome.Text = produto.Nome;
                TxtCodigo.Text = produto.Codigo;
                TxtValor.Text = produto.Valor.ToString("F2");
                ProdutoEditado = produto;
            }
            else
            {
                TxtTitulo.Text = "Novo Produto";
                ProdutoEditado = new Produto();
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text))
            {
                MessageBox.Show("O nome do produto é obrigatório.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtNome.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtCodigo.Text))
            {
                MessageBox.Show("O código é obrigatório.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCodigo.Focus();
                return;
            }

            if (!decimal.TryParse(TxtValor.Text, out decimal valor) || valor <= 0)
            {
                MessageBox.Show("O valor deve ser um número válido maior que zero.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtValor.Focus();
                return;
            }

            ProdutoEditado.Nome = TxtNome.Text.Trim();
            ProdutoEditado.Codigo = TxtCodigo.Text.Trim();
            ProdutoEditado.Valor = valor;

            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
