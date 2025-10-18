﻿using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace Game_Caro
{
    partial class GameCaro : Form
    {
        #region Properties
        GameBoard board;
        SocketManager socket;
        string PlayerName;

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

        private void Board_GameOver(object sender, EventArgs e)
        {
            MessageBox.Show("Board_GameOver first:" + PlayerName);
            EndGame();
            MessageBox.Show("Board_GameOver next:" + PlayerName);
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
                }
                catch { }
            }
                
            pn_GameBoard.Enabled = true;
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
                if (board.PlayMode == 1)
                {
                    try
                    {
                        socket.Send(new SocketData((int)SocketCommand.QUIT, "", new Point()));
                    } catch { }

                    socket.CloseConnect();
                    MessageBox.Show("Disconnected from LAN.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            board.PlayMode = 3;
            NewGame();
            board.StartAI();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
                return;

            PlayerName = board.ListPlayers[socket.IsServer ? 0 : 1].Name;
            txt_Chat.Text += "- " + PlayerName + ": " + txt_Message.Text + "\r\n";

            socket.Send(new SocketData((int)SocketCommand.SEND_MESSAGE, txt_Chat.Text, new Point()));
            Listen();
        }
        #endregion

        #region LAN settings
        private void GameCaro_Shown(object sender, EventArgs e)
        {
            txt_IP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(txt_IP.Text))
                txt_IP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
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
            PlayerName = board.ListPlayers[board.CurrentPlayer == 1 ? 0 : 1].Name;

            switch (data.Command)
            {
                case (int)SocketCommand.SEND_POINT:
                    // Có thay đổi giao diện muốn chạy ngọt phải để trong đây
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
                    break;

                case (int)SocketCommand.NEW_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        NewGame();
                        pn_GameBoard.Enabled = false;
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
                        MessageBox.Show("engame: " + PlayerName);
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

                default:
                    break;
            }

            Listen();
        }
        #endregion

        #endregion

        private void lbl_About_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
