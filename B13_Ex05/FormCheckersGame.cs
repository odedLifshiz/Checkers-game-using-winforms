// -----------------------------------------------------------------------
// <copyright file="FormCheckersGame.cs" company="Hewlett-Packard">
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
    public class FormCheckersGame : Form
    {
        private const int k_ButtonSize = 40;
        private const char k_EmptyString = ' ';
        private const char k_Player1RegularPawnSymbol = 'O';
        private const char k_Player2RegularPawnSymbol = 'X';
        private const char k_Player1KingSymbol = 'Q';
        private const char k_Player2KingSymbol = 'K';
        private readonly CheckersData m_CheckersData;
        private readonly int m_BoardSize;
        private Label m_LabelPlayer1 = new Label();
        private Label m_LabelPlayer2 = new Label();
        private Button[,] m_Board;
        private bool m_MouseDown = false;
        private Button m_ButtonStartOfMove;

        public FormCheckersGame(FormInitializeGame i_FormInitializeGame)
        {
            this.Text = "Damka";
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            Player player1 = new Player(
                i_FormInitializeGame.FirstPlayerName,
                ePlayerType.human,
                0,
                eCellType.Player1RegularPawn,
                eCellType.Player1King,
                true);
            Player player2 = new Player(
                i_FormInitializeGame.SecondPlayerName == "[Computer]" ? "Computer"
                : i_FormInitializeGame.SecondPlayerName,
                i_FormInitializeGame.CheckBoxOfPlayer2IsChecked ? ePlayerType.human : ePlayerType.computer,
                0,
                eCellType.Player2RegularPawn,
                eCellType.Player2King,
                false);

            if (i_FormInitializeGame.RadioButtonBoardSize6X6IsChecked)
            {
                m_BoardSize = 6;
            }
            else if (i_FormInitializeGame.RadioButtonBoardSize8X8IsChecked)
            {
                m_BoardSize = 8;
            }
            else
            {
                m_BoardSize = 10;
            }

            m_CheckersData = new CheckersData(m_BoardSize, player1, player2);
            m_CheckersData.GameOverOccured += checkersData_GameOver;
            m_CheckersData.BoardChangeOccured += checkersData_BoardChanged;
            m_Board = new Button[m_BoardSize, m_BoardSize];
            Size = new Size((m_BoardSize * k_ButtonSize) + 100, (m_BoardSize * k_ButtonSize) + 100);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            initializeControls();
            displayBoard();
        }

        private void initializeControls()
        {
            // initialize labels
            m_LabelPlayer1.Size = new Size(100, 20);
            m_LabelPlayer1.Text = m_CheckersData.Player1.Name + ": " + m_CheckersData.Player1.Score;
            m_LabelPlayer1.Location = new Point(50 + k_ButtonSize, 20);
            m_LabelPlayer2.Size = new Size(100, 20);
            m_LabelPlayer2.Text = m_CheckersData.Player2.Name + ": " + m_CheckersData.Player2.Score;
            m_LabelPlayer2.Location = new Point(50 + ((m_BoardSize - 2) * k_ButtonSize), m_LabelPlayer1.Height);
            this.Controls.AddRange(
               new Control[]
               { 
                 m_LabelPlayer1, 
                 m_LabelPlayer2
               });

            updatePlayersLabel();

            // initilize board
            int startX = 50;
            int startY = m_LabelPlayer1.Height + 30;
            int currentX;
            int currentY;

            for (int row = 0; row < m_BoardSize; row++)
            {
                for (int col = 0; col < m_BoardSize; col++)
                {
                    m_Board[row, col] = new Button();
                    m_Board[row, col].Size = new Size(k_ButtonSize, k_ButtonSize);
                    currentX = startX + (row * k_ButtonSize);
                    currentY = startY + (col * k_ButtonSize);
                    m_Board[row, col].Location = new Point(currentX, currentY);
                    if (((row + col) % 2) == 0)
                    {
                        m_Board[row, col].Enabled = false;
                        m_Board[row, col].BackColor = Color.Gray;
                    }

                    this.Controls.Add(m_Board[row, col]);
                    m_Board[row, col].Click += new EventHandler(m_Button_Click);
                }
            }
        }

        private void updatePlayersLabel()
        {
            m_LabelPlayer1.Text = m_CheckersData.Player1.Name + ": " + m_CheckersData.Player1.Score;
            m_LabelPlayer2.Text = m_CheckersData.Player2.Name + ": " + m_CheckersData.Player2.Score;
        }

        // displays the board, only used at initialization of the board
        private void displayBoard()
        {
            for (int row = 0; row < m_BoardSize; row++)
            {
                for (int col = 0; col < m_BoardSize; col++)
                {
                    m_Board[col, row].Text = getStringRepresentationOfCellType(m_CheckersData.Board[row, col]);
                }
            }
        }

        private string getStringRepresentationOfCellType(eCellType i_CellType)
        {
            string result = null;

            switch (i_CellType)
            {
                case eCellType.EMPTY:
                   result = string.Empty;
                    break;
                case eCellType.Player1King:
                   result = k_Player1KingSymbol.ToString();
                    break;
                case eCellType.Player1RegularPawn:
                   result = k_Player1RegularPawnSymbol.ToString();
                    break;
                case eCellType.Player2King:
                   result = k_Player2KingSymbol.ToString();
                    break;
                case eCellType.Player2RegularPawn:
                   result = k_Player2RegularPawnSymbol.ToString();
                    break;
                default: break;          
            }

            return result;
        }

        private void m_Button_Click(object sender, EventArgs e)
        {
            if (!m_MouseDown)
            {
                m_ButtonStartOfMove = sender as Button;
                handleStartOfMove(m_ButtonStartOfMove);
            }
            else
            {
                Button endOfMove = sender as Button;
                handleEndOfMove(m_ButtonStartOfMove, endOfMove);

                // handle a computer move
                if (!m_CheckersData.Player1Turn &&
                    m_CheckersData.Player2.TypeOfPlayer == ePlayerType.computer)
                {
                    m_CheckersData.DoComputerTurn();
                }
            }
        }

        private void handleStartOfMove(Button i_ButtonClicked)
        {
            // get the index in the button array of the button that was pressed
            int colOfButtonClicked = (i_ButtonClicked.Left - m_Board[0, 0].Location.X) / k_ButtonSize;
            int rowOfButtonClicked = (i_ButtonClicked.Location.Y - m_Board[0, 0].Location.Y) / k_ButtonSize;
            bool validStartButton = false;

            // check if the cell in the board contains a pawn that that belongs to
            // the player that acts now
            if (m_CheckersData.Player1Turn)
            {
                if (m_CheckersData.Board[rowOfButtonClicked, colOfButtonClicked] == eCellType.Player1RegularPawn ||
                    m_CheckersData.Board[rowOfButtonClicked, colOfButtonClicked] == eCellType.Player1King)
                {
                    validStartButton = true;
                }
            }
            else
            {
                if (m_CheckersData.Board[rowOfButtonClicked, colOfButtonClicked] == eCellType.Player2RegularPawn ||
                    m_CheckersData.Board[rowOfButtonClicked, colOfButtonClicked] == eCellType.Player2King)
                {
                    validStartButton = true;
                }
            }

            if (validStartButton)
            {
                i_ButtonClicked.BackColor = Color.Blue;
                m_ButtonStartOfMove = i_ButtonClicked;
                m_MouseDown = true;
            }
        }

        private void handleEndOfMove(Button i_ButtonStartOfMove, Button i_ButtonClicked)
        {
            int colOfStartButton = (i_ButtonStartOfMove.Left - m_Board[0, 0].Location.X) / k_ButtonSize;
            int rowOfStartButton = (i_ButtonStartOfMove.Location.Y - m_Board[0, 0].Location.Y) / k_ButtonSize;
            int colOfButtonClicked = (i_ButtonClicked.Left - m_Board[0, 0].Location.X) / k_ButtonSize;
            int rowOfButtonClicked = (i_ButtonClicked.Location.Y - m_Board[0, 0].Location.Y) / k_ButtonSize;
            bool theClickedButtonIsTheSameAsStartButtom = i_ButtonStartOfMove.Equals(i_ButtonClicked);
            if (theClickedButtonIsTheSameAsStartButtom)
            {
                i_ButtonStartOfMove.BackColor = Color.White;
                m_MouseDown = false;
            }
            else
            {
                CheckersMove move = new CheckersMove(
                rowOfStartButton,
                colOfStartButton,
                rowOfButtonClicked,
                colOfButtonClicked);
                eMoveStatusCode moveStatusCode =
                    m_CheckersData.CheckIfMoveIsValid(move);

                switch (moveStatusCode)
                {
                    case eMoveStatusCode.InvalidCoordinates:
                        showTryAgainMessage("Invalid move");
                        break;
                    case eMoveStatusCode.MustJump:
                        showTryAgainMessage("Must jump");
                        break;
                    case eMoveStatusCode.Successful:
                        m_CheckersData.DoMakeMove(move);
                        break;
                    default: break;
                }

                i_ButtonStartOfMove.BackColor = Color.White;
                m_MouseDown = false;
            }
        }

        private void showTryAgainMessage(string i_Message)
        {
            MessageBox.Show(
                     "Try again",
                      i_Message,
                     MessageBoxButtons.OK);
        }

        // handle a board change event - is called by the logic class when a change occurs.
        private void checkersData_BoardChanged(object sender, BoardChangeEventArgs e)
        {
            m_Board[e.CheckersMove.FromCol, e.CheckersMove.FromRow].Text = k_EmptyString.ToString();
            m_Board[e.CheckersMove.ToCol, e.CheckersMove.ToRow].Text = getStringRepresentationOfCellType(e.DestCellType);
            if (e.IsJump)
            {
                m_Board[e.ColOfPawnRemoved, e.RowOfPawnRemoved].Text = k_EmptyString.ToString();
            }
        }

        // handle a end of game event - is called by the logic class when end of game is reached
        private void checkersData_GameOver(object sender, GameOverEventArgs e)
        {
            bool exitGame = false;
            m_Board[e.LastMove.FromCol, e.LastMove.FromRow].BackColor = Color.White;
            switch (e.GameOverStatusCode)
            {
                case eGameOverStatusCode.Draw:
                    exitGame = showMessage("Tie");
                    break;
                case eGameOverStatusCode.Player1Won:
                    exitGame = showMessage(m_CheckersData.Player1.Name + " Won!");
                    break;
                case eGameOverStatusCode.Player2Won:
                    exitGame = showMessage(m_CheckersData.Player2.Name + " Won!");
                    break;
                default: break;
            }

            if (exitGame)
            {
                this.Close();
            }
            else
            {
                m_MouseDown = false;
                updatePlayersLabel();
                displayBoard();
            }
        }

        private bool showMessage(string i_Message)
        {
            string message = string.Format("{0}{1}Another Round?", i_Message, Environment.NewLine);
            return MessageBox.Show(
            message,
               "Damka",
           MessageBoxButtons.YesNo) == DialogResult.No;
        }
    }
}
