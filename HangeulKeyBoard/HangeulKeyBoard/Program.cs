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
            myLetterMerge.InputKey('ㆍ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅡ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅣ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㄱ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅅ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅂ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅜ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey(' ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅅ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㆍ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅣ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅇ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey(' ');
            Console.WriteLine(myLetterMerge.GetString());
            myLetterMerge.InputKey('ㅇ');
            Console.WriteLine(myLetterMerge.GetString());

            myLetterMerge.ClearString();
            myLetterMerge.InputKey('ㅇ');
            Console.WriteLine(myLetterMerge.GetString());
        }
    }
}
