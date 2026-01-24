using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class UserModuleForm : Form
    {
        // Get the connection string from App.config
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        }

        public UserModuleForm()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPass.Text != txtRepass.Text)
                {
                    MessageBox.Show("Password didn't Match!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Are you sure you wanna Save this user?", "Saving Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(GetConnectionString()))
                    using (SqlCommand cm = new SqlCommand(
                        "INSERT INTO tbUser(username, fullname, password, phone) VALUES (@username, @fullname, @password, @phone)", con))
                    {
                        cm.Parameters.AddWithValue("@username", txtUserName.Text);
                        cm.Parameters.AddWithValue("@fullname", txtFullName.Text);
                        cm.Parameters.AddWithValue("@password", txtPass.Text);
                        cm.Parameters.AddWithValue("@phone", txtPhone.Text);

                        con.Open();
                        cm.ExecuteNonQuery();
                    }

                    MessageBox.Show("User has been successfully Saved!");
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPass.Text != txtRepass.Text)
                {
                    MessageBox.Show("Password didn't Match!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to Update this user?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(GetConnectionString()))
                    using (SqlCommand cm = new SqlCommand(
                        "UPDATE tbUser SET fullname = @fullname, password = @password, phone = @phone WHERE username = @username", con))
                    {
                        cm.Parameters.AddWithValue("@fullname", txtFullName.Text);
                        cm.Parameters.AddWithValue("@password", txtPass.Text);
                        cm.Parameters.AddWithValue("@phone", txtPhone.Text);
                        cm.Parameters.AddWithValue("@username", txtUserName.Text);

                        con.Open();
                        cm.ExecuteNonQuery();
                    }

                    MessageBox.Show("User has been successfully Updated!");
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
        }

        public void Clear()
        {
            txtUserName.Clear();
            txtFullName.Clear();
            txtPass.Clear();
            txtRepass.Clear();
            txtPhone.Clear();
        }
    }
}