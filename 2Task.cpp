#include <iostream>
#include <vector>
#include <random>
#include <algorithm>
#include <iomanip>
#include <string>
#include "header.h"

using namespace std;

void  keyGeneration(vector<unsigned char>& key) {
    mt19937_64 gen(random_device{}());
    uniform_int_distribution<int> numbers(0, 9);
    for (int i = 1; i <= 16; i++) {
        key.push_back('0' + numbers(gen));
    }
}

vector<unsigned char> AddRoundKey(vector<unsigned char>& first, vector<unsigned char>& second) {
    vector<unsigned char> temp;
    for (int i = 0; i < 4; ++i) {
        temp.push_back(first[i] ^ second[i]);
    }
    return temp;
}

void ShiftElmnts(vector<unsigned char>& state) {
    vector<unsigned char> temp(state.size());
    for (int i = 1; i < state.size(); i++) {
        temp[i - 1] = state[i]; //сдвиг элементов влево
    }
    temp[state.size() - 1] = state[0]; //последний элемент становится первым

    state = temp;
}

unsigned char galuaMn(unsigned char a, unsigned char b) {
    unsigned char result = 0;
    unsigned char hi_bit_set;
    for (unsigned char i = 0; i < 8; i++) {
        if (b & 1) {
            result ^= a;
        }
        hi_bit_set = a & 0x80;
        a <<= 1;
        if (hi_bit_set) {
            a ^= 0x1b;
        }
        b >>= 1;
    }
    return result;
}

vector<unsigned char> tabl_2() {
    vector<unsigned char> tabl_with_2(256);
    for (int i = 0; i < 256; ++i) {
        tabl_with_2[i] = galuaMn(i, 2);
    }
    return tabl_with_2;
}

// Генерация таблицы tabl_with_3
vector<unsigned char> tabl_3() {
    vector<unsigned char> tabl_with_3(256);
    for (int i = 0; i < 256; ++i) {
        tabl_with_3[i] = galuaMn(i, 3);
    }
    return tabl_with_3;
}

vector<unsigned char> tabl_14() {
    vector<unsigned char> tabl_with_14(256);
    for (int i = 0; i < 256; ++i) {
        tabl_with_14[i] = galuaMn(i, 14);
    }
    return tabl_with_14;
}

vector<unsigned char> tabl_9() {
    vector<unsigned char> tabl_with_9(256);
    for (int i = 0; i < 256; ++i) {
        tabl_with_9[i] = galuaMn(i, 9);
    }
    return tabl_with_9;
}

vector<unsigned char> tabl_13() {
    vector<unsigned char> tabl_with_13(256);
    for (int i = 0; i < 256; ++i) {
        tabl_with_13[i] = galuaMn(i, 13);
    }
    return tabl_with_13;
}

vector<unsigned char> tabl_11() {
    vector<unsigned char> tabl_with_11(256);
    for (int i = 0; i < 256; ++i) {
        tabl_with_11[i] = galuaMn(i, 11);
    }
    return tabl_with_11;
}

void MnMatrix(vector<unsigned char>& state) {
    vector<unsigned char> tabl_with_2 = tabl_2(); //операции умножения на 2 и 3 к столбцам матрицы данных

    vector<unsigned char> tabl_with_3 = tabl_3();

    vector<unsigned char> temp;
    temp.push_back(tabl_with_2[state[0]] ^ tabl_with_3[state[1]] ^ state[2] ^ state[3]);
    temp.push_back(tabl_with_2[state[1]] ^ tabl_with_3[state[2]] ^ state[0] ^ state[3]);
    temp.push_back(tabl_with_2[state[2]] ^ tabl_with_3[state[3]] ^ state[0] ^ state[1]);
    temp.push_back(tabl_with_2[state[3]] ^ tabl_with_3[state[0]] ^ state[1] ^ state[2]);
    state = temp;
}

