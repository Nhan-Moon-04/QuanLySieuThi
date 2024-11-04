using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
  
namespace QuanLySieuThi
{
    public partial class Dangnhap : Form
    {
        public static string sqlcon = @"Data Source=NGUYEN-NHAN;Initial Catalog=QLST2;Integrated Security=True";


        public static SqlConnection mycon;
        public static SqlCommand com;
        public static SqlDataAdapter ad;
        public static DataTable dt;
        public static SqlCommandBuilder bd;

        public static string getNameUser(string fullname )
        {
            return fullname;
        }

        public static string username;
        public Dangnhap()
        {
            InitializeComponent();
        }
        public static void Chuoiketnoi(string chuoi, DataGridView db1)
        {
            try
            {

                ad = new SqlDataAdapter(chuoi, sqlcon);
                dt = new DataTable();
                bd = new SqlCommandBuilder(ad);
                ad.Fill(dt);
                db1.DataSource = dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối " + ex, "Thông báo ! ");

            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox1.Checked)
            {
                txt_mk.UseSystemPasswordChar = true;

            }
            else
                txt_mk.UseSystemPasswordChar = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_tk.Text) || string.IsNullOrWhiteSpace(txt_mk.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK);
                return;
            }

            string sql = "SELECT IsAdmin FROM UserAccount WHERE Username = @Username AND Password = @Password";

            using (SqlConnection mycon = new SqlConnection(sqlcon))
            {
                mycon.Open();
                using (SqlCommand com = new SqlCommand(sql, mycon))
                {
                    com.Parameters.AddWithValue("@Username", txt_tk.Text.Trim());
                    com.Parameters.AddWithValue("@Password", txt_mk.Text.Trim());

                    var result = com.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Username hoặc password sai! Bạn vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }

                    // Safely cast to int
                    int getRole;
                    if (result is int)
                    {
                        getRole = (int)result;
                    }
                    else if (result is bool)
                    {
                        // If IsAdmin is a bit type in SQL Server, convert to int
                        getRole = (bool)result ? 1 : 0;
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra. Vui lòng thử lại.", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }

                    if (getRole == 1)
                    {
                        MessageBox.Show("Bạn đang nhập vào tài khoản Admin", "Thông báo", MessageBoxButtons.OK);
                        main2 a1 = new main2();
                        a1.lb_quyen.Text = GetFullname(0, txt_tk.Text.Trim()) + " (Quản trị)";
                        a1.Show();
                    }
                    else
                    {
                        MessageBox.Show("Bạn đang nhập vào tài khoản Nhân Viên", "Thông báo", MessageBoxButtons.OK);
                        banhang.banhang bh = new banhang.banhang();
                        bh.lb_quyen.Text = GetFullname(1, txt_tk.Text.Trim()) + " (Nhân viên)";
                        bh.Show();
                    }

                    this.Hide();
                }
            }
        }


        private string GetFullname(int role, string username)
        {
            string fullname = string.Empty;
            string sql = "SELECT FullName FROM UserAccount WHERE Username = @Username";

            using (SqlConnection mycon = new SqlConnection(sqlcon))
            {
                mycon.Open();
                using (SqlCommand com = new SqlCommand(sql, mycon))
                {
                    com.Parameters.AddWithValue("@Username", username);

                    var result = com.ExecuteScalar();
                    if (result != null)
                    {
                        fullname = result.ToString();
                    }
                }
            }

            return fullname;
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát không ?", "Thông báo ", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                main1 a = new main1();
                a.Show();
                this.Hide();
            }
        }

        private void Dangnhap_Load(object sender, EventArgs e)
        {

        }
        private void Dangnhap_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Create an instance of the main1 form
            main1 mainForm = new main1();

            // Show the main1 form
            mainForm.Show();

            // Optionally hide the login form instead of closing it
            this.Hide();

            // Prevent the application from exiting if the login form is closed
            e.Cancel = true;
        }

    }
}
