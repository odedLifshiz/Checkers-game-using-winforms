// -----------------------------------------------------------------------
// <copyright file="CheckersGameStarter.cs" company="Hewlett-Packard">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace B13_Ex05
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;
    using System.Drawing;
    using CheckersLogic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    /// 
    public class CheckersGameStarter
    {
        public static void RunGame()
        {
            FormInitializeGame formInitializeGame = new FormInitializeGame();

            if (formInitializeGame.ShowDialog() == DialogResult.OK)
            {
                // if the initial parameters are not ok show a message
                if (formInitializeGame.FirstPlayerName.Length == 0 || (formInitializeGame.CheckBoxOfPlayer2IsChecked
                    && formInitializeGame.SecondPlayerName.Length == 0))
                {
                    if (MessageBox.Show(
                        "Invalid Parameters",
                        "Please enter Parameters Again",
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        RunGame();
                    }
                }
                else
                {
                    FormCheckersGame formCheckersGame = new FormCheckersGame(formInitializeGame);
                    formCheckersGame.ShowDialog();
                }
            }
        }
    }
}
