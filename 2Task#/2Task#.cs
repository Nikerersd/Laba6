using System;
using System.Collections.Generic;

namespace AES
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string text;
            Console.Write("Input text for cipher >> ");
            text = Console.ReadLine();

            List<List<List<byte>>> block;   //генерация матрицы для работы
            createBlocks(text, out block);
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("The encryption block: ");
            foreach (var i in block)
            {
                foreach (var j in i)
                {
                    foreach (var k in j)
                    {
                        Console.Write("{0,4} ", k);
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine("\n----------------------------------------------");

            List<byte> key = new List<byte>();
            keyGeneration(key);
            Console.Write("128-bit master key: "); //генерируем рандомный ключ 
            foreach (var i in key)
            {
                Console.Write(i);
            }
            Console.WriteLine("\n----------------------------------------------");

            List<List<byte>> roundKeys;
            KeyExpansion(key, out roundKeys);
            Console.WriteLine("Generated keys: ");
            foreach (var i in roundKeys)
            {
                foreach (var j in i)
                {
                    Console.Write("{0:X4} ", j);
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n----------------------------------------------");

            List<List<byte>> PREV = new List<List<byte>>(4);
            for (int i = 0; i < 4; i++)
            {
                PREV.Add(new List<byte>(4));
                for (int j = 0; j < 4; j++)
                {
                    PREV[i].Add(0);
                }
            }
            List<List<byte>> DEFOLT = new List<List<byte>>(PREV);
            List<List<byte>> TEK = new List<List<byte>>();
            for (int j = 0; j < 4; j++) {
                TEK.Add(new List<byte>());
                for (int k = 0; k < 4; k++)
                {
                    // Пример заполнения TEK[j][k] какими-то данными (это зависит от вашей логики)
                    TEK[j].Add(0); // Замените 0 на нужное значение
                }
            }
            List<List<List<byte>>> ECRYPT = new List<List<List<byte>>>();

            for (int i = 0; i < block.Count; i++)
            {

                List<List<byte>> res = new List<List<byte>>(4);
                for (int j = 0; j < 4; j++)
                {
                    res.Add(new List<byte>(4));
                    for (int k = 0; k < 4; k++)
                    {
                        res[j].Add((byte)(TEK[j][k] ^ block[i][j][k]));
                    }
                }
                ECRYPT.Add(res);

                PREV = TEK;
            }

            Console.WriteLine("The final cipher after encryphion:\n");
            foreach (var t in ECRYPT)
            {
                foreach (var i in t)
                {
                    foreach (var j in i)
                    {
                        Console.Write("{0,4} ", j);
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\n----------------------------------------------");

            PREV = DEFOLT;
            List<List<List<byte>>> DECRYPT = new List<List<List<byte>>>();

            for (int i = 0; i < ECRYPT.Count; ++i)
            {
                List<List<byte>> res = new List<List<byte>>(4);
                for (int j = 0; j < 4; j++)
                {
                    res.Add(new List<byte>(4));
                    for (int k = 0; k < 4; k++)
                    {
                        res[j].Add((byte)(ECRYPT[i][j][k] ^ TEK[j][k]));
                    }
                }
                DECRYPT.Add(res);
                PREV = TEK;
            }

            List<List<List<byte>>> DECRYPT_1 = new List<List<List<byte>>>();

            for (int i = 0; i < ECRYPT.Count; ++i)
            {
                List<List<byte>> res = new List<List<byte>>(4);
                for (int j = 0; j < 4; j++)
                {
                    res.Add(new List<byte>(4));
                    for (int k = 0; k < 4; k++)
                    {
                        res[j].Add(TEK[j][k]);
                    }
                }
                DECRYPT_1.Add(res);
            }

            Console.WriteLine("Message after decryption:\n");
            for (int i = 0; i < DECRYPT.Count; i++)
            {
                List<List<byte>> decrypt = DECRYPT[i];
                for (int j = 0; j < decrypt.Count; j++)
                {
                    for (int k = 0; k < decrypt[j].Count; k++)
                    {
                        Console.Write((char)decrypt[k][j]);
                    }
                }
            }
            Console.WriteLine("\n----------------------------------------------");

        }

        static void createBlocks(string text, out List<List<List<byte>>> block)
        {
            while (text.Length % 16 != 0)
            {
                text += ' ';
            }

            List<List<List<byte>>> temporary = new List<List<List<byte>>>();
            List<List<byte>> sixteen = new List<List<byte>>(4);
            for (int i = 0; i < 4; i++)
            {
                sixteen.Add(new List<byte>(4));
                for (int j = 0; j < 4; j++)
                {
                    sixteen[i].Add(0);
                }
            }

            for (int i = 0; i < text.Length; ++i)
            {
                int a = (i % 16) % 4;
                int b = (i % 16) / 4;
                sixteen[a][b] = (byte)text[i];

                if ((i + 1) % 16 == 0)
                {
                    temporary.Add(sixteen);
                    sixteen = new List<List<byte>>(4);
                    for (int j = 0; j < 4; j++)
                    {
                        sixteen.Add(new List<byte>(4));
                        for (int k = 0; k < 4; k++)
                        {
                            sixteen[j].Add(0);
                        }
                    }
                }
            }

            block = temporary;
        }

        static void keyGeneration(List<byte> key)
        {
            Random random = new Random();
            for (int i = 1; i <= 16; i++)
            {
                key.Add((byte)random.Next(0, 9));
            }
        }

        static void SubBytes(List<byte> state)
        {
            List<byte> Sbox = new List<byte> {
               0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
               0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
               0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
               0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
               0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
               0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
               0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
               0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
               0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
               0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
               0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
               0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
               0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
               0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
               0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
               0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
            };

            List<byte> temp = new List<byte>();
            foreach (var i in state)
            {
                temp.Add(Sbox[i]);
            }
            state = temp;
        }

        static void InversionSubBytes(List<byte> state)
        {
            List<byte> InvSbox = new List<byte> {
                0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
                0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
                0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
                0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
                0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
                0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
                0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
                0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
                0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
                0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
                0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
                0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
                0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
                0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
                0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
                0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
            };
            List<byte> temp = new List<byte>();
            foreach (var i in state)
            {
                temp.Add(InvSbox[i]);
            }
            state = temp;
        }

        static void ShiftElmnts(List<byte> state)
        {
            List<byte> temp = new List<byte>(state.Count);
            for (int i = 1; i < state.Count; i++)
            {
                temp[i - 1] = state[i]; //сдвиг элементов влево
            }
            temp[state.Count - 1] = state[0]; //последний элемент становится первым

            state = temp;
        }

        static byte galuaMn(byte a, byte b)
        {
            byte result = 0;
            byte hi_bit_set;
            for (byte i = 0; i < 8; i++)
            {
                if ((b & 1) == 1)
                {
                    result ^= a;
                }
                hi_bit_set = (byte)(a & 0x80);
                a <<= 1;
                if (hi_bit_set != 0)
                {
                    a ^= 0x1b;
                }
                b >>= 1;
            }
            return result;
        }

        static List<byte> tabl_2()
        {
            List<byte> mult_by_2 = new List<byte>(256);
            for (int i = 0; i < 256; ++i)
            {
                mult_by_2.Add(galuaMn((byte)i, 2));
            }
            return mult_by_2;
        }

        static List<byte> tabl_3()
        {
            List<byte> mult_by_3 = new List<byte>(256);
            for (int i = 0; i < 256; ++i)
            {
                mult_by_3.Add(galuaMn((byte)i, 3));
            }
            return mult_by_3;
        }

        static List<byte> tabl_14()
        {
            List<byte> mult_by_14 = new List<byte>(256);
            for (int i = 0; i < 256; ++i)
            {
                mult_by_14.Add(galuaMn((byte)i, 14));
            }
            return mult_by_14;
        }

        static List<byte> tabl_9()
        {
            List<byte> mult_by_9 = new List<byte>(256);
            for (int i = 0; i < 256; ++i)
            {
                mult_by_9.Add(galuaMn((byte)i, 9));
            }
            return mult_by_9;
        }

        static List<byte> tabl_13()
        {
            List<byte> mult_by_13 = new List<byte>(256);
            for (int i = 0; i < 256; ++i)
            {
                mult_by_13.Add(galuaMn((byte)i, 13));
            }
            return mult_by_13;
        }

        static List<byte> tabl_11()
        {
            List<byte> mult_by_11 = new List<byte>(256);
            for (int i = 0; i < 256; ++i)
            {
                mult_by_11.Add(galuaMn((byte)i, 11));
            }
            return mult_by_11;
        }

        static void MnMatrix(List<byte> state)
        {
            List<byte> mult_by_2 = tabl_2(); //операции умножения на 2 и 3 к столбцам матрицы данных

            List<byte> mult_by_3 = tabl_3();

            List<byte> temp = new List<byte>();
            temp.Add((byte)(mult_by_2[state[0]] ^ mult_by_3[state[1]] ^ state[2] ^ state[3]));
            temp.Add((byte)(mult_by_2[state[1]] ^ mult_by_3[state[2]] ^ state[0] ^ state[3]));
            temp.Add((byte)(mult_by_2[state[2]] ^ mult_by_3[state[3]] ^ state[0] ^ state[1]));
            temp.Add((byte)(mult_by_2[state[3]] ^ mult_by_3[state[0]] ^ state[1] ^ state[2]));
            state = temp;
        }

        static void KeyExpansion(List<byte> key, out List<List<byte>> roundKeys)
        {
            List<byte> Rcon = new List<byte> {
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

            List<byte> temp = new List<byte>();
            int i = 0;
            int Nk = 4;
            int Nb = 4;
            int Nr = 10;

            roundKeys = new List<List<byte>>(Nb * (Nr + 1));
            for (int j = 0; j < Nb * (Nr + 1); j++)
            {
                roundKeys.Add(new List<byte>(4));
                for (int k = 0; k < 4; k++)
                {
                    roundKeys[j].Add(0);
                }
            }

            while (i < Nk)
            {
                temp = new List<byte> { key[4 * i], key[4 * i + 1], key[4 * i + 2], key[4 * i + 3] };
                roundKeys[i] = temp;
                i++;
            }

            i = Nk;
            while (i < (Nb * (Nr + 1)))
            {
                temp = roundKeys[i - 1];
                if (i % Nk == 4)
                {
                    ShiftElmnts(temp);
                    SubBytes(temp);
                    for (int k = 0; k < temp.Count; ++k)
                    {
                        temp[k] = (byte)(temp[k] ^ Rcon[i / Nk]);
                    }
                }
                else if (Nk > 6 && (i % Nk == 4))
                {
                    SubBytes(temp);
                }
                for (int j = 0; j < temp.Count; ++j)
                {
                    roundKeys[i][j] = (byte)(roundKeys[i - Nk][j] ^ temp[j]);
                }
                i++;
            }
        }

        static List<byte> AddRoundKey(List<byte> first, List<byte> second)
        {
            List<byte> temp = new List<byte>();
            for (int i = 0; i < 4; ++i)
            {
                temp.Add((byte)(first[i] ^ second[i]));
            }
            return temp;
        }

        static void InversionShiftElmnts(List<byte> state)
        {
            List<byte> temp = new List<byte>(state.Count);
            for (int i = 0; i < state.Count; i++)
            {
                temp[i] = state[(i - (i % 4) * (i % 4)) % state.Count];
            }
            state = temp;
        }

        static void InversionMnMatrix(List<byte> state)
        {
            List<byte> mult_by_14 = tabl_14();
            List<byte> mult_by_9 = tabl_9();
            List<byte> mult_by_13 = tabl_13();
            List<byte> mult_by_11 = tabl_11();

            List<byte> temp = new List<byte>();
            temp.Add((byte)(mult_by_14[state[0]] ^ mult_by_9[state[1]] ^ mult_by_13[state[2]] ^ mult_by_11[state[3]]));
            temp.Add((byte)(mult_by_14[state[1]] ^ mult_by_9[state[2]] ^ mult_by_13[state[3]] ^ mult_by_11[state[0]]));
            temp.Add((byte)(mult_by_14[state[2]] ^ mult_by_9[state[3]] ^ mult_by_13[state[0]] ^ mult_by_11[state[1]]));
            temp.Add((byte)(mult_by_14[state[3]] ^ mult_by_9[state[0]] ^ mult_by_13[state[1]] ^ mult_by_11[state[2]]));
            state = temp;
        }
    }
}


