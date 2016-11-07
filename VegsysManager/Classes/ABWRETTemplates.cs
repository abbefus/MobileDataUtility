﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VegsysManager.Classes
{
    class WhiteFormV1_0
    {
        public short F_COLUMN_START = 5;
        public Dictionary<string, short> F_ROW = new Dictionary<string, short>
        {
            { "F1_1", 6 },
            { "F1_2", 7 },
            { "F1_3", 8 },
            { "F1_4", 9 },

            { "F2_1", 11 },
            { "F2_2", 12 },
            { "F2_3", 13 },
            { "F2_4", 14 },
            { "F2_5", 15 },

            { "F3_1", 17 },
            { "F3_2", 18 },
            { "F3_3", 19 },
            { "F3_4", 20 },
            { "F3_5", 21 },
            { "F3_6", 22 },
            
            { "F4_1", 24 },
            { "F4_2", 25 },
            { "F4_3", 26 },
            { "F4_4", 27 },
            { "F4_5", 28 },
            { "F4_6", 29 },

            { "F5_1", 31 },
            { "F5_2", 32 },
            { "F5_3", 33 },
            { "F5_4", 34 },
            { "F5_5", 35 },

            { "F6_1", 36 },

            { "F7_1", 37 },

            { "F8_1", 39 },
            { "F8_2", 40 },
            { "F8_3", 41 },
            { "F8_4", 42 },
            { "F8_5", 43 },

            { "F9_1", 45 },
            { "F9_2", 46 },
            { "F9_3", 47 },
            { "F9_4", 48 },
            { "F9_5", 49 },

            { "F10_1", 52 },
            { "F10_2", 53 },
            { "F10_3", 54 },
            { "F10_4", 55 },
            { "F10_5", 56 },

            { "F11_1", 58 },
            { "F11_2", 59 },
            { "F11_3", 60 },

            { "F12_1", 62 },
            { "F12_2", 63 },
            { "F12_3", 64 },
            { "F12_4", 65 },
            { "F12_5", 66 },
            { "F12_6", 67 },
            { "F12_7", 68 },

            { "F13_1", 69 },
           
            { "F14_1", 71 },
            { "F14_2", 72 },
            { "F14_3", 73 },
            { "F14_4", 74 },
            { "F14_5", 75 },
            { "F14_6", 76 },

            { "F15_1", 78 },
            { "F15_2", 79 },
            { "F15_3", 80 },
            { "F15_4", 81 },
            { "F15_5", 82 },

            { "F16_1", 84 },
            { "F16_2", 85 },
            { "F16_3", 86 },
            { "F16_4", 87 },
            { "F16_5", 88 },
            { "F16_6", 89 },

            { "F17_1", 91 },
            { "F17_2", 92 },
            { "F17_3", 93 },

            { "F18_1", 95 },
            { "F18_2", 96 },
            { "F18_3", 97 },

            { "F19_1", 98 },

            { "F20_1", 99 },

            { "F21_1", 100 },

            { "F22_1", 101 },

            { "F23_1", 103 },
            { "F23_2", 104 },
            { "F23_3", 105 },
            { "F23_4", 106 },

            { "F24_1", 107 },

            { "F25_1", 109 },
            { "F25_2", 110 },
            { "F25_3", 111 },
            { "F25_4", 112 },
            { "F25_5", 113 },

            { "F26_1", 115 },
            { "F26_2", 116 },
            { "F26_3", 117 },
            { "F26_4", 118 },
            { "F26_5", 119 },

            { "F27_1", 121 },
            { "F27_2", 122 },
            { "F27_3", 123 },

            { "F28_1", 125 },
            { "F28_2", 126 },
            { "F28_3", 127 },

            { "F29_1", 129 },
            { "F29_2", 130 },
            { "F29_3", 131 },
            { "F29_4", 132 },

            { "F30_1", 134 },
            { "F30_2", 135 },
            { "F30_3", 136 },
            { "F30_4", 137 },
            { "F30_5", 138 },

            { "F31_1", 140 },
            { "F31_2", 141 },
            { "F31_3", 142 },
            { "F31_4", 143 },

            { "F32_1", 145 },
            { "F32_2", 146 },
            { "F32_3", 147 },
            { "F32_4", 148 },
            { "F32_5", 149 },

            { "F33_1", 151 },
            { "F33_2", 152 },
            { "F33_3", 153 },
            { "F33_4", 154 },
            { "F33_5", 155 },
            { "F33_6", 156 },
            { "F33_7", 157 },
            { "F33_8", 158 },

            { "F34_1", 160 },
            { "F34_2", 161 },

            { "F35_1", 163 },
            { "F35_2", 164 },
            { "F35_3", 165 },
            { "F35_4", 166 },
            { "F35_5", 167 },

            { "F36_1", 169 },
            { "F36_2", 170 },

            { "F37_1", 172 },
            { "F37_2", 173 },
            { "F37_3", 174 },
            { "F37_4", 175 },
            { "F37_5", 176 },

            { "F38_1", 178 },
            { "F38_2", 179 },
            { "F38_3", 180 },
            { "F38_4", 181 },
            { "F38_5", 182 },

            { "F39_1", 184 },
            { "F39_2", 185 },
            { "F39_3", 186 },

            { "F40_1", 188 },
            { "F40_2", 189 },
            { "F40_3", 190 },
            { "F40_4", 191 },
            { "F40_5", 192 },

            { "F41_1", 194 },
            { "F41_2", 195 },
            { "F41_3", 196 },
            { "F41_4", 197 },
            { "F41_5", 198 },

            { "F42_1", 200 },
            { "F42_2", 201 },
            { "F42_3", 202 },

            { "F43_1", 204 },
            { "F43_2", 205 },
            { "F43_3", 206 },

            { "F44_1", 208 },
            { "F44_2", 209 },
            { "F44_3", 210 },
            { "F44_4", 211 },

            { "F45_1", 213 },
            { "F45_2", 214 },
            { "F45_3", 215 },
            { "F45_4", 216 },

            { "F46_1", 218 },
            { "F46_2", 219 },
            { "F46_3", 220 },
            { "F46_4", 221 },
            { "F46_5", 222 },

            { "F47_1", 224 },
            { "F47_2", 225 },
            { "F47_3", 226 },
            { "F47_4", 227 },
            { "F47_5", 228 },

            { "F48_1", 230 },
            { "F48_2", 231 },
            { "F48_3", 232 },
            { "F48_4", 233 },

            { "F49_1", 235 },
            { "F49_2", 236 },

            { "F50_1", 238 },
            { "F50_2", 239 },
            { "F50_3", 240 },
            { "F50_4", 241 },
            { "F50_5", 242 },

            { "F51_1", 244 },
            { "F51_2", 245 },
            { "F51_3", 246 },
            { "F51_4", 247 },

            { "F52_1", 249 },
            { "F52_2", 250 },
            { "F52_3", 251 },
            { "F52_4", 252 },
            { "F52_5", 253 },

            { "F53_1", 255 },
            { "F53_2", 256 },

            { "F54_1", 257 },

            { "F55_1", 259 },
            { "F55_2", 260 },
            { "F55_3", 261 },
            { "F55_4", 262 },
            { "F55_5", 263 },
            { "F55_6", 264 },

            { "F56_1", 266 },
            { "F56_2", 267 },
            { "F56_3", 268 },

            { "F57_1", 270 },
            { "F57_2", 271 },
            { "F57_3", 272 },
            { "F57_4", 273 },

            { "F58_1", 275 },
            { "F58_2", 276 },
            { "F58_3", 277 },
            { "F58_4", 278 },

            { "F59_1", 280 },
            { "F59_2", 281 },
            { "F59_3", 282 },
            { "F59_4", 283 },
            { "F59_5", 284 },
            { "F59_6", 285 },

            { "F60_1", 287 },
            { "F60_2", 288 },
            { "F60_3", 289 },
            { "F60_4", 290 },

            { "F61_1", 291 },

            { "F62_1", 292 },

            { "F63_1", 294 },
            { "F63_2", 295 },
            { "F63_3", 296 },
            { "F63_4", 297 },
            { "F63_5", 298 },
            { "F63_6", 299 },
            { "F63_7", 300 },
            { "F63_8", 301 },

            { "F64_1", 303 },
            { "F64_2", 304 },
            { "F64_3", 305 },

            { "F65_1", 307 },
            { "F65_2", 308 },
            { "F65_3", 309 },
            { "F65_4", 310 },

            { "F66_1", 312 },
            { "F66_2", 313 },
            { "F66_3", 314 },
            { "F66_4", 315 },
            { "F66_5", 316 },

            { "F67_1", 318 },
            { "F67_2", 319 },
            { "F67_3", 320 },
            { "F67_4", 321 },

            { "F68_1", 323 },
            { "F68_2", 324 },
            { "F68_3", 325 },
            { "F68_4", 326 },

            { "F69_1", 328 },
            { "F69_2", 329 },
            { "F69_3", 330 },
            { "F69_4", 331 },
            { "F69_5", 332 },
            { "F69_6", 333 },

            { "F70_1", 335 }
        };
    }
}
