namespace TicTacToe
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblBoardSizeInfo = new System.Windows.Forms.Label();
			this.numBoardSize = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.btnApply = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numBoardSize)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblBoardSizeInfo);
			this.groupBox1.Controls.Add(this.numBoardSize);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(18, 19);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox1.Size = new System.Drawing.Size(597, 156);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Board Configuration";
			this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
			// 
			// lblBoardSizeInfo
			// 
			this.lblBoardSizeInfo.AutoSize = true;
			this.lblBoardSizeInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBoardSizeInfo.Location = new System.Drawing.Point(286, 70);
			this.lblBoardSizeInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblBoardSizeInfo.Name = "lblBoardSizeInfo";
			this.lblBoardSizeInfo.Size = new System.Drawing.Size(244, 29);
			this.lblBoardSizeInfo.TabIndex = 2;
			this.lblBoardSizeInfo.Text = "(win 5 in a row to win)";
			// 
			// numBoardSize
			// 
			this.numBoardSize.Location = new System.Drawing.Point(170, 67);
			this.numBoardSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.numBoardSize.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.numBoardSize.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.numBoardSize.Name = "numBoardSize";
			this.numBoardSize.Size = new System.Drawing.Size(108, 38);
			this.numBoardSize.TabIndex = 1;
			this.numBoardSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numBoardSize.ValueChanged += new System.EventHandler(this.NumBoardSize_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(32, 70);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(227, 31);
			this.label1.TabIndex = 0;
			this.label1.Text = "Board size (n × n)";
			// 
			// btnApply
			// 
			this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnApply.Location = new System.Drawing.Point(286, 202);
			this.btnApply.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(159, 56);
			this.btnApply.TabIndex = 1;
			this.btnApply.Text = "Apply";
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCancel.Location = new System.Drawing.Point(456, 202);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(159, 56);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// SettingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(633, 277);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Settings";
			this.Load += new System.EventHandler(this.SettingsForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numBoardSize)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblBoardSizeInfo;
        private System.Windows.Forms.NumericUpDown numBoardSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}