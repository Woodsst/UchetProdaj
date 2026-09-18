using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UchetProdaj.Properties;

namespace UchetProdaj.src.DBCommands
{
    public class DbCommands : IDisposable
    {
        private OleDbConnection conn;
        private OleDbDataAdapter _adapter;
        private DataTable _table;

        private Dictionary<string, string> tablesRequests = new Dictionary<string, string>()
        {
            { "Товары", "SELECT * FROM [Товары]" },
            { "Торговые предприятия", "Select * FROM [Торговые предприятия]" },
            { "Города", "Select * FROM [Города]" },
            { "Группы товаров", "Select * FROM [Группы товаров]" },
            { "Продажи", "Select * FROM [Продажи]" },
        };

        public string[] GetTables()
        {
            return tablesRequests.Keys.ToArray();
        }

        public DataTable GetAllDataFromTable(string tableName)
        {
            return Request(tablesRequests[tableName]);
        }

        public DbCommands()
        {
            conn = new OleDbConnection(Settings.Default.productUchetConnectionString);
            conn.Open();
        }

        private DataTable Request(string request)
        {
            using (var a = new OleDbDataAdapter(request, conn))
            {
                var t = new DataTable();
                a.Fill(t);
                return t;
            }
        }

        public DataTable GetProducts()
        {
            return Request("SELECT * FROM [Товары]");
        }

        public DataTable GetCustomers()
        {
            return Request("SELECT * FROM [Торговые предприятия]");
        }

        public DataTable GetSales()
        {
            _adapter = new OleDbDataAdapter("SELECT * FROM [Продажи]", conn);

            var builder = new OleDbCommandBuilder(_adapter)
            {
                QuotePrefix = "[",
                QuoteSuffix = "]"
            };
            _adapter.InsertCommand = builder.GetInsertCommand();
            _adapter.UpdateCommand = builder.GetUpdateCommand();
            _adapter.DeleteCommand = builder.GetDeleteCommand();

            _table = new DataTable();
            _adapter.Fill(_table);
            return _table;
        }

        public int SaveChanges()
        {
            if (_adapter == null || _table == null) return 0;

            if (!ChangeChecker()) return 0;
            int affected;
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    _adapter.InsertCommand.Transaction = tx;
                    _adapter.UpdateCommand.Transaction = tx;
                    _adapter.DeleteCommand.Transaction = tx;

                    affected = _adapter.Update(_table);
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }

            _table.AcceptChanges();
            return affected;
        }

        public void Dispose()
        {
            conn?.Close();
            conn?.Dispose();
        }

        private bool ChangeChecker()
        {
            var changes = _table.GetChanges();
            if (changes == null) return false;

            var data = new StringBuilder();

            foreach (DataRow row in changes.Rows)
            {
                foreach (DataColumn col in changes.Columns)
                {
                    if (row.RowState == DataRowState.Modified)
                    {
                        object oldValue = row[col, DataRowVersion.Original];
                        object newValue = row[col, DataRowVersion.Current];

                        if (!Equals(oldValue, newValue))
                        {
                            data.Append($"Изменён столбец '{col.ColumnName}': {oldValue} -> {newValue}");
                        }
                    }
                    else if (row.RowState == DataRowState.Added)
                    {
                        object newValue = row[col, DataRowVersion.Current];
                        data.Append($"Добавлено: {col.ColumnName} = {newValue}");
                    }
                    else if (row.RowState == DataRowState.Deleted)
                    {
                        object oldValue = row[col, DataRowVersion.Original];
                        data.Append($"Удалено: {col.ColumnName} = {oldValue}");
                    }
                }
            }
            AddChangeRecordInAuditTable(data.ToString());
            return true;
        }

        private void AddChangeRecordInAuditTable(string dataForAdd)
        {
            if (string.IsNullOrWhiteSpace(dataForAdd))
                return;

            const string sql = "INSERT INTO [Аудит] ([Изменения]) VALUES (?)";

            using (var cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("?", OleDbType.LongVarWChar).Value = dataForAdd;
                cmd.ExecuteNonQuery();
            }
        }

        public Dictionary<string, int> GetProductsList()
        {
            return GetOneColumnData("SELECT [Код товара], [название товара] FROM [Товары]");
        }
        public Dictionary<string, int> GetCompanyList()
        {
            return GetOneColumnData("SELECT [Код торгового предприятия], [Наименование] FROM [Торговые предприятия]");
        }
        private Dictionary<string, int> GetOneColumnData(string sql)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            DataTable productsTable = Request(sql);
            foreach (DataRow r in productsTable.Rows)
            {
                result.Add(r.ItemArray[1].ToString(), int.Parse(r.ItemArray[0].ToString()));
            }
            return result;
        }

        public int AddSell(int product, int company, int count)
        {
            string sql = "INSERT INTO [Продажи] ([Товар], [Торговое предприятие], [Кол-во]) VALUES (?, ?, ?)";

            using (OleDbCommand command = new OleDbCommand(sql, conn))
            {
                command.Parameters.AddWithValue("?", product);
                command.Parameters.AddWithValue("?", company);
                command.Parameters.AddWithValue("?", count);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected;
            }
        }
    }
}
