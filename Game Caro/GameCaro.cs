using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace TicTacToe
{
    partial class GameCaro : Form
    {
        #region Properties
        GameBoard board;
        SocketManager socket;
        string PlayerName;
        bool IsHost;
        int BoardSize;
        bool? LastHostGoesFirst = null; // Lưu lại lựa chọn ai đi trước trong LAN mode (null = chưa xác định)

        public GameCaro(string playerName, string ipAddress, bool isHost, int gameMode, int boardSize = 10)
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            PlayerName = playerName;
            IsHost = isHost;
            BoardSize = boardSize;
            
            board = new GameBoard(pn_GameBoard, txt_PlayerName, pb_Avatar);
            
            // Set the board size
            board.SetBoardSize(BoardSize);
            
            // Adjust panel size based on board dimensions
            AdjustBoardPanelSize();
            
            // Set the player name from login form
            board.SetPlayerName(playerName, 0);
            
            board.PlayerClicked += Board_PlayerClicked;
            board.GameOver += Board_GameOver;

            pgb_CountDown.Step = Constance.CountDownStep;
            pgb_CountDown.Maximum = Constance.CountDownTime;

            tm_CountDown.Interval = Constance.CountDownInterval;
            socket = new SocketManager();
            
            socket.IP = ipAddress;
            
            txt_Chat.Text = "";
            txt_PlayerName.Text = playerName;
            txt_IP.Text = ipAddress;
            
            // Make sure these fields are read-only
            txt_PlayerName.ReadOnly = true;
            txt_IP.ReadOnly = true;

            // Update form title to show board size and cells to win
            this.Text = $"Tic Tac Toe - Bàn cờ {BoardSize}x{BoardSize} - Thắng {board.CellsToWin} ô liên tiếp";

            // Set up the chat availability based on game mode
            SetChatAvailability(gameMode);

            NewGame();
            
            // Set up game based on selected mode
            switch (gameMode)
            {
                case 1: // LAN Mode
                    SetupLanGame(isHost);
                    break;
                case 2: // Same PC Mode
                    SetupSamePcGame();
                    break;
                case 3: // AI Mode
                    SetupAiGame();
                    break;
            }
        }
        
        private void AdjustBoardPanelSize()
        {
            // Calculate the required panel size based on board dimensions and cell size
            int panelWidth = BoardSize * Constance.CellWidth;
            int panelHeight = BoardSize * Constance.CellHeight;
            
            // Ensure the panel is large enough to display all cells
            pn_GameBoard.Width = panelWidth;
            pn_GameBoard.Height = panelHeight;
            
            // You can also adjust form size if necessary
            int formWidthDifference = this.Width - pn_GameBoard.Width;
            int formHeightDifference = this.Height - pn_GameBoard.Height;
            
            this.Width = panelWidth + formWidthDifference;
            this.Height = panelHeight + formHeightDifference;
        }
        
        private void SetChatAvailability(int gameMode)
        {
            // Chat is only available in LAN mode (mode 1)
            bool isChatAvailable = gameMode == 1;
            txt_Message.Enabled = isChatAvailable;
            btn_Send.Enabled = isChatAvailable;
            txt_Chat.Enabled = isChatAvailable;
            
            if (!isChatAvailable)
            {
                txt_Chat.Text = "Tính năng chat không khả dụng trong chế độ này.";
            }
        }
        
        private void SetupLanGame(bool isHost)
        {
            board.PlayMode = 1;
            
            if (isHost)
            {
                socket.IsServer = true;
                socket.CreateServer();
                
                // Tạo form chờ kết nối
                WaitingConnectionForm waitingForm = new WaitingConnectionForm("You are the server. Waiting for opponent to connect...");
                
                // Tạo thread kiểm tra kết nối
                Thread checkConnectionThread = new Thread(() =>
                {
                    // Chờ cho đến khi client kết nối (kiểm tra socket client đã được tạo chưa)
                    while (!socket.IsClientConnected())
                    {
                        Thread.Sleep(500); // Kiểm tra mỗi 500ms
                    }
                    
                    // Đóng form chờ
                    waitingForm.CloseFormSafely();
                });
                checkConnectionThread.IsBackground = true;
                checkConnectionThread.Start();
                
                // Hiển thị form chờ (block cho đến khi đóng)
                waitingForm.ShowDialog();
                
                // Gửi tên của Host cho Client
                socket.Send(new SocketData((int)SocketCommand.SEND_PLAYER_NAME, PlayerName, new Point()));
                
                // Chờ nhận tên của Client
                Thread.Sleep(100); // Đợi một chút để đảm bảo dữ liệu được gửi
                
                // Sau khi client đã kết nối, hiển thị form chọn ai đi trước
                using (LANFirstMoveForm lanFirstMoveForm = new LANFirstMoveForm())
                {
                    if (lanFirstMoveForm.ShowDialog() == DialogResult.OK)
                    {
                        bool hostGoesFirst = lanFirstMoveForm.HostGoesFirst;
                        LastHostGoesFirst = hostGoesFirst; // Lưu lại lựa chọn
                        
                        // Gửi lựa chọn cho Client
                        if (hostGoesFirst)
                        {
                            socket.Send(new SocketData((int)SocketCommand.HOST_GOES_FIRST, "", new Point()));
                            pn_GameBoard.Enabled = true;  // Host được đi trước
                        }
                        else
                        {
                            socket.Send(new SocketData((int)SocketCommand.CLIENT_GOES_FIRST, "", new Point()));
                            pn_GameBoard.Enabled = false; // Client được đi trước
                        }
                        
                        // Vẽ bàn cờ trước, sau đó thiết lập người chơi đầu tiên
                        board.DrawGameBoard();
                        board.SetFirstPlayer(hostGoesFirst, true);
                    }
                    else
                    {
                        // Nếu hủy dialog, mặc định Host đi trước
                        LastHostGoesFirst = true; // Lưu lại lựa chọn
                        socket.Send(new SocketData((int)SocketCommand.HOST_GOES_FIRST, "", new Point()));
                        pn_GameBoard.Enabled = true;
                        
                        board.DrawGameBoard();
                        board.SetFirstPlayer(true, true);
                    }
                }
                
                Listen(); // Bắt đầu lắng nghe sau khi gửi lựa chọn
            }
            else
            {
                if (socket.ConnectServer())
                {
                    socket.IsServer = false;
                    
                    // Gửi tên của Client cho Host
                    socket.Send(new SocketData((int)SocketCommand.SEND_PLAYER_NAME, PlayerName, new Point()));
                    
                    // Chờ nhận tên của Host và lựa chọn ai đi trước
                    MessageBox.Show("Connection successful! Waiting for host to choose who goes first...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    Listen(); // Bắt đầu lắng nghe ngay
                }
                else
                {
                    MessageBox.Show("Cannot connect to IP address: " + socket.IP, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    board.PlayMode = 2; // Switch to 2-player same computer mode as fallback
                }
            }
        }
        
        private void SetupSamePcGame()
        {
            board.PlayMode = 2;
            pn_GameBoard.Enabled = true;
        }
        
        private void SetupAiGame()
        {
            board.PlayMode = 3;
            pn_GameBoard.Enabled = true;
            
            // Show dialog to choose who goes first
            using (AIFirstMoveForm aiFirstMoveForm = new AIFirstMoveForm())
            {
                if (aiFirstMoveForm.ShowDialog() == DialogResult.OK)
                {
                    // Set the property on the board
                    board.AIGoesFirst = aiFirstMoveForm.AIGoesFirst;
					var humanAvatar = Image.FromFile(Application.StartupPath + @"\images\Quan.jpg"); // hoặc ảnh người sẵn có
					var aiAvatar = Image.FromFile(Application.StartupPath + @"\images\Lisa.jpg"); // hoặc ảnh máy sẵn có

					if (board.AIGoesFirst)
					{
						// AI là player 0, Human là player 1
						board.ListPlayers[0].Name = "Computer";
						board.ListPlayers[0].Avatar = aiAvatar;

						board.ListPlayers[1].Name = "You";
						board.ListPlayers[1].Avatar = humanAvatar;

						board.CurrentPlayer = 0; // lượt hiện tại là AI
					}
					else
					{
						// Human là player 0, AI là player 1
						board.ListPlayers[0].Name = "You";
						board.ListPlayers[0].Avatar = humanAvatar;

						board.ListPlayers[1].Name = "Computer";
						board.ListPlayers[1].Avatar = aiAvatar;

						board.CurrentPlayer = 0; // lượt hiện tại là You
					}

					// ✅ gọi lại logic 2-player để cập nhật ảnh/tên ở UI
					board.RefreshCurrentPlayerUI();
                    //thua--datfix
                    // Nếu AI đi trước thì để UI render xong rồi mới cho AI đi
     //               if (board.AIGoesFirst)
					//{
					//	this.Refresh();
					//	Application.DoEvents();
					//	board.StartAI();
					//}


					// Nếu AI đi trước, thì cho AI đi ngay lập tức
					if (board.AIGoesFirst)
                    {
                        // Đảm bảo mọi thứ được render xong
                        this.Refresh();
                        Application.DoEvents();
                        
                        // Trực tiếp gọi StartAI
                        board.StartAI();
                    }
                }
                else
                {
                    // Mặc định - AI đi trước nếu người chơi hủy dialog
                    board.AIGoesFirst = true;
                    
                    // Đảm bảo mọi thứ được render xong
                    this.Refresh();
                    Application.DoEvents();
                    
                    // Trực tiếp gọi StartAI
                    board.StartAI();
                }
            }
        }
        
        // Keep the original constructor for compatibility, though it won't be used anymore
        public GameCaro()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += GameCaro_KeyDown;

            Control.CheckForIllegalCrossThreadCalls = false;

            board = new GameBoard(pn_GameBoard, txt_PlayerName, pb_Avatar);            
            board.PlayerClicked += Board_PlayerClicked;
            board.GameOver += Board_GameOver;

            pgb_CountDown.Step = Constance.CountDownStep;
            pgb_CountDown.Maximum = Constance.CountDownTime;

            tm_CountDown.Interval = Constance.CountDownInterval;
            socket = new SocketManager();

            txt_Chat.Text = "";

            NewGame();
        }
        #endregion

        #region Methods
        private void GameCaro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                ShowHowToPlay();
                e.Handled = true;
            }
        }

        private void ShowHowToPlay()
        {
            HowToPlayForm helpForm = new HowToPlayForm();
            helpForm.ShowDialog();
        }

        private void ShowContactMe()
        {
            ContactMe contactForm = new ContactMe();
            contactForm.ShowDialog();
        }

        private void ShowAbout()
        {
            frmAbout aboutForm = new frmAbout();
            aboutForm.ShowDialog();
        }

        void NewGame()
        {
            pgb_CountDown.Value = 0;
            tm_CountDown.Stop();

            undoToolStripMenuItem.Enabled = true;
            redoToolStripMenuItem.Enabled = true;

            btn_Undo.Enabled = true;
            btn_Redo.Enabled = true;

            // Cập nhật kích thước bàn cờ nếu đã thay đổi qua Settings
            board.SetBoardSize(BoardSize);
            AdjustBoardPanelSize();
            
            // Cập nhật tiêu đề của form để hiển thị kích thước bàn cờ mới
            this.Text = $"Tic Tac Toe - Bàn cờ {BoardSize}x{BoardSize} - Thắng {board.CellsToWin} ô liên tiếp";

            // Reset symbols về mặc định trước khi vẽ bàn cờ
            board.ResetSymbols();
            
            board.DrawGameBoard();
        }

        void EndGame()
        {
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;

            btn_Undo.Enabled = false;
            btn_Redo.Enabled = false;

            tm_CountDown.Stop();
            pn_GameBoard.Enabled = false;
        }

        private void GameCaro_Load(object sender, EventArgs e)
        {
            lbl_About.Text = "This project is an internet-based Tic Tac Toe game developed using C# programming language. Created by Group 5.";
            tm_About.Enabled = true;
        }

        private void GameCaro_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                e.Cancel = true;
            else
            {
                try
                {
                    socket.Send(new SocketData((int)SocketCommand.QUIT, "", new Point()));
                } catch { }
            }
        }

        private void Board_PlayerClicked(object sender, BtnClickEvent e)
        {
            tm_CountDown.Start(); 
            pgb_CountDown.Value = 0;

            if (board.PlayMode == 1)
            {
                try
                {
                    pn_GameBoard.Enabled = false;
                    socket.Send(new SocketData((int)SocketCommand.SEND_POINT, "", e.ClickedPoint));

                    undoToolStripMenuItem.Enabled = false;
                    redoToolStripMenuItem.Enabled = false;

                    btn_Undo.Enabled = false;
                    btn_Redo.Enabled = false;

                    Listen();
                }
                catch
                {
                    EndGame();
                    MessageBox.Show("Could not connect to the opponent.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

		//private void Board_GameOver(object sender, EventArgs e)
		//{
		//    EndGame();

		//    if (board.PlayMode == 1)
		//        socket.Send(new SocketData((int)SocketCommand.END_GAME, "", new Point()));
		//}
		private void Board_GameOver(object sender, EventArgs e)
		{
			// Xác định người thắng dựa trên nước đi cuối cùng
			if (board.StkUndoStep != null && board.StkUndoStep.Count > 0)
			{
				var last = board.StkUndoStep.Peek();          // nước đi cuối
				int winnerIdx = last.CurrentPlayer;           // 0 hoặc 1

				string msg;
				if (board.PlayMode == 3) // Chơi với máy (AI)
				{
					//int aiIndex = board.AIGoesFirst ? 0 : 1;
					//msg = (winnerIdx == aiIndex) ? "Computer wins!" : "You win!";
					string winnerNameAI = board.ListPlayers[winnerIdx].Name;
					msg = (winnerNameAI == "Computer") ? "Computer wins!" : "You win!";
                    MessageBox.Show(msg, "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
				else
				{
					// 2 người trên cùng máy hoặc qua LAN: hiển thị tên người thắng
					string winnerName = board.ListPlayers[winnerIdx].Name;
					msg = $"\"{winnerName}\" wins!";
				}

				//MessageBox.Show(msg, "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			EndGame();  // dừng đồng hồ, khóa bàn cờ

			if (board.PlayMode == 1)
				socket.Send(new SocketData((int)SocketCommand.END_GAME, "", new Point()));
		}


		private void Tm_CountDown_Tick(object sender, EventArgs e)
        {
            pgb_CountDown.PerformStep();

            if (pgb_CountDown.Value >= pgb_CountDown.Maximum)
            {
                EndGame();

                if (board.PlayMode == 1)
                    socket.Send(new SocketData((int)SocketCommand.TIME_OUT, "", new Point()));
            }                                   
        }

        private void Tm_About_Tick(object sender, EventArgs e)
        {
            lbl_About.Location = new Point(lbl_About.Location.X, lbl_About.Location.Y - 2);

            if (lbl_About.Location.Y + lbl_About.Height < 0)
                lbl_About.Location = new Point(lbl_About.Location.X, Grb_About.Height - 10);
        }

        #region MenuStrip
        private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();

            if (board.PlayMode == 1)
            {
                try
                {
                    socket.Send(new SocketData((int)SocketCommand.NEW_GAME, "", new Point()));
                    
                    // Sau khi gửi lệnh NEW_GAME, cần thiết lập lại symbols dựa trên LastHostGoesFirst
                    if (LastHostGoesFirst.HasValue)
                    {
                        board.SetFirstPlayer(LastHostGoesFirst.Value, IsHost);
                        
                        // Thiết lập pn_GameBoard.Enabled dựa trên ai đi trước
                        if (IsHost)
                        {
                            pn_GameBoard.Enabled = LastHostGoesFirst.Value; // Host enabled nếu Host đi trước
                        }
                        else
                        {
                            pn_GameBoard.Enabled = !LastHostGoesFirst.Value; // Client enabled nếu Client đi trước
                        }
                        
                        // Bắt đầu đếm ngược nếu đến lượt của người chơi này
                        if (pn_GameBoard.Enabled)
                        {
                            tm_CountDown.Start();
                        }
                    }
                }
                catch { }
            }
            else
            {
                pn_GameBoard.Enabled = true;
            }
            
            // Nếu đang ở chế độ chơi với máy và máy đi trước, thực hiện nước đi của máy
            if (board.PlayMode == 3 && board.AIGoesFirst)
            {
                // Đảm bảo form được hiển thị đầy đủ
                this.Refresh();
                Application.DoEvents();
                
                // Cho máy đi nước đầu tiên
                board.StartAI();
            }

			// Thông báo cho người dùng biết có thể thay đổi cài đặt
			MessageBox.Show("A new game has been created. You can change the board size in Options > Settings before starting a match.",
				"Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pgb_CountDown.Value = 0;
            board.Undo();

            if (board.PlayMode == 1)
                socket.Send(new SocketData((int)SocketCommand.UNDO, "", new Point()));
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // pgb_CountDown.Value = 0;
            board.Redo();

            if (board.PlayMode == 1)
                socket.Send(new SocketData((int)SocketCommand.REDO, "", new Point()));
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ViaLANToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Method kept for compatibility with menu item
            board.PlayMode = 1;
            NewGame();

            socket.IP = txt_IP.Text;

            if (!socket.ConnectServer())
            {
                socket.IsServer = true;
                pn_GameBoard.Enabled = true;
                socket.CreateServer();
                MessageBox.Show("You are hosting the game.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                socket.IsServer = false;
                pn_GameBoard.Enabled = false;
                Listen();
                MessageBox.Show("Connection established!!!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SameComToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Method kept for compatibility with menu item
            if (board.PlayMode == 1)
            {
                try
                {
                    socket.Send(new SocketData((int)SocketCommand.QUIT, "", new Point()));
                } catch { }

                socket.CloseConnect();
                MessageBox.Show("Disconnected from LAN.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            board.PlayMode = 2;
            NewGame();
        }

        private void PlayerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (board.PlayMode == 1)
            {
                try
                {
                    socket.Send(new SocketData((int)SocketCommand.QUIT, "", new Point()));
                } catch { }

                socket.CloseConnect();
                MessageBox.Show("Disconnected from LAN.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Lưu lại trạng thái AI đi trước nếu đã từng chơi với AI
            bool previousAIGoesFirst = board.PlayMode == 3 ? board.AIGoesFirst : true;
            
            board.PlayMode = 3;
            NewGame();
            
            // Đặt lại trạng thái AI đi trước đã lưu
            board.AIGoesFirst = previousAIGoesFirst;
            
            // Hiển thị dialog chọn người đi trước 
            SetupAiGame();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cho phép mở Settings khi:
            // 1. Ván chơi đã kết thúc (pn_GameBoard.Enabled == false)
            // 2. Chưa bắt đầu ván chơi nào (board.StkUndoStep == null)
            // 3. Đã bắt đầu ván mới nhưng chưa đánh ô nào (board.StkUndoStep.Count == 0)
            if (pn_GameBoard.Enabled == false || board.StkUndoStep == null || board.StkUndoStep.Count == 0)
            {
                // Tạo form Settings với kích thước bàn cờ hiện tại
                SettingsForm settingsForm = new SettingsForm(BoardSize);
                
                // Đăng ký xử lý sự kiện khi kích thước bàn cờ thay đổi
                settingsForm.BoardSizeChanged += (s, args) => 
                {
                    BoardSize = args.NewBoardSize;
                    
                    // Nếu đang ở ván mới chưa đánh ô nào, áp dụng thay đổi ngay lập tức
                    if (board.StkUndoStep != null && board.StkUndoStep.Count == 0)
                    {
                        NewGame(); // Áp dụng kích thước bàn cờ mới ngay lập tức
                    }
                };
                
                // Hiển thị form Settings
                settingsForm.ShowDialog();
            }
            else
            {
				MessageBox.Show("You need to finish the current game or start a new one before changing the settings!",
				"Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

        private void HowToPlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHowToPlay();
        }

        private void ContactMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowContactMe();
        }

        private void AboutThisGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }
        #endregion     

        #region Button Settings
        private void Btn_LAN_Click(object sender, EventArgs e)
        {
            ViaLANToolStripMenuItem_Click(sender, e);
        }

        private void Btn_SameCom_Click(object sender, EventArgs e)
        {
            SameComToolStripMenuItem_Click(sender, e);
        }

        private void Btn_AI_Click(object sender, EventArgs e)
        {
            PlayerToolStripMenuItem1_Click(sender, e);
        }

        private void Btn_Undo_Click(object sender, EventArgs e)
        {
            UndoToolStripMenuItem_Click(sender, e);
        }

        private void Btn_Redo_Click(object sender, EventArgs e)
        {
            RedoToolStripMenuItem_Click(sender, e);
        }

        private void Btn_Send_Click(object sender, EventArgs e)
        {
            if (board.PlayMode != 1)
            {
                MessageBox.Show("Tính năng không khả dụng khi chơi với máy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_Message.Text))
                return;

            txt_Chat.Text += "- " + PlayerName + ": " + txt_Message.Text + "\r\n";
            
            // Tự động cuộn xuống cuối để hiển thị tin nhắn mới nhất
            ScrollChatToBottom();

            socket.Send(new SocketData((int)SocketCommand.SEND_MESSAGE, txt_Chat.Text, new Point()));
            
            // Clear the message text box after sending
            txt_Message.Text = "";
            
            Listen();
        }
        #endregion

        #region LAN settings
        private void GameCaro_Shown(object sender, EventArgs e)
        {
            // We no longer need to set IP here as it's set from the login form
        }

        private void Listen()
        {
            Thread ListenThread = new Thread(() =>
            {
                try
                {
                    SocketData data = (SocketData)socket.Receive();
                    ProcessData(data);
                }
                catch { }
            });

            ListenThread.IsBackground = true;
            ListenThread.Start();
        }

        private void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketCommand.SEND_POINT:
                    // Có thay đổi giao diện muốn chạy ngọt phải để trong đây
                    this.Invoke((MethodInvoker)(() =>
                    {
                        board.OtherPlayerClicked(data.Point);
                        pn_GameBoard.Enabled = true;

                        pgb_CountDown.Value = 0;
                        tm_CountDown.Start();

                        undoToolStripMenuItem.Enabled = true;
                        redoToolStripMenuItem.Enabled = true;

                        btn_Undo.Enabled = true;
                        btn_Redo.Enabled = true;
                    }));
                    break;

                case (int)SocketCommand.SEND_MESSAGE:
                    txt_Chat.Text = data.Message;
                    // Tự động cuộn xuống cuối để hiển thị tin nhắn mới nhất
                    ScrollChatToBottom();
                    break;

                case (int)SocketCommand.NEW_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        NewGame();
                        
                        // Sau khi tạo game mới, thiết lập lại symbols dựa trên LastHostGoesFirst
                        if (LastHostGoesFirst.HasValue)
                        {
                            board.SetFirstPlayer(LastHostGoesFirst.Value, IsHost);
                            
                            // Thiết lập pn_GameBoard.Enabled dựa trên ai đi trước
                            if (IsHost)
                            {
                                pn_GameBoard.Enabled = LastHostGoesFirst.Value; // Host enabled nếu Host đi trước
                            }
                            else
                            {
                                pn_GameBoard.Enabled = !LastHostGoesFirst.Value; // Client enabled nếu Client đi trước
                            }
                            
                            // Bắt đầu đếm ngược nếu đến lượt của người chơi này
                            if (pn_GameBoard.Enabled)
                            {
                                tm_CountDown.Start();
                            }
                        }
                        else
                        {
                            pn_GameBoard.Enabled = false;
                        }
                    }));
                    break;

                case (int)SocketCommand.UNDO:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        pgb_CountDown.Value = 0;
                        board.Undo();
                    }));
                    break;

                case (int)SocketCommand.REDO:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        // pgb_CountDown.Value = 0;
                        board.Redo();
                    }));
                    break;

                case (int)SocketCommand.END_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        EndGame();
                        MessageBox.Show(PlayerName + " is the winner ♥ !!!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;

                case (int)SocketCommand.TIME_OUT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        EndGame();
                        MessageBox.Show("Time has expired !!!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;

                case (int)SocketCommand.QUIT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        tm_CountDown.Stop();
                        EndGame();
                    
                        board.PlayMode = 2;
                        socket.CloseConnect();

                        MessageBox.Show("The opponent has disconnected.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;

                case (int)SocketCommand.HOST_GOES_FIRST:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        // Lưu lại lựa chọn
                        LastHostGoesFirst = true;
                        
                        // Vẽ bàn cờ trước
                        board.DrawGameBoard();
                        
                        // Host đi trước, Client đi sau
                        pn_GameBoard.Enabled = false; // Client chờ đến lượt
                        board.SetFirstPlayer(true, false); // false vì đây là Client
                        MessageBox.Show("Host will go first. Wait for your turn!", "Turn Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;

                case (int)SocketCommand.CLIENT_GOES_FIRST:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        // Lưu lại lựa chọn
                        LastHostGoesFirst = false;
                        
                        // Vẽ bàn cờ trước
                        board.DrawGameBoard();
                        
                        // Client đi trước, Host đi sau
                        pn_GameBoard.Enabled = true; // Client được đi trước
                        board.SetFirstPlayer(false, false); // false vì đây là Client
                        tm_CountDown.Start(); // Bắt đầu đếm ngược
                        MessageBox.Show("You go first!", "Turn Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    break;

                case (int)SocketCommand.SEND_PLAYER_NAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        // Nhận tên của người chơi đối phương
                        string opponentName = data.Message;
                        
                        // Cập nhật tên người chơi
                        if (IsHost)
                        {
                            // Host: Player 0 là Host, Player 1 là Client
                            board.ListPlayers[0].Name = PlayerName;
                            board.ListPlayers[1].Name = opponentName;
                        }
                        else
                        {
                            // Client: Player 0 là Host, Player 1 là Client
                            board.ListPlayers[0].Name = opponentName;
                            board.ListPlayers[1].Name = PlayerName;
                        }
                        
                        // Cập nhật UI nếu cần
                        board.RefreshCurrentPlayerUI();
                    }));
                    break;

                default:
                    break;
            }

            Listen();
        }
        #endregion

        private void lbl_About_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        /// <summary>
        /// Tự động cuộn xuống cuối cùng của txt_Chat để hiển thị tin nhắn mới nhất
        /// </summary>
        private void ScrollChatToBottom()
        {
            if (txt_Chat.InvokeRequired)
            {
                txt_Chat.Invoke(new MethodInvoker(() =>
                {
                    txt_Chat.SelectionStart = txt_Chat.Text.Length;
                    txt_Chat.ScrollToCaret();
                }));
            }
            else
            {
                txt_Chat.SelectionStart = txt_Chat.Text.Length;
                txt_Chat.ScrollToCaret();
            }
        }
        #endregion
    }
}
