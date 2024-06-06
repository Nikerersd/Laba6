#include <iostream>
#include <random>
#include <vector>
#include <ctime>
#include <cstdlib>
#include <algorithm>

using namespace std;

int getRandomNumber() {
    random_device rd;
    mt19937_64 random_(rd());
    return random_() % 101;
}

vector<int> findIncreasingSequence(const vector<int>& row) {
    vector<int> bestSequence;
    vector<int> currentSequence;

    for (size_t i = 0; i < row.size(); ++i) {
        currentSequence.clear();
        currentSequence.push_back(row[i]);

        for (size_t j = i + 1; j < row.size(); ++j) {
            if (row[j] > currentSequence.back()) {
                currentSequence.push_back(row[j]);
            }
        }

        if (bestSequence.empty() || (currentSequence.front() < bestSequence.front()) ||
            (currentSequence.front() == bestSequence.front() && currentSequence.size() > bestSequence.size())) {
            bestSequence = currentSequence;
        }
    }

    return bestSequence;
}

int main() {

    const int M = 5;
    const int N = 10;

    vector<vector<int>> matrix(M, vector<int>(N));

    for (int i = 0; i < M; ++i) {
        for (int j = 0; j < N; ++j) {
            matrix[i][j] = getRandomNumber();
        }
    }

    cout << "Полученная матрица:" << endl;
    for (const auto& row : matrix) {
        for (int num : row) {
            cout << num << " ";
        }
        cout << endl;
    }

    vector<int> resultingSequence;

    for (const auto& row : matrix) {
        vector<int> sequence = findIncreasingSequence(row);

        if (resultingSequence.empty() || 
            (!sequence.empty() && sequence.front() < resultingSequence.front()) ||
            (!sequence.empty() && sequence.front() == resultingSequence.front() && sequence.size() > resultingSequence.size())) {
            resultingSequence = sequence;
        }
    }

    cout << "Возрастающая последовательность с минимальным элементом:" << endl;
    for (int num : resultingSequence) {
        cout << num << " ";
    }
    cout << endl;

    return 0;
}
