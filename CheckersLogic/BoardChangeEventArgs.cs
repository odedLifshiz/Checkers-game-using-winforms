// -----------------------------------------------------------------------
// <copyright file="BoardChangeEventArgs.cs" company="Hewlett-Packard">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CheckersLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BoardChangeEventArgs : EventArgs
    {
        private CheckersMove m_CheckersMove;
        private eCellType m_CellTypeOfDestination;
        private bool m_IsJump;
        private int m_ColOfPawnRemoved;
        private int m_RowOfPawnRemoved;

        public CheckersMove CheckersMove
        {
            get 
            {
                return m_CheckersMove; 
            }

            set 
            { 
                m_CheckersMove = value; 
            }
        }

        public eCellType DestCellType
        {
            get
            { 
                return m_CellTypeOfDestination; 
            }

            set 
            { 
                m_CellTypeOfDestination = value;
            }
        }

        public bool IsJump
        {
            get 
            { 
                return m_IsJump;
            }

            set
            { 
                m_IsJump = value; 
            }
        }

        public int ColOfPawnRemoved
        {
            get 
            {
                return m_ColOfPawnRemoved;
            }

            set
            { 
                m_ColOfPawnRemoved = value;
            }
        }

        public int RowOfPawnRemoved
        {
            get
            { 
             return m_RowOfPawnRemoved;
                }

            set
            {
                m_RowOfPawnRemoved = value;
                }
        }
    }
}
