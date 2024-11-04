    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    namespace QuanLySieuThi.quanly
    { 
        public partial class sanpham : Form
        {
        public string chuoi = @"
        SELECT sanpham.MaSP, sanpham.TenSP, nhacungcap.TenNCC, sanpham.GiaNhap, 
               sanpham.GiaBan, sanpham.Solg, sanpham.HSD, sanpham.NoiSX, 
               sanpham.DonViTinh, sanpham.UserID, UserAccount.FullName
        FROM sanpham
        INNER JOIN nhacungcap ON sanpham.MaNCC = nhacungcap.MaNCC
        INNER JOIN UserAccount On sanpham.UserID = UserAccount.UserID";


        public sanpham()
                {
                    InitializeComponent();
                    chuoiketnoi.Chuoiketnoi_Data(chuoi, dta1);
                    clear();

                }

        public void clear()
        {
            if (dta1.Rows.Count > 0)
            {
                dta1.Columns[0].HeaderText = "Mã Sản phẩm"; dta1.Columns[0].Width = 110;
                dta1.Columns[1].HeaderText = "Tên sản phẩm"; dta1.Columns[1].Width = 150;
                dta1.Columns[2].HeaderText = "Nhà cung cấp"; dta1.Columns[2].Width = 110;
                dta1.Columns[3].HeaderText = "Giá nhập"; dta1.Columns[3].Width = 110;
                dta1.Columns[4].HeaderText = "Giá bán"; dta1.Columns[4].Width = 110;
                dta1.Columns[5].HeaderText = "Số lượng"; dta1.Columns[5].Width = 110;
                dta1.Columns[6].HeaderText = "Hạn sử dụng"; dta1.Columns[6].Width = 110;
                dta1.Columns[7].HeaderText = "Nơi sản xuất"; dta1.Columns[7].Width = 110;
                dta1.Columns[8].HeaderText = "Đơn vị tính"; dta1.Columns[8].Width = 110;
                dta1.Columns[9].HeaderText = "ID_User"; dta1.Columns[9].Width = 110;
                dta1.Columns[10].HeaderText = "FullName"; dta1.Columns[10].Width = 150;

                mancc();
                txt_masp.Focus();
                txt_masp.Text = "";
                txt_tensp.Text = "";
                txt_mancc.Text = "";
                txt_gianhap.Text = "";
                txt_giaban.Text = "";
                txt_solg.Text = "";
                txt_hsd.Text = "";
                txt_nsx.Text = "";
                txt_dvt.Text = "";

                // Hiển thị số lượng sản phẩm
                lbl_kq.Text = (dta1.Rows.Count - 1).ToString() + " SP";
            }
        }

        private void txt_search_TextChanged(object sender, EventArgs e)
                {
                    string load1 = "Select * from SanPham where TenSP like N'%" + txt_search.Text + "%' ";
                    chuoiketnoi.timkiem(load1, dta1);
                    clear();
                }




        private void mancc()
        {
            string load_mancc = "SELECT MaNCC, TenNCC FROM NhaCungCap";
            chuoiketnoi.xulycbx(load_mancc, txt_mancc);
            txt_mancc.DisplayMember = "TenNCC";
            txt_mancc.ValueMember = "MaNCC";
        }
        private void sanpham_Load(object sender, EventArgs e)
                {
                    txtNguoiNhap.Text = lb_quyen.Text;

                    txtNguoiNhap.ReadOnly = true;
                }



        private void dta1_Click(object sender, EventArgs e)
        {
            if (dta1.CurrentRow != null && dta1.CurrentRow.Index >= 0)
            {
                int curow = dta1.CurrentRow.Index;

                // Gán các giá trị từ DataGridView vào các ô nhập liệu
                txt_masp.Text = dta1.Rows[curow].Cells["MaSP"].Value?.ToString() ?? "";
                txt_tensp.Text = dta1.Rows[curow].Cells["TenSP"].Value?.ToString() ?? "";
                txt_mancc.Text = dta1.Rows[curow].Cells["TenNCC"].Value?.ToString() ?? "";
                txt_gianhap.Text = dta1.Rows[curow].Cells["GiaNhap"].Value?.ToString() ?? "";
                txt_giaban.Text = dta1.Rows[curow].Cells["GiaBan"].Value?.ToString() ?? "";
                txt_solg.Text = dta1.Rows[curow].Cells["Solg"].Value?.ToString() ?? "";
                txt_hsd.Text = dta1.Rows[curow].Cells["HSD"].Value?.ToString() ?? "";
                txt_nsx.Text = dta1.Rows[curow].Cells["NoiSX"].Value?.ToString() ?? "";
                txt_dvt.Text = dta1.Rows[curow].Cells["DonViTinh"].Value?.ToString() ?? "";
                txtNguoiNhap.Text = dta1.Rows[curow].Cells["FullName"].Value?.ToString() ?? "";

                // Cập nhật trạng thái nút và cho phép sửa/xóa
                txt_masp.Enabled = false;
                btn_them.Enabled = false;
                bnt_sua.Enabled = true;
                btn_xoa.Enabled = true;
            }
        }


        private void btn_them_Click(object sender, EventArgs e)
            {
                if (string.IsNullOrWhiteSpace(txt_tensp.Text) ||
                    string.IsNullOrWhiteSpace(txt_mancc.Text) ||
                    string.IsNullOrWhiteSpace(txt_gianhap.Text) ||
                    string.IsNullOrWhiteSpace(txt_giaban.Text) ||
                    string.IsNullOrWhiteSpace(txt_solg.Text) ||
                    string.IsNullOrWhiteSpace(txt_hsd.Text) ||
                    string.IsNullOrWhiteSpace(txt_nsx.Text) ||
                    string.IsNullOrWhiteSpace(txt_dvt.Text))
                {
                    MessageBox.Show("Bạn chưa nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string sql1 = "INSERT INTO sanpham (TenSP, MaNCC, GiaNhap, GiaBan, Solg, HSD, NoiSX, DonViTinh, NguoiNhap) " +
                              "VALUES (@TenSP, @MaNCC, @GiaNhap, @GiaBan, @Solg, @HSD, @NoiSX, @DonViTinh, @NguoiNhap)";

                using (SqlConnection connection = new SqlConnection(chuoiketnoi.sqlcon))
                {
                    using (SqlCommand cmd = new SqlCommand(sql1, connection))
                    {
                        cmd.Parameters.AddWithValue("@TenSP", txt_tensp.Text);
                        cmd.Parameters.AddWithValue("@MaNCC", txt_mancc.SelectedValue);
                        cmd.Parameters.AddWithValue("@GiaNhap", txt_gianhap.Text);
                        cmd.Parameters.AddWithValue("@GiaBan", txt_giaban.Text);
                        cmd.Parameters.AddWithValue("@Solg", txt_solg.Text);
                        cmd.Parameters.AddWithValue("@HSD", txt_hsd.Value);
                        cmd.Parameters.AddWithValue("@NoiSX", txt_nsx.Text);
                        cmd.Parameters.AddWithValue("@DonViTinh", txt_dvt.Text);
                        cmd.Parameters.AddWithValue("@NguoiNhap", txtNguoiNhap.Text);

                        try
                        {
                            connection.Open();
                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                chuoiketnoi.Chuoiketnoi_Data(chuoi, dta1); // Tải lại dữ liệu vào DataGridView
                                clear();
                            }
                            else
                            {
                                MessageBox.Show("Không thể thêm sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void label11_Click(object sender, EventArgs e)
        {

        }
        private void bnt_sua_Click(object sender, EventArgs e)
            {
                string sql = "Update sanpham set tensp = N'" + txt_tensp.Text + "',mancc = N'" + txt_mancc.SelectedValue + "',gianhap = '" + txt_gianhap.Text + "',giaban = '" + txt_giaban.Text + "',solg = '" + txt_solg.Text + "',hsd = '" + txt_hsd.Value + "',noisx = N'" + txt_nsx.Text + "',donvitinh = N'" + txt_dvt.Text + "',nguoinhap= N'" + txtNguoiNhap.Text + "' where masp='"+txt_masp.Text+"'";
                chuoiketnoi.Update(sql);
                chuoiketnoi.Chuoiketnoi_Data(chuoi, dta1);
                clear();
            }

            private void btn_xoa_Click(object sender, EventArgs e)
            {
                string sql = "Delete from sanpham where masp= '" + txt_masp.Text + "'";
                chuoiketnoi.Execute(sql);
                chuoiketnoi.Chuoiketnoi_Data(chuoi, dta1);
                clear();
            }

            private void btn_in_Click(object sender, EventArgs e)
            {
           
                    string duongdan = "";
                    String tenfile = "ThongTinSanPham";
                    XuatExecl.export_phieu(dta1, duongdan, tenfile, lbl_kq.Text);
                    MessageBox.Show("Xuất file thành công ", "Thông báo ", MessageBoxButtons.OK);       
            }

            private void btn_reset_Click(object sender, EventArgs e)
            {
                clear();
            }

            private void btn_Thoat_Click(object sender, EventArgs e)
            {
                if (MessageBox.Show("Bạn có muốn thoát không ? ", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    this.Close();
            }

            private void txt_gianhap_KeyPress(object sender, KeyPressEventArgs e)
            {

                if (!Char.IsControl(e.KeyChar) && !Char.IsNumber(e.KeyChar))
                    e.Handled = true;
            }

            private void txt_giaban_KeyPress(object sender, KeyPressEventArgs e)
            {

                if (!Char.IsControl(e.KeyChar) && !Char.IsNumber(e.KeyChar))
                    e.Handled = true;
            }

            private void txt_solg_KeyPress(object sender, KeyPressEventArgs e)
            {

                if (!Char.IsControl(e.KeyChar) && !Char.IsNumber(e.KeyChar))
                    e.Handled = true;
            }

            private void lbl_kq_Click(object sender, EventArgs e)
            {

            }

            private void txt_mancc_SelectedIndexChanged(object sender, EventArgs e)
            {

            }

            private void dta1_CellContentClick(object sender, DataGridViewCellEventArgs e)
            {

            }

            private void label10_Click(object sender, EventArgs e)
            {

            }

            private void groupBox2_Enter(object sender, EventArgs e)
            {

            }
        }
    }
