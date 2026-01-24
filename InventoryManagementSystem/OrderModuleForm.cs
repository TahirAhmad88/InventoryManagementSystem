using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class OrderModuleForm : Form
    {
        public OrderModuleForm()
        {
            InitializeComponent();
            LoadCustomer();
            LoadProduct();
        }

        private string GetConnectionString()
        {
            // Reads the connection string named "con" from App.config  
            return ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadCustomer()
        {
            int i = 0;
            dgvCustomer.Rows.Clear();

            string query = "SELECT cid, cname FROM tbCustomer WHERE CONCAT(cid, cname) LIKE @search";

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@search", "%" + txtSearchCust.Text + "%");
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        i++;
                        dgvCustomer.Rows.Add(i, reader[0].ToString(), reader[1].ToString());
                    }
                }
            }
        }

        public void LoadProduct()
        {
            int i = 0;
            dgvProduct.Rows.Clear();

            string query = "SELECT * FROM tbProduct WHERE CONCAT(pid,pname,pqty,pprice,pdescription, pcategory) LIKE @search";

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@search", "%" + txtSearchProd.Text + "%");
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        i++;
                        dgvProduct.Rows.Add(i,
                            reader[0].ToString(), // pid  
                            reader[1].ToString(), // pname  
                            reader[2].ToString(), // pqty  
                            reader[3].ToString(), // pprice  
                            reader[4].ToString(), // pdescription  
                            reader[5].ToString()); // pcategory  
                    }
                }
            }
        }

        private void txtSearchCust_TextChanged(object sender, EventArgs e)
        {
            LoadCustomer();
        }

        private void txtSearchProd_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Guard against header clicks  
            if (e.RowIndex < 0)
                return;

            txtPid.Text = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtPName.Text = dgvProduct.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtPrice.Text = dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString();

            // Update total when product is selected
            UpdateTotal();
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            txtCId.Text = dgvCustomer.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtCName.Text = dgvCustomer.Rows[e.RowIndex].Cells[2].Value.ToString();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCId.Text))
                {
                    MessageBox.Show("Please select Customer!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPid.Text))
                {
                    MessageBox.Show("Please select Product!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check available quantity
                long availableQty;
                if (!GetQty(out availableQty) || availableQty < Convert.ToInt64(UDQty.Value))
                {
                    MessageBox.Show("Insufficient stock quantity!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to insert this Order?", "Saving Record",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                // Insert order  
                string insertQuery = @"  
                    INSERT INTO tbOrder(odate, pid, cid, qty, price, total)  
                    VALUES (@odate, @pid, @cid, @qty, @price, @total)";

                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                using (SqlCommand cm = new SqlCommand(insertQuery, con))
                {
                    cm.Parameters.AddWithValue("@odate", dtOrder.Value);
                    cm.Parameters.AddWithValue("@pid", Convert.ToInt32(txtPid.Text));
                    cm.Parameters.AddWithValue("@cid", Convert.ToInt32(txtCId.Text));
                    cm.Parameters.AddWithValue("@qty", Convert.ToInt64(UDQty.Value));
                    cm.Parameters.AddWithValue("@price", Convert.ToInt64(txtPrice.Text));
                    cm.Parameters.AddWithValue("@total", Convert.ToInt64(txtTotal.Text));

                    con.Open();
                    cm.ExecuteNonQuery();
                }

                // Decrease product quantity  
                string updateQtyQuery = "UPDATE tbProduct SET pqty = (pqty - @pqty) WHERE pid = @pid";
                using (SqlConnection con2 = new SqlConnection(GetConnectionString()))
                using (SqlCommand upd = new SqlCommand(updateQtyQuery, con2))
                {
                    upd.Parameters.AddWithValue("@pqty", Convert.ToInt64(UDQty.Value));
                    upd.Parameters.AddWithValue("@pid", Convert.ToInt32(txtPid.Text));
                    con2.Open();
                    upd.ExecuteNonQuery();
                }

                MessageBox.Show("Order has been successfully Inserted.");
                Clear();
                LoadProduct();
                LoadCustomer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Clear()
        {
            txtCId.Clear();
            txtCName.Clear();

            txtPid.Clear();
            txtPName.Clear();

            txtPrice.Clear();
            UDQty.Value = 1;
            txtTotal.Clear();
            dtOrder.Value = DateTime.Now;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public bool GetQty(out long availableQty)
        {
            availableQty = 0;

            // Require a valid pid
            if (string.IsNullOrWhiteSpace(txtPid.Text))
                return false;

            string query = "SELECT pqty FROM tbProduct WHERE pid = @pid";

            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@pid", Convert.ToInt32(txtPid.Text));
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        availableQty = dr.IsDBNull(0) ? 0 : Convert.ToInt64(dr[0]);
                        return true;
                    }
                }
            }

            return false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            // Ensure a product is selected  
            if (string.IsNullOrWhiteSpace(txtPid.Text))
            {
                UDQty.Value = 1;
                txtTotal.Text = "0";
                return;
            }

            // Get available quantity
            long available;
            if (GetQty(out available))
            {
                long requested = Convert.ToInt64(UDQty.Value);

                // Check if requested quantity exceeds available
                if (requested > available)
                {
                    MessageBox.Show("Instock quantity is not enough.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // Set to the maximum available quantity, but at least 1
                    UDQty.Value = Math.Max(1, Math.Min(available, UDQty.Maximum));
                    requested = Convert.ToInt64(UDQty.Value);
                }

                // Calculate total
                if (requested > 0 && !string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    long price;
                    if (long.TryParse(txtPrice.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out price))
                    {
                        long total = price * requested;
                        txtTotal.Text = total.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        txtTotal.Text = "0";
                    }
                }
                else
                {
                    txtTotal.Text = "0";
                }
            }
            else
            {
                // Product not found or invalid PID
                txtTotal.Text = "0";
            }
        }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            UpdateTotal();
        }
    }
}