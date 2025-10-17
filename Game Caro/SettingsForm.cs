using System;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class SettingsForm : Form
    {
        // L?u tr? gi� tr? k�ch th??c b�n c? hi?n t?i
        private int currentBoardSize;
        // L?u tr? ch? ?? ch?i hi?n t?i
        private int currentPlayMode;
        
        // C�c ?i?u khi?n cho ch? ?? ch?i (ch�ng ta s? th�m v�o form)
        private GroupBox groupPlayMode;
        private RadioButton rbAIMode;
        private RadioButton rbSamePCMode;
        private RadioButton rbLANMode;
        
        // Event ?? th�ng b�o khi k�ch th??c b�n c? thay ??i
        public event EventHandler<BoardSizeChangedEventArgs> BoardSizeChanged;
        // Event ?? th�ng b�o khi ch? ?? ch?i thay ??i
        public event EventHandler<PlayModeChangedEventArgs> PlayModeChanged;

        public SettingsForm(int currentBoardSize, int currentPlayMode = 2)
        {
            InitializeComponent();
            this.currentBoardSize = currentBoardSize;
            this.currentPlayMode = currentPlayMode;
            
            // Th�m c�c ?i?u khi?n cho ch? ?? ch?i
            AddPlayModeControls();
        }
        
        private void AddPlayModeControls()
        {
            // T?o GroupBox
            groupPlayMode = new GroupBox();
            groupPlayMode.Location = new System.Drawing.Point(12, 118);
            groupPlayMode.Size = new System.Drawing.Size(398, 120);
            groupPlayMode.Text = "Ch? ?? ch?i";
            groupPlayMode.TabIndex = 3;
            
            // T?o RadioButton cho ch? ?? LAN
            rbLANMode = new RadioButton();
            rbLANMode.AutoSize = true;
            rbLANMode.Location = new System.Drawing.Point(20, 30);
            rbLANMode.Size = new System.Drawing.Size(146, 21);
            rbLANMode.TabIndex = 0;
            rbLANMode.Text = "Ch?i qua LAN";
            
            // T?o RadioButton cho ch? ?? Same PC
            rbSamePCMode = new RadioButton();
            rbSamePCMode.AutoSize = true;
            rbSamePCMode.Location = new System.Drawing.Point(20, 60);
            rbSamePCMode.Size = new System.Drawing.Size(185, 21);
            rbSamePCMode.TabIndex = 1;
            rbSamePCMode.Text = "2 ng??i ch?i tr�n 1 m�y";
            
            // T?o RadioButton cho ch? ?? AI
            rbAIMode = new RadioButton();
            rbAIMode.AutoSize = true;
            rbAIMode.Location = new System.Drawing.Point(20, 90);
            rbAIMode.Size = new System.Drawing.Size(134, 21);
            rbAIMode.TabIndex = 2;
            rbAIMode.Text = "Ch?i v?i m�y (AI)";
            
            // Th�m c�c RadioButton v�o GroupBox
            groupPlayMode.Controls.Add(rbLANMode);
            groupPlayMode.Controls.Add(rbSamePCMode);
            groupPlayMode.Controls.Add(rbAIMode);
            
            // Th�m GroupBox v�o form
            this.Controls.Add(groupPlayMode);
            
            // ?i?u ch?nh v? tr� c�c n�t
            btnApply.Location = new System.Drawing.Point(191, 250);
            btnCancel.Location = new System.Drawing.Point(304, 250);
            
            // ?i?u ch?nh k�ch th??c form
            this.ClientSize = new System.Drawing.Size(422, 300);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // Thi?t l?p gi� tr? ban ??u cho NumericUpDown
            numBoardSize.Value = currentBoardSize;
            
            // C?p nh?t th�ng tin ?i?u ki?n th?ng
            UpdateWinConditionInfo();
            
            // ??ng k� s? ki?n ValueChanged
            numBoardSize.ValueChanged += NumBoardSize_ValueChanged;
        }

        private void NumBoardSize_ValueChanged(object sender, EventArgs e)
        {
            UpdateWinConditionInfo();
        }

        private void UpdateWinConditionInfo()
        {
            int boardSize = (int)numBoardSize.Value;
            int cellsToWin = (boardSize <= 5) ? 3 : 5;

			lblBoardSizeInfo.Text = $"(win {cellsToWin} in a row to win)";
		}

        private void btnApply_Click(object sender, EventArgs e)
        {
            int newBoardSize = (int)numBoardSize.Value;
            
            // X�c ??nh ch? ?? ch?i m?i
            int newPlayMode = 2; // M?c ??nh l� Same PC
            
            if (rbLANMode.Checked)
                newPlayMode = 1;
            else if (rbSamePCMode.Checked)
                newPlayMode = 2;
            else if (rbAIMode.Checked)
                newPlayMode = 3;
            
            // N?u k�ch th??c b�n c? ?� thay ??i, th�ng b�o qua s? ki?n
            if (newBoardSize != currentBoardSize)
            {
                BoardSizeChanged?.Invoke(this, new BoardSizeChangedEventArgs(newBoardSize));
                currentBoardSize = newBoardSize;
				MessageBox.Show("Changes will take effect when a new game starts.", "Notification", MessageBoxButtons.OK,
								   MessageBoxIcon.Information);
			}
            
            // N?u ch? ?? ch?i ?� thay ??i, th�ng b�o qua s? ki?n
            if (newPlayMode != currentPlayMode)
            {
                PlayModeChanged?.Invoke(this, new PlayModeChangedEventArgs(newPlayMode));
                currentPlayMode = newPlayMode;
            }
            
            if (newBoardSize != currentBoardSize || newPlayMode != currentPlayMode)
            {
                MessageBox.Show("Changes will take effect when a new game starts.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }

    // L?p EventArgs t�y ch?nh ?? truy?n k�ch th??c b�n c? m?i
    public class BoardSizeChangedEventArgs : EventArgs
    {
        public int NewBoardSize { get; private set; }

        public BoardSizeChangedEventArgs(int newBoardSize)
        {
            NewBoardSize = newBoardSize;
        }
    }
    
    // L?p EventArgs t�y ch?nh ?? truy?n ch? ?? ch?i m?i
    public class PlayModeChangedEventArgs : EventArgs
    {
        public int NewPlayMode { get; private set; }

        public PlayModeChangedEventArgs(int newPlayMode)
        {
            NewPlayMode = newPlayMode;
        }
    }
}