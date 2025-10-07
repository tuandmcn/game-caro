using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game_Caro
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
        
        // Added properties for configurable board size
        private int boardSize = 10;  // Default size 10x10
        private int cellsToWin = 5;  // Default 5 in a row to win

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
            board.Enabled = true;
            board.Controls.Clear();

            StkUndoStep = new Stack<PlayInfo>();
            StkRedoStep = new Stack<PlayInfo>();

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
            if (StkRedoStep.Count <= 0)
                return false;

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

            // Xét chặn 2 đầu - Check if win condition is valid (not blocked on both sides)
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

            // Xét chặn 2 đầu - Check if win condition is valid (not blocked on both sides)
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

            // Xét chặn 2 đầu - Check if win condition is valid (not blocked on both sides)
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

            // Xét chặn 2 đầu - Check if win condition is valid (not blocked on both sides)
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
                MessageBox.Show("Hòa cờ !!!");
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

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn.BackgroundImage != null)
                return; // Nếu ô đã được đánh thì ko cho đánh lại

            btn.BackgroundImage = ListPlayers[CurrentPlayer].Symbol;
           
            StkUndoStep.Push(new PlayInfo(GetButtonCoordinate(btn), CurrentPlayer, btn.BackgroundImage));
            StkRedoStep.Clear();

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();

            if (playerClicked != null)
                playerClicked(this, new BtnClickEvent(GetButtonCoordinate(btn)));

            if (IsEndGame())
                EndGame();

            if (!(IsAI) && playMode == 3)
                StartAI();

            IsAI = false;
        }

        public void OtherPlayerClicked(Point point)
        {
            Button btn = MatrixPositions[point.Y][point.X];

            if (btn.BackgroundImage != null)
                return; // Nếu ô đã được đánh thì ko cho đánh lại

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

            // Duyệt từ trên xuống
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

            // Duyệt từ dưới lên
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

            /* Nếu ManCells == 1 => bị chặn 1 đầu => lấy điểm phòng ngự tại vị trí này nhưng 
            nên cộng thêm 1 để tăng phòng ngự cho máy cảnh giác hơn vì đã bị chặn 1 đầu */

            TotalScore -= ArrDefenseScore[ManCells + 1];
            TotalScore += ArrAttackScore[ComCells];

            return TotalScore;
        }

        private long AttackVertical(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duyệt từ trái sang phải
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

            // Duyệt từ phải sang trái
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

            /* Nếu ManCells == 1 => bị chặn 1 đầu => lấy điểm phòng ngự tại vị trí này nhưng 
            nên cộng thêm 1 để tăng phòng ngự cho máy cảnh giác hơn vì đã bị chặn 1 đầu */

            TotalScore -= ArrDefenseScore[ManCells + 1];
            TotalScore += ArrAttackScore[ComCells];

            return TotalScore;
        }

        private long AttackMainDiag(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duyệt trái trên
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

            // Duyệt phải dưới
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

            /* Nếu ManCells == 1 => bị chặn 1 đầu => lấy điểm phòng ngự tại vị trí này nhưng 
            nên cộng thêm 1 để tăng phòng ngự cho máy cảnh giác hơn vì đã bị chặn 1 đầu */

            TotalScore -= ArrDefenseScore[ManCells + 1];
            TotalScore += ArrAttackScore[ComCells];

            return TotalScore;
        }

        private long AttackExtraDiag(int CurrRow, int CurrCol)
        {
            long TotalScore = 0;
            int ComCells = 0;
            int ManCells = 0;

            // Duyệt phải trên
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

            // Duyệt trái dưới
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

            /* Nếu ManCells == 1 => bị chặn 1 đầu => lấy điểm phòng ngự tại vị trí này nhưng 
            nên cộng thêm 1 để tăng phòng ngự cho máy cảnh giác hơn vì đã bị chặn 1 đầu */

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

            // Duyệt từ trên xuống
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

            // Duyệt từ dưới lên
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

            // Duyệt từ trái sang phải
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

            // Duyệt từ phải sang trái
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

            // Duyệt trái trên
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

            // Duyệt phải dưới
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

            // Duyệt phải trên
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

            // Duyệt trái dưới
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

            if (StkUndoStep.Count == 0) // mới bắt đầu thì cho đánh giữa bàn cờ
                MatrixPositions[Constance.nRows / 4][Constance.nCols / 4].PerformClick();
            else
            {
                Point AiPos = FindAiPos();
                MatrixPositions[AiPos.X][AiPos.Y].PerformClick();
            }
        }
        #endregion
        #endregion
    }
}