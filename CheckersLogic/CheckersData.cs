// -----------------------------------------------------------------------
// <copyright file="CheckersData.cs" company="Hewlett-Packard">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CheckersLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Threading;

    // an object of this class contains the board and all of the logic behind the checkers game
    public enum eCellType
    {
        [Description(" ")]
        EMPTY,
        [Description("O")]
        Player1RegularPawn,
        [Description("Q")]
        Player1King,
        [Description("X")]
        Player2RegularPawn,
        [Description("K")]
        Player2King,
    }

    // values that represent each move status code
    public enum eMoveStatusCode
    {
        Successful,
        MustJump,
        InvalidCoordinates
    }

       public enum eGameOverStatusCode
    {
        Player1Won,
        Player2Won,
        Draw
    }

    public delegate void BoardChangeEventHandler(object sender, BoardChangeEventArgs e);

    public delegate void GameOverEventHandler(object sender, GameOverEventArgs e);

    public class CheckersData
    {
        private readonly eCellType[,] r_Board;
        private readonly int r_BoardSize;
        private readonly Player r_Player1;
        private readonly Player r_Player2;
        private bool m_Player1Turn;

        public event GameOverEventHandler m_GameOver;

        public event BoardChangeEventHandler m_BoardChangeOccured;
       
        public CheckersData(int i_BoardSize, Player i_Player1, Player i_Player2)
        {
            r_BoardSize = i_BoardSize;
            r_Board = new eCellType[r_BoardSize, r_BoardSize];
            r_Player1 = i_Player1;
            r_Player2 = i_Player2;
            m_Player1Turn = true;
            initializeBoard();
        }

        public GameOverEventHandler GameOverDelegate
        {
            get 
            { 
                return m_GameOver;
            }

            set 
            { 
                m_GameOver = value;
            }
        }

        public bool Player1Turn
        {
            get { return m_Player1Turn; }
        }

        public eCellType[,] Board
        {
            get
            {
                return this.r_Board;
            }
        }

        public Player Player1
        {
            get { return r_Player1; }
        }

        public Player Player2
        {
            get 
            { 
                return r_Player2;
            }
        }

        public BoardChangeEventHandler BoardChangeOccured
        {
            get 
            { 
                return m_BoardChangeOccured;
            }

            set 
            { 
                m_BoardChangeOccured = value;
            }
        }

        public GameOverEventHandler GameOverOccured
        {
            get
            {
                return m_GameOver;
            }

            set
            {
                m_GameOver = value;
            }
        }

        // initializes the cells of the board to their starting values
        private void initializeBoard()
        {
            int numberOfRowsOfSoldiersForEachPlayer = (r_BoardSize / 2) - 1;
            for (int row = 0; row < r_BoardSize; row++)
            {
                for (int col = 0; col < r_BoardSize; col++)
                {
                    // if this is not an empty cell
                    if (!(((row + col) % 2) == 0))
                    {
                        if (row < numberOfRowsOfSoldiersForEachPlayer)
                        {
                            r_Board[row, col] = eCellType.Player1RegularPawn;
                        }
                        else if (row > r_BoardSize - numberOfRowsOfSoldiersForEachPlayer - 1)
                        {
                            r_Board[row, col] = eCellType.Player2RegularPawn;
                        }
                        else
                        {
                            r_Board[row, col] = eCellType.EMPTY;
                        }
                    }
                    else
                    {
                        r_Board[row, col] = eCellType.EMPTY;
                    }
                }
            }
        }

        /**
        * Return a list containing all the legal CheckersMoves
        * for the specified player on the current board.  If the player
        * has no legal moves, null is returned. 
         * If the returned value is non-null, it consists
        * entirely of jump moves or entirely of regular moves, since
        * if the player can jump, the only legal moves are jumps.
        */
        public List<CheckersMove> GetLegalMoves(Player i_Player)
        {
            eCellType playerPawnSybol = i_Player.PawnSymbol;
            eCellType playerKingSymbol = i_Player.KingSymbol;
            List<CheckersMove> listOfMoves = new List<CheckersMove>();
            
            listOfMoves = getLegalJumps(i_Player);
            bool playerCanMakeAJump = listOfMoves.Count > 0;

            /*  If any jump moves were found, then the user must jump, so we don't 
            add any regular moves. However, if no jumps were found, check for
            any legal regular moves.  Look at each square on the board.
            If that square contains one of the player's pawns, look at a possible
            move in each of the four directions from that square.  If there is 
            a legal move in that direction, put it in the moves list.
            */
            if (!playerCanMakeAJump)
            {
                listOfMoves = getValidMovesThatAreNotJumps(i_Player);
            }

            // If no legal moves have been found, return null.  Otherwise, return the list of moves
            if (listOfMoves.Count() == 0)
            {
                listOfMoves = null;
            }
           
            return listOfMoves;
        }
         
        // return all valid jumps for player
        private List<CheckersMove> getLegalJumps(Player i_Player)
        {
            eCellType playerPawnSybol = i_Player.PawnSymbol;
            eCellType playerKingSymbol = i_Player.KingSymbol;
            List<CheckersMove> listOfJumps = new List<CheckersMove>();

           /*check for any possible jumps.  Look at each square on the board.
           If that square contains one of the player's pieces, look at a possible
           jump in each of the four directions from that square.  If there is 
           a legal jump in that direction, put it in the moves list.
           */
            for (int row = 0; row < r_BoardSize; row++)
            {
                for (int col = 0; col < r_BoardSize; col++)
                {
                    // check for all 4 possible jumps
                    if (r_Board[row, col] == i_Player.PawnSymbol || r_Board[row, col] == i_Player.KingSymbol)
                    {
                        if (canJump(i_Player, row, col, row + 1, col + 1, row + 2, col + 2))
                        {
                            listOfJumps.Add(new CheckersMove(row, col, row + 2, col + 2));
                        }

                        if (canJump(i_Player, row, col, row + 1, col - 1, row + 2, col - 2))
                        {
                            listOfJumps.Add(new CheckersMove(row, col, row + 2, col - 2));
                        }

                        if (canJump(i_Player, row, col, row - 1, col - 1, row - 2, col - 2))
                        {
                            listOfJumps.Add(new CheckersMove(row, col, row - 2, col - 2));
                        }

                        if (canJump(i_Player, row, col, row - 1, col + 1, row - 2, col + 2))
                        {
                            listOfJumps.Add(new CheckersMove(row, col, row - 2, col + 2));
                        }
                    }
                }
            }

            return listOfJumps;
        }

        // return all valid moves that are not jumps for player
        private List<CheckersMove> getValidMovesThatAreNotJumps(Player i_Player)
        {
            eCellType playerPawnSybol = i_Player.PawnSymbol;
            eCellType playerKingSymbol = i_Player.KingSymbol;
            List<CheckersMove> listOfMoves = new List<CheckersMove>();

            for (int row = 0; row < r_BoardSize; row++)
            {
                for (int col = 0; col < r_BoardSize; col++)
                {
                    // check 4 possible moves
                    if (r_Board[row, col] == playerPawnSybol || r_Board[row, col] == playerKingSymbol)
                    {
                        if (checkRegularMove(i_Player, row, col, row + 1, col - 1))
                        {
                            listOfMoves.Add(new CheckersMove(row, col, row + 1, col - 1));
                        }

                        if (checkRegularMove(i_Player, row, col, row - 1, col - 1))
                        {
                            listOfMoves.Add(new CheckersMove(row, col, row - 1, col - 1));
                        }

                        if (checkRegularMove(i_Player, row, col, row + 1, col + 1))
                        {
                            listOfMoves.Add(new CheckersMove(row, col, row + 1, col + 1));
                        }

                        if (checkRegularMove(i_Player, row, col, row - 1, col + 1))
                        {
                            listOfMoves.Add(new CheckersMove(row, col, row - 1, col + 1));
                        }
                    }
                }
            }

            return listOfMoves;
        }
        
        /**    
        * This method checks if a move can be made from r1,c1 to r3,c3
        * it is assumed that the player has a pawn in r1 c1. 
        */
        private bool checkRegularMove(Player i_Player, int i_R1, int i_C1, int i_R2, int i_C2)
        {
            bool isValidMove = true;

            if (i_R2 < 0 || i_R2 >= r_BoardSize || i_C2 < 0 || i_C2 >= r_BoardSize)
            {
                isValidMove = false;
            }
            else if (Math.Abs(i_R1 - i_R2) != 1 || Math.Abs(i_C1 - i_C2) != 1)
            {
                isValidMove = false;
            }
            else if (r_Board[i_R2, i_C2] != eCellType.EMPTY)
            {
                isValidMove = false;
            }
            else if (r_Board[i_R1, i_C1] == eCellType.Player1RegularPawn && i_R1 >= i_R2)
            {
                isValidMove = false;
            }
            else if (r_Board[i_R1, i_C1] == eCellType.Player2RegularPawn && i_R1 <= i_R2)
            {
                isValidMove = false;
            }

            return isValidMove;
        }

        private bool isJump(CheckersMove i_Move)
        {
            // Test whether this move is a jump.  It is assumed that
            // the move is legal.  In a jump, the piece moves two
            // rows.  (In a regular move, it only moves one row.)
            return isJump(i_Move.FromRow, i_Move.FromCol, i_Move.ToRow, i_Move.ToCol);
        }

        private bool isJump(int i_FromRow, int i_FromCol, int i_ToRow, int i_ToCol)
        {
            bool isJump = (Math.Abs(i_FromRow - i_ToRow) == 2) && (Math.Abs(i_FromCol - i_ToCol) == 2);

            return isJump;
        }

        /**    
        * This method checks if a jump can be made from r1,c1 over r2,c2 to r3,c3
        * it is assumed that the player has a pawn in r1 c1. 
        */
        private bool canJump(Player i_Player, int i_FromRow, int i_FromCol, int i_MiddleRow, int i_MiddleCol, int i_ToRow, int i_ToCol)
        {
            bool canJump = true;

            if (i_ToRow < 0 || i_ToRow >= r_BoardSize || i_ToCol < 0 || i_ToCol >= r_BoardSize)
            {
                canJump = false;
            }
            else if (r_Board[i_ToRow, i_ToCol] != eCellType.EMPTY)
            {
                canJump = false;
            }
            else if (r_Board[i_MiddleRow, i_MiddleCol] == eCellType.EMPTY)
            {
                canJump = false;
            }
            else if ((r_Board[i_FromRow, i_FromCol] == eCellType.Player1RegularPawn || r_Board[i_FromRow, i_FromCol] == eCellType.Player1King)
                && (r_Board[i_MiddleRow, i_MiddleCol] == eCellType.Player1RegularPawn || r_Board[i_MiddleRow, i_MiddleCol] == eCellType.Player1King))
            {
                canJump = false;
            }
            else if ((r_Board[i_FromRow, i_FromCol] == eCellType.Player2RegularPawn || r_Board[i_FromRow, i_FromCol] == eCellType.Player2King)
                && (r_Board[i_MiddleRow, i_MiddleCol] == eCellType.Player2RegularPawn || r_Board[i_MiddleRow, i_MiddleCol] == eCellType.Player2King))
            {
                canJump = false;
            }
            else if ((r_Board[i_FromRow, i_FromCol] == eCellType.Player1RegularPawn) && (i_ToRow < i_FromRow))
            {
                canJump = false;
            }
            else if ((r_Board[i_FromRow, i_FromCol] == eCellType.Player1RegularPawn) && (i_ToRow < i_FromRow))
            {
                canJump = false;
            }
            else if ((r_Board[i_FromRow, i_FromCol] == eCellType.Player2RegularPawn) && (i_ToRow > i_FromRow))
            {
                canJump = false;
            }

            return canJump;
        }

        // Check if a move is valid given all the possible moves
        // nothing is assumed on the validity of the move
        // this method uses canJump and the checkRegularMove methods, both methods assume we have the move 
        // source row and col has a pawn, we check that condition and than use them 
        // to check the move.
        // an enum is returned to represent the validity of the move
        public eMoveStatusCode CheckIfMoveIsValid(CheckersMove i_Move)
        {
            Player playerThatActsNow = getPlayerThatActsNow();
            List<CheckersMove> listOfMoves = GetLegalMoves(playerThatActsNow);
            eMoveStatusCode moveStatusCode = eMoveStatusCode.Successful;

            // check the source row and col coordinates are on the board
            if (i_Move.FromRow < 0 || i_Move.FromRow >= r_BoardSize || i_Move.FromCol < 0 || i_Move.FromCol >= r_BoardSize)
            {
                moveStatusCode = eMoveStatusCode.InvalidCoordinates;
            }
            else if (r_Board[i_Move.FromRow, i_Move.FromCol] != playerThatActsNow.PawnSymbol && r_Board[i_Move.FromRow, i_Move.FromCol] != playerThatActsNow.KingSymbol)
            {
                moveStatusCode = eMoveStatusCode.InvalidCoordinates;
            }
            else if (isJump(listOfMoves[0]) && !isJump(i_Move.FromRow, i_Move.FromCol, i_Move.ToRow, i_Move.ToCol))
            {
                moveStatusCode = eMoveStatusCode.MustJump;
            }
            else if (isJump(i_Move.FromRow, i_Move.FromCol, i_Move.ToRow, i_Move.ToCol))
            {
                int r2 = (i_Move.FromRow + i_Move.ToRow) / 2;
                int c2 = (i_Move.FromCol + i_Move.ToCol) / 2;

                if (!canJump(playerThatActsNow, i_Move.FromRow, i_Move.FromCol, r2, c2, i_Move.ToRow, i_Move.ToCol))
                {
                    moveStatusCode = eMoveStatusCode.InvalidCoordinates;
                }
            }
            else
            {
                // else the move is a regular move check if it is a valid move
                if (!checkRegularMove(playerThatActsNow, i_Move.FromRow, i_Move.FromCol, i_Move.ToRow, i_Move.ToCol))
                {
                    moveStatusCode = eMoveStatusCode.InvalidCoordinates;
                }
            }

            return moveStatusCode;
        }

        // performs the move and checks for additional jumps
        public bool DoMakeMove(CheckersMove i_Move)
        {
            Player playerThatActsNow = getPlayerThatActsNow();
            bool hasMoreJumps = false;
            List<CheckersMove> jumps;

            // performs the given move, it is assumed the move is valid
            makeMove(playerThatActsNow, i_Move);

            /* If the move was a jump, check if the player has another
             jump from the square that the player just jumped to. 
             if so return true
             */
            if (isJump(i_Move) && (jumps = getLegalJumpsFrom(playerThatActsNow, i_Move.ToRow, i_Move.ToCol)) != null)
            {
                hasMoreJumps = true;
            }

            // if the player doesen't have additional jumps the turn switches to the other player
            if (!hasMoreJumps)
            {
                m_Player1Turn = !m_Player1Turn;
            }

            // notify the display a change a board change has occured
            BoardChanged(i_Move);

            // check if the game is over
            checkIfGameEnded(playerThatActsNow, i_Move);

            return hasMoreJumps;
        }

        private void BoardChanged(CheckersMove i_Move)
        {
            BoardChangeEventArgs e = new BoardChangeEventArgs();
            e.CheckersMove = i_Move;
            e.DestCellType = r_Board[i_Move.ToRow, i_Move.ToCol];
            e.IsJump = isJump(i_Move);
            if (e.IsJump)
            {
                e.RowOfPawnRemoved = (i_Move.FromRow + i_Move.ToRow) / 2;
                e.ColOfPawnRemoved = (i_Move.FromCol + i_Move.ToCol) / 2;
            }

            OnBoardChange(e);      
        }

        private void checkIfGameEnded(Player i_PlayerThatActedNow, CheckersMove i_LastMove)
        {
            Player theOtherPlayer = i_PlayerThatActedNow == r_Player1 ? r_Player2 : r_Player1;

            if (GetLegalMoves(theOtherPlayer) == null)
            {
                if (GetLegalMoves(i_PlayerThatActedNow) == null)
                {
                    GameOver(eGameOverStatusCode.Draw, i_LastMove);
                }
                else
                {
                    GameOver(
                        i_PlayerThatActedNow.Equals(r_Player1) ?
                        eGameOverStatusCode.Player1Won :
                        eGameOverStatusCode.Player2Won,
                        i_LastMove);
                }
            }
        }

        private void GameOver(eGameOverStatusCode i_eGameEndedStatusCode, CheckersMove i_LastMove)
        {
            GameOverEventArgs e = new GameOverEventArgs();
            e.LastMove = i_LastMove;
            e.GameOverStatusCode = i_eGameEndedStatusCode;

            // update score
            switch (i_eGameEndedStatusCode)
            {
                case eGameOverStatusCode.Draw:
                    break;
                case eGameOverStatusCode.Player1Won:
                    r_Player1.Score++;
                    break;
                case eGameOverStatusCode.Player2Won:
                    r_Player2.Score++;
                    break;
                default: break;
            }

            m_Player1Turn = true;
            initializeBoard();
            OnGameOver(e);   
        }

        protected virtual void OnBoardChange(BoardChangeEventArgs e)
        {
            if(BoardChangeOccured != null)
            {
                BoardChangeOccured.Invoke(this, e);
            }
        }

        protected virtual void OnGameOver(GameOverEventArgs e)
        {
            if(GameOverOccured != null)
            {
                GameOverOccured.Invoke(this, e);
            }
        }
 
        /**
     * Return a list of the legal jumps that the specified player can
     * make starting from the specified row and column. If no such
     * jumps are possible, null is returned.  The logic is similar
     * to the logic of the getLegalMoves() method.
     */
        private List<CheckersMove> getLegalJumpsFrom(Player i_Player, int i_Row, int i_Col)
        {
            List<CheckersMove> listOfMoves = new List<CheckersMove>();

            if (r_Board[i_Row, i_Col] == i_Player.PawnSymbol || r_Board[i_Row, i_Col] == i_Player.KingSymbol)
            {
                if (canJump(i_Player, i_Row, i_Col, i_Row + 1, i_Col + 1, i_Row + 2, i_Col + 2))
                {
                    listOfMoves.Add(new CheckersMove(i_Row, i_Col, i_Row + 2, i_Col + 2));
                }

                if (canJump(i_Player, i_Row, i_Col, i_Row - 1, i_Col + 1, i_Row - 2, i_Col + 2))
                {
                    listOfMoves.Add(new CheckersMove(i_Row, i_Col, i_Row - 2, i_Col + 2));
                }

                if (canJump(i_Player, i_Row, i_Col, i_Row + 1, i_Col - 1, i_Row + 2, i_Col - 2))
                {
                    listOfMoves.Add(new CheckersMove(i_Row, i_Col, i_Row + 2, i_Col - 2));
                }

                if (canJump(i_Player, i_Row, i_Col, i_Row - 1, i_Col - 1, i_Row - 2, i_Col - 2))
                {
                    listOfMoves.Add(new CheckersMove(i_Row, i_Col, i_Row - 2, i_Col - 2));
                }
            }

            if (listOfMoves.Count == 0)
            {
                listOfMoves = null;
            }

            return listOfMoves;
        }

        /**
        * Make the specified move.  It is assumed that move
        * is non-null and that the move it represents is legal.
        */
        private void makeMove(Player i_Player, CheckersMove i_Move)
        {
            makeMove(i_Player, i_Move.FromRow, i_Move.FromCol, i_Move.ToRow, i_Move.ToCol);
        }

        /**
           * Make the move from fromRow,fromCol to (toRow,toCol).  It is
           * assumed that this move is legal. If the move is a jump, the
           * jumped piece is removed from the board.  If a piece moves
           * the last row on the opponent's side of the board, the 
           * piece becomes a king.
           */
        private void makeMove(Player i_Player, int i_FromRow, int i_FromCol, int i_ToRow, int i_ToCol)
        {
            r_Board[i_ToRow, i_ToCol] = r_Board[i_FromRow, i_FromCol];
            r_Board[i_FromRow, i_FromCol] = eCellType.EMPTY;
            if (Math.Abs(i_FromRow - i_ToRow) == 2)
            {
                // The move is a jump.  Remove the piece we jumped above from the board;
                int jumpedPawnRow = (i_FromRow + i_ToRow) / 2;
                int jumpPawnCol = (i_FromCol + i_ToCol) / 2;
                r_Board[jumpedPawnRow, jumpPawnCol] = eCellType.EMPTY;
            }

            // if the player that moves down reached the bottom row with a regular pawn make this pawn a king
            if (i_ToRow == r_BoardSize - 1 && i_Player.MovesDown && !(r_Board[i_ToRow, i_ToCol] == eCellType.Player2King))
            {
                r_Board[i_ToRow, i_ToCol] = eCellType.Player1King;
            }

            // if the player that moves up reached row 0 with a regualr pawn make this pawn a king
            if (i_ToRow == 0 && !i_Player.MovesDown && !(r_Board[i_ToRow, i_ToCol] == eCellType.Player2King))
            {
                r_Board[i_ToRow, i_ToCol] = eCellType.Player2King;
            }
        }

        // performs a computer turn
        public void DoComputerTurn()
        {
            List<CheckersMove> computerMoves = GetLegalMoves(r_Player2);
            int startingIndexOfList = 0;
            int endIndexOfList = computerMoves.Count - 1;
            CheckersMove computerMove;
            Random random = new Random();
            int randomNumber = random.Next(startingIndexOfList, endIndexOfList);
            computerMove = computerMoves.ElementAt(randomNumber);
            if (DoMakeMove(computerMove))
            {
                DoComputerTurn();
            }
        }

        private Player getPlayerThatActsNow()
        {
            return m_Player1Turn ? r_Player1 : r_Player2;
        }
    }
}
