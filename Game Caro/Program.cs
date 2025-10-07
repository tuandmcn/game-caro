using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Caro
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Show login form first
            LoginForm loginForm = new LoginForm();
            DialogResult result = loginForm.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                // If login was successful, start the main game form
                GameCaro gameCaro = new GameCaro(loginForm.PlayerName, loginForm.IPAddress, loginForm.IsHost, loginForm.GameMode, loginForm.BoardSize);
                Application.Run(gameCaro);
            }
        }
    }
}
