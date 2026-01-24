using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class CustomerForm : Form
    {
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;

        public CustomerForm()
        {
            InitializeComponent();
            LoadCustomer();
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        }

        public void LoadCustomer()
        {
            int i = 0;
            dgvCustomer.Rows.Clear();

            string conStr = GetConnectionString();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM tbCustomer", con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            i++;
                            dgvCustomer.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString());
                        }
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CustomerModuleForm moduleForm = new CustomerModuleForm();
            moduleForm.btnSave.Enabled = true;
            moduleForm.btnUpdate.Enabled = false;
            moduleForm.ShowDialog();
            LoadCustomer();
        }

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvCustomer.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                CustomerModuleForm userModule = new CustomerModuleForm();
                userModule.lblCId.Text = dgvCustomer.Rows[e.RowIndex].Cells[1].Value.ToString();
                userModule.txtCName.Text = dgvCustomer.Rows[e.RowIndex].Cells[2].Value.ToString();
                userModule.txtCPhone.Text = dgvCustomer.Rows[e.RowIndex].Cells[4].Value.ToString();

                userModule.btnSave.Enabled = false;
                userModule.btnUpdate.Enabled = true;
                userModule.ShowDialog();
                LoadCustomer();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you wanna Delete this Customer?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string conStr = GetConnectionString();
                    using (SqlConnection con = new SqlConnection(conStr))
                    {
                        using (SqlCommand delCmd = new SqlCommand("DELETE FROM tbCustomer WHERE cid = @cid", con))
                        {
                            delCmd.Parameters.AddWithValue("@cid", dgvCustomer.Rows[e.RowIndex].Cells[1].Value); // adjust as needed
                            con.Open();
                            delCmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Customer has been successfully deleted!");
                    LoadCustomer();
                }
            }

            LoadCustomer();
        }
    }
}