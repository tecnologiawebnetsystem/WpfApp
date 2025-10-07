using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.Views;
using System;

namespace WpfApp.ViewModels
{
    
    public class PessoasViewModel : BaseViewModel
    {
        
        private readonly DataService _dataService;
        
  
        public DataService DataService => _dataService;
        
    
        private readonly CepService _cepService;
        
        private string _nome;
        private string _cpf;
        private string _email;
        private string _telefone;
        private DateTime _dataNascimento;
        private string _cep;
        private string _logradouro;
        private string _numero;
        private string _complemento;
        private string _bairro;
        private string _cidade;
        private string _estado;
        
  
        public string Nome
        {
            get => _nome;
            set => SetProperty(ref _nome, value);
        }

        public string CPF
        {
            get => _cpf;
            set
            {
                if (SetProperty(ref _cpf, value))
                {
                    ValidarCpfEmTempoReal();
                }
            }
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Telefone
        {
            get => _telefone;
            set => SetProperty(ref _telefone, value);
        }

        public DateTime DataNascimento
        {
            get => _dataNascimento;
            set => SetProperty(ref _dataNascimento, value);
        }

        public string CEP
        {
            get => _cep;
            set => SetProperty(ref _cep, value);
        }

        public string Logradouro
        {
            get => _logradouro;
            set => SetProperty(ref _logradouro, value);
        }

        public string Numero
        {
            get => _numero;
            set => SetProperty(ref _numero, value);
        }

        public string Complemento
        {
            get => _complemento;
            set => SetProperty(ref _complemento, value);
        }

        public string Bairro
        {
            get => _bairro;
            set => SetProperty(ref _bairro, value);
        }

        public string Cidade
        {
            get => _cidade;
            set => SetProperty(ref _cidade, value);
        }

        public string Estado
        {
            get => _estado;
            set => SetProperty(ref _estado, value);
        }

   
        private Pessoa _pessoaSelecionada;
        
 
        private string _filtroNome;
        
    
        private string _filtroCpf;
        
  
        private bool _modoEdicao;

      
        private bool _buscandoCep;
        public bool BuscandoCep
        {
            get => _buscandoCep;
            set => SetProperty(ref _buscandoCep, value);
        }

    
        private bool _cpfValido;
        public bool CpfValido
        {
            get => _cpfValido;
            set => SetProperty(ref _cpfValido, value);
        }


        private string _cpfMensagemValidacao;
        public string CpfMensagemValidacao
        {
            get => _cpfMensagemValidacao;
            set => SetProperty(ref _cpfMensagemValidacao, value);
        }


        public ObservableCollection<Pessoa> Pessoas { get; set; }
        
       
        public ObservableCollection<Pessoa> TodasPessoas { get; set; }
        
  
        public ObservableCollection<Pedido> PedidosPessoa { get; set; }

   
        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set
            {
                if (_pessoaSelecionada != null)
                {
                    _pessoaSelecionada.PropertyChanged -= PessoaSelecionada_PropertyChanged;
                }

                if (SetProperty(ref _pessoaSelecionada, value))
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
                
                if (_pessoaSelecionada != null)
                {
                    _pessoaSelecionada.PropertyChanged += PessoaSelecionada_PropertyChanged;
                 
                    ValidarCpfEmTempoReal();
                }
                
  
                CarregarPedidosPessoa();
                
                if (_pessoaSelecionada != null)
                {
                    Nome = _pessoaSelecionada.Nome;
                    CPF = _pessoaSelecionada.CPF;
                    Email = _pessoaSelecionada.Email;
                    Telefone = _pessoaSelecionada.Telefone;
                    DataNascimento = _pessoaSelecionada.DataNascimento ?? DateTime.Now;
                    CEP = _pessoaSelecionada.CEP;
                    Logradouro = _pessoaSelecionada.Logradouro;
                    Numero = _pessoaSelecionada.Numero;
                    Complemento = _pessoaSelecionada.Complemento;
                    Bairro = _pessoaSelecionada.Bairro;
                    Cidade = _pessoaSelecionada.Cidade;
                    Estado = _pessoaSelecionada.Estado;
                }
            }
        }


