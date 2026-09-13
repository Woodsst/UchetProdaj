namespace UchetProdaj.Main
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.sell = new System.Windows.Forms.Button();
            this.SaveChangesClick = new System.Windows.Forms.Button();
            this.otchet = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.LoadTable = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // sell
            // 
            this.sell.Location = new System.Drawing.Point(12, 12);
            this.sell.Name = "sell";
            this.sell.Size = new System.Drawing.Size(75, 23);
            this.sell.TabIndex = 1;
            this.sell.Text = "Продажа";
            this.sell.UseVisualStyleBackColor = true;
            this.sell.Click += new System.EventHandler(this.sell_Click);
            // 
            // SaveChangesClick
            // 
            this.SaveChangesClick.Location = new System.Drawing.Point(381, 352);
            this.SaveChangesClick.Name = "SaveChangesClick";
            this.SaveChangesClick.Size = new System.Drawing.Size(75, 23);
            this.SaveChangesClick.TabIndex = 2;
            this.SaveChangesClick.Text = "Сохранить";
            this.SaveChangesClick.UseVisualStyleBackColor = true;
            this.SaveChangesClick.Click += new System.EventHandler(this.SaveChangesClick_Click);
            // 
            // otchet
            // 
            this.otchet.Location = new System.Drawing.Point(93, 12);
            this.otchet.Name = "otchet";
            this.otchet.Size = new System.Drawing.Size(75, 23);
            this.otchet.TabIndex = 3;
            this.otchet.Text = "Отчет";
            this.otchet.UseVisualStyleBackColor = true;
            this.otchet.Click += new System.EventHandler(this.otchet_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(328, 41);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // LoadTable
            // 
            this.LoadTable.Location = new System.Drawing.Point(374, 12);
            this.LoadTable.Name = "LoadTable";
            this.LoadTable.Size = new System.Drawing.Size(75, 23);
            this.LoadTable.TabIndex = 5;
            this.LoadTable.Text = "Загрузить";
            this.LoadTable.UseVisualStyleBackColor = true;
            this.LoadTable.Click += new System.EventHandler(this.LoadTable_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 68);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(444, 278);
            this.dataGridView1.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 389);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.LoadTable);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.otchet);
            this.Controls.Add(this.SaveChangesClick);
            this.Controls.Add(this.sell);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Учёт продаж";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button sell;
        private System.Windows.Forms.Button SaveChangesClick;
        private System.Windows.Forms.Button otchet;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button LoadTable;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
