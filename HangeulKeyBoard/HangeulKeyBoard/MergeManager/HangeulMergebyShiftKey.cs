using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangeulKeyBoard.MergeManager
{
    public class HangeulMergebyShiftKey : IHangeulMerge
    {
        //미완성
        #region declaration
        public class MergeSetting
        {
            //아직 안만듬
            public bool mergeDoubleConsonant = false;
        }

        public MergeSetting setting;


        
        private string subString = null;
        private List<int?> charList = new List<int?>();
        private bool isPoped;
        private bool isHanguel;
        private bool hasVowel;

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
            'ㅣ'};
        private readonly char[] lastIndex = {
            'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 
            'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ',
            'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ',
            'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 
            'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 
            'ㅍ', 'ㅎ' };

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
                hasVowel = false;
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
            }else if(BasicUnicode.consonant <= _intKey && _intKey <= BasicUnicode.vowel + 21)
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
                //입력받은 문자가 한글문자 키보드에 존재하지 않은 경우
                return false;
            }
        }
        #endregion

        #region InputKey
        //문자를 입력받을 때 합칠수 있을만큼 합치고 나머지는 내보냅니다.
        public string InputKey(char _key)
        {
            if (isPoped)
            {
                subString = null;
                hasVowel = false;
            }

            if (canMerge(_key))
            {
                Merge(_key);
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
                int?[] tempHangeul = new int?[3] { null, null, null };

                foreach(int tempInt in charList)
                {
                    //자음인지 모음인지 확인
                    if (tempInt >= BasicUnicode.vowel)
                    {
                        if (tempHangeul[1] == null)
                        {
                            //첫 모음 저장
                            tempHangeul[1] = tempInt - BasicUnicode.vowel + BasicUnicode.middleLetter;
                        }
                        else
                        {
                            //겹치는 모음 확인
                            tempHangeul[1] = middleAddDict[((int)tempHangeul[1] - BasicUnicode.middleLetter, tempInt - BasicUnicode.vowel)]
                                + BasicUnicode.middleLetter;
                        }
                    }
                    else
                    {
                        if (tempHangeul[1] == null)
                        {
                            //초성
                            if (setting.mergeDoubleConsonant)
                            {
                                //쌍자음 관련
                            }
                            else
                            {
                                tempHangeul[0] = consonantToFirstDict[tempInt - BasicUnicode.consonant] + BasicUnicode.firstLetter;
                            }
                        }
                        else
                        {
                            //종성
                            if (tempHangeul[2] == null)
                            {
                                //더 좋은 방법?
                                tempHangeul[2] = Array.IndexOf(lastIndex, Convert.ToChar(tempInt)) + BasicUnicode.lastLetter;
                            }
                            else
                            {
                                tempHangeul[2] = lastAddDict[((int)tempHangeul[2] - BasicUnicode.lastLetter, consonantToLastDict[tempInt - BasicUnicode.consonant])]
                                    + BasicUnicode.lastLetter;
                            }

                        }
                    }
                }
                //합치기!
                if (tempHangeul[0] == null)
                {
                    subString = Convert.ToChar(tempHangeul[1]).ToString();
                    return;
                }else if (tempHangeul[1] == null)
                {
                    subString = Convert.ToChar(tempHangeul[0]).ToString();
                    return;
                }
                else if(tempHangeul[2] == null)
                {
                    subString = Convert.ToChar(BasicUnicode.full + ((tempHangeul[0] - BasicUnicode.firstLetter) * 21 + tempHangeul[1] - BasicUnicode.middleLetter) * 28 ).ToString();
                    return;
                }
                else
                {
                    subString = Convert.ToChar(BasicUnicode.full + ((tempHangeul[0] - BasicUnicode.firstLetter) * 21 + tempHangeul[1] - BasicUnicode.middleLetter) * 28 + tempHangeul[2] - BasicUnicode.lastLetter + 1).ToString();
                    return;
                }
            }
        }

        #endregion

        #region Clear
        //Clear
        public void Clear()
        {
            charList.Clear();
            subString = null;
            hasVowel = false;
            isPoped = false;
            isHanguel = true;
        }

        private void Clear(char? _key)
        {
            //갉ㅣ => 갈기 이렇게 되는 경우를 고려해야 함
            if (hangeulConsonantInput.Contains((char)_key) || hangeulVowelInput.Contains((char)_key))
            {
                if (hangeulConsonantInput.Contains((char)charList.Last()) && hangeulVowelInput.Contains((char)_key))
                {
                    //두번 눌러서 쌍자음이 되는 경우도 고려해야함
                    if (setting.mergeDoubleConsonant)
                    {

                    }
                    else
                    {
                        int? tempChar = charList.Last();
                        Merge(null);
                        charList.Clear();
                        charList.Add(Convert.ToInt32(tempChar));
                        charList.Add(Convert.ToInt32(_key));
                        return;
                    }
                }
            }
            else
            {
                charList.Clear();
                charList.Add(Convert.ToInt32(_key));
                subString = _key.ToString();
                return;
            }
        }

        #endregion

        #region main Func
        public HangeulMergebyShiftKey()
        {
            setting = new MergeSetting();
            setting.mergeDoubleConsonant = false;
            isPoped = false;
        }

        #endregion
    }
}
