using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SimpleSmtpMailSender.Annotations;
using SimpleSmtpMailSender.Properties;

namespace SimpleSmtpMailSender
{
    public class MainWindowViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        // Команда "Отправить".
        public RelayCommand SendCommand { get; }
        // Команда "Очистить лог".
        public RelayCommand CleanLogCommand { get; }

        /// <summary>
        /// Доступность UI главного окна программы для взаимодействия с пользователем.
        /// </summary>
        public bool MainWindowIsEnabled
        {
            get => _mainWindowIsEnabled;
            set
            {
                if (value == _mainWindowIsEnabled) return;
                _mainWindowIsEnabled = value;
                OnPropertyChanged(nameof(MainWindowIsEnabled));
            }
        }

        #region Поля для хранения данных, отображаемых во вью.
        [Required]
        [EmailAddress]
        public string From
        {
            get => _from;
            set
            {
                _from = value; 
                OnPropertyChanged(nameof(From));
                ValidateProperty(nameof(From), value);
                Settings.Default.From = value;
                Settings.Default.Save();

                // Если пользователь выбрал автоматически генерировать текст письма, то
                if (FillBodyTextAutomatically)
                {
                    // Генерация текста письма.
                    Body = GenerateMailBodyText();
                }
            }
        }

        [Required]
        [EmailAddress]
        public string To
        {
            get => _to;
            set
            {
                _to = value;
                OnPropertyChanged(nameof(To));
                ValidateProperty(nameof(To), value);
                Settings.Default.To = value;
                Settings.Default.Save();

                // Если пользователь выбрал автоматически генерировать текст письма, то
                if (FillBodyTextAutomatically)
                {
                    // Генерация текста письма.
                    Body = GenerateMailBodyText();
                }
            }
        }

        [Required]
        [MinLength(5)]
        public string Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                OnPropertyChanged(nameof(Subject));
                ValidateProperty(nameof(Subject), value);
                Settings.Default.Subject = value;
                Settings.Default.Save();

