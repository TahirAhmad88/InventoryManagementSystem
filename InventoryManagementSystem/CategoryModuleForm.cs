using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class CategoryModuleForm : Form
    {
        public CategoryModuleForm()
        {
            InitializeComponent();
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure, you wanna Save this Category?", "Saving Record",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string conStr = GetConnectionString();
                    using (SqlConnection con = new SqlConnection(conStr))
                    using (SqlCommand cm = new SqlCommand("INSERT INTO tbCategory(catname) VALUES (@catname)", con))
                    {
                        cm.Parameters.AddWithValue("@catname", txtCatName.Text);
                        con.Open();
                        cm.ExecuteNonQuery();
                    }
                    MessageBox.Show("Category has been successfully Saved!");
                    Clear();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void Clear()
        {
            txtCatName.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure, you wanna Update this Category?", "Update Record",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string conStr = GetConnectionString();
                    using (SqlConnection con = new SqlConnection(conStr))
                    using (SqlCommand cm = new SqlCommand(
                        "UPDATE tbCategory SET catname = @catname WHERE catid = @cid", con))
                    {
                        cm.Parameters.AddWithValue("@catname", txtCatName.Text);
                        cm.Parameters.AddWithValue("@cid", lblCatId.Text);
                        con.Open();
                        cm.ExecuteNonQuery();
                    }
                    MessageBox.Show("Category has been successfully Updated!");
                    Clear();
                    Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}