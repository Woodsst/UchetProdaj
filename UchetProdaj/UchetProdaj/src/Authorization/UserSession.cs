namespace UchetProdaj.Authorization
{
    /// <summary>
    /// Сессия приложения: логин и роль пользователя, определённые при входе; отсюда берутся права для проверки действий.
    ///
    /// Свойства:
    /// string Login — логин вошедшего пользователя; до входа равен null; только чтение извне.
    /// UserRole Role — роль вошедшего пользователя; до входа равна UserRole.None; только чтение извне.
    /// string RoleDisplayName — русское название текущей роли для интерфейса; только чтение.
    /// bool IsAuthenticated — признак того, что пользователь вошёл; только чтение.
    /// </summary>
    internal static class UserSession
    {
        internal static string Login { get; private set; }

        internal static UserRole Role { get; private set; }

        internal static string RoleDisplayName
        {
            get { return UserRoles.GetDisplayName(Role); }
        }

        internal static bool IsAuthenticated
        {
            get { return Login != null; }
        }

        internal static void Start(string login, UserRole role)
        {
            Login = login;
            Role = role;
        }
    }
}
