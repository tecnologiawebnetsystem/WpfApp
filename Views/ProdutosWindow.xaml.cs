using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.Views
{
    public partial class ProdutosWindow : Window
    {
        private readonly DataService _dataService;
        private List<Produto> _todosProdutos;
        private List<Produto> _produtosFiltrados;

        public ProdutosWindow()
        {
            InitializeComponent();
            _dataService = new DataService();
            CarregarProdutos();
        }

        private void CarregarProdutos()
        {
            _todosProdutos = _dataService.CarregarProdutos();
            _produtosFiltrados = new List<Produto>(_todosProdutos);
            AtualizarGrid();
        }

        private void AtualizarGrid()
        {
            DgProdutos.ItemsSource = null;
            DgProdutos.ItemsSource = _produtosFiltrados;
            TxtTotal.Text = _produtosFiltrados.Count.ToString();
        }

        private void CmbTipoPesquisa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbTipoPesquisa == null || PnlPesquisaSimples == null || PnlFaixaValor == null)
                return;

            if (CmbTipoPesquisa.SelectedIndex == 2) // Faixa de Valor
            {
                PnlPesquisaSimples.Visibility = Visibility.Collapsed;
                PnlFaixaValor.Visibility = Visibility.Visible;
            }
            else
            {
                PnlPesquisaSimples.Visibility = Visibility.Visible;
                PnlFaixaValor.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            if (CmbTipoPesquisa.SelectedIndex == 0) // Produto (Nome)
            {
                var termo = TxtPesquisa.Text.ToLower().Trim();
                _produtosFiltrados = string.IsNullOrWhiteSpace(termo)
                    ? new List<Produto>(_todosProdutos)
                    : _todosProdutos.Where(p => p.Nome.ToLower().Contains(termo)).ToList();
            }
            else if (CmbTipoPesquisa.SelectedIndex == 1) // Código
            {
                var termo = TxtPesquisa.Text.ToLower().Trim();
                _produtosFiltrados = string.IsNullOrWhiteSpace(termo)
                    ? new List<Produto>(_todosProdutos)
                    : _todosProdutos.Where(p => p.Codigo.ToLower().Contains(termo)).ToList();
            }
            else if (CmbTipoPesquisa.SelectedIndex == 2) // Faixa de Valor
            {
                decimal valorInicial = 0;
                decimal valorFinal = decimal.MaxValue;

                if (!string.IsNullOrWhiteSpace(TxtValorInicial.Text))
                    decimal.TryParse(TxtValorInicial.Text, out valorInicial);

                if (!string.IsNullOrWhiteSpace(TxtValorFinal.Text))
                    decimal.TryParse(TxtValorFinal.Text, out valorFinal);

                _produtosFiltrados = _todosProdutos
                    .Where(p => p.Valor >= valorInicial && p.Valor <= valorFinal)
                    .ToList();
            }

            AtualizarGrid();
        }

        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarProdutos();
            TxtPesquisa.Text = string.Empty;
            TxtValorInicial.Text = string.Empty;
            TxtValorFinal.Text = string.Empty;
            CmbTipoPesquisa.SelectedIndex = 0;
        }

        private void BtnNovoProduto_Click(object sender, RoutedEventArgs e)
        {
            var modal = new ProdutoCadastroModal(null);
            if (modal.ShowDialog() == true)
            {
                var novoProduto = modal.ProdutoEditado;
                novoProduto.Id = _todosProdutos.Any() ? _todosProdutos.Max(p => p.Id) + 1 : 1;
                _todosProdutos.Add(novoProduto);
                _dataService.SalvarProdutos(_todosProdutos);
                CarregarProdutos();
            }
        }

        private void BtnVisualizar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var produto = button?.Tag as Produto;
            if (produto != null)
            {
                var modal = new ProdutoVisualizacaoModal(produto);
                modal.ShowDialog();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var produto = button?.Tag as Produto;
            if (produto != null)
            {
                var modal = new ProdutoCadastroModal(produto);
                if (modal.ShowDialog() == true)
                {
                    var produtoOriginal = _todosProdutos.FirstOrDefault(p => p.Id == produto.Id);
                    if (produtoOriginal != null)
                    {
                        produtoOriginal.Nome = modal.ProdutoEditado.Nome;
                        produtoOriginal.Codigo = modal.ProdutoEditado.Codigo;
                        produtoOriginal.Valor = modal.ProdutoEditado.Valor;
                        _dataService.SalvarProdutos(_todosProdutos);
                        CarregarProdutos();
                    }
                }
            }
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var produto = button?.Tag as Produto;
            if (produto != null)
            {
                var resultado = MessageBox.Show(
                    $"Deseja realmente excluir o produto '{produto.Nome}'?",
                    "Confirmar Exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    _todosProdutos.RemoveAll(p => p.Id == produto.Id);
                    _dataService.SalvarProdutos(_todosProdutos);
                    CarregarProdutos();
                }
            }
        }
    }
}
