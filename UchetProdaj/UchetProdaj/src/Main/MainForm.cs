using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using UchetProdaj.Authorization;
using UchetProdaj.Properties;
using UchetProdaj.src.DBCommands;
using UchetProdaj.src.Main;

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
        private DbCommands dbCommands;

        /// <summary>Создаёт главное окно, показывает в заголовке роль и логин пользователя и настраивает доступность кнопок по его правам.</summary>
        public MainForm()
        {
            InitializeComponent();
            SetRoleTitle();
            SetRolePermissionPossibilities();
            dbCommands = new DbCommands();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoGenerateColumns = true;
            LoadTables(dbCommands.GetTables());
        }

        private void LoadTables(string[] tableNames)
        {
            comboForTables.Items.AddRange(tableNames);
        }

        /// <summary> и отключает кнопки действий, недоступных его роли.</summary>
        private void SetRolePermissionPossibilities()
        {
            sell.Enabled = RolePermissions.Can(UserSession.Role, Permission.ViewSales);
            LoadReport.Enabled = RolePermissions.Can(UserSession.Role, Permission.ViewSales);

            LoadReport.Enabled = RolePermissions.Can(UserSession.Role, Permission.EditSales);
            SaveChangesClick.Enabled = RolePermissions.Can(UserSession.Role, Permission.EditSales);
        }

        /// <summary> Подставляет в заголовок окна роль и логин пользователя /// </summary>
        private void SetRoleTitle()
        {
            Text = UserSession.IsAuthenticated
                ? string.Format("Учёт продаж — {0} ({1})", UserSession.RoleDisplayName, UserSession.Login)
                : "Учёт продаж — " + UserSession.RoleDisplayName;
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
            SellForm sellForm = new SellForm(dbCommands);
            sellForm.ShowDialog();
        }

        /// <summary>Обработчик кнопки «Сохранить»: проверяет право роли на изменение данных (само сохранение ещё не реализовано).</summary>
        private void SaveChangesClick_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission(Permission.EditSales)) return;
            try
            {
                dataGridView1.EndEdit();
                int affected = dbCommands.SaveChanges();
                MessageBox.Show($"Сохранено изменений: {affected}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void otchet_Click(object sender, EventArgs e)
        {
            
        }

        private void LoadTable_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboForTables.Text == "")
                {
                    MessageBox.Show("Укажите имя таблицы");
                    return;
                }
                dataGridView1.DataSource = dbCommands.GetAllDataFromTable(comboForTables.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            dbCommands?.Dispose();
            base.OnFormClosed(e);
        }

        private void LoadTable_Click_1(object sender, EventArgs e)
        {

        }
    }
}
