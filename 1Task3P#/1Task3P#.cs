using System;
using System.Collections.Generic;

namespace ChessBoard
{
    enum Piece { EMPTY, BLACK_QUEEN, BLACK_KNIGHT, WHITE_KING };

    struct Position
    {
        public int row, col;
        public Position(int r, int c)
        {
            row = r;
            col = c;
        }
    }

    class Program
    {
        const int BOARD_SIZE = 8;
        static Random rand = new Random();

        static void PrintBoard(Piece[,] board)
        {
            for (int r = 0; r < BOARD_SIZE; ++r)
            {
                for (int c = 0; c < BOARD_SIZE; ++c)
                {
                    switch (board[r, c])
                    {
                        case Piece.EMPTY: Console.Write(". "); break;
                        case Piece.BLACK_QUEEN: Console.Write("Q "); break;
                        case Piece.BLACK_KNIGHT: Console.Write("N "); break;
                        case Piece.WHITE_KING: Console.Write("K "); break;
                    }
                }
                Console.WriteLine();
            }
        }

        static bool IsAttackedByQueen(Piece[,] board, Position queen, Position king)
        {
            if (queen.row == king.row)
            {
                int startCol = Math.Min(queen.col, king.col) + 1;
                int endCol = Math.Max(queen.col, king.col);
                for (int c = startCol; c < endCol; ++c)
                {
                    if (board[queen.row, c] != Piece.EMPTY) return false;
                }
                return true;
            }
            else if (queen.col == king.col)
            {
                int startRow = Math.Min(queen.row, king.row) + 1;
                int endRow = Math.Max(queen.row, king.row);
                for (int r = startRow; r < endRow; ++r)
                {
                    if (board[r, queen.col] != Piece.EMPTY) return false;
                }
                return true;
            }
            else if (Math.Abs(queen.row - king.row) == Math.Abs(queen.col - king.col))
            {
                int rowStep = (king.row > queen.row) ? 1 : -1;
                int colStep = (king.col > queen.col) ? 1 : -1;
                int steps = Math.Abs(queen.row - king.row);
                for (int i = 1; i < steps; ++i)
                {
                    if (board[queen.row + i * rowStep, queen.col + i * colStep] != Piece.EMPTY) return false;
                }
                return true;
            }
            return false;
        }

        static bool IsAttackedByKnight(Position knight, Position king)
        {
            int dr = Math.Abs(knight.row - king.row);
            int dc = Math.Abs(knight.col - king.col);
            return (dr == 2 && dc == 1) || (dr == 1 && dc == 2);
        }

        static void Main(string[] args)
        {
            Piece[,] board = new Piece[BOARD_SIZE, BOARD_SIZE];
            Position kingPosition;

            kingPosition.row = rand.Next(BOARD_SIZE);
            kingPosition.col = rand.Next(BOARD_SIZE);
            board[kingPosition.row, kingPosition.col] = Piece.WHITE_KING;

            int numQueens = rand.Next(1, BOARD_SIZE);
            int numKnights = rand.Next(1, BOARD_SIZE);

            for (int i = 0; i < numQueens; ++i)
            {
                int row, col;
                do
                {
                    row = rand.Next(BOARD_SIZE);
                    col = rand.Next(BOARD_SIZE);
                } while (board[row, col] != Piece.EMPTY);
                board[row, col] = Piece.BLACK_QUEEN;
            }

            for (int i = 0; i < numKnights; ++i)
            {
                int row, col;
                do
                {
                    row = rand.Next(BOARD_SIZE);
                    col = rand.Next(BOARD_SIZE);
                } while (board[row, col] != Piece.EMPTY);
                board[row, col] = Piece.BLACK_KNIGHT;
            }

            PrintBoard(board);

            Console.WriteLine("King's position: (" + (kingPosition.row + 1) + ", " + (kingPosition.col + 1) + ")");
            Console.WriteLine("Attacking pieces:");

            for (int r = 0; r < BOARD_SIZE; ++r)
            {
                for (int c = 0; c < BOARD_SIZE; ++c)
                {
                    if (board[r, c] == Piece.BLACK_QUEEN)
                    {
                        Position queenPosition = new Position(r, c);
                        if (IsAttackedByQueen(board, queenPosition, kingPosition))
                        {
                            Console.WriteLine("Queen at (" + (r + 1) + ", " + (c + 1) + ")");
                        }
                    }
                }
            }

            for (int r = 0; r < BOARD_SIZE; ++r)
            {
                for (int c = 0; c < BOARD_SIZE; ++c)
                {
                    if (board[r, c] == Piece.BLACK_KNIGHT)
                    {
                        Position knightPosition = new Position(r, c);
                        if (IsAttackedByKnight(knightPosition, kingPosition))
                        {
                            Console.WriteLine("Knight at (" + (r + 1) + ", " + (c + 1) + ")");
                        }
                    }
                }
            }
        }
    }
}