void KeyExpansion(vector<unsigned char>& key, vector<vector<unsigned char>>& roundKeys) {
    const vector<unsigned char> Rcon = {
        0x00, 0x00, 0x00, 0x00,
        0x01, 0x00, 0x00, 0x00,
        0x02, 0x00, 0x00, 0x00,
        0x04, 0x00, 0x00, 0x00,
        0x08, 0x00, 0x00, 0x00,
        0x10, 0x00, 0x00, 0x00,
        0x20, 0x00, 0x00, 0x00,
        0x40, 0x00, 0x00, 0x00,
        0x80, 0x00, 0x00, 0x00,
        0x1b, 0x00, 0x00, 0x00,
        0x36, 0x00, 0x00, 0x00
    };

    vector<unsigned char> temp;
    int i = 0;
    int Nk = 4;
    int Nb = 4;
    int Nr = 10;

    roundKeys.resize(Nb * (Nr + 1), vector<unsigned char>(4));

    while (i < Nk) {
        temp = { key[4 * i], key[4 * i + 1], key[4 * i + 2], key[4 * i + 3] };
        roundKeys[i] = temp;
        i++;
    }

    i = Nk;
    while (i < (Nb * (Nr + 1))) {
        temp = roundKeys[i - 1];
        if (i % Nk == 4) {
            ShiftElmnts(temp);
            SubBytes(temp);
            for (int k = 0; k < temp.size(); ++k) {
                temp[k] = temp[k] ^ Rcon[i / Nk];
            }
        }
        else if (Nk > 6 && (i % Nk == 4)) {
            SubBytes(temp);
        }
        for (int j = 0; j < temp.size(); ++j) {
            roundKeys[i][j] = roundKeys[i - Nk][j] ^ temp[j];
        }
        i++;
    }
}

void InversionShiftElmnts(vector<unsigned char>& state) {
    vector<unsigned char> temp(state.size());
    for (int i = 0; i < state.size(); i++) {
        temp[i] = state[(i - (i % 4) * (i % 4)) % state.size()];
    }
    state = temp;
}

void InversionMnMatrix(vector<unsigned char>& state) {
    vector<unsigned char> tabl_with_14 = tabl_14();
    vector<unsigned char> tabl_with_9 = tabl_9();
    vector<unsigned char> tabl_with_13 = tabl_13();
    vector<unsigned char> tabl_with_11 = tabl_11();

    vector<unsigned char> temp;
    temp.push_back(tabl_with_14[state[0]] ^ tabl_with_9[state[1]] ^ tabl_with_13[state[2]] ^ tabl_with_11[state[3]]);
    temp.push_back(tabl_with_14[state[1]] ^ tabl_with_9[state[2]] ^ tabl_with_13[state[3]] ^ tabl_with_11[state[0]]);
    temp.push_back(tabl_with_14[state[2]] ^ tabl_with_9[state[3]] ^ tabl_with_13[state[0]] ^ tabl_with_11[state[1]]);
    temp.push_back(tabl_with_14[state[3]] ^ tabl_with_9[state[0]] ^ tabl_with_13[state[1]] ^ tabl_with_11[state[2]]);
    state = temp;
}

void createBlocks(string& text, vector<vector<vector<unsigned char>>>& block) {
    while (text.size() % 16 != 0) {
        text += ' ';
    }

    vector<vector<vector<unsigned char>>> temporary;
    vector<vector<unsigned char>> sixteen(4, vector<unsigned char>(4));

    for (int i = 0; i < text.size(); ++i) {
        int a = (i % 16) % 4;
        int b = (i % 16) / 4;
        sixteen[a][b] = text[i];

        if ((i + 1) % 16 == 0) {
            temporary.push_back(sixteen);
            sixteen = vector<vector<unsigned char>>(4, vector<unsigned char>(4));
        }
    }

    block = temporary;
}

vector<vector<unsigned char>> Encrypt(vector<vector<unsigned char>>& block, vector<vector<unsigned char>>& roundKeys, vector<unsigned char> key) {

    vector<vector<unsigned char>> temp(4, vector<unsigned char>(4, 0)); 
    for (int i = 0; i < 4; ++i) {
        temp[i] = AddRoundKey(block[i], roundKeys[i]);
    }
    
    for (int i = 1; i <= 9; i++) { 
        for (int j = 0; j <= 3; j++) { 
            SubBytes(temp[j]);
            ShiftElmnts(temp[j]);
            MnMatrix(temp[j]);
            temp[j] = AddRoundKey(temp[j], roundKeys[i]);
        }
    }
    
    for (int j = 0; j <= 3; j++) {
        SubBytes(temp[j]);
        ShiftElmnts(temp[j]);
        temp[j] = AddRoundKey(temp[j], roundKeys[10]);
    }

    return temp;
}

