using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class OrderForm : Form
    {
        public OrderForm()
        {
            InitializeComponent();
            LoadOrder();
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        }

        public void LoadOrder()
        {
            decimal total = 0;
            int i = 0;
            dgvOrder.Rows.Clear();

            string query =
                @"SELECT orderid, odate, O.pid, P.pname, O.cid, C.cname, qty, price, total  
                  FROM tbOrder AS O  
                  JOIN tbCustomer AS C ON O.cid = C.cid  
                  JOIN tbProduct AS P ON O.pid = P.pid  
                  WHERE CONCAT(orderid, odate, O.pid, P.pname, O.cid, C.cname, qty, price) LIKE @search";

            string conStr = GetConnectionString();
            using (SqlConnection con = new SqlConnection(conStr))
            using (SqlCommand cm = new SqlCommand(query, con))
            {
                cm.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                con.Open();
                using (SqlDataReader dr = cm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        i++;
                        dgvOrder.Rows.Add(
                            i,
                            dr[0].ToString(),
                            Convert.ToDateTime(dr[1].ToString()).ToString("dd/MM/yyyy"),
                            dr[2].ToString(),
                            dr[3].ToString(),
                            dr[4].ToString(),
                            dr[5].ToString(),
                            dr[6].ToString(),
                            dr[7].ToString(),
                            dr[8].ToString()
                        );
                        total += Convert.ToDecimal(dr[8]);
                    }
                }
            }

            lblQty.Text = i.ToString();
            lblTotal.Text = total.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OrderModuleForm moduleForm = new OrderModuleForm();
            moduleForm.ShowDialog();
            LoadOrder();
        }

        private void dgvOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvOrder.Columns[e.ColumnIndex].Name;

            if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you wanna Delete this Order?", "Delete Record",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string delQuery = "DELETE FROM tbOrder WHERE orderid = @orderid";
                    string conStr = GetConnectionString();
                    using (SqlConnection con = new SqlConnection(conStr))
                    using (SqlCommand delCmd = new SqlCommand(delQuery, con))
                    {
                        delCmd.Parameters.AddWithValue("@orderid", dgvOrder.Rows[e.RowIndex].Cells[1].Value.ToString());
                        con.Open();
                        delCmd.ExecuteNonQuery();
                    }

                    // Increase product quantity back  
                    string updateQtyQuery =
                        "UPDATE tbProduct SET pqty = pqty + @pqty WHERE pid = @pid";
                    string pidValue = dgvOrder.Rows[e.RowIndex].Cells[3].Value.ToString(); // pid column index may vary  
                    string qtyValueStr = dgvOrder.Rows[e.RowIndex].Cells[7].Value.ToString(); // qty/quantity in order  
                    using (SqlConnection con2 = new SqlConnection(conStr))
                    using (SqlCommand updCmd = new SqlCommand(updateQtyQuery, con2))
                    {
                        updCmd.Parameters.AddWithValue("@pqty", Convert.ToInt64(qtyValueStr));
                        updCmd.Parameters.AddWithValue("@pid", pidValue);
                        con2.Open();
                        updCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Order has been successfully deleted!");
                }
            }

            LoadOrder();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadOrder();
        }
    }
}
       