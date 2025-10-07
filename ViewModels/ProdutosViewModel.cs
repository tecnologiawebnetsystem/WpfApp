using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class ProdutosViewModel : BaseViewModel
    {
        private readonly DataService _dataService;
        private Produto _produtoSelecionado;
        private string _filtroNome;
        private string _filtroCodigo;
        private decimal? _filtroValorMin;
        private decimal? _filtroValorMax;
        private bool _modoEdicao;

        public ObservableCollection<Produto> Produtos { get; set; }

        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set
            {
                if (SetProperty(ref _produtoSelecionado, value))
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string FiltroNome
        {
            get => _filtroNome;
            set => SetProperty(ref _filtroNome, value);
        }

        public string FiltroCodigo
        {
            get => _filtroCodigo;
            set => SetProperty(ref _filtroCodigo, value);
        }

        public decimal? FiltroValorMin
        {
            get => _filtroValorMin;
            set => SetProperty(ref _filtroValorMin, value);
        }

        public decimal? FiltroValorMax
        {
            get => _filtroValorMax;
            set => SetProperty(ref _filtroValorMax, value);
        }

        public bool ModoEdicao
        {
            get => _modoEdicao;
            set
            {
                if (SetProperty(ref _modoEdicao, value))
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand PesquisarCommand { get; }
        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }

        public ProdutosViewModel()
        {
            _dataService = new DataService();
            Produtos = new ObservableCollection<Produto>();

            PesquisarCommand = new RelayCommand(_ => Pesquisar());
            IncluirCommand = new RelayCommand(_ => Incluir());
            EditarCommand = new RelayCommand(_ => Editar(), _ => ProdutoSelecionado != null && !ModoEdicao);
            SalvarCommand = new RelayCommand(_ => Salvar(), _ => ModoEdicao);
            ExcluirCommand = new RelayCommand(_ => Excluir(), _ => ProdutoSelecionado != null && !ModoEdicao);

            CarregarProdutos();
        }

        private void CarregarProdutos()
        {
            Produtos.Clear();
            var produtos = _dataService.ObterProdutos();
            foreach (var produto in produtos)
            {
                Produtos.Add(produto);
            }
        }

        private void Pesquisar()
        {
            Produtos.Clear();
            var resultado = _dataService.PesquisarProdutos(FiltroNome, FiltroCodigo, FiltroValorMin, FiltroValorMax);
            foreach (var produto in resultado)
            {
                Produtos.Add(produto);
            }
        }

        private void Incluir()
        {
            System.Diagnostics.Debug.WriteLine("[v0] ProdutosViewModel.Incluir() chamado");
            ProdutoSelecionado = new Produto
            {
                Nome = string.Empty,
                Codigo = string.Empty,
                Valor = 0
            };
            ModoEdicao = true;
            System.Diagnostics.Debug.WriteLine($"[v0] ModoEdicao = {ModoEdicao}, ProdutoSelecionado criado");
            System.Diagnostics.Debug.WriteLine($"[v0] ProdutoSelecionado.Nome = '{ProdutoSelecionado.Nome}' (Length: {ProdutoSelecionado.Nome.Length})");
            
            OnPropertyChanged(nameof(ProdutoSelecionado));
            OnPropertyChanged(nameof(ModoEdicao));
        }

        private void Editar()
        {
            ModoEdicao = true;
        }

        private void Salvar()
        {
            if (string.IsNullOrWhiteSpace(ProdutoSelecionado.Nome))
            {
                CustomMessageBox.Show("Nome é obrigatório!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ProdutoSelecionado.Codigo))
            {
                CustomMessageBox.Show("Código é obrigatório!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ProdutoSelecionado.Valor <= 0)
            {
                CustomMessageBox.Show("Valor deve ser maior que zero!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _dataService.SalvarProduto(ProdutoSelecionado);
            ModoEdicao = false;
            CarregarProdutos();
            CustomMessageBox.Show("Produto salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Excluir()
        {
            var result = CustomMessageBox.Show("Deseja realmente excluir este produto?", "Confirmação",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _dataService.ExcluirProduto(ProdutoSelecionado.Id);
                CarregarProdutos();
                ProdutoSelecionado = null;
                CustomMessageBox.Show("Produto excluído com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
