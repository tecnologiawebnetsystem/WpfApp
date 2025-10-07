using System.Windows;

namespace WpfApp
{
    /// <summary>
    /// Classe principal da aplicação.
    /// É a primeira coisa que roda quando você abre o programa.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Método que roda quando o programa inicia.
        /// Aqui você pode colocar código que precisa executar antes de abrir a janela principal.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Aqui você pode adicionar código de inicialização se precisar
            // Por exemplo: verificar se tem atualizações, carregar configurações, etc
        }
    }
}
