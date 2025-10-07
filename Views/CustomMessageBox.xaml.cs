using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace WpfApp.Views
{
    public partial class CustomMessageBox : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        private CustomMessageBox(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            InitializeComponent();
            
            TitleText.Text = title;
            MessageText.Text = message;
            
            ConfigurarIcone(icon);
            
            ConfigurarBotoes(buttons);
            
            // A janela agora abre normalmente sem animação
        }

        private void ConfigurarIcone(MessageBoxImage icon)
        {
            switch (icon)
            {
                case MessageBoxImage.Information:
                    IconText.Text = "ℹ";
                    IconText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"));
                    HeaderBorder.Background = new LinearGradientBrush(
                        (Color)ColorConverter.ConvertFromString("#2196F3"),
                        (Color)ColorConverter.ConvertFromString("#1976D2"),
                        90);
                    break;
                    
                case MessageBoxImage.Warning:
                    IconText.Text = "⚠";
                    IconText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800"));
                    HeaderBorder.Background = new LinearGradientBrush(
                        (Color)ColorConverter.ConvertFromString("#FF9800"),
                        (Color)ColorConverter.ConvertFromString("#F57C00"),
                        90);
                    break;
                    
                case MessageBoxImage.Error:
                    IconText.Text = "✖";
                    IconText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                    HeaderBorder.Background = new LinearGradientBrush(
                        (Color)ColorConverter.ConvertFromString("#E74C3C"),
                        (Color)ColorConverter.ConvertFromString("#C0392B"),
                        90);
                    break;
                    
                case MessageBoxImage.Question:
                    IconText.Text = "?";
                    IconText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9C27B0"));
                    HeaderBorder.Background = new LinearGradientBrush(
                        (Color)ColorConverter.ConvertFromString("#9C27B0"),
                        (Color)ColorConverter.ConvertFromString("#7B1FA2"),
                        90);
                    break;
                    
                default:
                    IconText.Text = "💬";
                    IconText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E"));
                    HeaderBorder.Background = new LinearGradientBrush(
                        (Color)ColorConverter.ConvertFromString("#2C3E50"),
                        (Color)ColorConverter.ConvertFromString("#1A252F"),
                        90);
                    break;
            }
        }

        private void ConfigurarBotoes(MessageBoxButton buttons)
        {
            ButtonPanel.Children.Clear();

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AdicionarBotao("OK", MessageBoxResult.OK, true);
                    break;
                case MessageBoxButton.OKCancel:
                    AdicionarBotao("Cancelar", MessageBoxResult.Cancel, false);
                    AdicionarBotao("OK", MessageBoxResult.OK, true);
                    break;
                case MessageBoxButton.YesNo:
                    AdicionarBotao("Não", MessageBoxResult.No, false);
                    AdicionarBotao("Sim", MessageBoxResult.Yes, true);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AdicionarBotao("Cancelar", MessageBoxResult.Cancel, false);
                    AdicionarBotao("Não", MessageBoxResult.No, false);
                    AdicionarBotao("Sim", MessageBoxResult.Yes, true);
                    break;
            }
        }

        private void AdicionarBotao(string texto, MessageBoxResult resultado, bool isPrimary)
        {
            var button = new Button
            {
                Content = texto,
                MinWidth = 110,
                Height = 42,
                Margin = new Thickness(10, 0, 0, 0),
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            if (isPrimary)
            {
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"));
                button.Foreground = Brushes.White;
                button.BorderThickness = new Thickness(0);
            }
            else
            {
                button.Background = Brushes.White;
                button.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D"));
                button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7"));
                button.BorderThickness = new Thickness(2);
            }

            button.Style = CreateButtonStyle(isPrimary);

            button.Click += (s, e) =>
            {
                Result = resultado;
                DialogResult = true;
                Close();
            };

            ButtonPanel.Children.Add(button);
        }

        private Style CreateButtonStyle(bool isPrimary)
        {
            var style = new Style(typeof(Button));
            
            var template = new ControlTemplate(typeof(Button));
            var factory = new FrameworkElementFactory(typeof(Border));
            factory.Name = "border";
            factory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            factory.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
            factory.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));
            factory.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            factory.SetValue(Border.PaddingProperty, new Thickness(20, 10, 20, 10));
            
            var shadow = new DropShadowEffect
            {
                Color = Colors.Black,
                Opacity = 0.15,
                BlurRadius = 8,
                ShadowDepth = 2
            };
            factory.SetValue(Border.EffectProperty, shadow);

            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            factory.AppendChild(contentFactory);

            template.VisualTree = factory;

            var hoverTrigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            if (isPrimary)
            {
                hoverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, 
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2980B9"))));
            }
            else
            {
                hoverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, 
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF0F1"))));
            }
            template.Triggers.Add(hoverTrigger);

            style.Setters.Add(new Setter(Button.TemplateProperty, template));
            
            return style;
        }

        public static MessageBoxResult Show(string message, string title = "Mensagem", 
            MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            Console.WriteLine($"[v0] CustomMessageBox.Show() chamado - Título: {title}");
            
            try
            {
                var dialog = new CustomMessageBox(message, title, buttons, icon);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
                
                Console.WriteLine($"[v0] Modal fechada - Resultado: {dialog.Result}");
                return dialog.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[v0] ERRO ao exibir CustomMessageBox: {ex.Message}");
                throw;
            }
        }
    }
}
