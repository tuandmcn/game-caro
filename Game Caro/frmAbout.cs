using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Caro
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
            this.Text = "How to Play";
            this.StartPosition = FormStartPosition.CenterParent;

            RichTextBox rtbAbout = new RichTextBox();
            //int paddingInPixels = 38; // 0.5 cm

            rtbAbout.ReadOnly = true;
            rtbAbout.BorderStyle = BorderStyle.None;
            rtbAbout.Dock = DockStyle.Fill;
            // Thiết lập khoảng cách lề bên trái và phải
            //rtbAbout.Margin = new Padding(
            //    paddingInPixels,  // Left
            //    this.Padding.Top, // Top (giữ nguyên hoặc đặt giá trị mong muốn)
            //    paddingInPixels,  // Right
            //    this.Padding.Bottom // Bottom (giữ nguyên hoặc đặt giá trị mong muốn)
            //);

            rtbAbout.BackColor = this.BackColor;
            rtbAbout.BackColor = Color.White;
            rtbAbout.ForeColor = Color.Black;

            // Tắt tính năng tự động phát hiện URL để tránh gạch chân link
            rtbAbout.DetectUrls = false;

            // Xóa nội dung cũ (nếu có)
            rtbAbout.Clear();

            // Add content with formatting
            rtbAbout.SelectionFont = new Font("Arial", 16, FontStyle.Bold);
            rtbAbout.SelectionColor = Color.Blue;
            rtbAbout.AppendText("\nTic Tac Toe Game\n\n");

            rtbAbout.SelectionFont = new Font("Arial", 11, FontStyle.Regular);
            rtbAbout.SelectionColor = Color.Black;
            rtbAbout.AppendText("This is the final project for the Computer-Controlled Application Development course.\n\n");

            rtbAbout.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbAbout.AppendText("Instructor:\n");
            rtbAbout.SelectionFont = new Font("Arial", 11, FontStyle.Regular);
            rtbAbout.AppendText("Dr. Nguyen Dang Khoa\n\n");

            rtbAbout.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbAbout.AppendText("Developed by:\n");
            rtbAbout.SelectionFont = new Font("Arial", 11, FontStyle.Regular);
            rtbAbout.AppendText("Group 5");
            this.Controls.Add(rtbAbout);
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
