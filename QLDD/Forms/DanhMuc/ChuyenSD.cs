using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KPBT.Modules;
using DevComponents.DotNetBar;
using System.Data.OleDb;

namespace KPBT.Forms.DanhMuc
{
    public partial class ChuyenSD : Office2007Form
    {
        string nam = "";
        public OleDbConnection cn1;
        public OleDbConnection cn2;
        public string dl = "";
        public ChuyenSD()
        {
            InitializeComponent();           
            cn1 = DC1.Ketnoi_Database(DC1.namdllv);
            //cn2 = DC1.Ketnoi_Databasechuyen(DC1.namchuyen);
        }      

        private void Thoai_DOINAM_Load(object sender, EventArgs e)
        {          
            lbfilemau.Text = "Đường dẫn thư muc: " + Settings1.Default.thumucchuyen.ToString();
            lbtm.Text = "Dữ liệu làm việc: " + Settings1.Default.namchuyen.ToString();
        }
        
        private void hocsinh()
        {            
            string sqldk = "Select hocsinh.*, phong.loptren From hocsinh inner join phong on hocsinh.tenphong= phong.tenphong"+
                " where loptren <> 'cuối cấp' and ht = 'Đi học'";                
            OleDbCommand cmd = new OleDbCommand (sqldk,cn1);
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string sqlin = "Insert into hocsinh (mshs,tenhs,gths,ntns,datoc,dchs,tenphong,ht,chieucao,cannang,danhgia)" +
                    " values (@mshs,@tenhs,@gths,@ntns,@datoc,@dchs,@tenphong,@ht,@chieucao,@cannang,@danhgia)";
                OleDbCommand cmdin = new OleDbCommand(sqlin,cn2);
                cmdin.CommandText = sqlin;
                cmdin.Parameters.Add("@mshs", OleDbType.VarChar).Value = dr["mshs"].ToString();
                cmdin.Parameters.Add("@tenhs", OleDbType.VarChar).Value = dr["tenhs"].ToString();
                cmdin.Parameters.Add("@gths", OleDbType.VarChar).Value = dr["gths"].ToString();
                cmdin.Parameters.Add("@ntns", OleDbType.VarChar).Value = dr["ntns"].ToString();
                cmdin.Parameters.Add("@datoc", OleDbType.VarChar).Value = dr["datoc"].ToString();
                cmdin.Parameters.Add("@dchs", OleDbType.VarChar).Value = dr["dchs"].ToString();
                cmdin.Parameters.Add("@tenphong", OleDbType.VarChar).Value = dr["loptren"].ToString();
                cmdin.Parameters.Add("@ht", OleDbType.VarChar).Value = dr["ht"].ToString();
                cmdin.Parameters.Add("@chieucao", OleDbType.VarChar).Value = classdc.dungchung.kieudouble(dr["chieucao"].ToString());
                cmdin.Parameters.Add("@cannang", OleDbType.VarChar).Value =  classdc.dungchung.kieudouble(dr["cannang"].ToString());
                cmdin.Parameters.Add("@danhgia", OleDbType.VarChar).Value = dr["danhgia"].ToString();
                cmdin.ExecuteNonQuery();
            }
            dr.Close();
        }

        private void chungtunx()
        {
            string sqldel = "Delete * From nxct where idnxct = 0";
            OleDbCommand cmddel = new OleDbCommand(sqldel, cn2);
            cmddel.ExecuteNonQuery();
            string sql = "Insert into nxct (idnxct,ntnx,ldnhapxuat) values (@idnxct,@ntnx,@ldnhapxuat)";
            OleDbCommand cmd = new OleDbCommand(sql,cn2);
            cmd.Parameters.Add("@idnxct", OleDbType.Integer).Value = 0;
            cmd.Parameters.Add("@ntnx", OleDbType.Date).Value = classdc.dungchung.thaydoi("01/01/" + nam);
            cmd.Parameters.Add("@ldnhapxuat", OleDbType.VarChar).Value = "Số dư từ năm trước chuyển sang";
            cmd.ExecuteNonQuery();
        }

