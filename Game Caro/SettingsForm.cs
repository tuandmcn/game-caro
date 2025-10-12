using System;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class SettingsForm : Form
    {
        // L?u tr? gi� tr? k�ch th??c b�n c? hi?n t?i
        private int currentBoardSize;
        
        // Event ?? th�ng b�o khi k�ch th??c b�n c? thay ??i
        public event EventHandler<BoardSizeChangedEventArgs> BoardSizeChanged;

        public SettingsForm(int currentBoardSize)
        {
            InitializeComponent();
            this.currentBoardSize = currentBoardSize;
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
            
            lblBoardSizeInfo.Text = $"(th?ng {cellsToWin} � li�n ti?p ?? th?ng)";
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            int newBoardSize = (int)numBoardSize.Value;
            
            // N?u k�ch th??c b�n c? ?� thay ??i, th�ng b�o qua s? ki?n
            if (newBoardSize != currentBoardSize)
            {
                BoardSizeChanged?.Invoke(this, new BoardSizeChangedEventArgs(newBoardSize));
                currentBoardSize = newBoardSize;
                MessageBox.Show("Thay ??i s? ???c �p d?ng khi b?t ??u v�n ch?i m?i.", "Th�ng b�o", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
}