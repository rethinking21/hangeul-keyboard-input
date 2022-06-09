using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangeulKeyBoard.MergeManager
{
    public interface IHangeulMerge
    {
        //clear
        void Clear();
        //현재 쌓인 음소들을 합쳐서 내보냅니다.
        char? MergeAndPop();
        //현재까지 쌓인 음소들을 합친걸 보여줍니다.
        char? ShowMerge();
        //키를 입력받으면서 조건이 되면 뱉어냅니다.
        char? InputKey(char _key);
    }

    public class BasicUnicode
    {
        public const int full = 44032; //55203
        public const int consonant = 12593; //12622
        public const int vowel = 12623; //12643
        public const int firstLetter = 4352; //4370
        public const int middleLetter = 4449; //4469
        public const int lastLetter = 4520; //4546
    }

    public class LetterMerge
    {
        private StringBuilder mainString = new StringBuilder();
        private char? subString;
        private IHangeulMerge mergeMethod;
        public bool isChanged;

        public char? SubString
        {
            get
            {
                if (subString == null)
                {
                    return ' ';
                }
                else
                {
                    return subString;
                }
            }
        }
        //현재 입력받는 
        public string GetString()
        {
            if (subString == null)
            {
                isChanged = false;
                return mainString.ToString();
            }
            else
            {
                isChanged = false;
                return mainString.ToString() + subString.ToString();
            }
        }
        public string GetStringWithoutSubString()
        {
            return mainString.ToString();
        }

        public void InputKey(char _key)
        {
            char? _checkString = mergeMethod.InputKey(_key);
            if (_checkString != null)
            {
                subString = null;
                mainString.Append(_checkString);
                isChanged = true;
            }
            else
            {
                subString = mergeMethod.ShowMerge();
                isChanged = true;
            }
        }

        public LetterMerge(IHangeulMerge _mergeMethod)
        {
            mergeMethod = _mergeMethod;
            subString = null;
            isChanged = false;
        }
    }
}
