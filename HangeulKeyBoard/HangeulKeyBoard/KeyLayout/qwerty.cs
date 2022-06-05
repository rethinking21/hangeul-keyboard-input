using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangeulKeyBoard.KeyLayout
{
    public class qwerty
    {
        string hanguelIndex = "ㅂㅈㄷㄱㅅㅛㅕㅑㅐㅔㅁㄴㅇㄹㅎㅗㅓㅏㅣㅋㅌㅊㅍㅠㅜㅡ";
        string hanguelShiftIndex = "ㅃㅉㄸㄲㅆㅛㅕㅑㅒㅖㅁㄴㅇㄹㅎㅗㅓㅏㅣㅋㅌㅊㅍㅠㅜㅡ";
        string engIndex = "qwertyuiopasdfghjklzxcvbnm";
        string engShiftIndex = "QWERTYUIOPASDFGHJKLZXCVBNM";

        //one key and dont use shift
        public char EngToHangeul(char _input)
        {
            if (engIndex.Contains(_input))
            {
                return hanguelIndex[engIndex.IndexOf(_input.ToString())];
            }
            else if (engShiftIndex.Contains(_input))
            {
                return hanguelShiftIndex[engShiftIndex.IndexOf(_input.ToString())];
            }
            return _input;
        }

        public char EngToHangeul(string _input)
        {
            return EngToHangeul(_input[0]);
        }

        public char EngToHangeul(string _input, bool _isShiftKeyDown)
        {
            if (_isShiftKeyDown)
            {
                return EngToHangeul(_input.ToUpper()[0]);
            }
            else
            {
                return EngToHangeul(_input.ToLower()[0]);
            }
        }
        //;;;
        public char EngToHangeul(char _input, bool _isShiftKeyDown)
        {
            if (_isShiftKeyDown)
            {
                return EngToHangeul(_input.ToString().ToUpper()[0]);
            }
            else
            {
                return EngToHangeul(_input.ToString().ToLower()[0]);
            }
        }

        public char HanguelToEng(char _input)
        {
            if (hanguelIndex.Contains(_input))
            {
                return engIndex[hanguelIndex.IndexOf(_input.ToString())];
            }else if (hanguelShiftIndex.Contains(_input))
            {
                return engShiftIndex[hanguelShiftIndex.IndexOf(_input.ToString())];
            }
            return _input;
        }
        
        
    }
}
