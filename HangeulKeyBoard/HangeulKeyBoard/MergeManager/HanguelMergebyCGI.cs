using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangeulKeyBoard.MergeManager
{
    //천지인 키보드
    //천지인 키보드는 여분의 char저장 공간이 더 있음!
    public class HanguelMergebyCGI : IHangeulMerge
    {
        #region declaration

        private string subString = null;
        
        private List<int?> charList = new List<int?>();
        private bool isPoped;

        bool splitIsPreviousConsonant = false;
        bool splitIsPreviousVowel = false;
        bool splitHasConsonant = false;
        bool splitHasVowel = false;
        bool splitHasTwoLastVowel = false;

        private List<int> SplitCharacterIndex = new List<int>();
        private List<int> voidSpaceIndex = new List<int>();
        public class MergeSetting
        {
        }

        public MergeSetting setting;

        #endregion

        #region Hanguel Index

        //기본 초중종성 조합 (종성은 없는 경우도 있음을 주의할것 +1 해야함)
        private readonly char[] firstIndex = {
            'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ',
            'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ',
            'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ',
            'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
        private readonly char[] middleIndex = {
            'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ',
            'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ',
            'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ',
            'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ',
            'ㅣ', 'ㆍ', 'ᆢ'}; // 아래아 주의!
        private readonly char[] lastIndex = {
            'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ',
            'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ',
            'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ',
            'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ',
            'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ',
            'ㅍ', 'ㅎ' };

        private readonly Dictionary<(char? _origin, char? _input), char?> CGIMiddleAddDict = new Dictionary<(char? _origin, char? _input), char?>()
        {
            // 'ㅣ' 추가
            {('ㅡ', 'ㅣ'), 'ㅢ'},
            {('ㆍ', 'ㅣ'), 'ㅓ'},
            {('ᆢ', 'ㅣ'), 'ㅕ'},
            {('ㅏ', 'ㅣ'), 'ㅐ'},
            {('ㅑ', 'ㅣ'), 'ㅒ'},
            {('ㅓ', 'ㅣ'), 'ㅔ'},
            {('ㅕ', 'ㅣ'), 'ㅖ'},
            {('ㅜ', 'ㅣ'), 'ㅟ'},
            {('ㅗ', 'ㅣ'), 'ㅚ'},
            {('ㅘ', 'ㅣ'), 'ㅙ'},
            {('ㅝ', 'ㅣ'), 'ㅞ'},
            {('ㅠ', 'ㅣ'), 'ㅝ'},

            //'ㅡ' 추가
            {('ㆍ', 'ㅡ'), 'ㅗ'},
            {('ᆢ', 'ㅡ'), 'ㅛ'},

            //'ㆍ' 추가
            {('ㆍ', 'ㆍ'), 'ᆢ'},
            {('ᆢ', 'ㆍ'), 'ㆍ'},
            {('ㅡ', 'ㆍ'), 'ㅜ'},
            {('ㅜ', 'ㆍ'), 'ㅠ'},
            {('ㅠ', 'ㆍ'), 'ㅜ'},
            {('ㅣ', 'ㆍ'), 'ㅏ'},
            {('ㅏ', 'ㆍ'), 'ㅑ'},
            {('ㅑ', 'ㆍ'), 'ㅏ'},
            {('ㅚ', 'ㆍ'), 'ㅘ'},
            {('ㅘ', 'ㆍ'), 'ㅚ'},
        };

        private readonly Dictionary<(char? _origin, char? _input), char?> CGIFirstAddDict = new Dictionary<(char? _origin, char? _input), char?>()
        {
            //ㄱ
            {('ㄱ', 'ㄱ'), 'ㅋ'},
            {('ㅋ', 'ㄱ'), 'ㄲ'},
            {('ㄲ', 'ㄱ'), 'ㄱ'},
            //ㄴ
            {('ㄴ', 'ㄴ'), 'ㄹ'},
            {('ㄹ', 'ㄴ'), 'ㄴ'},
            //ㄷ
            {('ㄷ', 'ㄷ'), 'ㅌ'},
            {('ㅌ', 'ㄷ'), 'ㄸ'},
            {('ㄸ', 'ㄷ'), 'ㄷ'},
            //ㅂ
            {('ㅂ', 'ㅂ'), 'ㅍ'},
            {('ㅍ', 'ㅂ'), 'ㅃ'},
            {('ㅃ', 'ㅂ'), 'ㅂ'},
            //ㅅ
            {('ㅅ', 'ㅅ'), 'ㅎ'},
            {('ㅎ', 'ㅅ'), 'ㅆ'},
            {('ㅆ', 'ㅅ'), 'ㅅ'},
            //ㅈ
            {('ㅈ', 'ㅈ'), 'ㅊ'},
            {('ㅊ', 'ㅈ'), 'ㅉ'},
            {('ㅉ', 'ㅈ'), 'ㅈ'},
            //ㅇ
            {('ㅇ', 'ㅇ'), 'ㅁ'},
            {('ㅁ', 'ㅇ'), 'ㅇ'},
        };

        private readonly Dictionary<(char? _origin, char? _input), char?> CGISpecialAddDict = new Dictionary<(char? _origin, char? _input), char?>()
        {
            {('.', '.'), ','},
            {(',', '.'), '?'},
            {('?', '.'), '!'},
            {('!', '.'), '.'},
        };

        private readonly Dictionary<(int _base, int _add), int> middleAddDict = new Dictionary<(int _base, int _add), int>()
        {
            {(8, 0), 10},
            {(8, 1), 11},
            {(8, 20), 12},
            {(13, 4), 15},
            {(13, 5), 16},
            {(13, 20), 17},
            {(18, 20), 19}
        };

        private readonly Dictionary<(int _base, int _add), int> lastAddDict = new Dictionary<(int _base, int _add), int>()
        {
            {('ㄱ' - BasicUnicode.consonant, 'ㅅ' - BasicUnicode.consonant), 'ㄳ' - BasicUnicode.consonant}, //ㄳ
            {('ㄴ' - BasicUnicode.consonant, 'ㅈ' - BasicUnicode.consonant), 'ㄵ' - BasicUnicode.consonant}, //ㄵ
            {('ㄴ' - BasicUnicode.consonant, 'ㅎ' - BasicUnicode.consonant), 'ㄶ' - BasicUnicode.consonant}, //ㄶ
            {('ㄹ' - BasicUnicode.consonant, 'ㄱ' - BasicUnicode.consonant), 'ㄺ' - BasicUnicode.consonant}, //ㄺ
            {('ㄹ' - BasicUnicode.consonant, 'ㅁ' - BasicUnicode.consonant), 'ㄻ' - BasicUnicode.consonant}, //ㄻ
            {('ㄹ' - BasicUnicode.consonant, 'ㅂ' - BasicUnicode.consonant), 'ㄼ' - BasicUnicode.consonant}, //ㄼ
            {('ㄹ' - BasicUnicode.consonant, 'ㅅ' - BasicUnicode.consonant), 'ㄽ' - BasicUnicode.consonant}, //ㄽ
            {('ㄹ' - BasicUnicode.consonant, 'ㅌ' - BasicUnicode.consonant), 'ㄾ' - BasicUnicode.consonant}, //ㄾ
            {('ㄹ' - BasicUnicode.consonant, 'ㅍ' - BasicUnicode.consonant), 'ㄿ' - BasicUnicode.consonant}, //ㄿ
            {('ㄹ' - BasicUnicode.consonant, 'ㅎ' - BasicUnicode.consonant), 'ㅀ' - BasicUnicode.consonant}, //ㅀ
            {('ㅂ' - BasicUnicode.consonant, 'ㅅ' - BasicUnicode.consonant), 'ㅄ' - BasicUnicode.consonant}, //ㅄ
        };

        //유니코드 배열 자음 -> 초성
        private readonly Dictionary<int, int> consonantToFirstDict = new Dictionary<int, int>()
       {
           {0, 0}, {1, 1}, {3, 2}, {6, 3}, {7, 4},
           {8, 5}, {16, 6}, {17, 7}, {18, 8}, {20, 9},
           {21, 10}, {22, 11}, {23, 12}, {24, 13}, {25, 14},
           {26, 15}, {27, 16}, {28, 17}, {29, 18},
       };

        //유니코드 배열 자음 -> 종성
        private readonly Dictionary<int, int> consonantToLastDict = new Dictionary<int, int>()
        {
            {0, 0}, {1, 1}, {2, 2}, {3, 3}, {4, 4},
            {5, 5}, {6, 6}, {8, 7}, {9, 8}, {10, 9},
            {11, 10}, {12, 11}, {13, 12}, {14, 13}, {15, 14},
            {16, 15}, {17, 16}, {19, 17}, {20, 18}, {21, 19},
            {22, 20}, {24, 21}, {25, 22}, {26, 23}, {27, 24},
            {28, 25}, {29, 26}
        };

        //들어올 수 있는 자음 입력값
        private readonly char[] hangeulConsonantInput = {
            'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ',
            'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ',
            'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ',
            'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
        //들어올 수 있는 모음 입력값
        private readonly char[] hangeulVowelInput = {
            'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ',
            'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ',
            'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ',
            'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ',
            'ㅣ'};

        #endregion

        #region Show Merge
        public string ShowMerge()
        {
            return subString;
        }
        #endregion

        #region MergeAndPop
        //문자를 합친 다음 내보냅니다.
        public string MergeAndPop()
        {
            if (isPoped)
            {
                subString = null;
            }
            isPoped = true;
            charList.Clear();
            return subString;
        }
        #endregion

        #region InputKey
        //문자를 입력받을 때 합칠수 있을만큼 합치고 나머지는 내보냅니다.
        public string InputKey(char _key)
        {
            if (isPoped)
            {
                subString = null;
            }
            Merge(_key);
            return subString; //예시, 나중에 고칠것
        }
        #endregion

        #region Merge

        private void Merge(char? _key)
        {
            if (_key != null)
            {
                if (_key == '\b')
                {
                    charList.RemoveAt(charList.Count - 1);
                }
                charList.Add(Convert.ToInt32(_key));
            }

            if (charList.Count == 1)
            {
                //한글자일 경우 바로 변환
                subString = Convert.ToChar(charList[0]).ToString();
                return;
            }
            else
            {
                //우선 키패드 합치는 과정
                CheckChangeCGISpecialKey(_key);
                CheckChangeCGIVowelKey(_key);
                CheckChangeCGIConsonantKey(_key);

                SpiltListByCharacter();

                subString = "";
                for (int splitIndex = 0; splitIndex < SplitCharacterIndex.Count - 1; splitIndex++)
                {
                    if (SplitCharacterIndex[splitIndex] < SplitCharacterIndex[splitIndex + 1])
                    {
                        char? charOutput = MergeCharBySplitIndex(SplitCharacterIndex[splitIndex], SplitCharacterIndex[splitIndex + 1]);
                        if (charOutput != null)
                        {
                            subString += charOutput.ToString();
                        }
                    }
                }
                return;
            }
        }


        #region check CGI key func
        private void CheckChangeCGISpecialKey(char? _key)
        {
            if (".,?!".Contains(Convert.ToChar(charList[0])) && _key == '.')
            {
                charList[0] = (int)CGISpecialAddDict[(Convert.ToChar(charList[0]), '.')];
                charList.RemoveAt(charList.Count - 1);
                subString = Convert.ToChar(charList[0]).ToString();
                return;
            }
        }

        private void CheckChangeCGIVowelKey(char? _key)
        {
            if (CGIMiddleAddDict.ContainsKey((Convert.ToChar(charList[charList.Count - 2]), _key)))
            {
                //모음 합치기
                charList[charList.Count - 2] = CGIMiddleAddDict[(Convert.ToChar(charList[charList.Count - 2]), _key)];
                charList.RemoveAt(charList.Count - 1);
            }
        }

        private void CheckChangeCGIConsonantKey(char? _key)
        {
            if (CGIFirstAddDict.ContainsKey((Convert.ToChar(charList[charList.Count - 2]), _key)))
            {
                //자음 합치기
                charList[charList.Count - 2] = (int)CGIFirstAddDict[(Convert.ToChar(charList[charList.Count - 2]), _key)];
                charList.RemoveAt(charList.Count - 1);
            }
        }
        #endregion

        #endregion

        #region Clear
        //Clear
        public void Clear()
        {
            charList.Clear();
            subString = null;
            isPoped = false;
        }

        private void Clear(char? _key)
        {
            
        }

        #endregion

        #region main Func
        public HanguelMergebyCGI()
        {
            isPoped = false;
        }

        #endregion

        #region split list by character

        public void SpiltListByCharacter()
        {
            SplitCharacterIndex.Clear();
            voidSpaceIndex.Clear();

            SplitResetBoolean();

            for (int index = 0; index < charList.Count; index++)
            {
                //모음
                if (hangeulVowelInput.Contains(Convert.ToChar(charList[index])))
                {
                    SplitCheckMergePossibleVowel(index);
                }
                //자음
                else if (firstIndex.Contains(Convert.ToChar(charList[index])))
                {
                    SplitCheckMergePossibleConsonant(index);
                }
                else
                {
                    //띄어쓰기
                    if (Convert.ToChar(charList[index]) == ' ' &&
                    (splitIsPreviousConsonant || splitIsPreviousConsonant))
                    {
                        voidSpaceIndex.Add(index);
                    }

                    //조합이 안되는 경우
                    SplitCharacterIndex.Add(index);
                    SplitResetBoolean();
                }
            }
            SplitCharacterIndex.Add(charList.Count);
        }

        //모음
        private void SplitCheckMergePossibleVowel(int _index)
        {
            //자음 바로 앞 (결합)
            if (splitIsPreviousConsonant)
            {
                SplitCharacterIndex.Add(_index - 1);
                splitHasTwoLastVowel = false;
            }
            //모음 바로 앞
            else if (splitIsPreviousVowel)
            {
                SplitCharacterIndex.Add(_index);
                SplitResetBoolean();
            }
            else
            {
                SplitCharacterIndex.Add(_index);
                SplitResetBoolean();
            }
            splitHasVowel = true;
            splitIsPreviousConsonant = false;
            splitIsPreviousVowel = true;
        }

        //자음
        private void SplitCheckMergePossibleConsonant(int _index)
        {
            //자음 바로 앞
            if (splitIsPreviousConsonant)
            {
                if (splitHasVowel)
                {
                    //종성 위치
                    if (splitHasTwoLastVowel)
                    {
                        SplitCharacterIndex.Add(_index);
                        SplitResetBoolean();
                    }
                    else if (lastAddDict.ContainsKey( ((int)charList[_index - 1] - BasicUnicode.consonant, (int)charList[_index] - BasicUnicode.consonant) ))
                    {
                        splitHasTwoLastVowel = true;
                    }
                    else
                    {
                        SplitCharacterIndex.Add(_index);
                        SplitResetBoolean();
                    }
                }
                else
                {
                    //초성 위치
                    SplitCharacterIndex.Add(_index);
                    SplitResetBoolean();
                }
            }
            //모음 바로 앞
            else if (splitIsPreviousVowel)
            {
                if (!splitHasConsonant)
                {
                    SplitCharacterIndex.Add(_index - 1);
                }
            }
            else
            {
                SplitCharacterIndex.Add(_index);
                SplitResetBoolean();
            }

            splitHasConsonant = true;
            splitIsPreviousConsonant = true;
            splitIsPreviousVowel = false;
        }

        private void SplitResetBoolean()
        {
            splitIsPreviousConsonant = false;
            splitIsPreviousVowel = false;
            splitHasConsonant = false;
            splitHasVowel = false;
            splitHasTwoLastVowel = false;
        }

        #endregion

        #region merge char by split
        public char? MergeCharBySplitIndex(int _firstIndex, int _lastIndex)
        {
            int diff = _lastIndex - _firstIndex;
            if (diff == 0)
                return null;
            else if (diff == 1) {
                if (voidSpaceIndex.Contains(_firstIndex))
                {
                    return null;
                }
                else
                {
                    return Convert.ToChar(charList[_firstIndex]);
                }
            }
            else if (diff == 2)
                return MergeOneChar(charList[_firstIndex], charList[_firstIndex + 1], null);
            else if (diff == 3)
                return MergeOneChar(charList[_firstIndex], charList[_firstIndex + 1], charList[_firstIndex + 2]);
            else // 4
            {
                return MergeOneChar(charList[_firstIndex], charList[_firstIndex + 1],
                    lastAddDict[((int)charList[_firstIndex + 2] - BasicUnicode.consonant, (int)charList[_firstIndex + 3] - BasicUnicode.consonant)] + BasicUnicode.lastLetter);
            }
        }

        private char MergeOneChar(int? _first, int? _middle, int? _last)
        {
            //한글 합치기!(한글 이외의 것은 오류가 날 수 있음)
            if (_first != null)
            {
                if(_middle == null)
                {
                    return (char)_first;
                }

                if((int)_first >= BasicUnicode.consonant)
                {
                    _first = consonantToFirstDict[Convert.ToInt32(_first) - BasicUnicode.consonant];
                }else if ((int)_first >= BasicUnicode.firstLetter)
                {
                    _first -= BasicUnicode.firstLetter;
                }
            }

            if(_middle != null)
            {
                if(_first == null)
                {
                    return (char)_middle;
                }

                if ((int)_middle >= BasicUnicode.vowel)
                {
                    _middle = _middle - BasicUnicode.vowel;
                }else if((int)_middle >= BasicUnicode.middleLetter)
                {
                    _middle -= BasicUnicode.middleLetter;
                }
            }

            if(_last != null)
            {
                if ((int)_last >= BasicUnicode.consonant)
                {
                    _last = consonantToLastDict[(int)_last - BasicUnicode.consonant];
                }else if((int)_last >= BasicUnicode.lastLetter)
                {
                    _last -= BasicUnicode.lastLetter;
                }
            }


            //주의 : 초성, 중성, 종성의 유니코드 값은 그 집단에 속한 유니코드 값이어야 함
            if (_last == null)
            {
                return Convert.ToChar(BasicUnicode.full + (((int)_first) * 21 + (int)_middle) * 28);
            }
            else
            {
                return Convert.ToChar(BasicUnicode.full + (((int)_first) * 21 + (int)_middle) * 28 + (int)_last + 1);
            }
        }

        #endregion
    }
}
