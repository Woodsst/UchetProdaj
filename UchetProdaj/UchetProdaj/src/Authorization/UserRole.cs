namespace UchetProdaj.Authorization
{
    /// <summary>
    /// Роль пользователя приложения; для пользователя, который ещё не вошёл в приложение, роль равна None.
    /// </summary>
    internal enum UserRole
    {
        None,
        Admin,
        Manager,
        Employee
    }

    /// <summary>
    /// Справочник ролей: ключи в файле .p.json, разбор значения role и русские названия для интерфейса.
    ///
    /// Поля:
    /// string AdminKey — ключ роли администратора в файле .p.json.
    /// string ManagerKey — ключ роли руководителя в файле .p.json.
    /// string EmployeeKey — ключ роли сотрудника в файле .p.json.
    /// </summary>
    internal static class UserRoles
    {
        internal const string AdminKey = "admin";
        internal const string ManagerKey = "manager";
        internal const string EmployeeKey = "employee";

        /// <summary>Разбирает значение поля role из файла .p.json в роль (регистр не важен, лишние пробелы по краям отбрасываются); для неизвестного значения возвращает false.</summary>
        internal static bool TryParse(string text, out UserRole role)
        {
            role = UserRole.None;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            switch (text.Trim().ToLowerInvariant())
            {
                case AdminKey:
                    role = UserRole.Admin;
                    return true;
                case ManagerKey:
                    role = UserRole.Manager;
                    return true;
                case EmployeeKey:
                    role = UserRole.Employee;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>Возвращает русское название роли для интерфейса.</summary>
        internal static string GetDisplayName(UserRole role)
        {
            switch (role)
            {
                case UserRole.Admin:
                    return "Администратор";
                case UserRole.Manager:
                    return "Руководитель";
                case UserRole.Employee:
                    return "Сотрудник";
                default:
                    return "роль не определена";
            }
        }
    }
}
