using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.dataGridView2.DataError += new DataGridViewDataErrorEventHandler(dataGridView2_DataError);
            this.Column3.ValueType = typeof(DateTime);
            this.Column1.ValueType = typeof(int);
            this.Column2.ValueType = typeof(string);
            this.Column4.ValueType = typeof(string);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            dataGridView2.RefreshEdit();
            anError.ThrowException = false;            
        }        

        private void button1_Click_1(object sender, EventArgs e) //save
        {         
            DataTable dt = new DataTable();
            dt.TableName = "TestTable";
            for (int i = 0; i < dataGridView2.Columns.Count; i++)
            {
                string headerText = dataGridView2.Columns[i].HeaderText;
                headerText = Regex.Replace(headerText, "[-/, ]", "_");

                DataColumn column = new DataColumn(headerText);
                dt.Columns.Add(column);
            }

            foreach (DataGridViewRow DataGVRow in dataGridView2.Rows)
            {
                DataRow dataRow = dt.NewRow();
                dataRow["Код"] = DataGVRow.Cells[0].Value;
                dataRow["Название"] = DataGVRow.Cells[1].Value;
                dataRow["Дата"] = DataGVRow.Cells[2].Value;
                dataRow["Запись"] = DataGVRow.Cells[3].Value;

                dt.Rows.Add(dataRow);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML|*.xml";
            sfd.InitialDirectory = Directory.GetCurrentDirectory();
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    XmlTextWriter xmlSave = new XmlTextWriter(sfd.FileName, Encoding.UTF8);
                    xmlSave.Formatting = Formatting.Indented;
                    ds.DataSetName = "Table";
                    ds.Tables[0].WriteXml(xmlSave);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e) //load
        {
            for (int i = 0; i < dataGridView2.Rows.Count - 1; i++) dataGridView2.Rows.RemoveAt(0);

            DataSet ds = new DataSet();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML|*.xml";
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XmlReader xmlFile = XmlReader.Create(ofd.FileName, new XmlReaderSettings());
                    ds.ReadXml(xmlFile);
                    //dataGridView1.DataSource = ds.Tables[0].DefaultView; 
                    for (int j = 0; j < ds.Tables[0].Rows.Count-1; j++)
                    {
                        DataRow row = ds.Tables[0].Rows[j];
                        dataGridView2.Rows.Add();
                        for (int i = 0; i < row.ItemArray.Length; i++)
                        {                            
                            dataGridView2.Rows[j].Cells[i].Value = row.ItemArray[i];
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void dataGridView2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {            
            void Swap(int j)
            {
                DataGridViewRow temp = dataGridView2.Rows[j];
                dataGridView2.Rows.Remove(temp);
                dataGridView2.Rows.Insert(j + 1, temp);
            }
            Stopwatch sw = new Stopwatch();

            int count = 0;
            if(e.ColumnIndex != 0)
            {
                for(int i=0; i<dataGridView2.Rows.Count-1; i++)
                    if (dataGridView2.Rows[i].Cells[e.ColumnIndex].Value == null) dataGridView2.Rows[i].Cells[e.ColumnIndex].Value = "";
            }

            if (radioButton1.Checked) // Пузырёк
            {
                sw.Start();
                for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
                {
                    for (var j = 0; j < dataGridView2.Rows.Count - i - 2; j++)
                    {                        
                        if (e.ColumnIndex != 0)
                        {
                            if (String.Compare(dataGridView2.Rows[j].Cells[e.ColumnIndex].Value.ToString(), dataGridView2.Rows[j + 1].Cells[e.ColumnIndex].Value.ToString()) > 0)
                            {
                                Swap(j);
                                count++;
                            }
                        }
                        else if (int.Parse(dataGridView2.Rows[j].Cells[e.ColumnIndex].Value.ToString()) > int.Parse(dataGridView2.Rows[j + 1].Cells[e.ColumnIndex].Value.ToString()))
                            {
                                Swap(j);
                                count++;
                            }

                    }
                }

                sw.Stop();
                label3.Text = "Время: " + sw.Elapsed;
                label1.Text = "Количество шагов: " + count;
            }

            else if (radioButton2.Checked) // Бин. вставки
            {
                sw.Start();
                for (int i = 1; i < dataGridView2.Rows.Count - 1; i++)
                {                  
                    int low = 0;
                    int high = i - 1;
                    string temp = dataGridView2.Rows[i].Cells[e.ColumnIndex].Value.ToString();

                    while (low <= high)
                    {
                        int mid = (low + high) / 2;
                        if (e.ColumnIndex != 0)
                        {
                            if (String.Compare(temp, dataGridView2.Rows[mid].Cells[e.ColumnIndex].Value.ToString()) < 0)
                            {
                                high = mid - 1;
                            }
                            else low = mid + 1;

                        }
                        else if (int.Parse(temp) > int.Parse(dataGridView2.Rows[mid].Cells[e.ColumnIndex].Value.ToString()))
                        {
                            low = mid + 1;
                        }
                        else high = mid - 1;
                    }

                    for (int j = i - 1; j >= low; j--)
                    {
                        Swap(j);
                        count++;
                    }
                }
                sw.Stop();
                label4.Text = "Время: " + sw.Elapsed;
                label2.Text = "Количество шагов: " + count;
            }
        }
    }
}
