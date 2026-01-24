using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class ProductModuleForm : Form
    {
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;

        public ProductModuleForm()
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
            comboCat.Items.Clear();
            string conStr = GetConnectionString();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT catname FROM tbCategory", con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboCat.Items.Add(reader[0].ToString());
                        }
                    }
                }
            }
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you wanna Save this Product?", "Saving Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string conStr = GetConnectionString();
                    using (SqlConnection con = new SqlConnection(conStr))
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO tbProduct(pname,pqty,pprice,pdescription,pcategory) VALUES (@pname,@pqty,@pprice,@pdescription,@pcategory)", con))
                    {
                        cmd.Parameters.AddWithValue("@pname", txtPName.Text);
                        cmd.Parameters.AddWithValue("@pqty", Convert.ToInt64(txtPQty.Text));
                        cmd.Parameters.AddWithValue("@pprice", Convert.ToInt64(txtPPrice.Text));
                        cmd.Parameters.AddWithValue("@pdescription", txtPDes.Text);
                        cmd.Parameters.AddWithValue("@pcategory", comboCat.Text);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Product has been successfully Saved!");
                    Clear();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Clear()
        {
            txtPName.Clear();
            txtPQty.Clear();
            txtPPrice.Clear();
            txtPDes.Clear();
            comboCat.Text = "";
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
                if (MessageBox.Show("Are you sure you wanna Update this Product?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string conStr = GetConnectionString();
                    using (SqlConnection con = new SqlConnection(conStr))
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE tbProduct SET pname=@pname, pqty=@pqty, pprice=@pprice, pdescription=@pdescription, pcategory=@pcategory WHERE pid = @pid", con))
                    {
                        cmd.Parameters.AddWithValue("@pname", txtPName.Text);
                        cmd.Parameters.AddWithValue("@pqty", Convert.ToInt64(txtPQty.Text));
                        cmd.Parameters.AddWithValue("@pprice", Convert.ToInt64(txtPPrice.Text));
                        cmd.Parameters.AddWithValue("@pdescription", txtPDes.Text);
                        cmd.Parameters.AddWithValue("@pcategory", comboCat.Text);
                        cmd.Parameters.AddWithValue("@pid", lblPid.Text); // or whatever you use for pid  

                        con.Open();
                        cm.ExecuteNonQuery();
                    }
                    MessageBox.Show("Product has been successfully Updated!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}