        private void thucpham()
        {
            string sqldel = "Delete * From nhxu where idnxct = 0";
            OleDbCommand cmddel = new OleDbCommand(sqldel, cn2);
            cmddel.ExecuteNonQuery();
            string sqldk = "Select * From dmtp";
            OleDbCommand cmd = new OleDbCommand(sqldk, cn1);
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                double kq1 = Connect.doubltt(Connect.TruyVandr("Select slnh from nhxu where matp = '" + dr["matp"].ToString() + "'"), "slnh");
                double kq2 = Connect.doubltt(Connect.TruyVandr("SELECT Sum([slxu])+Sum([slxuccnt])+Sum([slxubpnt])+Sum([slxumg])+Sum([slxuccmg])+Sum([slxubpmg]) AS Xuat" +
                    " FROM nhxu where matp = '" + dr["matp"].ToString() + "'"), "Xuat");
                double tien = Connect.doubltt(Connect.TruyVandr("SELECT Sum([ttnh]) AS Tien from nhxu where matp = '" + dr["matp"].ToString() + "'"), "Tien") -
                    Connect.doubltt(Connect.TruyVandr("SELECT Sum([ttxu]) AS Tien from nhxu where matp = '" + dr["matp"].ToString() + "'"), "Tien") -
                    Connect.doubltt(Connect.TruyVandr("SELECT Sum(ttxumg) AS Tien from nhxu where matp = '" + dr["matp"].ToString() + "'"), "Tien");
                if (kq1 - kq2 > 0)
                {
                    string sqlin = "insert into nhxu (idnxct,matp,tentp,donvitinh,dongia,chokho,slnh,ttnh) values (@idnxct,@matp,@tentp,@donvitinh,@dongia,@chokho,@slnh,@ttnh)";
                    OleDbCommand cmdin = new OleDbCommand(sqlin, cn2);
                    cmdin.Parameters.Add("@idnxct", OleDbType.VarChar).Value = "0";
                    cmdin.Parameters.Add("@matp", OleDbType.VarChar).Value = dr["matp"].ToString();
                    cmdin.Parameters.Add("@tentp", OleDbType.VarChar).Value = dr["tentp"].ToString();
                    cmdin.Parameters.Add("@donvitinh", OleDbType.VarChar).Value = dr["donvitinh"].ToString();
                    cmdin.Parameters.Add("@dongia", OleDbType.VarChar).Value = (tien / (kq1 - kq2)).ToString("n2");
                    cmdin.Parameters.Add("@chokho", OleDbType.VarChar).Value = dr["chokho"].ToString();
                    cmdin.Parameters.Add("@slnh", OleDbType.Double).Value = kq1 - kq2;
                    cmdin.Parameters.Add("@ttnh", OleDbType.Double).Value = tien;
                    cmdin.ExecuteNonQuery();
                }
            }
            dr.Close();
        }
        private void labelX3_Click(object sender, EventArgs e)
        {
            if (dl.IndexOf("QLDD") == -1)
            {
                MessageBox.Show("Đây không phải là csdl, bạn chọn lại", "Thông báo");
            }
            else
            {
                cn2 = DC1.Ketnoi_Databasechuyen(DC1.namchuyen);
                string sqldel = "delete * From hocsinh";
                OleDbCommand cmd = new OleDbCommand(sqldel, cn2);
                cmd.ExecuteNonQuery();
                hocsinh();
                chungtunx();
                thucpham();
                MessageBox.Show("Bạn đã chuyển số dư sang năm mới thành công ", "Thông báo");
                this.Close();
            }
        }

        private void labelX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void labelX7_Click(object sender, EventArgs e)
        {
            try
            {
                string vung = "", namdl = "";
                OpenFileDialog fd = new OpenFileDialog();
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    dl = fd.FileName.ToString();
                    if (dl.IndexOf(".mdb") == -1)
                    {
                        MessageBox.Show("Không tồn tại file dữ liệu", "Thông báo");
                    }
                    else
                    {
                        if (dl.IndexOf("QLDD.mdb") != -1)
                        {
                            MessageBox.Show("Đây là dữ liệu mẫu bạn không thể chọn đựơc!", "Thông báo");
                        }
                        else
                        {
                            vung = dl.Substring(0, dl.Length - 12);
                            namdl = dl.Substring(dl.Length - 12, 12);
                        }
                    }
                    lbfilemau.Text = "Đường dẫn thư muc: " + vung;
                    lbtm.Text = "Dữ liệu làm việc: " + namdl;
                    nam = namdl.Substring(4, 4);
                    Settings1.Default.thumucchuyen = vung;
                    Settings1.Default.Save();
                    Settings1.Default.namchuyen = namdl;
                    Settings1.Default.Save();
                    DC1.namchuyen = dl;
                }
            }
            catch
            {
                MessageBox.Show("Có lỗi xảy ra, bạn hãy chọn lại năm dữ liệu", "Thông báo");
            }
        }

        private void ChuyenSD_FormClosed(object sender, FormClosedEventArgs e)
        {
            Modules.DC1.iform = 1;
        }        
    }
}