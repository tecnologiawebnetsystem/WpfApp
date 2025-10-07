using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class PedidoViewModel : BaseViewModel
    {
        private readonly DataService _dataService;
        private readonly int _pessoaId;
        private Pedido _pedido;
        private Produto _produtoSelecionado;
        private int _quantidade;
        private decimal _valorTotal;

        public ObservableCollection<Produto> ProdutosDisponiveis { get; set; }
        public ObservableCollection<ItemPedido> ItensPedido { get; set; }
        public Array FormasPagamento => Enum.GetValues(typeof(FormaPagamento));

        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set => SetProperty(ref _produtoSelecionado, value);
        }

        public int Quantidade
        {
            get => _quantidade;
            set => SetProperty(ref _quantidade, value);
        }

        public decimal ValorTotal
        {
            get => _valorTotal;
            set => SetProperty(ref _valorTotal, value);
        }

        public FormaPagamento FormaPagamentoSelecionada { get; set; }

        public ICommand AdicionarProdutoCommand { get; }
        public ICommand RemoverProdutoCommand { get; }
        public ICommand FinalizarPedidoCommand { get; }

        public bool PedidoFinalizado { get; private set; }

        public PedidoViewModel(int pessoaId)
        {
            _dataService = new DataService();
            _pessoaId = pessoaId;
            _pedido = new Pedido { PessoaId = pessoaId };
            
            ProdutosDisponiveis = new ObservableCollection<Produto>();
            ItensPedido = new ObservableCollection<ItemPedido>();
            Quantidade = 1;

            AdicionarProdutoCommand = new RelayCommand(_ => AdicionarProduto(), 
                _ => ProdutoSelecionado != null && Quantidade > 0);
            RemoverProdutoCommand = new RelayCommand(RemoverProduto);
            FinalizarPedidoCommand = new RelayCommand(_ => FinalizarPedido(), 
                _ => ItensPedido.Any());

            CarregarProdutos();
        }

        private void CarregarProdutos()
        {
            var produtos = _dataService.ObterProdutos();
            foreach (var produto in produtos)
            {
                ProdutosDisponiveis.Add(produto);
            }
        }

        private void AdicionarProduto()
        {
            var itemExistente = ItensPedido.FirstOrDefault(i => i.ProdutoId == ProdutoSelecionado.Id);
            
            if (itemExistente != null)
            {
                itemExistente.Quantidade += Quantidade;
            }
            else
            {
                var novoItem = new ItemPedido
                {
                    ProdutoId = ProdutoSelecionado.Id,
                    NomeProduto = ProdutoSelecionado.Nome,
                    Quantidade = Quantidade,
                    ValorUnitario = ProdutoSelecionado.Valor
                };
                ItensPedido.Add(novoItem);
            }

            CalcularValorTotal();
            Quantidade = 1;
        }

        private void RemoverProduto(object parameter)
        {
            if (parameter is ItemPedido item)
            {
                ItensPedido.Remove(item);
                CalcularValorTotal();
            }
        }

        private void CalcularValorTotal()
        {
            ValorTotal = ItensPedido.Sum(i => i.Subtotal);
        }

        private void FinalizarPedido()
        {
            if (!ItensPedido.Any())
            {
                CustomMessageBox.Show("Adicione pelo menos um produto ao pedido!", "Validação", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _pedido.Itens = ItensPedido.ToList();
            _pedido.FormaPagamento = FormaPagamentoSelecionada;
            _pedido.CalcularValorTotal();

            _dataService.SalvarPedido(_pedido);
            PedidoFinalizado = true;

            CustomMessageBox.Show("Pedido finalizado com sucesso!", "Sucesso", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
