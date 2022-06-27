using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangeulKeyBoard.MergeManager
{
    #region IHangeulMerge
    public interface IHangeulMerge
    {
        //clear
        void Clear();
        //현재 쌓인 음소들을 합쳐서 내보냅니다.
        string MergeAndPop();
        //현재까지 쌓인 음소들을 합친걸 보여줍니다.
        string ShowMerge();
        //키를 입력받으면서 조건이 되면 뱉어냅니다.
        string InputKey(char _key);
    }
    #endregion

    #region BasicUnicode(int)
    public class BasicUnicode
    {
        //https://ko.wikipedia.org/wiki/%EC%9C%A0%EB%8B%88%EC%BD%94%EB%93%9C_3000~3FFF
        public const int full = 44032; //55203
        public const int consonant = 12593; //12622
        public const int vowel = 12623; //12643
        public const int firstLetter = 4352; //4370
        public const int middleLetter = 4449; //4469
        public const int lastLetter = 4520; //4546
    }
    #endregion

    public class LetterMerge
    {
        #region declaration
        private StringBuilder mainString = new StringBuilder();
        private string subString;
        private IHangeulMerge mergeMethod;
        public bool isChanged;

        public string SubString
        {
            get
            {
                if (subString == null)
                {
                    return " ";
                }
                else
                {
                    return subString;
                }
            }
        }
        #endregion

        //현재 입력받은 문자를 보여줍니다.
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
            string _checkString = mergeMethod.InputKey(_key);
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
