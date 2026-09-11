using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;

namespace UchetProdaj.Authorization
{
    /// <summary>
    /// Проверка учётных данных пользователя по файлу .p.json
    /// (формат: { "логин": "пароль" }) рядом с исполняемым файлом.
    /// </summary>
    internal static class AuthService
    {
        private const string UsersFileName = ".p.json";

        /// <summary>Полный путь к файлу с данными пользователей (рядом с исполняемым файлом).</summary>
        internal static string UsersFilePath
        {
            get { return Path.Combine(Application.StartupPath, UsersFileName); }
        }

        /// <summary>Логин пользователя, успешно прошедшего авторизацию.</summary>
        internal static string CurrentUserName;

        /// <summary>
        /// Проверяет пару логин/пароль по файлу .p.json. Файл перечитывается при каждом вызове.
        /// </summary>
        /// <param name="login">Логин (пробелы по краям убирает вызывающий код).</param>
        /// <param name="password">Пароль в том виде, в каком его ввёл пользователь.</param>
        /// <param name="error">Текст ошибки, если проверка не прошла.</param>
        /// <returns>true, если логин и пароль совпали; иначе false.</returns>
        internal static bool TryAuthenticate(string login, string password, out string error)
        {
            error = null;

            if (!File.Exists(UsersFilePath))
            {
                error = "Файл с данными пользователей не найден:" + Environment.NewLine + UsersFilePath;
                return false;
            }

            Dictionary<string, string> users;
            try
            {
                string json = File.ReadAllText(UsersFilePath, Encoding.UTF8);
                var serializer = new DataContractJsonSerializer(
                    typeof(Dictionary<string, string>),
                    new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    users = (Dictionary<string, string>)serializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                error = "Не удалось прочитать файл с данными пользователей:" + Environment.NewLine +
                        UsersFilePath + Environment.NewLine + ex.Message;
                return false;
            }

            string storedPassword;
            if (users != null && users.TryGetValue(login, out storedPassword) && storedPassword == password)
            {
                CurrentUserName = login;
                return true;
            }

            error = "Неверный логин или пароль.";
            return false;
        }
    }
}
