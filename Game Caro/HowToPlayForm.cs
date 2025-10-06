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
    public partial class HowToPlayForm : Form
    {
        public HowToPlayForm()
        {
            InitializeComponent();
            this.Text = "How to Play";
            this.StartPosition = FormStartPosition.CenterParent;
            //this.Size = new Size(500, 350);

            RichTextBox guide = new RichTextBox();
            guide.ReadOnly = true;
            guide.BorderStyle = BorderStyle.None;
            guide.Dock = DockStyle.Fill;
            guide.BackColor = this.BackColor;
            guide.BackColor = Color.White;
            guide.ForeColor = Color.Black;

            guide.Font = new Font("Segoe UI", 12);
            guide.Text =
                "\n" +
                "🎮 TIC TAC TOE RULES 🎮\n\n" +
                "• Two players take turns marking X and O on the board.\n\n" +
                "• If you choose the 3x3 mode: the winner is the player who gets 3 consecutive marks " +
                "(horizontally, vertically, or diagonally).\n\n" +
                "• If you choose a board larger than 5x5: the winner is the player who gets 5 consecutive marks " +
                "(horizontally, vertically, or diagonally).\n\n" +
                "• A sequence that is blocked at both ends does not count as a win.\n\n" +
                "✨ Have fun playing!";

            this.Controls.Add(guide);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void HowToPlayForm_Load(object sender, EventArgs e)
        {

        }

        private void RichTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
