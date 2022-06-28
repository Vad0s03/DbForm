using Microsoft.Data.Sqlite;
using System;
namespace DbForm
{
    public partial class Form1 : Form
    {
        SqliteConnection? Connection;
        string DbPath = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (ConfigXML configXML = new())
            {
                DbPath = configXML.XmlGetNamedItem("DbPath");
            }

            try
            {
                Connection = new SqliteConnection($"Data Source = {DbPath}");
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно открыть файл: " + DbPath);
            }
            InitializeComboBox1();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    DbPath = openFileDialog.FileName;
                }
            }

            try
            {
                Connection = new SqliteConnection($"Data Source = {DbPath}");
            }
            catch (Exception)
            {

            }
            InitializeComboBox1();

            MessageBox.Show("Выбран файл:\n\n" + DbPath);

            using (ConfigXML configXML = new())
            {
                configXML.XmlWrite("DbPath", DbPath);
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            DbCommand(SqlCommandBox.Text, DataTb);
            tabControl1.SelectedIndex = 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DbCommand("Select * from " + comboBox1.SelectedItem.ToString(), DataTb2);
        }

        /// <summary>
        /// Обновляет список таблиц в БД
        /// </summary>
        private void InitializeComboBox1()
        {
            if (Connection != null)
            {
                Connection.Open();
                SqliteCommand command = Connection.CreateCommand();

                comboBox1.Items.Clear();

                command.CommandText = "SELECT name FROM sqlite_schema";

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        int j = 0;
                        while (reader.Read()) // построчно считываем данные
                        {
                            comboBox1.Items.Add(reader.GetValue(j).ToString());
                        }
                    }
                }
                Connection.Close();
            }
        }

        /// <summary>
        /// Выполняет запрос SQLite и заполняет таблицу результатами
        /// </summary>
        private void DbCommand(string dbRequest, DataGridView dataTb)
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Open();
                    SqliteCommand command = Connection.CreateCommand();

                    command.CommandText = dbRequest;
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        dataTb.ColumnCount = reader.FieldCount;
                        dataTb.Rows.Clear();

                        if (reader.HasRows) // если есть данные
                        {
                            int j = 0;
                            while (reader.Read())   // построчно считываем данные
                            {
                                dataTb.Rows.Add(1);
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    dataTb[i, j].Value = reader.GetValue(i).ToString();
                                }
                                j++;
                            }
                        }                        
                        SqlErrorBox.Text = string.Empty;
                        SqlErrorBox.BackColor = Color.White;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dataTb.Columns[i].HeaderText = reader.GetName(i);
                            dataTb.Columns[i].HeaderCell.Style.Alignment =
                                DataGridViewContentAlignment.MiddleCenter;
                            dataTb.Columns[i].Width = DataTb.Width / reader.FieldCount;
                        }

                    }
                }
                catch (Exception ex)
                {
                    SqlErrorBox.Text = ex.Message;
                    SqlErrorBox.BackColor = Color.DarkRed;
                }
                finally
                {
                    Connection.Close();
                }                                 
            }
        }

    }
}