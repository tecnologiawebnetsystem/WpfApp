namespace WpfApp.Views
{
    public partial class PessoaDetalhesModal : Window
    {
        private readonly Pessoa _pessoa;
        private readonly DataService _dataService;
        private List<Pedido> _pedidos;

        public PessoaDetalhesModal(Pessoa pessoa, DataService dataService)
        {

        private void BtnIncluirPedido_Click(object sender, RoutedEventArgs e)
        {
            var modal = new IncluirPedidoModal(_pessoa, _dataService);
            if (modal.ShowDialog() == true)
            {
                // Recarregar pedidos
                CarregarPedidos();
            }
        }

    }
}
