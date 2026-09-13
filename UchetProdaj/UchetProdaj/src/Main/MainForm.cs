using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using UchetProdaj.Authorization;
using UchetProdaj.Properties;

namespace UchetProdaj.Main
{
    /// <summary>
    /// Главное окно приложения («Учёт продаж»), открывается после успешной авторизации;
    /// состав доступных действий зависит от роли вошедшего пользователя (UserSession).
    ///
    /// Поля:
    /// OleDbDataAdapter adapter — адаптер для чтения таблицы «Продажи».
    /// DataTable table — прочитанная таблица «Продажи», привязанная к сетке dataGridView1.
    /// </summary>
    public partial class MainForm : Form
    {
        private OleDbDataAdapter adapter;
        private DataTable table;

        /// <summary>Создаёт главное окно, показывает в заголовке роль и логин пользователя и настраивает доступность кнопок по его правам.</summary>
        public MainForm()
        {
            InitializeComponent();
            ApplyRoleToInterface();
        }

        /// <summary>Подставляет в заголовок окна роль и логин пользователя и отключает кнопки действий, недоступных его роли.</summary>
        private void ApplyRoleToInterface()
        {
            Text = UserSession.IsAuthenticated
                ? string.Format("Учёт продаж — {0} ({1})", UserSession.RoleDisplayName, UserSession.Login)
                : "Учёт продаж — " + UserSession.RoleDisplayName;
            sell.Enabled = RolePermissions.Can(UserSession.Role, Permission.ViewSales);
            SaveChangesClick.Enabled = RolePermissions.Can(UserSession.Role, Permission.EditSales);
        }

        /// <summary>Проверяет право роли на действие: показывает сообщение и возвращает false, если действие недоступно (защита в обработчиках кнопок).</summary>
        private bool EnsurePermission(Permission permission)
        {
            if (RolePermissions.Can(UserSession.Role, permission)) return true;

            MessageBox.Show(this, "Недостаточно прав для этого действия. Текущая роль: " + UserSession.RoleDisplayName + ".",
                "Доступ запрещён", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        /// <summary>Обработчик кнопки «Продажа»: проверяет право роли на просмотр продаж и загружает таблицу «Продажи» в сетку.</summary>
        private void sell_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission(Permission.ViewSales)) return;

            string connStr = Settings.Default.productUchetConnectionString;

            adapter = new OleDbDataAdapter("SELECT * FROM Продажи", connStr);
            table = new DataTable();
            adapter.Fill(table);

            dataGridView1.DataSource = table;
        }

        /// <summary>Обработчик кнопки «Сохранить»: проверяет право роли на изменение данных (само сохранение ещё не реализовано).</summary>
        private void SaveChangesClick_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission(Permission.EditSales)) return;
        }

        private void otchet_Click(object sender, EventArgs e)
        {

        }

        private void LoadTable_Click(object sender, EventArgs e)
        {

        }
    }
}
