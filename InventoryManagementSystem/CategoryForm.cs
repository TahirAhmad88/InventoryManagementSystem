using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class CategoryForm : Form
    {
        public CategoryForm()
        {
            InitializeComponent();
            LoadCategory();
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        }

        public void LoadCategory()
        {
            int i = 0;
            dgvCategory.Rows.Clear();

            string conStr = GetConnectionString();
            using (SqlConnection con = new SqlConnection(conStr))
            using (SqlCommand cm = new SqlCommand("SELECT * FROM tbCategory", con))
            {
                con.Open();
                using (SqlDataReader dr = cm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        i++;
                        dgvCategory.Rows.Add(i, dr[0].ToString(), dr[1].ToString());
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CategoryModuleForm formModule = new CategoryModuleForm();
            formModule.btnSave.Enabled = true;
            formModule.btnUpdate.Enabled = false;
            formModule.ShowDialog();
            LoadCategory();
        }

        private void dgvCategory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvCategory.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                CategoryModuleForm userModule = new CategoryModuleForm();
                userModule.lblCatId.Text = dgvCategory.Rows[e.RowIndex].Cells[1].Value.ToString();
                userModule.txtCatName.Text = dgvCategory.Rows[e.RowIndex].Cells[2].Value.ToString();

                userModule.btnSave.Enabled = false;
                userModule.btnUpdate.Enabled = true;
                userModule.ShowDialog();
                LoadCategory();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you wanna Delete this Category?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string conStr = GetConnectionString();
                    using (SqlConnection con = new SqlConnection(conStr))
                    using (SqlCommand delCmd = new SqlCommand("DELETE FROM tbCategory WHERE catid = @cid", con))
                    {
                        delCmd.Parameters.AddWithValue("@cid", dgvCategory.Rows[e.RowIndex].Cells[1].Value.ToString());
                        con.Open();
                        delCmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Category has been successfully deleted!");
                    LoadCategory();
                }
            }

            LoadCategory();
        }
    }
}