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

            HanguelMergebyCGI keyboard = new HanguelMergebyCGI();
            LetterMerge myLetterMerge = new LetterMerge(keyboard);

            myLetterMerge.InputKey('ㄱ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㄱ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㄱ');
            Console.WriteLine(myLetterMerge.GetString());
        }
    }
}
