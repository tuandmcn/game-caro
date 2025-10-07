# Final Project of Develop Applications from Computer Course

## Login Form Features:
- Game mode selection:
  - LAN multiplayer (Host or Join)
  - Same PC (two players on the same computer)
  - Play against AI
- Player name input
- IP address input for LAN connections
- Board size configuration (3x3 to 20x20)

### Implementation Logic:
The login form (`LoginForm.cs`) serves as the entry point for the game, allowing users to configure their gameplay experience before the main game window opens:

1. **Game Mode Selection:**
   - Three distinct game modes are implemented as separate buttons:
     - `btn_CreateGame`/`btn_JoinGame`: Sets `GameMode = 1` for LAN play, with `IsHost` flag determining server/client role
     - `btn_SamePC`: Sets `GameMode = 2` for local multiplayer on the same computer
     - `btn_PlayAI`: Sets `GameMode = 3` for playing against computer AI
   - Each button click handler validates inputs and sets appropriate properties before closing the form with `DialogResult.OK`

2. **Input Validation:**
   - `ValidateInputs()` method checks for:
     - Non-empty player name (required for all modes)
     - Valid IP address (required only for LAN modes)
     - Board size within acceptable range (3-20)
   - Displays informative error messages for invalid inputs

3. **Data Flow:**
   - Form properties (PlayerName, IPAddress, BoardSize, IsHost, GameMode) are passed to the main GameCaro form
   - The GameCaro constructor uses these values to:
     - Configure the board size with `board.SetBoardSize(BoardSize)`
     - Set up appropriate network connections for LAN mode
     - Initialize the correct game logic based on the selected mode via `SetupLanGame()`, `SetupSamePcGame()`, or `SetupAiGame()`

4. **Board Size Configuration:**
   - `numBoardSize` numeric up/down control allows selecting board dimensions
   - Selected value is stored in `BoardSize` property and passed to the main game
   - The game dynamically adjusts its visual representation and win conditions based on this value


## Game Settings:
- Adjustable board size through Options > Settings menu
- Board size can be changed before starting a new game
- Win condition adjusts automatically based on board size
- Dynamic resizing of game panel based on board dimensions

### Implementation Logic:
The game settings system (`SettingsForm.cs` and related code in `GameCaro.cs`) provides a flexible way to configure the game board during gameplay:

1. **Settings Dialog Architecture:**
   - `SettingsForm` class implements a modal dialog with board size configuration
   - Uses custom event pattern with `BoardSizeChangedEventArgs` to communicate changes back to the main form
   - Constructor receives current board size to maintain state consistency: `public SettingsForm(int currentBoardSize)`

2. **Change Detection & Notification:**
   - Event-based pattern notifies the main form when settings are changed:
     ```csharp
     public event EventHandler<BoardSizeChangedEventArgs> BoardSizeChanged;
     ```
   - Changes are only communicated when "Apply" is clicked and values are actually modified:
     ```csharp
     if (newBoardSize != currentBoardSize)
     {
         BoardSizeChanged?.Invoke(this, new BoardSizeChangedEventArgs(newBoardSize));
     }
     ```

3. **Dynamic Win Condition Rules:**
   - Win conditions automatically adjust based on board size:
     ```csharp
     int cellsToWin = (boardSize <= 5) ? 3 : 5;
     ```
   - Small boards (≤ 5×5) require 3-in-a-row to win
   - Larger boards require 5-in-a-row to win
   - UI feedback shows current win condition via `lblBoardSizeInfo.Text`

4. **Safe Change Application:**
   - Changes are only allowed at safe points in gameplay to prevent disruption:
     ```csharp
     if (pn_GameBoard.Enabled == false || board.StkUndoStep == null || board.StkUndoStep.Count == 0)
     ```
   - Settings can only be changed when:
     - Game has ended
     - No game has started yet
     - A new game was started but no moves made yet
   - User receives clear feedback when settings cannot be changed

5. **Visual Adaptation:**
   - `AdjustBoardPanelSize()` method recalculates UI layout based on new board dimensions:
     ```csharp
     int panelWidth = BoardSize * Constance.CellWidth;
     int panelHeight = BoardSize * Constance.CellHeight;
     ```
   - Both game panel and form resize to accommodate new board dimensions
   - Title bar updates to reflect current board size and win conditions:
     ```csharp
     this.Text = $"Game Caro - Bàn cờ {BoardSize}x{BoardSize} - Thắng {board.CellsToWin} ô liên tiếp";
     ```


## Chat System:
- Real-time chat available only in LAN multiplayer mode
- Chat feature automatically disabled in Same PC and AI game modes
- Visual indicators inform players when chat is not available

### Implementation Logic:
The chat system provides real-time communication between players in LAN mode while being intelligently disabled in other game modes:

1. **Mode-Based Availability Management:**
   - Chat functionality is conditionally enabled based on game mode using the `SetChatAvailability()` method:
     ```csharp
     private void SetChatAvailability(int gameMode)
     {
         bool isChatAvailable = gameMode == 1; // Only available in LAN mode
         txt_Message.Enabled = isChatAvailable;
         btn_Send.Enabled = isChatAvailable;
         txt_Chat.Enabled = isChatAvailable;
         
         if (!isChatAvailable)
         {
             txt_Chat.Text = "Tính năng chat không khả dụng trong chế độ này.";
         }
     }
     ```
   - This method is called during form initialization to configure UI components appropriately

2. **Network Message Protocol:**
   - Chat messages are transmitted using the same socket infrastructure as game moves
   - Messages are sent as `SocketData` objects with the `SEND_MESSAGE` command type:
     ```csharp
     socket.Send(new SocketData((int)SocketCommand.SEND_MESSAGE, txt_Chat.Text, new Point()));
     ```
   - The complete chat history is sent rather than individual messages to ensure consistency

3. **UI Integration & Threading:**
   - Message sending is handled by the `Btn_Send_Click` event handler:
     ```csharp
     private void Btn_Send_Click(object sender, EventArgs e)
     {
         if (board.PlayMode != 1)
         {
             MessageBox.Show("Tính năng không khả dụng khi chơi với máy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
             return;
         }
         // Message sending logic...
     }
     ```
   - Each sent message includes player name for identification: `"- " + PlayerName + ": " + txt_Message.Text`
   - Additional validation prevents sending empty messages with `string.IsNullOrWhiteSpace(txt_Message.Text)`

4. **Asynchronous Message Reception:**
   - Messages from the opponent are processed in the `ProcessData()` method's switch statement:
     ```csharp
     case (int)SocketCommand.SEND_MESSAGE:
         txt_Chat.Text = data.Message;
         break;
     ```
   - This occurs within a background thread created by the `Listen()` method
   - Thread safety is ensured by setting `Control.CheckForIllegalCrossThreadCalls = false`

5. **Graceful Degradation:**
   - Feedback is provided when attempting to use chat in non-LAN modes
   - Clear visual indication shows when chat is disabled
   - Error handling prevents crashes when network connectivity issues occur


