using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class LoginForm : Form
    {
        public string PlayerName { get; private set; }
        public string IPAddress { get; private set; }
        public bool IsHost { get; private set; }
        public int GameMode { get; private set; } // 1: LAN, 2: Same PC, 3: vs AI
        public int BoardSize { get; private set; } // Number of cells for the board (both rows and columns)

        private SocketManager socket;

        public LoginForm()
        {
            InitializeComponent();
            socket = new SocketManager();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            string localIP = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            if (string.IsNullOrEmpty(localIP))
                localIP = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);

            txt_IP.Text = localIP;
            
            // Set default board size
            numBoardSize.Value = 10;
            
            // Update win condition info
            UpdateWinConditionInfo();
            
            // Add event handler for board size changes
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
            
            lblBoardSizeInfo.Text = $"(Win {cellsToWin} in a row to win)";
        }

        private void btn_HostGame_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                PlayerName = txt_PlayerName.Text;
                IPAddress = txt_IP.Text;
                BoardSize = (int)numBoardSize.Value;
                IsHost = true;
                GameMode = 1; // LAN Mode
                DialogResult = DialogResult.OK;
            }
        }

        private void btn_JoinGame_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                PlayerName = txt_PlayerName.Text;
                IPAddress = txt_IP.Text;
                BoardSize = (int)numBoardSize.Value;
                IsHost = false;
                GameMode = 1; // LAN Mode
                DialogResult = DialogResult.OK;
            }
        }

        private void btn_SamePC_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_PlayerName.Text))
            {
                PlayerName = txt_PlayerName.Text;
                IPAddress = txt_IP.Text;
                BoardSize = (int)numBoardSize.Value;
                IsHost = false;
                GameMode = 2; // Same PC Mode
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Please enter your player name!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_PlayAI_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_PlayerName.Text))
            {
                PlayerName = txt_PlayerName.Text;
                IPAddress = txt_IP.Text;
                BoardSize = (int)numBoardSize.Value;
                IsHost = false;
                GameMode = 3; // AI Mode
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Please enter your player name!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txt_PlayerName.Text))
            {
                MessageBox.Show("Please enter your player name!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_IP.Text))
            {
                MessageBox.Show("Please enter the IP address!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Check board size
            int boardSize = (int)numBoardSize.Value;
            if (boardSize < 3 || boardSize > 20)
            {
				MessageBox.Show("The board size must be between 3 and 20!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
            }

            return true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}