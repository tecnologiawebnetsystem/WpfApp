using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.Views
{
    public partial class NovoPedidoWindow : Window
    {
        private readonly DataService _dataService;
        private ObservableCollection<ItemPedido> _itens;
        private decimal _valorTotal;

        public NovoPedidoWindow()
        {
            InitializeComponent();
            _dataService = new DataService();
            _itens = new ObservableCollection<ItemPedido>();
            dgItens.ItemsSource = _itens;

            CarregarDados();
        }

        private void CarregarDados()
        {
            // Carregar clientes
            var pessoas = _dataService.ObterPessoas();
            cmbCliente.ItemsSource = pessoas;

            // Carregar produtos
            var produtos = _dataService.ObterProdutos();
            cmbProduto.ItemsSource = produtos;
        }

        private void BtnAdicionarProduto_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProduto.SelectedItem == null)
            {
                CustomMessageBox.Show("Selecione um produto!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQuantidade.Text, out int quantidade) || quantidade <= 0)
            {
                CustomMessageBox.Show("Digite uma quantidade válida!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var produto = (Produto)cmbProduto.SelectedItem;

            var item = new ItemPedido
            {
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                Quantidade = quantidade,
                ValorUnitario = produto.Preco
            };

            _itens.Add(item);
            AtualizarValorTotal();

            // Limpar seleção
            cmbProduto.SelectedIndex = -1;
            txtQuantidade.Text = "1";

            if (_itens.Count > 0)
            {
                tabControl.SelectedIndex = 2; // Aba de Pagamento
            }
        }

        private void BtnRemoverProduto_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext as ItemPedido;

            if (item != null)
            {
                _itens.Remove(item);
                AtualizarValorTotal();
            }
        }

        private void AtualizarValorTotal()
        {
            _valorTotal = _itens.Sum(i => i.Subtotal);
            txtValorTotal.Text = _valorTotal.ToString("C");
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // Validações
            if (cmbCliente.SelectedValue == null)
            {
                CustomMessageBox.Show("Selecione um cliente!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                tabControl.SelectedIndex = 0;
                return;
            }

            if (_itens.Count == 0)
            {
                CustomMessageBox.Show("Adicione pelo menos um produto ao pedido!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                tabControl.SelectedIndex = 1;
                return;
            }

            // Determinar forma de pagamento
            FormaPagamento formaPagamento;
            if (rbDinheiro.IsChecked == true)
                formaPagamento = FormaPagamento.Dinheiro;
            else if (rbCartao.IsChecked == true)
                formaPagamento = FormaPagamento.Cartao;
            else
                formaPagamento = FormaPagamento.Boleto;

            // Criar pedido
            var pedido = new Pedido
            {
                PessoaId = (int)cmbCliente.SelectedValue,
                FormaPagamento = formaPagamento,
                Itens = _itens.ToList()
            };

            pedido.CalcularValorTotal();

            // Salvar
            _dataService.AdicionarPedido(pedido);

            CustomMessageBox.Show("Pedido criado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
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