vector<vector<unsigned char>> Decrypt(vector<vector<unsigned char>>& block, vector<vector<unsigned char>>& roundKeys, vector<unsigned char> key) {
    vector<vector<unsigned char>> roundKeysDec(44, vector<unsigned char>(4));
    KeyExpansion(key, roundKeysDec);

    vector<vector<unsigned char>> temporary(4, vector<unsigned char>(4, 0));

    for (int j = 0; j <= 3; j++) {
        temporary[j] = AddRoundKey(block[j], roundKeys[10]);
    }

    for (int i = 9; i >= 1; i--) {
        for (int j = 0; j <= 3; j++) {
            temporary[j] = AddRoundKey(temporary[j], roundKeys[i]);
            InversionMnMatrix(temporary[j]);
            InversionShiftElmnts(temporary[j]);
            InversionSubBytes(temporary[j]);
        }
    }

    for (int j = 0; j <= 3; j++) {
        temporary[j] = AddRoundKey(temporary[j], roundKeys[0]);
    }
    for (int j = 0; j <= 3; j++) {
        InversionShiftElmnts(temporary[j]);
        InversionSubBytes(temporary[j]);
    }

    return temporary;
}

int main() {
    system("chcp 1251");
    string text;
    cout << "Input text: ";
    getline(cin, text);

    vector<vector<vector<unsigned char>>> block;   //генерация матрицы для работы
    createBlocks(text, block);
    cout << "----------------------------------------------";
    cout << "\nThe encryption block: \n";
    for (auto i : block) {
        for (auto j : i) {
            for (auto k : j) {
                cout << setw(4) << k << " ";
            }
            cout << endl;
        }
    }
    cout << "\n----------------------------------------------" << endl;

    vector<unsigned char> key;
     keyGeneration(key);
    cout << "128-bit key: ";
    for (auto i : key) {
        cout << i;
    }
    cout << "\n----------------------------------------------\n";

    vector<vector<unsigned char>> roundKeys;
    KeyExpansion(key, roundKeys);
    cout << "Generated round keys: \n";
    for (auto i : roundKeys) {
        for (auto j : i) {
            cout << hex << setw(4) << static_cast<int>(j) << " ";

        }
        cout << endl;
    }
    cout << "\n----------------------------------------------" << endl;

    vector<vector<unsigned char>> PREV(4, vector<unsigned char>(4, 0));
    vector<vector<unsigned char>> DEFOLT = PREV;
    vector<vector<unsigned char>> TEK;
    vector<vector<vector<unsigned char>>> ECRYPT;

    for (int i = 0; i < block.size(); i++) {

        vector<vector<unsigned char>> res(4, vector<unsigned char>(4, 0));
        vector<vector<unsigned char>> B = block[i];
        TEK = Encrypt(PREV, roundKeys, key);

        for (int i = 0; i < 4; ++i) {
            for (int j = 0; j < 4; ++j) {
                res[i][j] = (TEK[i][j] ^ B[i][j]);

            }
        }
        ECRYPT.push_back(res);

        PREV = TEK;
    }

    cout << "Cipher after all rounds: \n" << endl;
    for (auto t : ECRYPT) {
        for (auto i : t) {
            for (auto j : i) {
                cout << setw(4) << j << " ";
            }
            cout << endl;
        }
    }

    cout << "\n----------------------------------------------\n";

    PREV = DEFOLT;
    vector<vector<vector<unsigned char>>> DECRYPT;

    for (int i = 0; i < ECRYPT.size(); ++i) {
        vector<vector<unsigned char>> res(4, vector<unsigned char>(4, 0));
        vector<vector<unsigned char>> B = ECRYPT[i];

        TEK = Encrypt(PREV, roundKeys, key);

        for (int i = 0; i < 4; ++i) {
            for (int j = 0; j < 4; ++j) {
                res[i][j] = (B[i][j] ^ TEK[i][j]);
            }
        }
        DECRYPT.push_back(res);
        PREV = TEK;
    }

    vector<vector<vector<unsigned char>>> DECRYPT_1;

    for (int i = 0; i < ECRYPT.size(); ++i) {
        vector<vector<unsigned char>> res(4, vector<unsigned char>(4, 0));
        vector<vector<unsigned char>> B = ECRYPT[i];

        TEK = Decrypt(B, roundKeys, key);

        for (int i = 0; i < 4; ++i) {
            for (int j = 0; j < 4; ++j) {
                res[i][j] = TEK[i][j];
            }
        }
        DECRYPT_1.push_back(res);
    }

    cout << "Decrypted message:\n" << endl;
    for (int i = 0; i < DECRYPT.size(); i++) {
        vector<vector<unsigned char>>& decrypt = DECRYPT[i];
        for (int j = 0; j < decrypt.size(); j++) {
            for (int k = 0; k < decrypt[j].size(); k++) {
                cout << decrypt[k][j];
            }
        }
    }
    cout << "\n----------------------------------------------\n" << endl;

    return 0;
}