                // Если пользователь выбрал автоматически генерировать текст письма, то
                if (FillBodyTextAutomatically)
                {
                    // Генерация текста письма.
                    Body = GenerateMailBodyText();
                }
            }
        }

        [Required]
        [MinLength(15)]
        public string Body
        {
            get => _body;
            set
            {
                _body = value;
                OnPropertyChanged(nameof(Body));
                ValidateProperty(nameof(Body), value);
             
                // Если пользователь выбрал не генерировать автоматически текст письма, то
                if (!FillBodyTextAutomatically)
                {
                    // Сохранение в настройках введённого пользователем текста письма.
                    Settings.Default.Body = value;
                    Settings.Default.Save();
                }
            }
        }

        [Required]
        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                OnPropertyChanged(nameof(Host));
                ValidateProperty(nameof(Host), value);
                Settings.Default.Host = value;
                Settings.Default.Save();

                // Если пользователь выбрал автоматически генерировать текст письма, то
                if (FillBodyTextAutomatically)
                {
                    // Генерация текста письма.
                    Body = GenerateMailBodyText();
                }
            }
        }

        [Required]
        [Range(typeof(int), "1", "99999", ErrorMessage = "Incorrect port number")]
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
                ValidateProperty(nameof(Port), value);
                Settings.Default.Port = value;
                Settings.Default.Save();

                // Если пользователь выбрал автоматически генерировать текст письма, то
                if (FillBodyTextAutomatically)
                {
                    // Генерация текста письма.
                    Body = GenerateMailBodyText();
                }
            }
        }

        /// <summary>
        /// Требуется ли заполнять текст письма автоматически, или пользователь может ввести текст самостоятельно.
        /// </summary>
        [Required]
        public bool EnableSsl
        {
            get => _enableSsl;
            set
            {
                _enableSsl = value;
                OnPropertyChanged(nameof(EnableSsl));
                ValidateProperty(nameof(EnableSsl), value);
                Settings.Default.EnableSsl = value;
                Settings.Default.Save();

                // Если пользователь выбрал автоматически генерировать текст письма, то
                if (FillBodyTextAutomatically)
                {
                    // Генерация текста письма.
                    Body = GenerateMailBodyText();
                }
            }
        }

        [Required]
        public bool UseDefaultCredentials
        {
            get => _useDefaultCredentials;
            set
            {
                _useDefaultCredentials = value;
                OnPropertyChanged(nameof(UseDefaultCredentials));
                ValidateProperty(nameof(UseDefaultCredentials), value);
                Settings.Default.UseDefaultCredentials = value;
                Settings.Default.Save();

                // Если пользователь выбрал автоматически генерировать текст письма, то
                if (FillBodyTextAutomatically)
                {
                    // Генерация текста письма.
                    Body = GenerateMailBodyText();
                }
            }
        }

        [Required]
        public bool FillBodyTextAutomatically
        {
            get => _fillBodyTextAutomatically;
            set
            {
                _fillBodyTextAutomatically = value;
                OnPropertyChanged(nameof(FillBodyTextAutomatically));
                ValidateProperty(nameof(FillBodyTextAutomatically), value);
                Settings.Default.FillBodyTextAutomatically = value;
                Settings.Default.Save();

                // Если пользователь выбрал автоматическое заполнение текста письма, то 
                if (value)
                {
                    // Вызов функции генерации текста письма.
                    Body = GenerateMailBodyText();
                }
                else
                {
                    // В поле ввода текста письма выводится сохранённый в настройках текст письма.
                    Body = Settings.Default.Body;
                }
            }
        }

        [Required]
        [MinLength(1)]
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
                ValidateProperty(nameof(UserName), value);
                Settings.Default.UserName = value;
                Settings.Default.Save();

                // Если пользователь выбрал автоматически генерировать текст письма, то
                if (FillBodyTextAutomatically)
                {
                    // Генерация текста письма.
                    Body = GenerateMailBodyText();
                }
            }
        }

        [Required]
        [MinLength(1)]
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                Settings.Default.Password = value;
                Settings.Default.Save();
            }
        }

        public string OutputText
        {
            get => _outputText;
            set
            {
                _outputText = value;
                OnPropertyChanged(nameof(OutputText));
            }
        }
        #endregion Поля для хранения данных, отображаемых во вью.

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _mainWindowIsEnabled;

        #region Переменные для хранения данных полей, отображаемых во вью.
        private string _from;
        private string _to;
        private string _subject;
        private string _body;
        private string _host;
        private string _userName;
        private string _password;
        private int _port;
        private bool _enableSsl;
        private bool _useDefaultCredentials;
        private bool _fillBodyTextAutomatically;
        private string _outputText;
        #endregion Переменные для хранения данных полей, отображаемых во вью.

        public MainWindowViewModel()
        {
            SendCommand = new RelayCommand(SendMail, CanSendMail);
            CleanLogCommand = new RelayCommand(CleanLog, CanCleanLog);
            // Вызов функции заполнения полей ввода на форме данными из настроек.
            FillWindowFields();
            // Уведомление пользователя о готовности к работе, чтобы область вывода сообщений не выглядела пустой.
            WriteLogLine("Ready.");
            ErrorsChanged += OnErrorsChanged;
            // Главное окно программы становится доступным для пользователя.
            MainWindowIsEnabled = true;
        }

        /// <summary>
        /// Заполнение полей ввода на форме данными из настроек.
        /// </summary>
        private void FillWindowFields()
        {
            _from = Settings.Default.From;
            OnPropertyChanged(nameof(From));
            ValidateProperty(nameof(From), _from);
            _to = Settings.Default.To;
            OnPropertyChanged(nameof(To));
            ValidateProperty(nameof(To), _to);
            _subject = Settings.Default.Subject;
            OnPropertyChanged(nameof(Subject));
            ValidateProperty(nameof(Subject), _subject);
            _host = Settings.Default.Host;
            OnPropertyChanged(nameof(Host));
            ValidateProperty(nameof(Host), _host);
            _port = Settings.Default.Port;
            OnPropertyChanged(nameof(Port));
            ValidateProperty(nameof(Port), _port);
            _enableSsl = Settings.Default.EnableSsl;
            OnPropertyChanged(nameof(EnableSsl));
            ValidateProperty(nameof(EnableSsl), _enableSsl);      
            _fillBodyTextAutomatically = Settings.Default.FillBodyTextAutomatically;
            OnPropertyChanged(nameof(FillBodyTextAutomatically));
            ValidateProperty(nameof(FillBodyTextAutomatically), _fillBodyTextAutomatically);
            _useDefaultCredentials = Settings.Default.UseDefaultCredentials;
            OnPropertyChanged(nameof(UseDefaultCredentials));
            ValidateProperty(nameof(UseDefaultCredentials), _useDefaultCredentials);
            _userName = Settings.Default.UserName;
            OnPropertyChanged(nameof(UserName));
            ValidateProperty(nameof(UserName), _userName);
            _password = Settings.Default.Password;
            OnPropertyChanged(nameof(Password));
            ValidateProperty(nameof(Password), _password);

            // Если пользователь выбрал автоматически генерировать текст письма, то
            if (FillBodyTextAutomatically)
            {
                // Вызов функции генерации текста письма.
                Body = GenerateMailBodyText();
            }
            else
            {
                _body = Settings.Default.Body;
                OnPropertyChanged(nameof(Body));
                ValidateProperty(nameof(Body), _body);
            }
        }

        /// <summary>
        /// Отправка почтового сообщения.
        /// !!! Вынести в отдельный класс Mailing.
        /// </summary>
        private async void SendMail()
        {
            WriteLogLine("Sending a notification by e-mail.");

            // Главное окно программы становится недоступным для пользователя.
            MainWindowIsEnabled = false;
            // Вызов функции отправки письма по электронной почте в асинхронном режиме.
            await Task.Run(SendMailAsync);
        }

        /// <summary>
        /// Отправка письма по электронной почте.
        /// !!! Вынести в отдельный класс Mailing.
        /// </summary>
        private void SendMailAsync()
        {
            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = UseDefaultCredentials;
                smtpClient.Credentials = new NetworkCredential(UserName, Password);
                smtpClient.Host = Host;
                smtpClient.Port = Port;
                smtpClient.EnableSsl = EnableSsl;

                try
                {
                    MailMessage mail = new MailMessage
                                       {
                                           From = new MailAddress(From)
                                       };
                    mail.To.Add(To);
                    mail.Subject = Subject;
                    mail.Body = Body;
                    smtpClient.Send(mail);
                    WriteLogLine($"An email with the subject '{Subject}' was sent from the mailbox '{From}' to '{To}'.");
                }
                catch (Exception e)
                {
                    WriteLogLine($"Error: {e}");
                }
            }

            WriteLogLine("Sending the notification by e-mail is completed.");
            WriteLogLine("---------");
            // Главное окно программы становится доступным для пользователя.
            MainWindowIsEnabled = true;
        }

        /// <summary>
        /// Формирование полезного для DevOps текста письма.
        /// !!! Вынести в отдельный класс CommonLogic.
        /// </summary>
        private string GenerateMailBodyText()
        {
            string result =
$@"This email was sent from the utility SimpleSmtpMailSender.

UserName: {UserName}
Host: {Host}
Port: {Port}
Use SSL: {EnableSsl}
Use default credentials: {UseDefaultCredentials}
From: {From}
To: {To}

Sender device hostname: {GenerateCurrentDeviceHostname()}
Sender device IPs: {GenerateCurrentDeviceIpAddress()}

Send date and time (UTC): {DateTime.UtcNow}";

            return result;
        }

        /// <summary>
        /// Получение сетевого имени текущего устройства.
        /// !!! Вынести в отдельный класс CommonLogic.
        /// </summary>
        private string GenerateCurrentDeviceHostname()
        {
            string result = Dns.GetHostName();

            return result;
        }

        /// <summary>
        /// Получение перечня IP-адресов текущего устройства.
        /// !!! Вынести в отдельный класс CommonLogic.
        /// </summary>
        private string GenerateCurrentDeviceIpAddress()
        {
            string result = string.Empty;
            
            IPAddress[] hostAddresses = Dns.GetHostAddresses(GenerateCurrentDeviceHostname());

            foreach (IPAddress hostAddress in hostAddresses)
            {
                result += $"\r\n\t{hostAddress}";
            }

            return result;
        }

        /// <summary>
        /// Очистка лога.
        /// </summary>
        private void CleanLog()
        {
            // Очистка текста лога событий в окне.
            OutputText = string.Empty;
        }

        /// <summary>
        /// Доступность кнопки "Отправить".
        /// </summary>
        private bool CanSendMail()
        {
            return !HasErrors;
        }

        /// <summary>
        /// Доступность кнопки "Очистить лог".
        /// </summary>
        private bool CanCleanLog()
        {
            return !string.IsNullOrWhiteSpace(OutputText);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Вывод (добавление) указанного текста в область вывода сообщений главного окна программы (лог).
        /// </summary>
        /// <param name="logMessageText"></param>
        private void WriteLogLine(string logMessageText)
        {
            OutputText = string.IsNullOrWhiteSpace(OutputText) ?
                $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()} {logMessageText}" :
                $"{OutputText}\r\n{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()} {logMessageText}";
        }

        #region Валидация
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public bool HasErrors => _errors.Count > 0;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey((propertyName)))
            {
                return _errors[propertyName];
            }

            return null;
        }

        private void ValidateProperty<T>(string propertyName, T value)
        {
            var results = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(this)
            {
                MemberName = propertyName
            };
            Validator.TryValidateProperty(value, context, results);

            if (results.Any())
            {
                _errors[propertyName] = results.Select(c => c.ErrorMessage).ToList();
            }
            else
            {
                _errors.Remove(propertyName);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            SendCommand.OnCanExecuteChanged();
            CleanLogCommand.OnCanExecuteChanged();
        }
        #endregion Валидация
    }
}