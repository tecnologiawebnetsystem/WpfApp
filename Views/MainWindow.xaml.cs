using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Services;
using WpfApp.Views;

namespace WpfApp.Views
{

    public partial class MainWindow : Window
    {

        private bool _menuCollapsed = false;
        

        private readonly DataService _dataService;

 
        public MainWindow()
        {
            InitializeComponent();
            _dataService = new DataService();
       
            CarregarEstatisticas();
            
       
            RemoverBotaoFecharDashboard();

        
            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
        }

    
        private void RemoverBotaoFecharDashboard()
        {
            // Aguarda o carregamento completo da interface antes de procurar o botão
            Dispatcher.InvokeAsync(() =>
            {
                // Procura o botão de fechar dentro da aba Dashboard
                var button = FindVisualChild<Button>(DashboardTab);
                if (button != null && button.Content?.ToString() == "✕")
                {
                    // Esconde o botão X da aba Dashboard
                    button.Visibility = Visibility.Collapsed;
                }
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        /// <summary>
        /// Método auxiliar para encontrar elementos visuais na árvore de controles.
        /// Usado para encontrar o botão de fechar dentro da aba Dashboard.
        /// </summary>
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            // Percorre todos os filhos do elemento pai
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                
                // Se encontrou o tipo que procuramos, retorna
                if (child is T typedChild)
                    return typedChild;
                
                // Se não encontrou, procura nos filhos deste elemento (recursão)
                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }


        private void CarregarEstatisticas()
        {
            Debug.WriteLine("[v0] Carregando estatísticas do Dashboard...");
            
            // Conta quantas pessoas, produtos e pedidos existem e mostra nos cards
            TotalPessoas.Text = _dataService.ObterPessoas().Count.ToString();
            TotalProdutos.Text = _dataService.ObterProdutos().Count.ToString();
            TotalPedidos.Text = _dataService.ObterPedidos().Count.ToString();
            
            Debug.WriteLine($"[v0] Total de pedidos: {TotalPedidos.Text}");
            
            CarregarUltimosPedidos();
        }


        private void CarregarUltimosPedidos()
        {
            Debug.WriteLine("[v0] Carregando últimos pedidos...");
            
            var todosPedidos = _dataService.ObterPedidos();
            var todasPessoas = _dataService.ObterPessoas();
            
            Debug.WriteLine($"[v0] Total de pedidos encontrados: {todosPedidos.Count}");
            
            // Pega os 10 pedidos mais recentes e cria objetos com as informações formatadas
            var ultimosPedidos = todosPedidos
                .OrderByDescending(p => p.DataPedido)
                .Take(10)
                .Select(p => new
                {
                    Id = $"#{p.Id}",
                    ClienteNome = todasPessoas.FirstOrDefault(pessoa => pessoa.Id == p.PessoaId)?.Nome ?? "Cliente não encontrado",
                    ValorTotal = p.ValorTotal,
                    Status = p.Status,
                    DataPedido = p.DataPedido
                })
                .ToList();
            
            Debug.WriteLine($"[v0] Últimos pedidos carregados: {ultimosPedidos.Count}");
            
            GridUltimosPedidos.ItemsSource = ultimosPedidos;
            
            Debug.WriteLine($"[v0] GridUltimosPedidos.Visibility: {GridUltimosPedidos.Visibility}");
            Debug.WriteLine($"[v0] GridUltimosPedidos.ActualHeight: {GridUltimosPedidos.ActualHeight}");
        }


        private void ToggleMenu(object sender, RoutedEventArgs e)
        {
            // Inverte o estado do menu
            _menuCollapsed = !_menuCollapsed;

            if (_menuCollapsed)
            {
                // Menu recolhido: só mostra os ícones
                Sidebar.Width = 60;
                MenuText1.Visibility = Visibility.Collapsed;
                MenuText2.Visibility = Visibility.Collapsed;     
                MenuText4.Visibility = Visibility.Collapsed;
                MenuText5.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Menu expandido: mostra ícones e textos
                Sidebar.Width = 250;
                MenuText1.Visibility = Visibility.Visible;
                MenuText2.Visibility = Visibility.Visible;
                MenuText4.Visibility = Visibility.Visible;
                MenuText5.Visibility = Visibility.Visible;
            }
        }


        private void MostrarNotificacoes(object sender, RoutedEventArgs e)
        {
            CustomMessageBox.Show("Você não tem novas notificações.", 
                "Notificações", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void AbrirConfiguracoes(object sender, RoutedEventArgs e)
        {
            CustomMessageBox.Show(
                $"Formato de dados atual: XML\n\n" +
                "Para alterar, use o seletor no topo da tela (ao lado das notificações).", 
                "Configurações", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }


        private void AbrirPessoas(object sender, RoutedEventArgs e)
        {
            // Verifica se já existe uma aba de Pessoas aberta
            foreach (TabItem tab in MainTabControl.Items)
            {
                if (tab.Header.ToString().Contains("Pessoas"))
                {
                    // Se já existe, só seleciona ela
                    MainTabControl.SelectedItem = tab;
                    return;
                }
            }

            // Se não existe, cria uma nova aba
            var pessoasWindow = new PessoasWindow();
            var novaAba = new TabItem
            {
                Header = "👥 Pessoas",
                Content = pessoasWindow.Content  // Pega o conteúdo da janela e coloca na aba
            };
            
            // Adiciona a nova aba e seleciona ela
            MainTabControl.Items.Add(novaAba);
            MainTabControl.SelectedItem = novaAba;
        }


        private void AbrirProdutos(object sender, RoutedEventArgs e)
        {
            // Verifica se já existe uma aba de Produtos aberta
            foreach (TabItem tab in MainTabControl.Items)
            {
                if (tab.Header.ToString().Contains("Produtos"))
                {
                    // Se já existe, só seleciona ela
                    MainTabControl.SelectedItem = tab;
                    return;
                }
            }

            // Se não existe, cria uma nova aba
            var produtosWindow = new ProdutosWindow();
            var novaAba = new TabItem
            {
                Header = "📦 Produtos",
                Content = produtosWindow.Content  // Pega o conteúdo da janela e coloca na aba
            };
            
            // Adiciona a nova aba e seleciona ela
            MainTabControl.Items.Add(novaAba);
            MainTabControl.SelectedItem = novaAba;
        }


        private void AbrirPedidos(object sender, RoutedEventArgs e)
        {
            // Verifica se já existe uma aba de Pedidos aberta
            foreach (TabItem tab in MainTabControl.Items)
            {
                if (tab.Header.ToString().Contains("Pedidos"))
                {
                    // Se já existe, só seleciona ela
                    MainTabControl.SelectedItem = tab;
                    return;
                }
            }

            // Se não existe, cria uma nova aba
            var pedidosWindow = new PedidosWindow();
            var novaAba = new TabItem
            {
                Header = "🛒 Pedidos",
                Content = pedidosWindow.Content  // Pega o conteúdo da janela e coloca na aba
            };
            
            // Adiciona a nova aba e seleciona ela
            MainTabControl.Items.Add(novaAba);
            MainTabControl.SelectedItem = novaAba;
        }


        private void FecharAba_Click(object sender, RoutedEventArgs e)
        {
            // Pega o botão que foi clicado e a aba correspondente
            var button = sender as Button;
            var tabItem = button?.Tag as TabItem;
            
            // Se não é a aba Dashboard, pode fechar
            if (tabItem != null && tabItem != DashboardTab)
            {
                // Remove a aba
                MainTabControl.Items.Remove(tabItem);
                
                // Volta pra aba Dashboard
                MainTabControl.SelectedItem = DashboardTab;
            }
        }

        
        private void AbrirInstagram(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://instagram.com") { UseShellExecute = true });
        }

  
        private void AbrirFacebook(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://facebook.com") { UseShellExecute = true });
        }


        private void AbrirLinkedIn(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://linkedin.com") { UseShellExecute = true });
        }

   
        private void AbrirYouTube(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://youtube.com") { UseShellExecute = true });
        }

    
        private void Sair(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void AbrirMenuPerfil(object sender, RoutedEventArgs e)
        {
            ModalPerfil.Visibility = Visibility.Visible;
        }

   
        private void FecharModalPerfil(object sender, MouseButtonEventArgs e)
        {
            ModalPerfil.Visibility = Visibility.Collapsed;
        }

     
        private void PrevenirFechamento(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;  
        }


        private void AbrirPerfil(object sender, RoutedEventArgs e)
        {
            ModalPerfil.Visibility = Visibility.Collapsed;
            CustomMessageBox.Show(
                "Tela de perfil em desenvolvimento.\n\n" +
                "Aqui você poderá editar seus dados pessoais, alterar senha e foto.",
                "Perfil",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

       
        private void SairDoSistema(object sender, RoutedEventArgs e)
        {
            ModalPerfil.Visibility = Visibility.Collapsed;
            Application.Current.Shutdown();
        }

       
        private void VoltarDashboard_Click(object sender, RoutedEventArgs e)
        {
            VoltarDashboard();
        }

       
        public void VoltarDashboard()
        {
            MainTabControl.SelectedItem = DashboardTab;
        }

        
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (MainTabControl.SelectedItem == DashboardTab)
            {
                
                CarregarEstatisticas();
            }
        }
    }
}
