using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UchetProdaj.src.DBCommands;

namespace UchetProdaj.src.Main
{
    public partial class SellForm : Form
    {
        private DbCommands db;
        private Dictionary<string, int> products;
        private Dictionary<string, int> companies;
        public SellForm(DbCommands db)
        {
            this.db = db;
            InitializeComponent();
            FillCombos();
        }

        private void FillCombos()
        {
            products = db.GetProductsList();
            companies = db.GetCompanyList();
            productComboBox.Items.AddRange(products.Keys.ToArray());
            companyComboBox.Items.AddRange(companies.Keys.ToArray());
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try 
            {
                if (productCount.Value <= 0)
                {
                    MessageBox.Show($"Кол-во товаров должно быть больше 0");
                }
                if (!products.ContainsKey(productComboBox.Text) || !companies.ContainsKey(companyComboBox.Text))
                {
                    MessageBox.Show($"Компания или товар указаны неверно");
                }
                int product = products[productComboBox.Text];
                int company = companies[companyComboBox.Text];
                int count = (int)productCount.Value;
                int result = db.AddSell(product, company, count); 
                if (result == 1)
                {
                    MessageBox.Show($"Добавленны данные о продаже\n{productComboBox.Text}\n{companyComboBox.Text}\n{productCount.Value} штук");
                }
                else
                {
                    MessageBox.Show($"Данные не добавлены");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Данные не добавлены");
            }
            
            }

        private void BackButton_Click(object sender, EventArgs e)
        {

        }
    }
}
