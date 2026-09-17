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
            //dataGridView1.DataSource = dbCommands.GetSales();
            //GenerateProductColumn();
            //GenerateCustomerColumn();
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
            dataGridView1.DataSource = dbCommands.GetProducts();
        }

        private void LoadTable_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void GenerateProductColumn()
        {
            if (dataGridView1.Columns.Contains("Товар"))
                dataGridView1.Columns.Remove("Товар");
            var productTable = dbCommands.GetProducts();
            var combo = new DataGridViewComboBoxColumn
            {
                Name = "Товар",
                DataPropertyName = "Товар",
                HeaderText = "Товар",
                DataSource = productTable,
                DisplayMember = "название товара",
                ValueMember = "Код товара",
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                Width = 200
            };
            dataGridView1.Columns.Add(combo);
            combo.DisplayIndex = 0;
        }
        private void GenerateCustomerColumn()
        {
            if (dataGridView1.Columns.Contains("Торговое предприятие"))
                dataGridView1.Columns.Remove("Торговое предприятие");
            var productTable = dbCommands.GetCustomers();
            var combo = new DataGridViewComboBoxColumn
            {
                Name = "Торговое предприятие",
                DataPropertyName = "Торговое предприятие",
                HeaderText = "Торговое предприятие",
                DataSource = productTable,
                DisplayMember = "Наименование",
                ValueMember = "Код торгового предприятия",
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                FillWeight = 120
            };
            dataGridView1.Columns.Add(combo);
            combo.DisplayIndex = 1;
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            dbCommands?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
