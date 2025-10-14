using System;
using System.Drawing;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class AIFirstMoveForm : Form
    {
        // Result of the dialog
        public bool AIGoesFirst { get; private set; }

        public AIFirstMoveForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Text = "Who goes first?";
            this.BackColor = Color.Lavender;
            this.AcceptButton = btnPlayerFirst;  // Set default button
            this.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AIFirstMoveForm));
            this.btnPlayerFirst = new System.Windows.Forms.Button();
            this.btnAIFirst = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnPlayerFirst
            // 
            this.btnPlayerFirst.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnPlayerFirst.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlayerFirst.Location = new System.Drawing.Point(44, 68);
            this.btnPlayerFirst.Name = "btnPlayerFirst";
            this.btnPlayerFirst.Size = new System.Drawing.Size(120, 45);
            this.btnPlayerFirst.TabIndex = 0;
            this.btnPlayerFirst.Text = "You go first";
            this.btnPlayerFirst.UseVisualStyleBackColor = false;
            this.btnPlayerFirst.Click += new System.EventHandler(this.btnPlayerFirst_Click);
            // 
            // btnAIFirst
            // 
            this.btnAIFirst.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnAIFirst.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAIFirst.Location = new System.Drawing.Point(190, 68);
            this.btnAIFirst.Name = "btnAIFirst";
            this.btnAIFirst.Size = new System.Drawing.Size(120, 45);
            this.btnAIFirst.TabIndex = 1;
            this.btnAIFirst.Text = "Computer goes first";
            this.btnAIFirst.UseVisualStyleBackColor = false;
            this.btnAIFirst.Click += new System.EventHandler(this.btnAIFirst_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(100, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose who goes first";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // AIFirstMoveForm
            // 
            this.ClientSize = new System.Drawing.Size(354, 137);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAIFirst);
            this.Controls.Add(this.btnPlayerFirst);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AIFirstMoveForm";
			//dat them
			this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnPlayerFirst;
        private System.Windows.Forms.Button btnAIFirst;
        private System.Windows.Forms.Label label1;

        private void btnPlayerFirst_Click(object sender, EventArgs e)
        {
            AIGoesFirst = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnAIFirst_Click(object sender, EventArgs e)
        {
            AIGoesFirst = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}