using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;


namespace QuanLySieuThi
{
   

    public partial class frmTaiKhoan : Form
    {
        public string chuoi = "select Username, Password, FullName,Email ,DateCreated, IsAdmin from UserAccount";
        public frmTaiKhoan()
        {
            InitializeComponent();
            chuoiketnoi.Chuoiketnoi_Data(chuoi, dta1);
            dta1.Columns[0].HeaderText = "Tài khoản"; dta1.Columns[0].Width = 90;
            dta1.Columns[1].HeaderText = "Mật khẩu"; dta1.Columns[1].Width = 90;
            dta1.Columns[2].HeaderText = "Họ và tên"; dta1.Columns[2].Width = 110;
            dta1.Columns[3].HeaderText = "Email"; dta1.Columns[3].Width = 110;
            dta1.Columns[4].HeaderText = "Ngày tạo"; dta1.Columns[3].Width = 110;
            dta1.Columns[5].HeaderText = "Quyền"; dta1.Columns[4].Width = 70;

            cbbRole.SelectedIndex = 0;
            
        }

        //private void btn_them_Click(object sender, EventArgs e)
        //{
        //    string uername = txt_tk.Text.Trim();
        //    string password = txt_mk.Text.Trim();
        //    string fullname = txtFullname.Text.Trim();
        //    string date = datecreate.Text.Trim();


        //    string role = cbbRole.SelectedIndex.ToString();
        //    if (txt_tk.Text == "" || txt_mk.Text == "" || fullname=="")
        //    {
        //        MessageBox.Show("Bạn chưa nhập đầy đủ thông tin! Vui lòng kiểm tra lại ", "Error", MessageBoxButtons.OK);
        //    }
        //    else
        //    {
        //        try
        //        {               
        //            string select = "select count(*) from taikhoan where username='" + txt_tk.Text + "'";
        //            string them1 = "insert into taikhoan Values ('" + uername + "','" + password + "','" + fullname + "','" + date + "','" + role + "')";
        //            chuoiketnoi.Them(select, txt_tk.Text.Trim(), txt_mk.Text.Trim(), them1, dta1);
        //            chuoiketnoi.Chuoiketnoi(chuoi, dta1);                  
        //            dta1.Columns[0].HeaderText = "Tài khoản"; 
        //            dta1.Columns[1].HeaderText = "Mật khẩu"; 
        //            dta1.Columns[2].HeaderText = "Họ và tên"; 
        //            dta1.Columns[3].HeaderText = "Ngày tạo";
        //            dta1.Columns[4].HeaderText = "Quyền";
        //            txt_tk.Clear();
        //            txt_mk.Clear();
        //            txtFullname.Clear();
        //            cbbRole.SelectedIndex = 0;


        //            txt_tk.Focus();
        //            btn_them.Enabled = true;
        //            btn_xoa.Enabled = false;
        //            btn_sua.Enabled = false;
        //        }
        //        catch
        //        {
        //            MessageBox.Show("Tài khoản đã tồn tại! Vui lòng nhập lại", "Error", MessageBoxButtons.OKCancel);
        //        }
        //    }
        //}


    private void btn_them_Click(object sender, EventArgs e)
    {
            string username = txt_tk.Text.Trim();
            string password = txt_mk.Text.Trim();
            string fullname = txtFullname.Text.Trim();
            string email = txtEmail.Text.Trim();
            string dateCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            int isAdmin = cbbRole.SelectedIndex == 0 ? 1 : 0;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Bạn chưa nhập đầy đủ thông tin! Vui lòng kiểm tra lại", "Error", MessageBoxButtons.OK);
                return;
            }

            // Validate email format
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ! Vui lòng nhập lại", "Error", MessageBoxButtons.OK);
                return;
            }

            string connectionString = "Data Source=NGUYEN-NHAN;Initial Catalog=QLST2;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    // Check if the username or email already exists
                    string selectQuery = "SELECT COUNT(*) FROM UserAccount WHERE Username = @username OR Email = @Email";
                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@username", username);
                        selectCommand.Parameters.AddWithValue("@Email", email);

                        int userExists = (int)selectCommand.ExecuteScalar();

