#include <iostream>
#include <vector>
#include <cstdlib>
#include <ctime>

using namespace std;

const int BOARD_SIZE = 8;
enum Piece { EMPTY, BLACK_QUEEN, BLACK_KNIGHT, WHITE_KING };

struct Position {
    int row, col;
};

void printBoard(const vector<vector<Piece>>& board) {
    for (int r = 0; r < BOARD_SIZE; ++r) {
        for (int c = 0; c < BOARD_SIZE; ++c) {
            switch (board[r][c]) {
                case EMPTY: cout << ". "; break;
                case BLACK_QUEEN: cout << "Q "; break;
                case BLACK_KNIGHT: cout << "N "; break;
                case WHITE_KING: cout << "K "; break;
            }
        }
        cout << endl;
    }
}

bool isAttackedByQueen(const vector<vector<Piece>>& board, const Position& queen, const Position& king) {
    if (queen.row == king.row) {
        int startCol = min(queen.col, king.col) + 1;
        int endCol = max(queen.col, king.col);
        for (int c = startCol; c < endCol; ++c) {
            if (board[queen.row][c] != EMPTY) return false;
        }
        return true;
    } else if (queen.col == king.col) {
        int startRow = min(queen.row, king.row) + 1;
        int endRow = max(queen.row, king.row);
        for (int r = startRow; r < endRow; ++r) {
            if (board[r][queen.col] != EMPTY) return false;
        }
        return true;
    } else if (abs(queen.row - king.row) == abs(queen.col - king.col)) {
        int rowStep = (king.row > queen.row) ? 1 : -1;
        int colStep = (king.col > queen.col) ? 1 : -1;
        int steps = abs(queen.row - king.row);
        for (int i = 1; i < steps; ++i) {
            if (board[queen.row + i * rowStep][queen.col + i * colStep] != EMPTY) return false;
        }
        return true;
    }
    return false;
}

bool isAttackedByKnight(const Position& knight, const Position& king) {
    int dr = abs(knight.row - king.row);
    int dc = abs(knight.col - king.col);
    return (dr == 2 && dc == 1) || (dr == 1 && dc == 2);
}

int main() {
    srand(time(nullptr));
    vector<vector<Piece>> board(BOARD_SIZE, vector<Piece>(BOARD_SIZE, EMPTY));
    Position kingPosition;
    
    kingPosition.row = rand() % BOARD_SIZE;
    kingPosition.col = rand() % BOARD_SIZE;
    board[kingPosition.row][kingPosition.col] = WHITE_KING;
    
    int numQueens = rand() % (BOARD_SIZE - 1) + 1;
    int numKnights = rand() % (BOARD_SIZE - 1) + 1;
    
    for (int i = 0; i < numQueens; ++i) {
        int row, col;
        do {
            row = rand() % BOARD_SIZE;
            col = rand() % BOARD_SIZE;
        } while (board[row][col] != EMPTY);
        board[row][col] = BLACK_QUEEN;
    }

    for (int i = 0; i < numKnights; ++i) {
        int row, col;
        do {
            row = rand() % BOARD_SIZE;
            col = rand() % BOARD_SIZE;
        } while (board[row][col] != EMPTY);
        board[row][col] = BLACK_KNIGHT;
    }

    printBoard(board);
    
    cout << "King's position: (" << kingPosition.row + 1 << ", " << kingPosition.col + 1 << ")" << endl;
    cout << "Attacking pieces:" << endl;
    
    for (int r = 0; r < BOARD_SIZE; ++r) {
        for (int c = 0; c < BOARD_SIZE; ++c) {
            if (board[r][c] == BLACK_QUEEN) {
                Position queenPosition = {r, c};
                if (isAttackedByQueen(board, queenPosition, kingPosition)) {
                    cout << "Queen at (" << r + 1 << ", " << c + 1 << ")" << endl;
                }
            }
        }
    }
    
    for (int r = 0; r < BOARD_SIZE; ++r) {
        for (int c = 0; c < BOARD_SIZE; ++c) {
            if (board[r][c] == BLACK_KNIGHT) {
                Position knightPosition = {r, c};
                if (isAttackedByKnight(knightPosition, kingPosition)) {
                    cout << "Knight at (" << r + 1 << ", " << c + 1 << ")" << endl;
                }
            }
        }
    }

    return 0;
}
