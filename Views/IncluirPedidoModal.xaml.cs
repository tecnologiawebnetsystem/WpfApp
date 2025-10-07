using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.Views
{
    public partial class IncluirPedidoModal : Window
    {
        private readonly Pessoa _pessoa;
        private readonly List<ProdutoSelecao> _produtosSelecao;
        private readonly DataService _dataService;

        public Pedido PedidoCriado { get; private set; }

        public IncluirPedidoModal(Pessoa pessoa, DataService dataService)
        {
            InitializeComponent();
            _pessoa = pessoa;
            _dataService = dataService;
            _produtosSelecao = new List<ProdutoSelecao>();

            CarregarDados();
        }

        private void CarregarDados()
        {
            txtCliente.Text = $"Cliente: {_pessoa.Nome}";

            var produtos = _dataService.CarregarProdutos();
            foreach (var produto in produtos)
            {
                _produtosSelecao.Add(new ProdutoSelecao
                {
                    Id = produto.Id,
                    Nome = produto.Nome,
                    Codigo = produto.Codigo,
                    Valor = produto.Valor,
                    Quantidade = 0,
                    Selecionado = false
                });
            }

            listaProdutos.ItemsSource = _produtosSelecao;
            AtualizarResumo();
        }

        private void ProdutoCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.DataContext is ProdutoSelecao produto)
            {
                if (checkBox.IsChecked == true && produto.Quantidade == 0)
                {
                    produto.Quantidade = 1;
                }
            }
            AtualizarResumo();
        }

        private void BtnDiminuir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is ProdutoSelecao produto)
            {
                if (produto.Quantidade > 0)
                {
                    produto.Quantidade--;
                    AtualizarResumo();
                }
            }
        }

        private void BtnAumentar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is ProdutoSelecao produto)
            {
                produto.Quantidade++;
                AtualizarResumo();
            }
        }

        private void AtualizarResumo()
        {
            var itensSelecionados = _produtosSelecao.Where(p => p.Selecionado).ToList();
            var totalItens = itensSelecionados.Sum(p => p.Quantidade);
            var valorTotal = itensSelecionados.Sum(p => p.Quantidade * p.Valor);

            txtItens.Text = $"{totalItens} {(totalItens == 1 ? "item selecionado" : "itens selecionados")}";
            txtValorTotal.Text = $"R$ {valorTotal:N2}";
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "Deseja realmente cancelar? O pedido será descartado.",
                "Confirmar Cancelamento",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        private void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            var itensSelecionados = _produtosSelecao.Where(p => p.Selecionado).ToList();

            if (!itensSelecionados.Any())
            {
                MessageBox.Show(
                    "Selecione pelo menos um produto para finalizar o pedido.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (itensSelecionados.Any(p => p.Quantidade <= 0))
            {
                MessageBox.Show(
                    "A quantidade de todos os produtos deve ser maior que zero.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var pedido = new Pedido
            {
                PessoaId = _pessoa.Id,
                DataVenda = DateTime.Now,
                Status = StatusPedido.Pendente,
                FormaPagamento = null
            };

            foreach (var item in itensSelecionados)
            {
                pedido.Itens.Add(new ItemPedido
                {
                    ProdutoId = item.Id,
                    NomeProduto = item.Nome,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.Valor
                });
            }

            pedido.CalcularValorTotal();
            _dataService.AdicionarPedido(pedido);
            PedidoCriado = pedido;

            MessageBox.Show(
                $"Pedido #{pedido.Id} finalizado com sucesso!\n\nValor Total: R$ {pedido.ValorTotal:N2}\nStatus: Pendente\nForma de Pagamento: A definir",
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }

    public class ProdutoSelecao : INotifyPropertyChanged
    {
        private bool _selecionado;
        private int _quantidade;

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public decimal Valor { get; set; }

        public bool Selecionado
        {
            get => _selecionado;
            set
            {
                _selecionado = value;
                OnPropertyChanged(nameof(Selecionado));
            }
        }

        public int Quantidade
        {
            get => _quantidade;
            set
            {
                if (value < 0) value = 0;
                _quantidade = value;
                OnPropertyChanged(nameof(Quantidade));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
