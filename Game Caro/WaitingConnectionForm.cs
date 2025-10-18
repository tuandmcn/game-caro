using System;
using System.Drawing;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class WaitingConnectionForm : Form
    {
        private Label lblMessage;
        private PictureBox pictureBox;
        private Label lblDots;
        private Timer animationTimer;
        private int dotCount = 0;

        public WaitingConnectionForm(string message)
        {
            InitializeComponent();
            lblMessage.Text = message;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false; // Không cho phép ?óng form
            
            // Kh?i t?o timer cho hi?u ?ng loading
            animationTimer = new Timer();
            animationTimer.Interval = 500; // C?p nh?t m?i 500ms
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            dotCount = (dotCount + 1) % 4;
            lblDots.Text = new string('.', dotCount);
        }

        private void InitializeComponent()
        {
            this.Text = "Information";
            this.ClientSize = new Size(450, 120);
            this.BackColor = Color.White;

            // PictureBox cho icon thông tin
            pictureBox = new PictureBox
            {
                Location = new Point(20, 30),
                Size = new Size(32, 32),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            
            // S? d?ng icon thông tin c?a h? th?ng
            pictureBox.Image = SystemIcons.Information.ToBitmap();

            // Label hi?n th? thông báo
            lblMessage = new Label
            {
                Location = new Point(65, 35),
                Size = new Size(320, 50),
                Font = new Font("Arial", 10, FontStyle.Regular),
                Text = "",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            // Label hi?n th? d?u ch?m loading
            lblDots = new Label
            {
                Location = new Point(385, 35),
                Size = new Size(40, 50),
                Font = new Font("Arial", 10, FontStyle.Bold),
                Text = "...",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.Controls.Add(pictureBox);
            this.Controls.Add(lblMessage);
            this.Controls.Add(lblDots);
        }

        // Ph??ng th?c ?? ?óng form t? thread khác
        public void CloseFormSafely()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    animationTimer.Stop();
                    animationTimer.Dispose();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }));
            }
            else
            {
                animationTimer.Stop();
                animationTimer.Dispose();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (animationTimer != null)
            {
                animationTimer.Stop();
                animationTimer.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}
