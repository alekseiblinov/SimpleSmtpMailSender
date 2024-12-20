using System;
using System.Net;

namespace SimpleSmtpMailSender
{
    /// <summary>
    /// Общие методы и данные.
    /// </summary>
    internal static class CommonLogic
    {
        /// <summary>
        /// Формирование полезного для DevOps текста письма.
        /// </summary>
        internal static string GenerateMailBodyText(MainWindowViewModel model)
        {
            string result =
                $@"This email was sent from the utility SimpleSmtpMailSender.

UserName: {model.UserName}
Host: {model.Host}
Port: {model.Port}
Use SSL: {model.EnableSsl}
Use default credentials: {model.UseDefaultCredentials}
From: {model.From}
To: {model.To}

Sender device hostname: {GenerateCurrentDeviceHostname()}
Sender device IPs: {GenerateCurrentDeviceIpAddress()}

Send date and time (UTC): {DateTime.UtcNow}";

            return result;
        }

        /// <summary>
        /// Получение сетевого имени текущего устройства.
        /// </summary>
        private static string GenerateCurrentDeviceHostname()
        {
            string result = Dns.GetHostName();

            return result;
        }

        /// <summary>
        /// Получение перечня IP-адресов текущего устройства.
        /// </summary>
        private static string GenerateCurrentDeviceIpAddress()
        {
            string result = string.Empty;
            
            IPAddress[] hostAddresses = Dns.GetHostAddresses(GenerateCurrentDeviceHostname());

            foreach (IPAddress hostAddress in hostAddresses)
            {
                result += $"\r\n\t{hostAddress}";
            }

            return result;
        }
    }
}