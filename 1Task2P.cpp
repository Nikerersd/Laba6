#include <iostream>
#include <random>
#include <vector>
#include <unordered_map>
#include <ctime>
#include <cstdlib>

using namespace std;

int getRandomNumber() {
    random_device rd;
    mt19937_64 random_(rd());
    return random_() % 101 - 50;
}

int main() {

    const int M = 5;
    const int N = 5;

    vector<vector<int>> matrix(M, vector<int>(N));

    for (int i = 0; i < M; ++i) {
        for (int j = 0; j < N; ++j) {
            matrix[i][j] = getRandomNumber();
        }
    }

    cout << "Матрица:" << endl;
    for (const auto& row : matrix) {
        for (const auto& elem : row) {
            cout << elem << " ";
        }
        cout << endl;
    }

    unordered_map<int, int> frequency;

    for (const auto& row : matrix) {
        for (const auto& elem : row) {
            ++frequency[elem];
        }
    }

    cout << "\nЧастота уникальных элементов:" << endl;
    for (const auto& pair : frequency) {
        cout << "Элемент " << pair.first << " встречается " << pair.second << " раз(а)." << endl;
    }

    return 0;
}