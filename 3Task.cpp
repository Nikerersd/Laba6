#include <iostream>
#include <vector>
#include <cmath>
#include <iomanip>

using namespace std;

void LU(vector<vector<double>> A, vector<double> b, vector<double>& x_k) {
    int n = A.size();
    vector<vector<double>> L(n, vector<double>(n, 0));
    vector<vector<double>> U(n, vector<double>(n, 0));

    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (j < i) {
                L[j][i] = 0;
            } else {
                L[j][i] = A[j][i];
                for (int k = 0; k < i; k++) {
                    L[j][i] -= L[j][k] * U[k][i];
                }
            }
        }

        for (int j = 0; j < n; j++) {
            if (j < i) {
                U[i][j] = 0;
            } else if (j == i) {
                U[i][j] = A[i][j];
                for (int k = 0; k < i; k++) {
                    U[i][j] -= L[i][k] * U[k][j];
                }
            } else {
                U[i][j] = A[i][j];
                for (int k = 0; k < i; k++) {
                    U[i][j] -= L[i][k] * U[k][j];
                }
                U[i][j] /= L[i][i];
            }
        }

        // Вывод L и U после каждой итерации
        cout << "\nL - матрица (" << i + 1 << " итерация):\n";
        for (int a = 0; a < n; a++) {
            for (int b = 0; b < n; b++) {
                if (a > b) {
                    cout << L[a][b] << "\t";
                } else if (a == b) {
                    cout << 1 << "\t";
                } else {
                    cout << 0 << "\t";
                }
            }
            cout << endl;
        }

        cout << "\nU - матрица (" << i + 1 << " итерация):\n";
        for (int a = 0; a < n; a++) {
            for (int b = 0; b < n; b++) {
                if (a <= b) {
                    cout << U[a][b] << "\t";
                } else {
                    cout << 0 << "\t";
                }
            }
            cout << endl;
        }
    }

    // Решение системы LUx = RIGHT
    for (int i = 0; i < n; i++) {
        x_k[i] = b[i];
        for (int j = 0; j < i; j++) {
            x_k[i] -= L[i][j] * x_k[j];
        }
        x_k[i] /= L[i][i];
    }

    for (int i = n - 1; i >= 0; i--) {
        for (int j = i + 1; j < n; j++) {
            x_k[i] -= U[i][j] * x_k[j];
        }
    }

    cout << "\nРешение системы:" << endl;
    for (int i = 0; i < n; i++) {
        cout << "x" << i + 1 << " = " << x_k[i] << endl;
    }
}

void simpleIteration(vector<vector<double>> C, vector<double>& f, vector<double>& x, int n, double epsilon) {
    vector<double> xNew(n);
    int k = 0;
    double maxDiff = 0.0;
    
    for (int i = 0; i < n; i++) {
        x[i] = 0.0;
    }

    cout << "N" << setw(10) << "x1" << setw(10) << "x2" << setw(10) << "x3" << setw(10) << "x4" << setw(10) << "εn" << endl;

    do {
        // Вычисляем новое приближение
        for (int i = 0; i < n; i++) {
            double sum = f[i];
            for (int j = 0; j < n; j++) {
                sum += C[i][j] * x[j];
            }
            xNew[i] = sum;
        }

        // Проверяем условие остановки
        maxDiff = 0.0;
        for (int i = 0; i < n; i++) {
            if (fabs(fabs(xNew[i]) - fabs(x[i])) > maxDiff) {
                maxDiff = fabs(fabs(xNew[i]) - fabs(x[i]));
            }
            x[i] = xNew[i];
        }
        k++;

        // Вывод результатов в таблицу
        cout << k << setw(10) << x[0] << setw(10) << x[1] << setw(10) << x[2] << setw(10) << x[3] << setw(10) << maxDiff << endl;
    } while (maxDiff > epsilon);

    cout << "Число итераций: " << k << endl;

    // Вывод сообщения о сходимости или расходимости
    if (maxDiff <= epsilon) {
        cout << "Метод сходится." << endl;
    } else {
        cout << "Метод расходится." << endl;
    }
}

int main() {
    vector<vector<double>> A = {
        {0.89, -0.04, 0.21, -18},
        {0.25, -1.23, 0.08, -0.09},
        {-0.21, 0.08, 0.8, -0.13},
        {0.15, -1.31, 0.06, -1.21}
    };
    vector<double> b = { -1.24, -1.21, 2.56, 0.89 };

    vector<double> x(4, 0.0);

    LU(A, b, x);

    for (int k = 0; k < 3; k++) {
            for (int i = k + 1; i < 4; i++) {
                double factor = A[i][k] / A[k][k];
                for (int j = k; j < 4; j++) {
                    A[i][j] -= factor * A[k][j];
                }
                b[i] -= factor * b[k];
            }
    }

    vector<vector<double>> C(4, vector<double>(4, 0));
    for (int i = 0; i < 4; i++) {
        for (int j = 0; j < 4; j++) {
            if (i != j) {
                C[i][j] = -A[i][j] / A[i][i]; // Заполняем матрицу C
            }
        }
    }

    vector<double> f(4, 0.0);
    for (int i = 0; i < 4; i++) {
        f[i] = b[i] / A[i][i];
    }

    simpleIteration(C, f, x, 4, 0.001);

    return 0;
}