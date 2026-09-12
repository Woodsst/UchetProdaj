using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;

namespace UchetProdaj.Authorization
{
    /// <summary>
    /// Проверка учётных данных пользователя по файлу .p.json
    /// (формат: { "логин": { "password": "пароль", "role": "роль" } }) рядом с исполняемым файлом.
    /// Успешный вход открывает сессию (UserSession) с ролью пользователя; при неизвестной роли вход отклоняется.
    /// Поддерживается и старый формат { "логин": "пароль" }: такие пользователи входят с ролью «Сотрудник».
    ///
    /// Поля:
    /// string UsersFileName — имя файла с учётными записями рядом с исполняемым файлом.
    /// </summary>
    internal static class AuthService
    {
        private const string UsersFileName = ".p.json";

        /// <summary>Полный путь к файлу с данными пользователей (рядом с исполняемым файлом).</summary>
        internal static string UsersFilePath
        {
            get { return Path.Combine(Application.StartupPath, UsersFileName); }
        }

        /// <summary>
        /// Проверяет пару логин/пароль по файлу .p.json. Файл перечитывается при каждом вызове.
        /// При успешной проверке открывает сессию (UserSession) с ролью из файла.
        /// </summary>
        /// <param name="login">Логин (пробелы по краям убирает вызывающий код).</param>
        /// <param name="password">Пароль в том виде, в каком его ввёл пользователь.</param>
        /// <param name="error">Текст ошибки, если проверка не прошла.</param>
        /// <returns>true, если логин, пароль и роль корректны; иначе false.</returns>
        internal static bool TryAuthenticate(string login, string password, out string error)
        {
            error = null;

            if (!File.Exists(UsersFilePath))
            {
                error = "Файл с данными пользователей не найден:" + Environment.NewLine + UsersFilePath;
                return false;
            }

            Dictionary<string, UserAccount> accounts;
            bool legacyFormat;
            if (!TryReadAccounts(out accounts, out legacyFormat, out error))
            {
                return false;
            }

            UserAccount account;
            if (accounts == null || !accounts.TryGetValue(login, out account) || account == null
                || account.Password != password)
            {
                error = "Неверный логин или пароль.";
                return false;
            }

            UserRole role;
            if (legacyFormat)
            {
                role = UserRole.Employee;
            }
            else if (!UserRoles.TryParse(account.Role, out role))
            {
                error = "Для пользователя «" + login + "» не задана корректная роль:" + Environment.NewLine +
                        UsersFilePath + Environment.NewLine +
                        "Допустимые роли: " + UserRoles.AdminKey + ", " + UserRoles.ManagerKey + ", " + UserRoles.EmployeeKey + ".";
                return false;
            }

            UserSession.Start(login, role);
            return true;
        }

        /// <summary>
        /// Читает учётные записи из файла .p.json: сначала пробует формат с ролями, затем старый формат { "логин": "пароль" };
        /// во втором случае роль не задана (legacyFormat = true) и вход выполняется с ролью «Сотрудник».
        /// </summary>
        /// <returns>false, если файл не удалось прочитать ни в одном из форматов; текст ошибки возвращается в error.</returns>
        private static bool TryReadAccounts(out Dictionary<string, UserAccount> accounts, out bool legacyFormat, out string error)
        {
            accounts = null;
            legacyFormat = false;

            string json;
            try
            {
                json = File.ReadAllText(UsersFilePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                error = "Не удалось прочитать файл с данными пользователей:" + Environment.NewLine +
                        UsersFilePath + Environment.NewLine + ex.Message;
                return false;
            }

            try
            {
                accounts = Deserialize<Dictionary<string, UserAccount>>(json);
                if (accounts != null)
                {
                    error = null;
                    return true;
                }
            }
            catch (SerializationException)
            {
            }

            try
            {
                Dictionary<string, string> passwords = Deserialize<Dictionary<string, string>>(json);
                if (passwords != null)
                {
                    accounts = new Dictionary<string, UserAccount>();
                    foreach (KeyValuePair<string, string> pair in passwords)
                    {
                        accounts.Add(pair.Key, new UserAccount { Password = pair.Value });
                    }

                    legacyFormat = true;
                    error = null;
                    return true;
                }
            }
            catch (SerializationException)
            {
            }

            error = "Не удалось прочитать файл с данными пользователей:" + Environment.NewLine + UsersFilePath +
                    Environment.NewLine + "Ожидается запись вида \"логин\": { \"password\": \"пароль\", \"role\": \"роль\" }.";
            return false;
        }

        private static T Deserialize<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(
                typeof(T),
                new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
