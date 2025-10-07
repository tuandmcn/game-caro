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
    public partial class ContactMe : Form
    {
        public ContactMe()
        {
            InitializeComponent();
            this.Text = "Contact me";
            this.StartPosition = FormStartPosition.CenterParent;
            //this.Size = new Size(500, 350);

            TransparentRichTextBox guide = new TransparentRichTextBox();
            guide.ReadOnly = true;
            guide.BorderStyle = BorderStyle.None;
            guide.Dock = DockStyle.Fill;
            guide.BackColor = this.BackColor;
            guide.BackColor = Color.White;
            guide.ForeColor = Color.Black;

            guide.Font = new Font("Segoe UI", 12);
            guide.Text =
                "\n📞 TEAM 5 CONTACT INFORMATION 📞\n\n" +
                "Team Leader: Nguyen Truong Son\n" +
                "Phone: +84 975 872 356\n" +
                "Email: sonhai12092000@gmail.com\n\n" +

                "Member: Do Manh Tuan\n" +
                "Phone: +84 982 653 579\n" +
                "Email: tuandmcn@hotmail.com\n\n" +

                "Member: Mai Xuan Dat\n" +
                "Phone: +84 962 488 231\n" +
                "Email: 25075015@vnu.edu.vn";
            this.Controls.Add(guide);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ContactMe_Load(object sender, EventArgs e)
        {

        }
    }
}
