using System;
using System.Drawing;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class LANFirstMoveForm : Form
    {
        public bool HostGoesFirst { get; private set; }

        public LANFirstMoveForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponent()
        {
            this.Text = "Who goes first?";
            this.ClientSize = new Size(400, 200);

            Label lblQuestion = new Label
            {
                Text = "Select who goes first:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(80, 30),
                Size = new Size(240, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnHostFirst = new Button
            {
                Text = "I go first (Host)",
                Font = new Font("Arial", 10, FontStyle.Regular),
                Location = new Point(50, 80),
                Size = new Size(140, 50),
                BackColor = Color.LightGreen
            };
            btnHostFirst.Click += (s, e) =>
            {
                HostGoesFirst = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            Button btnClientFirst = new Button
            {
                Text = "Opponent goes first",
                Font = new Font("Arial", 10, FontStyle.Regular),
                Location = new Point(210, 80),
                Size = new Size(140, 50),
                BackColor = Color.LightBlue
            };
            btnClientFirst.Click += (s, e) =>
            {
                HostGoesFirst = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(lblQuestion);
            this.Controls.Add(btnHostFirst);
            this.Controls.Add(btnClientFirst);
        }
    }
}
