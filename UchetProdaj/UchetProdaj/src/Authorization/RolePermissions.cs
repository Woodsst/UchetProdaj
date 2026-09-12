namespace UchetProdaj.Authorization
{
    /// <summary>Действия приложения, доступные не всем ролям.</summary>
    internal enum Permission
    {
        ViewSales, EditSales
    }

    /// <summary>
    /// Матрица прав: какие действия доступны каждой роли.
    /// Новое ограничиваемое действие добавляется значением в Permission и правилом в методе Can.
    /// </summary>
    internal static class RolePermissions
    {
        /// <summary>Проверяет, разрешено ли роли указанное действие: просмотр продаж — всем трём ролям, изменение данных — администратору и руководителю; роли None и неизвестным значениям запрещено всё.</summary>
        internal static bool Can(UserRole role, Permission permission)
        {
            switch (permission)
            {
                case Permission.ViewSales:
                    return role == UserRole.Admin || role == UserRole.Manager || role == UserRole.Employee;
                case Permission.EditSales:
                    return role == UserRole.Admin || role == UserRole.Manager;
                default:
                    return false;
            }
        }
    }
}
