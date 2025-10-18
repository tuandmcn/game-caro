using System;
using System.Drawing;
using System.Windows.Forms;

namespace TicTacToe
{
    // Extension ?? thêm ph??ng th?c SetFirstPlayer cho GameBoard
    partial class GameBoard
    {
        // Ph??ng th?c ?? thi?t l?p ng??i ch?i nào ?i tr??c trong LAN mode
        public void SetFirstPlayer(bool hostGoesFirst, bool isHost)
        {
            // Trong LAN mode:
            // - Player 0 luôn là Host
            // - Player 1 luôn là Client
            // - CurrentPlayer xác ??nh ai ?i ti?p theo
            // - Ng??i ?i tr??c luôn ?ánh X, ng??i ?i sau ?ánh O
            
            // Load symbols
            Image xSymbol = Image.FromFile(Application.StartupPath + "\\images\\X.png");
            Image oSymbol = Image.FromFile(Application.StartupPath + "\\images\\O.png");
            
            if (hostGoesFirst)
            {
                // Host ?i tr??c => Host ?ánh X, Client ?ánh O
                ListPlayers[0].Symbol = xSymbol; // Host = X
                ListPlayers[1].Symbol = oSymbol; // Client = O
                CurrentPlayer = 0; // Host ?i tr??c
            }
            else
            {
                // Client ?i tr??c => Client ?ánh X, Host ?ánh O
                ListPlayers[0].Symbol = oSymbol; // Host = O
                ListPlayers[1].Symbol = xSymbol; // Client = X
                CurrentPlayer = 1; // Client ?i tr??c
            }
            
            // C?p nh?t UI hi?n th? ng??i ch?i hi?n t?i
            RefreshCurrentPlayerUI();
        }
        
        // Ph??ng th?c ?? reset symbols v? m?c ??nh
        public void ResetSymbols()
        {
            // Reset v? m?c ??nh: Player 0 = X, Player 1 = O
            ListPlayers[0].Symbol = Image.FromFile(Application.StartupPath + "\\images\\X.png");
            ListPlayers[1].Symbol = Image.FromFile(Application.StartupPath + "\\images\\O.png");
        }
    }
}
