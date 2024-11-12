using System.Windows;
using System.Windows.Controls;
using SimpleSmtpMailSender.Properties;

namespace SimpleSmtpMailSender
{
    /// <summary>
    /// Главное окно программы.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Подключение вью ко вьюмодели.
            DataContext = new MainWindowViewModel();
            // Контрол PasswordBox не позволяет делать binding, поэтому значение устанавливается здесь.
            psbPassword.Password = Settings.Default.Password;
        }

        /// <summary>
        /// Обработка изменения текста пароля пользователем.
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((dynamic)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Автоматическая прокрутка текста лога до конца вниз.
            ((TextBox)sender).ScrollToEnd();
        }
    }
}
