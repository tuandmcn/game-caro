using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TicTacToe
{
    class GameBoard
    {
        #region Properties
        private Panel board; 

        private int currentPlayer;
        private TextBox playerName;
        private PictureBox avatar;

        private List<Player> listPlayers;
        private List<List<Button>> matrixPositions;

        private event EventHandler<BtnClickEvent> playerClicked;
        private event EventHandler gameOver;

        private Stack<PlayInfo> stkUndoStep;
        private Stack<PlayInfo> stkRedoStep;

        private int playMode = 0;
        private bool IsAI = false;

        // Added property to track if AI goes first
        private bool aiGoesFirst = true;

        // Thêm thu?c tính ?? ki?m tra n?u bàn c? ?ã ???c v? xong
        private bool boardReady = false;

        // Added properties for configurable board size
        private int boardSize = 10;  // Default size 10x10
        private int cellsToWin = 5;  // Default 5 in a row to win

		// Gi?i h?n t?ng s? l?n UNDO + REDO cho m?i ng??i ch?i trong m?t ván
		private const int OP_LIMIT = 5;
		// opCount[0] cho Player 0 (AI n?u PlayMode=3), opCount[1] cho Player 1 (ng??i)
		private int[] opCount = new int[2];

		public Panel Board
        {
            get { return board; }
            set { board = value; }
        }

        public int CurrentPlayer
        {
            get { return currentPlayer; }
            set { currentPlayer = value; }
        }

        public TextBox PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        public PictureBox Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }

        public List<Player> ListPlayers
        {
            get { return listPlayers; }
            set { listPlayers = value; }
        }

        public List<List<Button>> MatrixPositions
        {
            get { return matrixPositions; }
            set { matrixPositions = value; }
        }

        public event EventHandler<BtnClickEvent> PlayerClicked
        {
            add { playerClicked += value; }
            remove { playerClicked -= value; }
        }

        public event EventHandler GameOver
        {
            add { gameOver += value; }
            remove { gameOver -= value; }
        }

        public Stack<PlayInfo> StkUndoStep
        {
            get { return stkUndoStep; }
            set { stkUndoStep = value; }
        }

        public Stack<PlayInfo> StkRedoStep
        {
            get { return stkRedoStep; }
            set { stkRedoStep = value; }
        }

        public int PlayMode
        {
            get { return playMode; }
            set { playMode = value; }
        }

        public bool AIGoesFirst
        {
            get { return aiGoesFirst; }
            set { aiGoesFirst = value; }
        }

        public bool BoardReady
        {
            get { return boardReady; }
        }

        public int BoardSize
        {
            get { return boardSize; }
            set { boardSize = value; }
        }

        public int CellsToWin
        {
            get { return cellsToWin; }
            set { cellsToWin = value; }
        }
        #endregion

        #region Initialize
        public GameBoard(Panel board, TextBox PlayerName, PictureBox Avatar)
        {
            this.Board = board;
            this.PlayerName = PlayerName;
            this.Avatar = Avatar;

            this.CurrentPlayer = 0;
            this.ListPlayers = new List<Player>()
            {
                new Player("Player 1", Image.FromFile(Application.StartupPath + "\\images\\Quan.jpg"),
                                        Image.FromFile(Application.StartupPath + "\\images\\X.png")),

                new Player("Player 2", Image.FromFile(Application.StartupPath + "\\images\\Lisa.jpg"),
                                   Image.FromFile(Application.StartupPath + "\\images\\O.png"))
            };
        }

        public void SetPlayerName(string playerName, int playerIndex = 0)
        {
            if (playerIndex >= 0 && playerIndex < ListPlayers.Count)
            {
                ListPlayers[playerIndex].Name = playerName;
            }
        }

        public void SetBoardSize(int size)
        {
            if (size >= 3 && size <= 20)
            {
                BoardSize = size;
                // Set win condition based on board size
                CellsToWin = (size <= 5) ? 3 : 5;
                
                // Update Constance values to match the new board size
                Constance.nRows = size;
                Constance.nCols = size;

                // FIXED: Always maintain a consistent cell size regardless of board size
                // The default is 35x35 pixels for all board sizes
                Constance.CellWidth = 35;
                Constance.CellHeight = 35;
            }
        }
        #endregion

        #region Methods       
        public void DrawGameBoard()
        {
            boardReady = false;
            board.Enabled = true;
            board.Controls.Clear();

            StkUndoStep = new Stack<PlayInfo>();
            StkRedoStep = new Stack<PlayInfo>();

            opCount[0] = 0;
            opCount[1] = 0;

			this.CurrentPlayer = 0;
            ChangePlayer();

            int LocX = 0;
            int LocY = 0;
            int nRows = Constance.nRows;
            int nCols = Constance.nCols;

            // Create fresh matrix positions
            MatrixPositions = new List<List<Button>>();

            for (int i = 0; i < nRows; i++)
            {
                MatrixPositions.Add(new List<Button>());
                LocX = 0; // Reset X position for each new row

                for (int j = 0; j < nCols; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Constance.CellWidth,
                        Height = Constance.CellHeight,
                        Location = new Point(LocX, LocY),
                        Tag = i.ToString(), // For row identification
                        BackColor = Color.Lavender,
                        BackgroundImageLayout = ImageLayout.Stretch
                    };

                    btn.Click += btn_Click;
                    MatrixPositions[i].Add(btn);
                    Board.Controls.Add(btn);

                    // Move X position to next cell
                    LocX += Constance.CellWidth;
                }

                // Move Y position to next row
                LocY += Constance.CellHeight;
            }

            boardReady = true;
        }

        private Point GetButtonCoordinate(Button btn)
        {
            int Vertical = Convert.ToInt32(btn.Tag);
            int Horizontal = MatrixPositions[Vertical].IndexOf(btn);

            Point Coordinate = new Point(Horizontal, Vertical);
            return Coordinate;
        }

        #region Undo & Redo
        public bool Undo()
        {
            if (StkUndoStep.Count <= 1)
                return false;

			// Gi?i h?n t?ng s? l?n UNDO+REDO theo ch? ?? ch?i
			int actor = this.CurrentPlayer;               // ng??i ?ang t?i l??t b?m Undo
			if (this.PlayMode == 3) actor = 1;            // Ch?i v?i máy: ch? ng??i th?t b? gi?i h?n

			if (opCount[actor] >= OP_LIMIT)
			{
				MessageBox.Show($"You have used all {OP_LIMIT} UNDO/REDO operations.",
				"Limit reached", MessageBoxButtons.OK, MessageBoxIcon.Information);

				return false;
			}
			opCount[actor]++; // ghi nh?n m?t l?n thao tác Undo/Redo


            PlayInfo OldPos = StkUndoStep.Peek();
            CurrentPlayer = OldPos.CurrentPlayer == 1 ? 0 : 1;

            bool IsUndo1 = UndoAStep();
            bool IsUndo2 = UndoAStep();

            return IsUndo1 && IsUndo2;
        }

        private bool UndoAStep()
        {
            if (StkUndoStep.Count <= 0)
                return false;

            PlayInfo OldPos = StkUndoStep.Pop();
            StkRedoStep.Push(OldPos);

            Button btn = MatrixPositions[OldPos.Point.Y][OldPos.Point.X];
            btn.BackgroundImage = null;

            if (StkUndoStep.Count <= 0)
                CurrentPlayer = 0;
            else
                OldPos = StkUndoStep.Peek();

            ChangePlayer();

            return true;
        }

        public bool Redo()
        {
			//if (StkRedoStep.Count <= 0)
			//    return false;
			if (StkRedoStep.Count <= 0)
			{
				MessageBox.Show("You must Undo before you can Redo.",
								"Notification",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information);
				return false;
			}

			// Gi?i h?n t?ng s? l?n UNDO+REDO theo ch? ?? ch?i
			int actor = this.CurrentPlayer;               // ng??i ?ang t?i l??t b?m Redo
			if (this.PlayMode == 3) actor = 1;            // Ch?i v?i máy: ch? ng??i th?t b? gi?i h?n

			if (opCount[actor] >= OP_LIMIT)
			{
				MessageBox.Show($"You have used all {OP_LIMIT} UNDO/REDO operations.",
				"Limit reached", MessageBoxButtons.OK, MessageBoxIcon.Information);

				return false;
			}
			opCount[actor]++; // ghi nh?n m?t l?n thao tác Undo/Redo


            PlayInfo OldPos = StkRedoStep.Peek();
            CurrentPlayer = OldPos.CurrentPlayer;

            bool IsRedo1 = RedoAStep();
            bool IsRedo2 = RedoAStep();

            return IsRedo1 && IsRedo2;
        }

        private bool RedoAStep()
        {
            if (StkRedoStep.Count <= 0)
                return false;

            PlayInfo OldPos = StkRedoStep.Pop();
            StkUndoStep.Push(OldPos);

            Button btn = MatrixPositions[OldPos.Point.Y][OldPos.Point.X];
            btn.BackgroundImage = OldPos.Symbol;

            if (StkRedoStep.Count <= 0)
                CurrentPlayer = OldPos.CurrentPlayer == 1 ? 0 : 1;
            else
                OldPos = StkRedoStep.Peek();

            ChangePlayer();

            return true;
        }
        #endregion

        #region Handling winning and losing
        private bool CheckHorizontal(int CurrRow, int CurrCol, Image PlayerSymbol)
        {
            int Count;

            // If not enough cells to the right for a win, return false immediately
            if (CurrCol > Constance.nCols - CellsToWin)
                return false;

            // Check if there are enough consecutive symbols horizontally
            for (Count = 1; Count < CellsToWin; Count++)
                if (MatrixPositions[CurrRow][CurrCol + Count].BackgroundImage != PlayerSymbol)
                    return false;

            // Xét ch?n 2 ??u - Check if win condition is valid (not blocked on both sides)
            if (CurrCol == 0 || CurrCol + Count == Constance.nCols)
                return true;

            if (MatrixPositions[CurrRow][CurrCol - 1].BackgroundImage == null || MatrixPositions[CurrRow][CurrCol + Count].BackgroundImage == null)
            {
                // Highlight the winning cells
                for (Count = 0; Count < CellsToWin; Count++)
                    MatrixPositions[CurrRow][CurrCol + Count].BackColor = Color.Lime;
                return true;
            }

            return false;
        }

        private bool CheckVertical(int CurrRow, int CurrCol, Image PlayerSymbol)
        {
            int Count;

            // If not enough cells below for a win, return false immediately
            if (CurrRow > Constance.nRows - CellsToWin)
                return false;

            // Check if there are enough consecutive symbols vertically
            for (Count = 1; Count < CellsToWin; Count++)
                if (MatrixPositions[CurrRow + Count][CurrCol].BackgroundImage != PlayerSymbol)
                    return false;

            // Xét ch?n 2 ??u - Check if win condition is valid (not blocked on both sides)
            if (CurrRow == 0 || CurrRow + Count == Constance.nRows)
                return true;

            if (MatrixPositions[CurrRow - 1][CurrCol].BackgroundImage == null || MatrixPositions[CurrRow + Count][CurrCol].BackgroundImage == null)
            {
                // Highlight the winning cells
                for (Count = 0; Count < CellsToWin; Count++)
                    MatrixPositions[CurrRow + Count][CurrCol].BackColor = Color.Lime;
                return true;
            }

            return false;
        }

        private bool CheckMainDiag(int CurrRow, int CurrCol, Image PlayerSymbol)
        {
            int Count;

            // If not enough cells diagonally for a win, return false immediately
            if (CurrRow > Constance.nRows - CellsToWin || CurrCol > Constance.nCols - CellsToWin)
                return false;

            // Check if there are enough consecutive symbols diagonally
            for (Count = 1; Count < CellsToWin; Count++)
                if (MatrixPositions[CurrRow + Count][CurrCol + Count].BackgroundImage != PlayerSymbol)
                    return false;

            // Xét ch?n 2 ??u - Check if win condition is valid (not blocked on both sides)
            if (CurrRow == 0 || CurrRow + Count == Constance.nRows || CurrCol == 0 || CurrCol + Count == Constance.nCols)
                return true;

            if (MatrixPositions[CurrRow - 1][CurrCol - 1].BackgroundImage == null || MatrixPositions[CurrRow + Count][CurrCol + Count].BackgroundImage == null)
            {
                // Highlight the winning cells
                for (Count = 0; Count < CellsToWin; Count++)
                    MatrixPositions[CurrRow + Count][CurrCol + Count].BackColor = Color.Lime;
                return true;
            }

            return false;
        }

        private bool CheckExtraDiag(int CurrRow, int CurrCol, Image PlayerSymbol)
        {
            int Count;

            // If not enough cells diagonally for a win, return false immediately
            if (CurrRow < CellsToWin - 1 || CurrCol > Constance.nCols - CellsToWin)
                return false;

            // Check if there are enough consecutive symbols diagonally
            for (Count = 1; Count < CellsToWin; Count++)
                if (MatrixPositions[CurrRow - Count][CurrCol + Count].BackgroundImage != PlayerSymbol)
                    return false;

            // Xét ch?n 2 ??u - Check if win condition is valid (not blocked on both sides)
            if (CurrRow == CellsToWin - 1 || CurrRow == Constance.nRows - 1 || CurrCol == 0 || CurrCol + Count == Constance.nCols)
                return true;

            if (MatrixPositions[CurrRow + 1][CurrCol - 1].BackgroundImage == null || MatrixPositions[CurrRow - Count][CurrCol + Count].BackgroundImage == null)
            {
                // Highlight the winning cells
                for (Count = 0; Count < CellsToWin; Count++)
                    MatrixPositions[CurrRow - Count][CurrCol + Count].BackColor = Color.Lime;
                return true;
            }

            return false;
        }

        private bool IsEndGame()
        {
            // Check for draw (board full)
            if (StkUndoStep.Count == Constance.nRows * Constance.nCols)
            {
                MessageBox.Show("Hòa c? !!!");
                return true;
            }

            bool IsWin = false;

            foreach (PlayInfo btn in StkUndoStep)
            {
                if (CheckHorizontal(btn.Point.Y, btn.Point.X, btn.Symbol))
                    IsWin = true;

                if (CheckVertical(btn.Point.Y, btn.Point.X, btn.Symbol))
                    IsWin = true;

                if (CheckMainDiag(btn.Point.Y, btn.Point.X, btn.Symbol))
                    IsWin = true;

                if (CheckExtraDiag(btn.Point.Y, btn.Point.X, btn.Symbol))
                    IsWin = true;
            }

            if (IsWin)
                return IsWin;
            return false;
        }
        #endregion

        #region 2 players
        public void EndGame()
        {
            if (gameOver != null)
                gameOver(this, new EventArgs());
        }

        private void ChangePlayer()
        {
            PlayerName.Text = ListPlayers[CurrentPlayer].Name;
            Avatar.Image = ListPlayers[CurrentPlayer].Avatar;
        }

		// Tái dùng logic c?a ch? ?? 2 ng??i
		public void RefreshCurrentPlayerUI()
		{
			PlayerName.Text = ListPlayers[CurrentPlayer].Name;
			Avatar.Image = ListPlayers[CurrentPlayer].Avatar;
		}
		private void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn.BackgroundImage != null)
                return; // N?u ô ?ã ???c ?ánh thì ko cho ?ánh l?i

            btn.BackgroundImage = ListPlayers[CurrentPlayer].Symbol;

            StkUndoStep.Push(new PlayInfo(GetButtonCoordinate(btn), CurrentPlayer, btn.BackgroundImage));
            StkRedoStep.Clear();

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();

            if (playerClicked != null)
                playerClicked(this, new BtnClickEvent(GetButtonCoordinate(btn)));

            if (IsEndGame())
            {
                EndGame();
                return;  // Don't continue with AI move if game is over
            }

            if (!(IsAI) && playMode == 3)
                StartAI();

            IsAI = false;
        }

        public void OtherPlayerClicked(Point point)
        {
            Button btn = MatrixPositions[point.Y][point.X];

            if (btn.BackgroundImage != null)
                return; // N?u ô ?ã ???c ?ánh thì ko cho ?ánh l?i

            btn.BackgroundImage = ListPlayers[CurrentPlayer].Symbol;

            StkUndoStep.Push(new PlayInfo(GetButtonCoordinate(btn), CurrentPlayer, btn.BackgroundImage));
            StkRedoStep.Clear();

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();

            if (IsEndGame())
                EndGame();
        }
        #endregion

        #region 1 player
        private long[] ArrAttackScore = new long[7] { 0, 64, 4096, 262144, 16777216, 1073741824, 68719476736 };
        private long[] ArrDefenseScore = new long[7] { 0, 8, 512, 32768, 2097152, 134217728, 8589934592 };

        #region Calculate attack score
        private long AttackHorizontal(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duy?t t? trên xu?ng
            for (int Count = 1; Count < 6 && CurrRow + Count < Constance.nRows; Count++)
            {
                if (MatrixPositions[CurrRow + Count][CurrCol].BackgroundImage == ListPlayers[0].Symbol)
                    ComCells += 1;
                else if (MatrixPositions[CurrRow + Count][CurrCol].BackgroundImage == ListPlayers[1].Symbol)
                {
                    ManCells += 1;
                    break;
                }
                else
                    break;
            }

            // Duy?t t? d??i lên
            for (int Count = 1; Count < 6 && CurrRow - Count >= 0; Count++)
            {
                if (MatrixPositions[CurrRow - Count][CurrCol].BackgroundImage == ListPlayers[0].Symbol)
                    ComCells += 1;
                else if (MatrixPositions[CurrRow - Count][CurrCol].BackgroundImage == ListPlayers[1].Symbol)
                {
                    ManCells += 1;
                    break;
                }
                else
                    break;
            }

            if (ManCells == 2)
                return 0;

            /* N?u ManCells == 1 => b? ch?n 1 ??u => l?y ?i?m phòng ng? t?i v? trí này nh?ng 
            nên c?ng thêm 1 ?? t?ng phòng ng?a cho máy c?nh giác h?n vì ?ã b? ch?n 1 ??u */

            TotalScore -= ArrDefenseScore[ManCells + 1];
            TotalScore += ArrAttackScore[ComCells];

            return TotalScore;
        }

        private long AttackVertical(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duy?t t? trái sang ph?i
            for (int Count = 1; Count < 6 && CurrCol + Count < Constance.nCols; Count++)
            {
                if (MatrixPositions[CurrRow][CurrCol + Count].BackgroundImage == ListPlayers[0].Symbol)
                    ComCells += 1;
                else if (MatrixPositions[CurrRow][CurrCol + Count].BackgroundImage == ListPlayers[1].Symbol)
                {
                    ManCells += 1;
                    break;
                }
                else
                    break;
            }

            // Duy?t t? ph?i sang trái
            for (int Count = 1; Count < 6 && CurrCol - Count >= 0; Count++)
            {
                if (MatrixPositions[CurrRow][CurrCol - Count].BackgroundImage == ListPlayers[0].Symbol)
                    ComCells += 1;
                else if (MatrixPositions[CurrRow][CurrCol - Count].BackgroundImage == ListPlayers[1].Symbol)
                {
                    ManCells += 1;
                    break;
                }
                else
                    break;
            }

            if (ManCells == 2)
                return 0;

            /* N?u ManCells == 1 => b? ch?n 1 ??u => l?y ?i?m phòng ng? t?i v? trí này nh?ng 
            nên c?ng thêm 1 ?? t?ng phòng ng?a cho máy c?nh giác h?n vì ?ã b? ch?n 1 ??u */

            TotalScore -= ArrDefenseScore[ManCells + 1];
            TotalScore += ArrAttackScore[ComCells];

            return TotalScore;
        }

        private long AttackMainDiag(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duy?t trái trên
            for (int Count = 1; Count < 6 && CurrCol + Count < Constance.nCols && CurrRow + Count < Constance.nRows; Count++)
            {
                if (MatrixPositions[CurrRow + Count][CurrCol + Count].BackgroundImage == ListPlayers[0].Symbol)
                    ComCells += 1;
                else if (MatrixPositions[CurrRow + Count][CurrCol + Count].BackgroundImage == ListPlayers[1].Symbol)
                {
                    ManCells += 1;
                    break;
                }
                else
                    break;
            }

            // Duy?t ph?i d??i
            for (int Count = 1; Count < 6 && CurrCol - Count >= 0 && CurrRow - Count >= 0; Count++)
            {
                if (MatrixPositions[CurrRow - Count][CurrCol - Count].BackgroundImage == ListPlayers[0].Symbol)
                    ComCells += 1;
                else if (MatrixPositions[CurrRow - Count][CurrCol - Count].BackgroundImage == ListPlayers[1].Symbol)
                {
                    ManCells += 1;
                    break;
                }
                else
                    break;
            }

            if (ManCells == 2)
                return 0;

            /* N?u ManCells == 1 => b? ch?n 1 ??u => l?y ?i?m phòng ng? t?i v? trí này nh?ng 
            nên c?ng thêm 1 ?? t?ng phòng ng?a cho máy c?nh giác h?n vì ?ã b? ch?n 1 ??u */

            TotalScore -= ArrDefenseScore[ManCells + 1];
            TotalScore += ArrAttackScore[ComCells];

            return TotalScore;
        }

        private long AttackExtraDiag(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duy?t ph?i trên
            for (int Count = 1; Count < 6 && CurrCol + Count < Constance.nCols && CurrRow - Count >= 0; Count++)
            {
                if (MatrixPositions[CurrRow - Count][CurrCol + Count].BackgroundImage == ListPlayers[0].Symbol)
                    ComCells += 1;
                else if (MatrixPositions[CurrRow - Count][CurrCol + Count].BackgroundImage == ListPlayers[1].Symbol)
                {
                    ManCells += 1;
                    break;
                }
                else
                    break;
            }

            // Duy?t trái d??i
            for (int Count = 1; Count < 6 && CurrCol - Count >= 0 && CurrRow + Count < Constance.nRows; Count++)
            {
                if (MatrixPositions[CurrRow + Count][CurrCol - Count].BackgroundImage == ListPlayers[0].Symbol)
                    ComCells += 1;
                else if (MatrixPositions[CurrRow + Count][CurrCol - Count].BackgroundImage == ListPlayers[1].Symbol)
                {
                    ManCells += 1;
                    break;
                }
                else
                    break;
            }

            if (ManCells == 2)
                return 0;

            /* N?u ManCells == 1 => b? ch?n 1 ??u => l?y ?i?m phòng ng? t?i v? trí này nh?ng 
            nên c?ng thêm 1 ?? t?ng phòng ng?a cho máy c?nh giác h?n vì ?ã b? ch?n 1 ??u */

            TotalScore -= ArrDefenseScore[ManCells + 1];
            TotalScore += ArrAttackScore[ComCells];

            return TotalScore;
        }
        #endregion

        #region Calculate defense score
        private long DefenseHorizontal(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duy?t t? trên xu?ng
            for (int Count = 1; Count < 6 && CurrRow + Count < Constance.nRows; Count++)
            {
                if (MatrixPositions[CurrRow + Count][CurrCol].BackgroundImage == ListPlayers[0].Symbol)
                {
                    ComCells += 1;
                    break;
                }
                else if (MatrixPositions[CurrRow + Count][CurrCol].BackgroundImage == ListPlayers[1].Symbol)
                    ManCells += 1;
                else
                    break;
            }

            // Duy?t t? d??i lên
            for (int Count = 1; Count < 6 && CurrRow - Count >= 0; Count++)
            {
                if (MatrixPositions[CurrRow - Count][CurrCol].BackgroundImage == ListPlayers[0].Symbol)
                {
                    ComCells += 1;
                    break;
                }
                else if (MatrixPositions[CurrRow - Count][CurrCol].BackgroundImage == ListPlayers[1].Symbol)
                    ManCells += 1;
                else
                    break;
            }

            if (ComCells == 2)
                return 0;

            TotalScore += ArrDefenseScore[ManCells];

            return TotalScore;
        }

        private long DefenseVertical(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duy?t t? trái sang ph?i
            for (int Count = 1; Count < 6 && CurrCol + Count < Constance.nCols; Count++)
            {
                if (MatrixPositions[CurrRow][CurrCol + Count].BackgroundImage == ListPlayers[0].Symbol)
                {
                    ComCells += 1;
                    break;
                }
                else if (MatrixPositions[CurrRow][CurrCol + Count].BackgroundImage == ListPlayers[1].Symbol)
                    ManCells += 1;
                else
                    break;
            }

            // Duy?t t? ph?i sang trái
            for (int Count = 1; Count < 6 && CurrCol - Count >= 0; Count++)
            {
                if (MatrixPositions[CurrRow][CurrCol - Count].BackgroundImage == ListPlayers[0].Symbol)
                {
                    ComCells += 1;
                    break;
                }
                else if (MatrixPositions[CurrRow][CurrCol - Count].BackgroundImage == ListPlayers[1].Symbol)
                    ManCells += 1;
                else
                    break;
            }

            if (ComCells == 2)
                return 0;

            TotalScore += ArrDefenseScore[ManCells];

            return TotalScore;
        }

        private long DefenseMainDiag(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duy?t trái trên
            for (int Count = 1; Count < 6 && CurrCol + Count < Constance.nCols && CurrRow + Count < Constance.nRows; Count++)
            {
                if (MatrixPositions[CurrRow + Count][CurrCol + Count].BackgroundImage == ListPlayers[0].Symbol)
                {
                    ComCells += 1;
                    break;
                }
                else if (MatrixPositions[CurrRow + Count][CurrCol + Count].BackgroundImage == ListPlayers[1].Symbol)
                    ManCells += 1;
                else
                    break;
            }

            // Duy?t ph?i d??i
            for (int Count = 1; Count < 6 && CurrCol - Count >= 0 && CurrRow - Count >= 0; Count++)
            {
                if (MatrixPositions[CurrRow - Count][CurrCol - Count].BackgroundImage == ListPlayers[0].Symbol)
                {
                    ComCells += 1;
                    break;
                }
                else if (MatrixPositions[CurrRow - Count][CurrCol - Count].BackgroundImage == ListPlayers[1].Symbol)
                    ManCells += 1;
                else
                    break;
            }

            if (ComCells == 2)
                return 0;

            TotalScore += ArrDefenseScore[ManCells];

            return TotalScore;
        }

        private long DefenseExtraDiag(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duy?t ph?i trên
            for (int Count = 1; Count < 6 && CurrCol + Count < Constance.nCols && CurrRow - Count >= 0; Count++)
            {
                if (MatrixPositions[CurrRow - Count][CurrCol + Count].BackgroundImage == ListPlayers[0].Symbol)
                {
                    ComCells += 1;
                    break;
                }
                else if (MatrixPositions[CurrRow - Count][CurrCol + Count].BackgroundImage == ListPlayers[1].Symbol)
                    ManCells += 1;
                else
                    break;
            }

            // Duy?t trái d??i
            for (int Count = 1; Count < 6 && CurrCol - Count >= 0 && CurrRow + Count < Constance.nRows; Count++)
            {
                if (MatrixPositions[CurrRow + Count][CurrCol - Count].BackgroundImage == ListPlayers[0].Symbol)
                {
                    ComCells += 1;
                    break;
                }
                else if (MatrixPositions[CurrRow + Count][CurrCol - Count].BackgroundImage == ListPlayers[1].Symbol)
                    ManCells += 1;
                else
                    break;
            }

            if (ComCells == 2)
                return 0;

            TotalScore += ArrDefenseScore[ManCells];

            return TotalScore;
        }
        #endregion

        private Point FindAiPos()
        {
            Point AiPos = new Point();
            long MaxScore = 0;

            for (int i = 0; i < Constance.nRows; i++)
            {
                for (int j = 0; j < Constance.nCols; j++)
                {
                    if (MatrixPositions[i][j].BackgroundImage == null)
                    {
                        long AttackScore = AttackHorizontal(i, j) + AttackVertical(i, j) + AttackMainDiag(i, j) + AttackExtraDiag(i, j);
                        long DefenseScore = DefenseHorizontal(i, j) + DefenseVertical(i, j) + DefenseMainDiag(i, j) + DefenseExtraDiag(i, j);
                        long TempScore = AttackScore > DefenseScore ? AttackScore : DefenseScore;

                        if (MaxScore < TempScore)
                        {
                            MaxScore = TempScore;
                            AiPos = new Point(i, j);
                        }
                    }
                }
            }

            return AiPos;
        }

        public void StartAI()
        {
            IsAI = true;

            try
            {
                // ??m b?o UI thread ???c ?u tiên tr??c khi ?ánh
                Application.DoEvents();

                Point movePosition;
                Random rand = new Random();
                
                if (StkUndoStep.Count == 0) 
                {
                    // T?o n??c ?i ??u tiên ng?u nhiên thay vì luôn ? g?n gi?a bàn c?

                    // Ph??ng pháp 1: Chia bàn c? thành 9 khu v?c và ch?n ng?u nhiên m?t khu v?c
                    // r?i ch?n m?t ?i?m ng?u nhiên trong khu v?c ?ó
                    int regionWidth = Math.Max(1, Constance.nCols / 3);
                    int regionHeight = Math.Max(1, Constance.nRows / 3);

                    int region = rand.Next(9); // 0-8 (3x3 regions)

                    // Xác ??nh v? trí d?a trên khu v?c ???c ch?n
                    int baseRow = (region / 3) * regionHeight;
                    int baseCol = (region % 3) * regionWidth;
                    
                    // Thêm m?t chút ng?u nhiên th?c s? trong khu v?c
                    int row = Math.Min(Constance.nRows - 1, baseRow + rand.Next(regionHeight));
                    int col = Math.Min(Constance.nCols - 1, baseCol + rand.Next(regionWidth));

                    // ??m b?o v? trí n?m trong gi?i h?n bàn c?
                    row = Math.Max(0, Math.Min(Constance.nRows - 1, row));
                    col = Math.Max(0, Math.Min(Constance.nCols - 1, col));

                    // Ph??ng pháp 2: N?u bàn c? nh?, ch?n ng?u nhiên hoàn toàn
                    if (Constance.nRows <= 5 || Constance.nCols <= 5)
                    {
                        row = rand.Next(Constance.nRows);
                        col = rand.Next(Constance.nCols);
                    }

                    movePosition = new Point(col, row);
                }
                else
                {
                    // Tìm n??c ?i t?t nh?t cho AI
                    Point AiPos = FindAiPos();
                    movePosition = new Point(AiPos.Y, AiPos.X);
                }

                // ??m b?o v? trí h?p l? và ô ch?a ???c ?ánh
                if (movePosition.X >= 0 && movePosition.X < Constance.nCols && 
                    movePosition.Y >= 0 && movePosition.Y < Constance.nRows &&
                    MatrixPositions[movePosition.Y][movePosition.X].BackgroundImage == null)
                {
                    // Tr?c ti?p ??t n??c ?i vào bàn c?
                    Button btn = MatrixPositions[movePosition.Y][movePosition.X];
                    btn.BackgroundImage = ListPlayers[CurrentPlayer].Symbol;

                    StkUndoStep.Push(new PlayInfo(movePosition, CurrentPlayer, btn.BackgroundImage));
                    StkRedoStep.Clear();

                    CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
                    ChangePlayer();

                    // Thông báo r?ng n??c ?i ?ã ???c th?c hi?n
                    if (playerClicked != null)
                        playerClicked(this, new BtnClickEvent(movePosition));

                    // Ki?m tra k?t thúc game
                    if (IsEndGame())
                        EndGame();
                }
                else
                {
                    // N?u v? trí ?ã ch?n không h?p l?, tìm m?t ô tr?ng ng?u nhiên
                    List<Point> emptyPositions = new List<Point>();

                    for (int i = 0; i < Constance.nRows; i++)
                    {
                        for (int j = 0; j < Constance.nCols; j++)
                        {
                            if (MatrixPositions[i][j].BackgroundImage == null)
                            {
                                emptyPositions.Add(new Point(j, i));
                            }
                        }
                    }

                    if (emptyPositions.Count > 0)
                    {
                        // Ch?n m?t v? trí ng?u nhiên t? các ô tr?ng
                        Point randomPos = emptyPositions[rand.Next(emptyPositions.Count)];

                        Button btn = MatrixPositions[randomPos.Y][randomPos.X];
                        btn.BackgroundImage = ListPlayers[CurrentPlayer].Symbol;

                        StkUndoStep.Push(new PlayInfo(randomPos, CurrentPlayer, btn.BackgroundImage));
                        StkRedoStep.Clear();

                        CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
                        ChangePlayer();

                        if (playerClicked != null)
                            playerClicked(this, new BtnClickEvent(randomPos));

                        if (IsEndGame())
                            EndGame();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not perform computer's move: " + ex.Message,
                    "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IsAI = false;
        }
        #endregion
        #endregion
    }
}