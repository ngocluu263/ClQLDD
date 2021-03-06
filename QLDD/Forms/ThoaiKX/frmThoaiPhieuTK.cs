﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Office;
using DevComponents.DotNetBar;
using KPBT.Modules;
using System.Data.OleDb;
using System.Threading;

namespace KPBT.Forms.ThoaiKX
{
    public partial class frmThoaiPhieuTK : Office2007Form
    {
        public frmThoaiPhieuTK()
        {
            InitializeComponent();
        }

        private void frmThoaiLuuMTAn_Load(object sender, EventArgs e)
        {           
            MtbNgayTT.Text = DateTime.Now.ToShortDateString();
        }
        private void appdc()
        {
            string sql = "insert into TblDC (TEN1,TEN2,TEN3) " +
                "Select nhxu.tentp,nhxu.donvitinh,nhxu.matp" +
                " From nhxu inner join nxct on nhxu.idnxct = nxct.idnxct" +
                " where nxct.ntnx <= #" + classdc.dungchung.thaydoi(MtbNgayTT.Text) + "# group by nhxu.tentp,nhxu.donvitinh,nhxu.matp";
            Connect.ThaoTac(sql);      
        }
        double soluongton(string matp)
        {
            double sln = 0, slx = 0;
            string sql = "Select nhxu.* From nhxu inner join nxct on nhxu.idnxct = nxct.idnxct Where nhxu.matp = '" + matp + "' and nxct.ntnx <= #" + classdc.dungchung.thaydoi(MtbNgayTT.Text) + "#";
            OleDbDataReader dr = Connect.TruyVandr(sql);
            while (dr.Read())
            {
                sln += classdc.dungchung.kieudouble(dr["slnh"].ToString());
                slx += classdc.dungchung.kieudouble(dr["slxu"].ToString()) + classdc.dungchung.kieudouble(dr["slxuccnt"].ToString()) + classdc.dungchung.kieudouble(dr["slxubpnt"].ToString()) + classdc.dungchung.kieudouble(dr["slxumg"].ToString()) + classdc.dungchung.kieudouble(dr["slxuccmg"].ToString()) + classdc.dungchung.kieudouble(dr["slxubpmg"].ToString());
            }
            dr.Close();
            return sln - slx;
        }
        private void updc()
        {            
            OleDbDataReader dr = Connect.TruyVandr("select * From TblDC");
            while (dr.Read())
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandText = "Update TblDC set SL1=@SL1 where TEN3 = '" + dr["TEN3"].ToString() + "'";
                cmd.Parameters.Add("@SL1", OleDbType.Double).Value = soluongton(dr["TEN3"].ToString());
                Connect.LuuDL(cmd);
            }
            dr.Close();
        }       
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string[] st = MtbNgayTT.Text.Split('/');
                Connect.ThaoTac("Delete * From TblDC");
                appdc();
                updc();
                DataSet dts = new DataSet();
                Connect.Loadds(dts, "Select * From TblDC where SL1 > 0 order by TEN1", "TblDC");
                Forms.frmHienThi frm = new Forms.frmHienThi();
                Reports.PhieuTonKho rpt = new KPBT.Reports.PhieuTonKho();
                rpt.NTN.Value = "Ngày " + st[0] + " tháng " + st[1] + " năm " + st[2];
                rpt.TenDV.Value = thongtinketxuat.Default.tendv.ToString();                
                rpt.ThuKho.Value = thongtinketxuat.Default.thukho.ToString();
                rpt.DataSource = dts;
                frm.printControl1.PrintingSystem = rpt.PrintingSystem;
                rpt.CreateDocument();
                frm.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Có lỗi trong quá trình tạo sổ. Bạn hãy kiểm tra lại", "Thông báo");
            }
        }

        private void MktNgayTT_Leave(object sender, EventArgs e)
        {
            try
            {
                MtbNgayTT.Text = DC1.ngaythang(MtbNgayTT.Text);
            }
            catch
            {
                MtbNgayTT.Text = string.Format("{0:dd/MM/yyyy}", DateTime.Today);
            }
        }        

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmThoaiSoCho_FormClosed(object sender, FormClosedEventArgs e)
        {
            Modules.DC1.iform = 1;
        }
        
    }
}
