using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangeulKeyBoard.MergeManager
{
    public class HangeulMergebyShiftKey : IHangeulMerge
    {
        public class MergeSetting
        {
            //아직 안만듬
            public bool mergeDoubleConsonant = false;
        }

        public MergeSetting setting;

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
            'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 
            'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 
            'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 
            'ㅎ' };

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
            {(0,20), 1},
            {(3,23), 4},
            {(3,29), 5},
            {(8, 0), 9},
            {(8, 16), 10},
            {(8, 17), 11},
            {(8, 20), 12},
            {(8, 27), 13},
            {(8, 28), 14},
            {(8, 29), 15},
            {(17, 20), 19},
        };

        private readonly char[] hangeulConsonantInput = {
            'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ',
            'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ',
            'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ',
            'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
        private readonly char[] hangeulVowelInput = {
            'ㅏ', 'ㅑ', 'ㅓ', 'ㅕ', 'ㅗ',
            'ㅛ', 'ㅜ', 'ㅠ', 'ㅡ', 'ㅣ', 
            'ㅐ', 'ㅒ', 'ㅔ', 'ㅖ' };
        

        private char? subString;
        private List<int?> charList = new List<int?>();
        private bool isPoped;
        private bool isHanguel;
        private bool hasVowel;

        //Clear
        public void Clear()
        {
            charList.Clear();
            subString = null;
            hasVowel = false;
            isPoped = false;
            hasVowel = false;
            isHanguel = true;
        }

        //문자를 합친 다음 내보냅니다.
        public char? MergeAndPop()
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

        public char? ShowMerge()
        {
            return subString;
        }

        //문자를 합칠 수 있는지 확인합니다.
        private bool canMerge(char _key)
        {
            if(subString == null)
            {
                //입력창이 비어있는 경우
                return true;
            }else if(BasicUnicode.consonant <= Convert.ToInt32(_key) && Convert.ToInt32(_key) <= BasicUnicode.vowel + 21)
            {
                //입력받은 문자가 한글문자 키보드에 존재하는 경우


                return true;
            }
            else
            {
                //입력받은 문자가 한글문자 키보드에 존재하지 않은 경우
                return false;
            }
        }

        //문자를 입력받을 때 합칠수 있을만큼 합치고 나머지는 내보냅니다.
        public char? InputKey(char _key)
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

        private void Merge(char? _key)
        {
            if (_key != null)
            {
                charList.Add(Convert.ToInt32(_key));
            }

            if (charList.Count == 1)
            {
                //한글자일 경우 바로 변환
                subString = Convert.ToChar(charList[0]);
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
                                tempHangeul[0] = tempInt - BasicUnicode.consonant + BasicUnicode.firstLetter;
                            }
                        }
                        else
                        {
                            //종성
                            if (tempHangeul[3] == null)
                            {
                                //더 좋은 방법?
                                tempHangeul[3] = Array.IndexOf(lastIndex, tempInt) + BasicUnicode.lastLetter;
                            }
                            else
                            {
                                tempHangeul[3] = lastAddDict[((int)tempHangeul[3] - BasicUnicode.lastLetter, tempInt - BasicUnicode.consonant)]
                                    + BasicUnicode.lastLetter;
                            }

                        }
                    }
                }
                //합치기!
            }
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
                subString = _key;
                return;
            }
        }

        public HangeulMergebyShiftKey()
        {
            setting = new MergeSetting();
            setting.mergeDoubleConsonant = true;
            isPoped = false;
        }
    }
}
