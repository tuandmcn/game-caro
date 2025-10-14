namespace TicTacToe
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_PlayerName = new System.Windows.Forms.TextBox();
            this.txt_IP = new System.Windows.Forms.TextBox();
            this.btn_HostGame = new System.Windows.Forms.Button();
            this.btn_JoinGame = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_SamePC = new System.Windows.Forms.Button();
            this.btn_PlayAI = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.numBoardSize = new System.Windows.Forms.NumericUpDown();
            this.lblBoardSizeInfo = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBoardSize)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 119);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Player name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 157);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "IP Address:";
            // 
            // txt_PlayerName
            // 
            this.txt_PlayerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PlayerName.Location = new System.Drawing.Point(134, 117);
            this.txt_PlayerName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txt_PlayerName.Name = "txt_PlayerName";
            this.txt_PlayerName.Size = new System.Drawing.Size(184, 23);
            this.txt_PlayerName.TabIndex = 2;
            // 
            // txt_IP
            // 
            this.txt_IP.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_IP.Location = new System.Drawing.Point(134, 152);
            this.txt_IP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txt_IP.Name = "txt_IP";
            this.txt_IP.Size = new System.Drawing.Size(184, 23);
            this.txt_IP.TabIndex = 3;
            // 
            // btn_HostGame
            // 
            this.btn_HostGame.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btn_HostGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_HostGame.Location = new System.Drawing.Point(30, 232);
            this.btn_HostGame.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_HostGame.Name = "btn_HostGame";
            this.btn_HostGame.Size = new System.Drawing.Size(100, 37);
            this.btn_HostGame.TabIndex = 4;
            this.btn_HostGame.Text = "Create Game";
            this.btn_HostGame.UseVisualStyleBackColor = false;
            this.btn_HostGame.Click += new System.EventHandler(this.btn_HostGame_Click);
            // 
            // btn_JoinGame
            // 
            this.btn_JoinGame.BackColor = System.Drawing.Color.LightGreen;
            this.btn_JoinGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_JoinGame.Location = new System.Drawing.Point(134, 232);
            this.btn_JoinGame.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_JoinGame.Name = "btn_JoinGame";
            this.btn_JoinGame.Size = new System.Drawing.Size(108, 37);
            this.btn_JoinGame.TabIndex = 5;
            this.btn_JoinGame.Text = "Join Game";
            this.btn_JoinGame.UseVisualStyleBackColor = false;
            this.btn_JoinGame.Click += new System.EventHandler(this.btn_JoinGame_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SkyBlue;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(332, 95);
            this.panel1.TabIndex = 6;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(73, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(213, 32);
            this.label3.TabIndex = 0;
            this.label3.Text = "TIC TAC TOE ♥";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(27, 213);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Play via LAN";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(27, 286);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "Play on this computer";
            // 
            // btn_SamePC
            // 
            this.btn_SamePC.BackColor = System.Drawing.Color.LightPink;
            this.btn_SamePC.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SamePC.Location = new System.Drawing.Point(30, 305);
            this.btn_SamePC.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_SamePC.Name = "btn_SamePC";
            this.btn_SamePC.Size = new System.Drawing.Size(129, 37);
            this.btn_SamePC.TabIndex = 9;
            this.btn_SamePC.Text = "Two Players";
            this.btn_SamePC.UseVisualStyleBackColor = false;
            this.btn_SamePC.Click += new System.EventHandler(this.btn_SamePC_Click);
            // 
            // btn_PlayAI
            // 
            this.btn_PlayAI.BackColor = System.Drawing.Color.Gold;
            this.btn_PlayAI.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_PlayAI.Location = new System.Drawing.Point(169, 305);
            this.btn_PlayAI.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_PlayAI.Name = "btn_PlayAI";
            this.btn_PlayAI.Size = new System.Drawing.Size(129, 37);
            this.btn_PlayAI.TabIndex = 10;
            this.btn_PlayAI.Text = "Play with computer";
            this.btn_PlayAI.UseVisualStyleBackColor = false;
            this.btn_PlayAI.Click += new System.EventHandler(this.btn_PlayAI_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(27, 186);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "Board size:";
            // 
            // numBoardSize
            // 
            this.numBoardSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numBoardSize.Location = new System.Drawing.Point(115, 184);
            this.numBoardSize.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.numBoardSize.Size = new System.Drawing.Size(52, 23);
            this.numBoardSize.TabIndex = 12;
            this.numBoardSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numBoardSize.ValueChanged += new System.EventHandler(this.NumBoardSize_ValueChanged);
            // 
            // lblBoardSizeInfo
            // 
            this.lblBoardSizeInfo.AutoSize = true;
            this.lblBoardSizeInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBoardSizeInfo.Location = new System.Drawing.Point(172, 186);
            this.lblBoardSizeInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBoardSizeInfo.Name = "lblBoardSizeInfo";
            this.lblBoardSizeInfo.Size = new System.Drawing.Size(153, 15);
            this.lblBoardSizeInfo.TabIndex = 13;
            this.lblBoardSizeInfo.Text = "(Win 5 in a row to win)";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 363);
            this.Controls.Add(this.lblBoardSizeInfo);
            this.Controls.Add(this.numBoardSize);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btn_PlayAI);
            this.Controls.Add(this.btn_SamePC);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_JoinGame);
            this.Controls.Add(this.btn_HostGame);
            this.Controls.Add(this.txt_IP);
            this.Controls.Add(this.txt_PlayerName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập - Tic Tac Toe";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBoardSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_PlayerName;
        private System.Windows.Forms.TextBox txt_IP;
        private System.Windows.Forms.Button btn_HostGame;
        private System.Windows.Forms.Button btn_JoinGame;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_SamePC;
        private System.Windows.Forms.Button btn_PlayAI;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numBoardSize;
        private System.Windows.Forms.Label lblBoardSizeInfo;
    }
}