        public string FiltroNome
        {
            get => _filtroNome;
            set
            {
                if (SetProperty(ref _filtroNome, value))
                {
                    PesquisarAutomatico();
                }
            }
        }


        public string FiltroCpf
        {
            get => _filtroCpf;
            set => SetProperty(ref _filtroCpf, value);
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

  
        private bool _filtrarPedidosEntregues;
        

        private bool _filtrarPedidosPagos;
        
  
        private bool _filtrarPedidosPendentes;

  
        public bool FiltrarPedidosEntregues
        {
            get => _filtrarPedidosEntregues;
            set
            {
                SetProperty(ref _filtrarPedidosEntregues, value);
      
                CarregarPedidosPessoa();
            }
        }


        public bool FiltrarPedidosPagos
        {
            get => _filtrarPedidosPagos;
            set
            {
                SetProperty(ref _filtrarPedidosPagos, value);
                CarregarPedidosPessoa();
            }
        }

   
        public bool FiltrarPedidosPendentes
        {
            get => _filtrarPedidosPendentes;
            set
            {
                SetProperty(ref _filtrarPedidosPendentes, value);
                CarregarPedidosPessoa();
            }
        }

      
        public ICommand PesquisarCommand { get; }
        
     
        public ICommand IncluirCommand { get; }
        
     
        public ICommand EditarCommand { get; }
        
       
        public ICommand SalvarCommand { get; }
        
    
        public ICommand ExcluirCommand { get; }
        
    
        public ICommand IncluirPedidoCommand { get; }
        

        public ICommand MarcarComoPagoCommand { get; }
        
    
        public ICommand MarcarComoEnviadoCommand { get; }
        
     
        public ICommand MarcarComoRecebidoCommand { get; }

      
        public ICommand BuscarEnderecoPorCepCommand { get; }


        public PessoasViewModel()
        {
            
            _dataService = new DataService();
            
            
            _cepService = new CepService();
            
            
            Pessoas = new ObservableCollection<Pessoa>();
            TodasPessoas = new ObservableCollection<Pessoa>();
            PedidosPessoa = new ObservableCollection<Pedido>();

            _dataNascimento = DateTime.Now;
            _nome = string.Empty;
            _cpf = string.Empty;
            _email = string.Empty;
            _telefone = string.Empty;
            _cep = string.Empty;
            _logradouro = string.Empty;
            _numero = string.Empty;
            _complemento = string.Empty;
            _bairro = string.Empty;
            _cidade = string.Empty;
            _estado = string.Empty;

            PesquisarCommand = new RelayCommand(_ => Pesquisar());
            
            
            IncluirCommand = new RelayCommand(_ => Incluir());
            
 
            EditarCommand = new RelayCommand(_ => Editar(), _ => PessoaSelecionada != null);
            

            SalvarCommand = new RelayCommand(_ => Salvar(), _ => ModoEdicao);
            
    
            ExcluirCommand = new RelayCommand(_ => Excluir(), _ => PessoaSelecionada != null && !ModoEdicao);
            

            IncluirPedidoCommand = new RelayCommand(_ => IncluirPedido(), _ => PessoaSelecionada != null && !ModoEdicao);
            

            MarcarComoPagoCommand = new RelayCommand(MarcarComoPago);
            MarcarComoEnviadoCommand = new RelayCommand(MarcarComoEnviado);
            MarcarComoRecebidoCommand = new RelayCommand(MarcarComoRecebido);


            BuscarEnderecoPorCepCommand = new RelayCommand(_ => BuscarEnderecoPorCep());

            CarregarPessoas();
        }


        private void CarregarPessoas()
        {
            Pessoas.Clear();
            TodasPessoas.Clear();
            var pessoas = _dataService.ObterPessoas();
            foreach (var pessoa in pessoas)
            {
                Pessoas.Add(pessoa);
                TodasPessoas.Add(pessoa);
            }
        }

        private void PesquisarAutomatico()
        {
            Pessoas.Clear();

            if (string.IsNullOrWhiteSpace(FiltroNome))
            {
                foreach (var pessoa in TodasPessoas)
                {
                    Pessoas.Add(pessoa);
                }
                return;
            }
            

            var filtroLower = FiltroNome.ToLower().Trim();
            var resultado = TodasPessoas.Where(p => 
                (p.Nome != null && p.Nome.ToLower().Contains(filtroLower)) ||
                (p.CPF != null && p.CPF.Replace(".", "").Replace("-", "").Contains(filtroLower.Replace(".", "").Replace("-", "")))
            ).ToList();
            
            foreach (var pessoa in resultado)
            {
                Pessoas.Add(pessoa);
            }
        }


        private void Pesquisar()
        {
            PesquisarAutomatico();
        }

        private void Incluir()
        {
            System.Diagnostics.Debug.WriteLine("[v0] PessoasViewModel.Incluir() chamado");
            

            PessoaSelecionada = new Pessoa
            {
                Nome = string.Empty,
                CPF = string.Empty,
                Email = string.Empty,
                Telefone = string.Empty,
                DataNascimento = DateTime.Now,
                CEP = string.Empty,
                Logradouro = string.Empty,
                Numero = string.Empty,
                Complemento = string.Empty,
                Bairro = string.Empty,
                Cidade = string.Empty,
                Estado = string.Empty
            };
            

            
            ModoEdicao = true;
            CpfValido = false;
            CpfMensagemValidacao = string.Empty;
            
            System.Diagnostics.Debug.WriteLine($"[v0] ModoEdicao = {ModoEdicao}, nova pessoa criada com campos vazios");
        }


        private void Editar()
        {
            ModoEdicao = true;
        }


        private void Salvar()
        {
            System.Diagnostics.Debug.WriteLine("[v0] PessoasViewModel.Salvar() chamado");
            
            if (PessoaSelecionada == null)
            {
                PessoaSelecionada = new Pessoa();
            }
            
            PessoaSelecionada.Nome = Nome;
            PessoaSelecionada.CPF = CPF;
            PessoaSelecionada.Email = Email;
            PessoaSelecionada.Telefone = Telefone;
            PessoaSelecionada.DataNascimento = DataNascimento;
            PessoaSelecionada.CEP = CEP;
            PessoaSelecionada.Logradouro = Logradouro;
            PessoaSelecionada.Numero = Numero;
            PessoaSelecionada.Complemento = Complemento;
            PessoaSelecionada.Bairro = Bairro;
            PessoaSelecionada.Cidade = Cidade;
            PessoaSelecionada.Estado = Estado;
            
            if (string.IsNullOrWhiteSpace(PessoaSelecionada.Nome))
            {
                CustomMessageBox.Show("Nome é obrigatório!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidationService.ValidarNome(PessoaSelecionada.Nome))
            {
                CustomMessageBox.Show("Nome inválido! Deve ter no mínimo 3 caracteres e não pode conter números.", 
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PessoaSelecionada.CPF))
            {
                CustomMessageBox.Show("CPF é obrigatório!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidationService.ValidarCPF(PessoaSelecionada.CPF))
            {
                CustomMessageBox.Show("CPF inválido!", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var cpfLimpo = PessoaSelecionada.CPF.Replace(".", "").Replace("-", "").Trim();
            var pessoasExistentes = _dataService.ObterPessoas();
            var cpfDuplicado = pessoasExistentes.Any(p => 
                p.Id != PessoaSelecionada.Id && 
                p.CPF.Replace(".", "").Replace("-", "").Trim() == cpfLimpo);

            if (cpfDuplicado)
            {
                CustomMessageBox.Show("Este CPF já está cadastrado no sistema!", 
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(PessoaSelecionada.Email))
            {
                if (!ValidationService.ValidarEmail(PessoaSelecionada.Email))
                {
                    CustomMessageBox.Show("Email inválido! Use o formato: exemplo@dominio.com", 
                        "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(PessoaSelecionada.Telefone))
            {
                if (!ValidationService.ValidarTelefone(PessoaSelecionada.Telefone))
                {
                    CustomMessageBox.Show("Telefone inválido! Use o formato: (11) 98765-4321", 
                        "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(PessoaSelecionada.CEP))
            {
                if (!ValidationService.ValidarFormatoCep(PessoaSelecionada.CEP))
                {
                    CustomMessageBox.Show("CEP inválido! Deve ter 8 dígitos.", 
                        "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            _dataService.SalvarPessoa(PessoaSelecionada);
            ModoEdicao = false;
            CarregarPessoas();
            CustomMessageBox.Show("Pessoa salva com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void Excluir()
        {
            var result = CustomMessageBox.Show("Deseja realmente excluir esta pessoa?", "Confirmação", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _dataService.ExcluirPessoa(PessoaSelecionada.Id);
                CarregarPessoas();
                PessoaSelecionada = null;
                CustomMessageBox.Show("Pessoa excluída com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void IncluirPedido()
        {
            if (PessoaSelecionada == null || PessoaSelecionada.Id == 0)
            {
                CustomMessageBox.Show("Selecione uma pessoa para incluir um pedido.", 
                    "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var modal = new IncluirPedidoModal(PessoaSelecionada, _dataService); 
            if (modal.ShowDialog() == true)
            {

                CarregarPedidosPessoa();
                CustomMessageBox.Show("Pedido criado com sucesso!", "Sucesso", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void CarregarPedidosPessoa()
        {
            PedidosPessoa.Clear();
            if (PessoaSelecionada != null)
            {

                var pedidos = _dataService.ObterPedidosPorPessoa(PessoaSelecionada.Id);
                

                if (FiltrarPedidosEntregues)
                {
                    pedidos = pedidos.Where(p => p.Status == StatusPedido.Recebido).ToList();
                }

                if (FiltrarPedidosPagos)
                {
                    pedidos = pedidos.Where(p => p.Status == StatusPedido.Pago).ToList();
                }
                

                if (FiltrarPedidosPendentes)
                {
                    pedidos = pedidos.Where(p => p.Status == StatusPedido.Pendente).ToList();
                }
                
                foreach (var pedido in pedidos)
                {
                    PedidosPessoa.Add(pedido);
                }
            }
        }

        private void MarcarComoPago(object parameter)
        {
            if (parameter is Pedido pedido)
            {
                _dataService.AtualizarStatusPedido(pedido.Id, StatusPedido.Pago);
                CarregarPedidosPessoa();
            }
        }

  
        private void MarcarComoEnviado(object parameter)
        {
            if (parameter is Pedido pedido)
            {
                _dataService.AtualizarStatusPedido(pedido.Id, StatusPedido.Enviado);
                CarregarPedidosPessoa();
            }
        }

   
        private void MarcarComoRecebido(object parameter)
        {
            if (parameter is Pedido pedido)
            {
                _dataService.AtualizarStatusPedido(pedido.Id, StatusPedido.Recebido);
                CarregarPedidosPessoa();
            }
        }


        public async void BuscarEnderecoPorCep()
        {
            if (string.IsNullOrWhiteSpace(CEP))
            {
                return;
            }

            var cep = CEP.Replace("-", "").Replace(".", "").Trim();

            if (cep.Length != 8)
            {
                return;
            }

            BuscandoCep = true;

            try
            {
                var endereco = await _cepService.BuscarEnderecoPorCep(cep);

                if (endereco != null)
                {
                    Logradouro = endereco.Logradouro;
                    Bairro = endereco.Bairro;
                    Cidade = endereco.Localidade;
                    Estado = endereco.Uf;
                    
                    CustomMessageBox.Show("Endereço encontrado! Preencha apenas o número.", 
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    CustomMessageBox.Show("CEP não encontrado. Verifique se digitou corretamente.", 
                        "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Erro ao buscar CEP: {ex.Message}", 
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BuscandoCep = false;
            }
        }


        private void PessoaSelecionada_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pessoa.CPF))
            {
                ValidarCpfEmTempoReal();
            }
        }


        private void ValidarCpfEmTempoReal()
        {
            if (string.IsNullOrWhiteSpace(CPF))
            {
                CpfValido = false;
                CpfMensagemValidacao = string.Empty;
                return;
            }

            var cpf = CPF.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length == 11)
            {
                if (ValidationService.ValidarCPF(CPF))
                {
                    CpfValido = true;
                    CpfMensagemValidacao = "✓ CPF válido";
                }
                else
                {
                    CpfValido = false;
                    CpfMensagemValidacao = "✗ CPF inválido";
                }
            }
            else
            {
                CpfValido = false;
                CpfMensagemValidacao = cpf.Length > 0 ? $"Digite {11 - cpf.Length} dígito(s)" : "";
            }
        }
    }
}