                        if (userExists > 0)
                        {
                            MessageBox.Show("Tài khoản hoặc email đã tồn tại! Vui lòng nhập lại", "Error", MessageBoxButtons.OKCancel);
                            return;
                        }
                    }



                    // Insert new user
                    string insertQuery = @"
                INSERT INTO UserAccount (Username, Password, FullName, Email, IsAdmin, DateCreated) 
                VALUES (@username, @password, @fullname, @Email, @IsAdmin, @dateCreated)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@username", username);
                        insertCommand.Parameters.AddWithValue("@password", password);
                        insertCommand.Parameters.AddWithValue("@fullname", fullname);
                        insertCommand.Parameters.AddWithValue("@Email", email);
                        insertCommand.Parameters.AddWithValue("@IsAdmin", isAdmin);
                        insertCommand.Parameters.AddWithValue("@dateCreated", dateCreated);

                        insertCommand.ExecuteNonQuery();
                    }

                    // Refresh the DataGridView
                    chuoiketnoi.Chuoiketnoi_Data(chuoi, dta1);

                    // Clear input fields
                    txt_tk.Clear();
                    txt_mk.Clear();
                    txtFullname.Clear();
                    txtEmail.Clear();
                    cbbRole.SelectedIndex = 0;
                    txt_tk.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
    }



    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }



    private void btn_nhaplai_Click(object sender, EventArgs e)
        {
            txt_tk.Clear();
            txt_mk.Clear();
            txtFullname.Clear();
            cbbRole.SelectedIndex = 0;
            txt_tk.Focus();
            txt_tk.Enabled = true;
            btn_them.Enabled = true;
            btn_xoa.Enabled = false;
            btn_sua.Enabled = false;
        }

        private void btn_sua_Click(object sender, EventArgs e)
        {
            string username = txt_tk.Text.Trim();
            string password = txt_mk.Text.Trim();
            string fullname = txtFullname.Text.Trim();
            string email = txtEmail.Text.Trim();
            string dateCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            int isAdmin = cbbRole.SelectedIndex == 0 ? 1 : 0;

            // Validate input fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Bạn chưa nhập đầy đủ thông tin! Vui lòng kiểm tra lại", "Error", MessageBoxButtons.OK);
                return;
            }

            // Validate email format
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ! Vui lòng nhập lại", "Error", MessageBoxButtons.OK);
                return;
            }

            string connectionString = "Data Source=NGUYEN-NHAN;Initial Catalog=QLST2;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    // Check if the email already exists for a different user
                    string selectQuery = "SELECT COUNT(*) FROM UserAccount WHERE Email = @Email AND Username <> @Username";
                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@Email", email);
                        selectCommand.Parameters.AddWithValue("@Username", username);

                        int emailExists = (int)selectCommand.ExecuteScalar();

                        if (emailExists > 0)
                        {
                            MessageBox.Show("Email đã tồn tại! Vui lòng nhập lại", "Error", MessageBoxButtons.OKCancel);
                            return;
                        }
                    }


                    // Update user information
                    string updateQuery = @"
            UPDATE UserAccount 
            SET Password = @password, FullName = @fullname, Email = @Email, IsAdmin = @IsAdmin, DateCreated = @dateCreated
            WHERE Username = @username";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@username", username);
                        updateCommand.Parameters.AddWithValue("@password", password); // Store hashed password
                        updateCommand.Parameters.AddWithValue("@fullname", fullname);
                        updateCommand.Parameters.AddWithValue("@Email", email);
                        updateCommand.Parameters.AddWithValue("@IsAdmin", isAdmin);
                        updateCommand.Parameters.AddWithValue("@dateCreated", dateCreated);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật thành công!", "Success", MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy người dùng để cập nhật!", "Error", MessageBoxButtons.OK);
                        }
                    }

                    // Refresh the DataGridView
                    chuoiketnoi.Chuoiketnoi_Data(chuoi, dta1);

                    // Clear input fields
                    txt_tk.Clear();
                    txt_mk.Clear();
                    txtFullname.Clear();
                    txtEmail.Clear();
                    cbbRole.SelectedIndex = 0;
                    txt_tk.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void btn_xoa_Click(object sender, EventArgs e)
        {
            string sql = "Delete from UserAccount  where Username = '" + txt_tk.Text.Trim() + "'";
            chuoiketnoi.Execute(sql);
            // MessageBox.Show("Bạn xóa thành công ! ", "Thông báo", MessageBoxButtons.OK);
            chuoiketnoi.Chuoiketnoi_Data(chuoi, dta1);
            dta1.Columns[0].HeaderText = "Tài khoản";
            dta1.Columns[1].HeaderText = "Mật khẩu";
            dta1.Columns[2].HeaderText = "Họ và tên";
            dta1.Columns[3].HeaderText = "Ngày tạo";
            dta1.Columns[4].HeaderText = "Quyền";
            txt_tk.Clear();
            txt_mk.Clear();
            txtFullname.Clear();
            txt_tk.Focus();
            txt_tk.Enabled = true;
            btn_them.Enabled = true;
            btn_xoa.Enabled = false;
            btn_sua.Enabled = false;
        }

        private void btn_ex_Click(object sender, EventArgs e)
        {
            string duongdan = "";
            string tenfile = "TaiKhoanAdmin";
            XuatExecl.exportecxel(dta1, duongdan, tenfile);
            MessageBox.Show("Xuất file thành công ", "Thông báo ", MessageBoxButtons.OK);
            MessageBox.Show("Duong dan file dc luu :" + duongdan + MessageBoxButtons.OK);
        }

        private void dta1_Click(object sender, EventArgs e)
        {
            // Check if there are any rows in the DataGridView
            if (dta1.Rows.Count > 0)
            {
                // Get the current row
                DataGridViewRow currentRow = dta1.CurrentRow;

                // Ensure the current row is not null
                if (currentRow != null)
                {
                    // Get the index of the current row
                    int curow = currentRow.Index;

                    // Ensure the index is valid before accessing cells
                    if (curow >= 0 && curow < dta1.Rows.Count)
                    {
                        // Ensure the cells exist before accessing them
                        if (dta1.Columns.Count > 4) // Assuming you have at least 5 columns
                        {
                            txt_tk.Text = currentRow.Cells[0].Value?.ToString(); // Username
                            txt_mk.Text = currentRow.Cells[1].Value?.ToString(); // Password
                            txtFullname.Text = currentRow.Cells[2].Value?.ToString(); // Full Name
                            txtEmail.Text = currentRow.Cells[3].Value?.ToString(); // Email
                            cbbRole.SelectedIndex = (currentRow.Cells[4].Value?.ToString() == "Quản trị viên") ? 0 : 1; // IsAdmin

                            txt_tk.Enabled = false;
                            btn_them.Enabled = false;
                            btn_sua.Enabled = true;
                            btn_xoa.Enabled = true;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Không có dữ liệu để chọn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void btn_thoat_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Bạn có chắc chắn muốn thoát không ?", "Thông báo ", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void tkadmin_Load(object sender, EventArgs e)
        {

        }

        private void dta1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dta1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void datecreate_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
