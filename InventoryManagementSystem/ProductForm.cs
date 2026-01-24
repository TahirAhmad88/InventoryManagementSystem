using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class ProductForm : Form
    {
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;

        public ProductForm()
        {
            InitializeComponent();
            LoadProduct();
        }

        public void LoadProduct()
        {
            int i = 0;
            dgvProduct.Rows.Clear();

            string conStr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM tbProduct WHERE CONCAT(pid,pname,pqty,pprice,pdescription, pcategory) LIKE @search";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Parameterize search to avoid SQL injection and handle special chars
                    cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            i++;
                            dgvProduct.Rows.Add(i,
                                dr[0].ToString(),
                                dr[1].ToString(),
                                dr[2].ToString(),
                                dr[3].ToString(),
                                dr[4].ToString(),
                                dr[5].ToString());
                        }
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ProductModuleForm formModule = new ProductModuleForm();
            formModule.btnSave.Enabled = true;
            formModule.btnUpdate.Enabled = false;
            formModule.ShowDialog();
            LoadProduct();
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvProduct.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                ProductModuleForm productModule = new ProductModuleForm();
                productModule.lblPid.Text = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
                productModule.txtPName.Text = dgvProduct.Rows[e.RowIndex].Cells[2].Value.ToString();
                productModule.txtPQty.Text = dgvProduct.Rows[e.RowIndex].Cells[3].Value.ToString();
                productModule.txtPPrice.Text = dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString();
                productModule.txtPDes.Text = dgvProduct.Rows[e.RowIndex].Cells[5].Value.ToString();
                productModule.comboCat.Text = dgvProduct.Rows[e.RowIndex].Cells[6].Value.ToString();

                productModule.btnSave.Enabled = false;
                productModule.btnUpdate.Enabled = true;
                productModule.ShowDialog();
                LoadProduct();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you wanna Delete this Product?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string conStr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(conStr))
                    {
                        using (SqlCommand delCmd = new SqlCommand("DELETE FROM tbProduct WHERE pid = @pid", con))
                        {
                            delCmd.Parameters.AddWithValue("@pid", dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString());
                            con.Open();
                            delCmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Product has been successfully deleted!");
                    LoadProduct();
                }
            }

            // LoadProduct at end to refresh list if needed
            LoadProduct();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }
    }
}