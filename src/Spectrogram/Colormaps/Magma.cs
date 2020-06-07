﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Spectrogram.Colormaps
{
    class Magma : Colormap
    {
        public override (byte r, byte g, byte b) GetRGB(byte value)
        {
            return (RGB[value, 0], RGB[value, 1], RGB[value, 2]);
        }

        private readonly byte[,] RGB =
        {
            {0, 0, 3},
            {0, 0, 4},
            {0, 0, 6},
            {1, 0, 7},
            {1, 1, 9},
            {1, 1, 11},
            {2, 2, 13},
            {2, 2, 15},
            {3, 3, 17},
            {4, 3, 19},
            {4, 4, 21},
            {5, 4, 23},
            {6, 5, 25},
            {7, 5, 27},
            {8, 6, 29},
            {9, 7, 32},
            {10, 7, 34},
            {11, 8, 36},
            {12, 9, 38},
            {13, 10, 40},
            {14, 10, 42},
            {15, 11, 45},
            {16, 12, 47},
            {17, 12, 49},
            {19, 13, 51},
            {20, 13, 54},
            {21, 14, 56},
            {22, 14, 58},
            {23, 15, 61},
            {25, 15, 63},
            {26, 16, 66},
            {27, 16, 68},
            {28, 16, 70},
            {30, 17, 73},
            {31, 17, 75},
            {33, 17, 78},
            {34, 17, 80},
            {36, 17, 83},
            {37, 17, 85},
            {39, 17, 87},
            {40, 17, 90},
            {42, 17, 92},
            {43, 17, 94},
            {45, 17, 97},
            {47, 16, 99},
            {49, 16, 101},
            {50, 16, 103},
            {52, 16, 105},
            {54, 15, 107},
            {55, 15, 108},
            {57, 15, 110},
            {59, 15, 112},
            {61, 15, 113},
            {62, 15, 114},
            {64, 15, 116},
            {66, 15, 117},
            {67, 15, 118},
            {69, 15, 119},
            {71, 15, 120},
            {72, 16, 120},
            {74, 16, 121},
            {76, 16, 122},
            {77, 17, 123},
            {79, 17, 123},
            {81, 18, 124},
            {82, 18, 124},
            {84, 19, 125},
            {85, 20, 125},
            {87, 20, 126},
            {88, 21, 126},
            {90, 21, 126},
            {92, 22, 127},
            {93, 23, 127},
            {95, 23, 127},
            {96, 24, 128},
            {98, 25, 128},
            {99, 25, 128},
            {101, 26, 128},
            {103, 26, 128},
            {104, 27, 129},
            {106, 28, 129},
            {107, 28, 129},
            {109, 29, 129},
            {110, 30, 129},
            {112, 30, 129},
            {113, 31, 129},
            {115, 32, 129},
            {117, 32, 129},
            {118, 33, 129},
            {120, 33, 129},
            {121, 34, 130},
            {123, 35, 130},
            {124, 35, 130},
            {126, 36, 130},
            {128, 36, 130},
            {129, 37, 129},
            {131, 37, 129},
            {132, 38, 129},
            {134, 39, 129},
            {136, 39, 129},
            {137, 40, 129},
            {139, 40, 129},
            {140, 41, 129},
            {142, 41, 129},
            {144, 42, 129},
            {145, 42, 129},
            {147, 43, 128},
            {148, 43, 128},
            {150, 44, 128},
            {152, 44, 128},
            {153, 45, 128},
            {155, 46, 127},
            {157, 46, 127},
            {158, 47, 127},
            {160, 47, 127},
            {161, 48, 126},
            {163, 48, 126},
            {165, 49, 126},
            {166, 49, 125},
            {168, 50, 125},
            {170, 50, 125},
            {171, 51, 124},
            {173, 51, 124},
            {175, 52, 123},
            {176, 52, 123},
            {178, 53, 123},
            {180, 53, 122},
            {181, 54, 122},
            {183, 55, 121},
            {185, 55, 121},
            {186, 56, 120},
            {188, 56, 120},
            {189, 57, 119},
            {191, 57, 118},
            {193, 58, 118},
            {194, 59, 117},
            {196, 59, 117},
            {198, 60, 116},
            {199, 61, 115},
            {201, 61, 115},
            {202, 62, 114},
            {204, 63, 113},
            {206, 63, 113},
            {207, 64, 112},
            {209, 65, 111},
            {210, 66, 110},
            {212, 67, 110},
            {213, 67, 109},
            {215, 68, 108},
            {216, 69, 107},
            {218, 70, 107},
            {219, 71, 106},
            {221, 72, 105},
            {222, 73, 104},
            {223, 74, 103},
            {225, 75, 103},
            {226, 76, 102},
            {227, 78, 101},
            {229, 79, 100},
            {230, 80, 100},
            {231, 81, 99},
            {232, 83, 98},
            {233, 84, 97},
            {234, 85, 97},
            {236, 87, 96},
            {237, 88, 95},
            {238, 90, 95},
            {239, 91, 94},
            {239, 93, 94},
            {240, 95, 93},
            {241, 96, 93},
            {242, 98, 93},
            {243, 100, 92},
            {243, 101, 92},
            {244, 103, 92},
            {245, 105, 92},
            {246, 107, 92},
            {246, 108, 92},
            {247, 110, 92},
            {247, 112, 92},
            {248, 114, 92},
            {248, 116, 92},
            {249, 118, 92},
            {249, 120, 92},
            {250, 121, 93},
            {250, 123, 93},
            {250, 125, 94},
            {251, 127, 94},
            {251, 129, 95},
            {251, 131, 95},
            {252, 133, 96},
            {252, 135, 97},
            {252, 137, 97},
            {252, 139, 98},
            {253, 140, 99},
            {253, 142, 100},
            {253, 144, 101},
            {253, 146, 102},
            {253, 148, 103},
            {254, 150, 104},
            {254, 152, 105},
            {254, 154, 106},
            {254, 156, 107},
            {254, 157, 108},
            {254, 159, 109},
            {254, 161, 110},
            {254, 163, 111},
            {254, 165, 112},
            {255, 167, 114},
            {255, 169, 115},
            {255, 171, 116},
            {255, 172, 118},
            {255, 174, 119},
            {255, 176, 120},
            {255, 178, 122},
            {255, 180, 123},
            {255, 182, 124},
            {255, 184, 126},
            {255, 185, 127},
            {255, 187, 129},
            {255, 189, 130},
            {255, 191, 132},
            {255, 193, 133},
            {255, 195, 135},
            {255, 197, 136},
            {255, 198, 138},
            {255, 200, 140},
            {255, 202, 141},
            {255, 204, 143},
            {254, 206, 144},
            {254, 208, 146},
            {254, 209, 148},
            {254, 211, 149},
            {254, 213, 151},
            {254, 215, 153},
            {254, 217, 155},
            {254, 219, 156},
            {254, 220, 158},
            {254, 222, 160},
            {254, 224, 162},
            {254, 226, 163},
            {253, 228, 165},
            {253, 230, 167},
            {253, 231, 169},
            {253, 233, 171},
            {253, 235, 172},
            {253, 237, 174},
            {253, 239, 176},
            {253, 241, 178},
            {253, 242, 180},
            {253, 244, 182},
            {253, 246, 184},
            {252, 248, 186},
            {252, 250, 188},
            {252, 251, 189},
            {252, 253, 191},
        };
    }
}
