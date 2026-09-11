using System;
using System.Windows.Forms;
using UchetProdaj.Authorization;
using UchetProdaj.Main;

namespace UchetProdaj
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (login.Length == 0 || password.Length == 0)
            {
                MessageBox.Show(this, "Введите логин и пароль.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string error;
            if (!AuthService.TryAuthenticate(login, password, out error))
            {
                MessageBox.Show(this, error, "Ошибка входа",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.SelectAll();
                txtPassword.Focus();
                return;
            }

            Hide();
            using (var mainForm = new MainForm())
            {
                mainForm.ShowDialog();
            }
            Close();
        }
    }
}
