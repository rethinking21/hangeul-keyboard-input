using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangeulKeyBoard.MergeManager;

namespace HangeulKeyBoard
{
    internal class Program
    {
        static void Main(string[] args)
        {

            HangeulMergebyShiftKey keyboard = new HangeulMergebyShiftKey();
            LetterMerge myLetterMerge = new LetterMerge(keyboard);

            myLetterMerge.InputKey('ㅂ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅜ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅔ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㄹ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅎ');
            Console.WriteLine(myLetterMerge.GetString());
        }
    }
}
