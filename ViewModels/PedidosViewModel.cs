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
    public class PedidosViewModel : BaseViewModel
    {
        private readonly DataService _dataService;
        private string _textoPesquisa;
        private ObservableCollection<Pedido> _pedidos;
        private ObservableCollection<Pedido> _pedidosFiltrados;
        private string _descricaoFiltroAtivo;

        public ObservableCollection<Pedido> Pedidos
        {
            get => _pedidos;
            set => SetProperty(ref _pedidos, value);
        }

        public ObservableCollection<Pedido> PedidosFiltrados
        {
            get => _pedidosFiltrados;
            set => SetProperty(ref _pedidosFiltrados, value);
        }

        public string TextoPesquisa
        {
            get => _textoPesquisa;
            set
            {
                SetProperty(ref _textoPesquisa, value);
                FiltrarPedidos();
            }
        }

        public string DescricaoFiltroAtivo
        {
            get => _descricaoFiltroAtivo;
            set => SetProperty(ref _descricaoFiltroAtivo, value);
        }

        public int TotalPedidos => Pedidos?.Count ?? 0;
        public int TotalPedidosFiltrados => PedidosFiltrados?.Count ?? 0;
        public decimal ValorTotalFiltrado => PedidosFiltrados?.Sum(p => p.ValorTotal) ?? 0;
        public decimal TicketMedio => TotalPedidosFiltrados > 0 ? ValorTotalFiltrado / TotalPedidosFiltrados : 0;

        public ICommand NovoPedidoCommand { get; }
        public ICommand AtualizarCommand { get; }
        public ICommand VisualizarDetalhesCommand { get; }
        public ICommand FiltrarHojeCommand { get; }
        public ICommand FiltrarSemanaCommand { get; }
        public ICommand FiltrarMesCommand { get; }
        public ICommand FiltrarPagamentoCommand { get; }
        public ICommand FiltrarStatusCommand { get; }
        public ICommand LimparFiltrosCommand { get; }

        public PedidosViewModel()
        {
            _dataService = new DataService();
            Pedidos = new ObservableCollection<Pedido>();
            PedidosFiltrados = new ObservableCollection<Pedido>();
            DescricaoFiltroAtivo = "Mostrando todos os pedidos";

            NovoPedidoCommand = new RelayCommand(_ => CustomMessageBox.Show("Funcionalidade de novo pedido em desenvolvimento.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information));
            AtualizarCommand = new RelayCommand(_ => CarregarPedidos());
            VisualizarDetalhesCommand = new RelayCommand(VisualizarDetalhes);
            FiltrarHojeCommand = new RelayCommand(_ => FiltrarPorPeriodo("hoje"));
            FiltrarSemanaCommand = new RelayCommand(_ => FiltrarPorPeriodo("semana"));
            FiltrarMesCommand = new RelayCommand(_ => FiltrarPorPeriodo("mes"));
            FiltrarPagamentoCommand = new RelayCommand(FiltrarPorPagamento);
            FiltrarStatusCommand = new RelayCommand(FiltrarPorStatus);
            LimparFiltrosCommand = new RelayCommand(_ => LimparFiltros());

            CarregarPedidos();
        }

        private void CarregarPedidos()
        {
            Pedidos.Clear();
            var pedidos = _dataService.ObterPedidos();
            
            foreach (var pedido in pedidos)
            {
                Pedidos.Add(pedido);
            }

            FiltrarPedidos();
            OnPropertyChanged(nameof(TotalPedidos));
            AtualizarEstatisticas();
        }

        private void FiltrarPedidos()
        {
            PedidosFiltrados.Clear();

            if (string.IsNullOrWhiteSpace(TextoPesquisa))
            {
                foreach (var pedido in Pedidos)
                {
                    PedidosFiltrados.Add(pedido);
                }
            }
            else
            {
                var termo = TextoPesquisa.ToLower();
                var filtrados = Pedidos.Where(p =>
                    p.Id.ToString().Contains(termo) ||
                    p.PessoaId.ToString().Contains(termo) ||
                    p.FormaPagamento.ToString().ToLower().Contains(termo)
                );

                foreach (var pedido in filtrados)
                {
                    PedidosFiltrados.Add(pedido);
                }
            }
            
            AtualizarEstatisticas();
        }

        private void FiltrarPorPeriodo(string periodo)
        {
            PedidosFiltrados.Clear();
            DateTime dataInicio;
            string descricao;

            switch (periodo.ToLower())
            {
                case "hoje":
                    dataInicio = DateTime.Today;
                    descricao = $"Mostrando pedidos de hoje ({DateTime.Today:dd/MM/yyyy})";
                    break;
                case "semana":
                    dataInicio = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    descricao = $"Mostrando pedidos desta semana (desde {dataInicio:dd/MM/yyyy})";
                    break;
                case "mes":
                    dataInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    descricao = $"Mostrando pedidos deste mês ({dataInicio:MMMM/yyyy})";
                    break;
                default:
                    return;
            }

            var filtrados = Pedidos.Where(p => p.DataPedido >= dataInicio);
            foreach (var pedido in filtrados)
            {
                PedidosFiltrados.Add(pedido);
            }

            DescricaoFiltroAtivo = descricao;
            AtualizarEstatisticas();
        }

        private void FiltrarPorPagamento(object parameter)
        {
            if (parameter is string formaPagamento)
            {
                PedidosFiltrados.Clear();
                
                var filtrados = Pedidos.Where(p => p.FormaPagamento.ToString() == formaPagamento);
                foreach (var pedido in filtrados)
                {
                    PedidosFiltrados.Add(pedido);
                }

                DescricaoFiltroAtivo = $"Mostrando pedidos pagos com {formaPagamento}";
                AtualizarEstatisticas();
            }
        }

        private void FiltrarPorStatus(object parameter)
        {
            if (parameter is string status)
            {
                PedidosFiltrados.Clear();
                
                var filtrados = Pedidos.Where(p => p.Status.ToString() == status);
                foreach (var pedido in filtrados)
                {
                    PedidosFiltrados.Add(pedido);
                }

                DescricaoFiltroAtivo = $"Mostrando pedidos com status: {status}";
                AtualizarEstatisticas();
            }
        }

        private void LimparFiltros()
        {
            PedidosFiltrados.Clear();
            foreach (var pedido in Pedidos)
            {
                PedidosFiltrados.Add(pedido);
            }

            DescricaoFiltroAtivo = "Mostrando todos os pedidos";
            AtualizarEstatisticas();
        }

        private void AtualizarEstatisticas()
        {
            OnPropertyChanged(nameof(TotalPedidosFiltrados));
            OnPropertyChanged(nameof(ValorTotalFiltrado));
            OnPropertyChanged(nameof(TicketMedio));
        }

        private void VisualizarDetalhes(object parameter)
        {
            if (parameter is Pedido pedido)
            {
                var detalhes = $"Pedido #{pedido.Id}\n\n";
                detalhes += $"Cliente ID: {pedido.PessoaId}\n";
                detalhes += $"Data: {pedido.DataPedido:dd/MM/yyyy HH:mm}\n";
                detalhes += $"Forma de Pagamento: {pedido.FormaPagamento}\n\n";
                detalhes += "Itens do Pedido:\n";
                detalhes += new string('-', 50) + "\n";

                foreach (var item in pedido.Itens)
                {
                    detalhes += $"{item.NomeProduto}\n";
                    detalhes += $"  Quantidade: {item.Quantidade} x {item.ValorUnitario:C} = {item.Subtotal:C}\n\n";
                }

                detalhes += new string('-', 50) + "\n";
                detalhes += $"VALOR TOTAL: {pedido.ValorTotal:C}";

                CustomMessageBox.Show(detalhes, "Detalhes do Pedido", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
