using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectSGBD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeDataGridViewChild();
        }

        private void InitializeDataGridViewChild()
        {
            dataGridViewChild.SelectionChanged += DataGridViewChild_SelectionChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Selectati persoana!");
                return;
            }
            string connectionString = "Data Source=DESKTOP-J9DDI9N;Initial Catalog=Halloween;Integrated Security=True;Connect " +
                "Timeout=30;Encrypt=False";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand deleteCommand = new SqlCommand("DELETE FROM Person WHERE CNP=@cnp", conn);
                    deleteCommand.Parameters.AddWithValue("@cnp", textBox1.Text);
                    int deletedRowCount = deleteCommand.ExecuteNonQuery();
                    MessageBox.Show("Numar de inregistrari sterse: " + deletedRowCount);
                    conn.Close();
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Invalid params!");
                return;
            }
            string connectionString = "Data Source=DESKTOP-J9DDI9N;Initial Catalog=Halloween;Integrated Security=True;Connect " +
                "Timeout=30;Encrypt=False";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand updateCmd = new SqlCommand("UPDATE Person SET PersonName=@name, Age=@age WHERE CNP=@cnp", conn);
                    updateCmd.Parameters.AddWithValue("@name", textBox2.Text);
                    updateCmd.Parameters.AddWithValue("@age", textBox3.Text);
                    updateCmd.Parameters.AddWithValue("@cnp", textBox1.Text);
                    int updateRowCount = updateCmd.ExecuteNonQuery();
                    MessageBox.Show("Numar inregistrari updatate: " + updateRowCount);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Mesajul erorii: \n{0}", ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connectingString = "Data Source=DESKTOP-J9DDI9N;Initial Catalog=Halloween;Integrated Security=True;Connect " +
                "Timeout=30;Encrypt=False";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectingString))
                {
                    conn.Open();
                    MessageBox.Show("Starea conexiunii: " + conn.State.ToString());
                    DataSet dataSet = new DataSet();
                    SqlDataAdapter parentAdapter = new SqlDataAdapter("SELECT * FROM Halloween_Candy", conn);
                    SqlDataAdapter childAdapter = new SqlDataAdapter("SELECT * FROM Person", conn);
                    parentAdapter.Fill(dataSet, "Halloween_Candy");
                    childAdapter.Fill(dataSet, "Person");
                    BindingSource parentBS = new BindingSource();
                    BindingSource childBS = new BindingSource();
                    parentBS.DataSource = dataSet.Tables["Halloween_Candy"];
                    dataGridViewParent.DataSource = parentBS;
                    DataColumn parentPK = dataSet.Tables["Halloween_Candy"].Columns["IdCandy"];
                    DataColumn childFK = dataSet.Tables["Person"].Columns["FavoriteCandy"];
                    DataRelation relation = new DataRelation("fk_parent_child", parentPK, childFK);
                    dataSet.Relations.Add(relation);
                    childBS.DataSource = parentBS;
                    childBS.DataMember = "fk_parent_child";
                    dataGridViewChild.DataSource = childBS;
                    conn.Close();
                }
            } catch (Exception ex)
            {
                MessageBox.Show (ex.Message.ToString());
            }
        }

        private void DataGridViewChild_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewChild.SelectedRows.Count > 0)
            {
                string connectingString = "Data Source=DESKTOP-J9DDI9N;Initial Catalog=Halloween;Integrated Security=True;Connect " +
                "Timeout=30;Encrypt=False";
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectingString))
                    {
                        int selectedIndex = dataGridViewChild.SelectedRows[0].Index;
                        conn.Open();
                        DataSet dataSet = new DataSet();
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Person", conn);
                        adapter.Fill(dataSet, "Person");
                        DataRow selectedRow = dataSet.Tables["Person"].Rows[selectedIndex];
                        textBox1.Text = selectedRow["CNP"].ToString();
                        textBox2.Text = selectedRow["PersonName"].ToString();
                        textBox3.Text = selectedRow["Age"].ToString();
                        conn.Close();
                    }
                } catch (System.Exception ex)
                {
                    MessageBox.Show (ex.Message.ToString());
                }
            } else
            {
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
        }

        private void dataGridViewChild_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Invalid params!");
                return;
            }
            if (dataGridViewParent.SelectedRows.Count > 0)
            {
                string connectionString = "Data Source=DESKTOP-J9DDI9N;Initial Catalog=Halloween;Integrated Security=True;Connect " +
                "Timeout=30;Encrypt=False";
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        int selectedIndex = dataGridViewParent.SelectedRows[0].Index;
                        con.Open();
                        DataSet data = new DataSet();
                        SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Halloween_Candy", con);
                        adapter.Fill(data, "Halloween_Candy");
                        DataRow dataRow = data.Tables["Halloween_Candy"].Rows[selectedIndex];
                        string candy = dataRow["IdCandy"].ToString();
                        SqlCommand addCmd = new SqlCommand("INSERT INTO Person(CNP, PersonName, Age, FavoriteCandy) VALUES (@cnp, @name, @age, @candy)", con);
                        addCmd.Parameters.AddWithValue("@cnp", textBox1.Text);
                        addCmd.Parameters.AddWithValue("@name", textBox2.Text);
                        addCmd.Parameters.AddWithValue("@age", textBox3.Text);
                        addCmd.Parameters.AddWithValue("@candy", candy);
                        int insertRowCount = addCmd.ExecuteNonQuery();
                        MessageBox.Show("Inregistrari inserate: " + insertRowCount);
                        con.Close();
                    }
                } catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Mesajul erorii: \n{0}", ex.Message);
                }
            } else
            {
                MessageBox.Show("Select candy!");
            }
        }
    }
}
