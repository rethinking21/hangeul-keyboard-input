using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangeulKeyBoard.MergeManager
{
    interface IHangeulMerge
    {
        //clear
        void Clear();
        //현재 쌓인 음소들을 합쳐서 내보냅니다.
        char? MergeAndPop();
        //현재까지 쌓인 음소들을 합친걸 보여줍니다.
        char? ShowMerge();
        //키를 입력받으면서 조건이 되면 뱉어냅니다.
        char? InputKey(char key);
    }

    public class LetterMerge
    {
        private StringBuilder mainString = new StringBuilder();

        private string[] subString;

        public string GetString()
        {
            return mainString.ToString();
        }
        public string GetStringWithoutSubString()
        {
            return mainString.ToString();
        }
    }
}
