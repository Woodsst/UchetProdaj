using System.Runtime.Serialization;

namespace UchetProdaj.Authorization
{
    /// <summary>
    /// Учётная запись из файла .p.json.
    ///
    /// Свойства:
    /// string Password — пароль пользователя; обычные чтение и запись значения из файла.
    /// string Role — роль в виде строки из файла; разбирается методом UserRoles.TryParse; обычные чтение и запись.
    /// </summary>
    [DataContract]
    internal class UserAccount
    {
        [DataMember(Name = "password")]
        internal string Password { get; set; }

        [DataMember(Name = "role")]
        internal string Role { get; set; }
    }
}
