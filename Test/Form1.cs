using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.dataGridView1.DataError += new DataGridViewDataErrorEventHandler(dataGridView1_DataError);
            this.Вход.ValueType = typeof(int);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
                

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.OpenFile();
        }


        private void Clear(int col)                 //Очистка столбца
        {
            for(int i=0; i<dataGridView1.RowCount-1; i++)
            {
                dataGridView1.Rows[i].Cells[col].Value = null;
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            dataGridView1.RefreshEdit();
            anError.ThrowException = false;            
        }        

        private void button3_Click(object sender, EventArgs e)        //Пузырёк
        {
            void Swap(ref int e1, ref int e2)
            {
                var temp = e1;
                e1 = e2;
                e2 = temp;
            }
            Clear(1);

            int[] array = new int[dataGridView1.RowCount - 1];
            int nulls = 0;
            int len = dataGridView1.RowCount - 1;

            for (int i = 0; i<len; i++)             //Копирование значений первой колонки в массив
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null && dataGridView1.Rows[i].Cells[0].Value.ToString() != string.Format(""))
                {
                    array[i-nulls] = int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                }
                else nulls++;

            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            len = len - nulls;
            int count = 0;
            for (var i = 1; i < len; i++)           // Сортировка
            {
                for (var j = 0; j < len - i; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        Swap(ref array[j], ref array[j + 1]);
                        count++;
                    }
                    
                }
            }
            stopwatch.Stop();

            for (int i = 0; i < len; i++)           // Вывод во второй столбец
            {
                dataGridView1.Rows[i].Cells[1].Value = array[i];
            }

            label3.Text = "Время: " + stopwatch.Elapsed;
            label1.Text = "Количество шагов: " + count;            
        }

        private void button4_Click(object sender, EventArgs e)        //Метод вставки с бинарным включением
        {            
            Clear(2);

            int[] array = new int[dataGridView1.RowCount - 1];
            int nulls = 0;
            int len = dataGridView1.RowCount - 1;

            for (int i = 0; i < len; i++)             //Копирование значений первой колонки в массив
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null && dataGridView1.Rows[i].Cells[0].Value.ToString() != string.Format(""))
                {
                    array[i - nulls] = int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                }
                else nulls++;

            }

            len = len - nulls;
            int count = 0;
            //int key, lo, hi, mid = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 1; i < len; i++)
            {
                int x = array[i];

                int j = Math.Abs(
                    Array.BinarySearch(array,
                                       0, i, x) + 1);

                System.Array.Copy(array, j,
                                  array, j + 1,
                                  i - j);

                array[j] = x;
                count += array.Length-j;
            }
            stopwatch.Stop();

            for (int i = 0; i < len; i++)           // Вывод во второй столбец
            {
                dataGridView1.Rows[i].Cells[2].Value = array[i];
            }

            label2.Text = "Количество шагов: " + count;
            label4.Text = "Время: " + stopwatch.Elapsed;
        }

        private void button1_Click_1(object sender, EventArgs e) //save
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(dataGridView1.Columns[0].Name);

            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    DataRow dRow = dt.NewRow();
            //    foreach (DataGridViewCell cell in row.Cells)
            //    {
            //        dRow[cell.ColumnIndex] = cell.Value;
            //    }
            //    dt.Rows.Add(dRow);
            //};

            object[] cellValues = new object[dataGridView1.Columns.Count];
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    cellValues[i] = row.Cells[i].Value;
                }
                dt.Rows.Add(cellValues);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML|*.xml";
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
                    //dt.WriteXml(sfd.FileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) //load
        {
            for (int i = 0; i < 3; i++) Clear(i);
            for(int i = 1; i < dataGridView1.Rows.Count; i++) dataGridView1.Rows.RemoveAt(1);

            DataSet ds = new DataSet();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML|*.xml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XmlReader xmlFile = XmlReader.Create(ofd.FileName, new XmlReaderSettings());
                    ds.ReadXml(xmlFile);
                    //dataGridView1.DataSource = ds.Tables[0].DefaultView; 
                    for (int j = 0; j< ds.Tables[0].Rows.Count; j++)
                    {
                        DataRow row = ds.Tables[0].Rows[j];
                        dataGridView1.Rows.Add();
                        for(int i = 0; i<row.ItemArray.Length; i++)
                        {
                            dataGridView1.Rows[j].Cells[i].Value = row.ItemArray[i];
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private int[][] getarray(int col)
        {
            int[][] array = new int[2][];
            array[0] = new int[dataGridView2.RowCount];
            array[1] = new int[dataGridView2.RowCount];
            for (int i = 0; i< dataGridView2.RowCount; i++)
            {                
                array[0][i] = i;
                array[1][i] = int.Parse(dataGridView2.Rows[i].Cells[col].Value.ToString());
            }

            return array;
        }

        private void dataGridView2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            void Swap(DataGridViewRow e1, DataGridViewRow e2)
            {
                var temp = e1;
                e1 = e2;
                e2 = temp;
            }

            int count = 0;
            for(int i=0; i< dataGridView2.Rows.Count; i++)
            {
                for (var j = 0; j < dataGridView2.Rows.Count - i; j++)
                {
                    if (dataGridView2.Rows[j].Cells[e.ColumnIndex].Value.ToString()[0] > dataGridView2.Rows[j + 1].Cells[e.ColumnIndex].Value.ToString()[0])
                    {
                        Swap(dataGridView2.Rows[j], dataGridView2.Rows[j + 1]);
                        count++;
                    }

                }

            }

            MessageBox.Show("Выбран столбец "+e.ColumnIndex);
        }
    }
}
