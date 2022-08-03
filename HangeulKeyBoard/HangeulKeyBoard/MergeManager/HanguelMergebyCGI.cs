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
        private string outString = null;
        private List<int?> charList = new List<int?>();
        private bool isPoped;
        private bool isHanguel;

        private List<int> SplitCharacterIndex = new List<int>();

        private char? extraString = null;

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
            {(0, 18), 2}, //ㄳ
            {(3, 21), 4}, //ㄵ
            {(3, 26), 5}, //ㄶ
            {(7, 0), 8}, //ㄺ
            {(7, 15), 9}, //ㄻ
            {(7, 16), 10}, //ㄼ
            {(7, 18), 11}, //ㄽ
            {(7, 24), 12}, //ㄾ
            {(7, 25), 13}, //ㄿ
            {(7, 26), 14}, //ㅀ
            {(16, 18), 17}, //ㅄ
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
            'ㅏ', 'ㅑ', 'ㅓ', 'ㅕ', 'ㅗ',
            'ㅛ', 'ㅜ', 'ㅠ', 'ㅡ', 'ㅣ',
            'ㅐ', 'ㅒ', 'ㅔ', 'ㅖ' };

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

        #region canMerge
        //문자를 합칠 수 있는지 확인합니다.
        private bool canMerge(char _key)
        {
            int _intKey = Convert.ToInt32(_key);
            if (subString == null)
            {
                //입력창이 비어있는 경우 -> 합치기 가능!
                return true;
            }
            else if (BasicUnicode.consonant <= _intKey && _intKey <= BasicUnicode.vowel + 21)
            {
                //입력받은 문자가 한글문자 키보드에 존재하는 경우 (자음 모음별 분리)
                if (_intKey >= BasicUnicode.vowel)
                {
                    //모음

                    return true;

                }
                else
                {
                    return true;
                }
            }
            else
            {
                //.,?! 문자 입력시
                if (".,?!".Contains(_key))
                {
                    return true;
                }

                //입력받은 문자가 한글문자 키보드에 존재하지 않은 경우
                return false;
            }
        }
        #endregion

        #region Merge

        private void Merge(char? _key)
        {
            if (_key != null)
            {
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
                CheckChangeCGISpecialKey();
                CheckChangeCGIVowelKey();
                CheckChangeCGIConsonantKey();

                #region merge char to string
                //하나씩 체크하면서 돌아가기
                subString = "";
                outString = ""; //출력전에 미리 나오는 것
                int?[] tempHangeul = new int?[3] { null, null, null };
                bool hasVowel = false;
                bool hasCons = false;
                bool hasOut = false;

                //글자 조합별로 체크로 분리시키기
                SpiltListByCharacter();

                for (int listIndex = 0; listIndex < charList.Count; listIndex++)
                {
                    char _char = Convert.ToChar(charList[listIndex]);

                    if (hangeulConsonantInput.Contains(_char))
                    {
                        //자음일 경우
                        if(listIndex == 0)
                        {
                            //처음 자음이 나온 경우
                            tempHangeul[0] = charList[0];
                            hasCons = true;
                        }
                        else if (hasVowel)
                        {
                            //모음이 이미 있을 경우 (종성)
                            if (hasOut)
                            {
                                //ex) 골ㅆ
                                tempHangeul[0] = charList[listIndex];
                            }
                            else
                            {
                                //나온게 없을 경우
                                if(tempHangeul[2] == null)
                                {
                                    tempHangeul[2] = consonantToLastDict[(int)charList[listIndex] - BasicUnicode.consonant] + BasicUnicode.lastLetter;
                                }
                                else
                                {
                                    //종성을 합칠 수 있는지 보기
                                    if (lastAddDict.ContainsKey(((int)tempHangeul[2], (int)charList[listIndex] - BasicUnicode.consonant)))
                                    {
                                        tempHangeul[2] = lastAddDict[((int)tempHangeul[2], (int)charList[listIndex] - BasicUnicode.consonant)] + BasicUnicode.lastLetter;
                                    }
                                    else
                                    {
                                        //합칠 수 없을 때 초성 으로 빼내기
                                        hasOut = true;
                                        outString += MergeOneChar(Convert.ToChar(tempHangeul[0]), Convert.ToChar(tempHangeul[1]), Convert.ToChar(tempHangeul[2])).ToString();
                                        tempHangeul[0] = charList[listIndex];
                                        tempHangeul[1] = null;
                                        tempHangeul[2] = null;
                                        hasVowel = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //모음이 없을 경우 (초성)
                            if (hasOut)
                            {
                                //해당 사항 없을듯
                            }
                            else
                            {

                            }
                        }
                    }
                    else if (hangeulVowelInput.Contains(_char))
                    {
                        //모음일 경우
                        if(listIndex == 0)
                        {
                            //처음 모음이 나온 경우
                            tempHangeul[1] = charList[0];
                            hasVowel = true;
                        }
                        else if (hasCons)
                        {
                            //자음이 나온 경우
                            if (hasVowel)
                            {
                                //모음도 나온 경우
                                tempHangeul[1] = middleAddDict[((int)tempHangeul[1], (int)charList[listIndex])];
                            }
                            else
                            {
                                //모음이 처음인 경우
                                tempHangeul[1] = charList[listIndex];
                                hasVowel = true;
                            }
                        }
                        else
                        {
                            //문자가 미리 나온 경우 ex)곬ㅗ -> 골소
                        }
                    }
                    else
                    {
                        //특문?
                        subString = _char.ToString();
                    }
                }
                #endregion

                // 임시
                subString = Convert.ToChar(charList[0]).ToString();
                return;

                //스페이스 키일 경우 분리 시키기
                //마지막 리스트와 
            }
        }

        private char MergeOneChar(char? _first, char? _middle, char? _last)
        {
            //초성, 중성, 종성의 유니코드 값이 아닐경우, 여기서 변경하기
            
            //한글 합치기!(한글 이외의 것은 오류가 날 수 있음)
            //주의 : 초성, 중성, 종성의 유니코드 값은 그 집단에 속한 유니코드 값이어야 함
            if (_first == null)
            {
                return (char)_middle;
            }
            else if (_middle == null)
            {
                return (char)_first;
            }
            else if (_last == null)
            {
                return Convert.ToChar(BasicUnicode.full + (((int)_first - BasicUnicode.firstLetter) * 21 + (int)_middle - BasicUnicode.middleLetter) * 28);
            }
            else
            {
                return Convert.ToChar(BasicUnicode.full + (((int)_first - BasicUnicode.firstLetter) * 21 + (int)_middle - BasicUnicode.middleLetter) * 28 + (int)_last - BasicUnicode.lastLetter + 1);
            }
        }

        #region check CGI key func
        private void CheckChangeCGISpecialKey()
        {
            if (".,?!".Contains(Convert.ToChar(charList[0])) && _key == '.')
            {
                charList[0] = (int)CGISpecialAddDict[(Convert.ToChar(charList[0]), '.')];
                charList.RemoveAt(charList.Count - 1);
                subString = Convert.ToChar(charList[0]).ToString();
                return;
            }
        }

        private void CheckChangeCGIVowelKey()
        {
            if (CGIMiddleAddDict.ContainsKey((Convert.ToChar(charList[charList.Count - 2]), _key)))
            {
                //모음 합치기
                charList[charList.Count - 2] = CGIMiddleAddDict[(Convert.ToChar(charList[charList.Count - 2]), _key)];
                charList.RemoveAt(charList.Count - 1);
            }
        }

        private void CheckChangeCGIConsonantKey()
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

        #region InputKey
        //문자를 입력받을 때 합칠수 있을만큼 합치고 나머지는 내보냅니다.
        public string InputKey(char _key)
        {
            if (isPoped)
            {
                subString = null;
            }

            if (canMerge(_key))
            {
                Merge(_key);
                //합치면서 내놓는 값이 있을 수 있음!
                return null;
            }
            else
            {
                //모음이 들어올 때 앞의 자음을 남겨두는 방법을 고려해야 함
                Clear(_key);
                isPoped = true;
                return subString;
            }
        }
        #endregion

        #region Clear
        //Clear
        public void Clear()
        {
            charList.Clear();
            subString = null;
            isPoped = false;
            isHanguel = true;
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

        public void SpiltListByCharacter()
        {
            SplitCharacterIndex.Clear();
            SplitCharacterIndex.Add(0);

            bool isPreviousConsonant = false;
            bool isPreviousVowel = false;
            bool hasVowel = false;
            bool hasConsonant = false;
            int firstIndex = 0;

            for (int index = 0; index < charList.Count; index++)
            {
                if (hangeulVowelInput.Contains(Convert.ToChar(charList)))
                {
                    if (isPreviousConsonant)
                    {
                        isPreviousConsonant = false;

                    }
                }else if (hangeulConsonantInput.Contains(Convert.ToChar(charList)))
                {

                }
                else
                {
                    //조합이 안되는 경우
                    SplitCharacterIndex.Add(index);
                }
            }


            SplitCharacterIndex.Add(charList.Count);
        }

        public char MergeCharBySplitIndex(int _firstIndex, int _lastIndex)
        {
            return '0';
        }
    }
}
