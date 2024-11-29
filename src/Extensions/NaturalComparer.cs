using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace PlayaApiV2.Extensions
{
    public class NaturalComparer : IComparer<string>, IComparer
    {
        public static readonly NaturalComparer Default = new NaturalComparer();

        private NaturalComparerOptions _naturalComparerOptions;
        private StringComparison _stringComparison;

        private enum TokenType
        {
            Nothing,
            Numerical,
            String
        }

        private struct StringParser
        {
            private TokenType _tokenType;
            private string _stringValue;
            private decimal _numericalValue;
            private int _idx;
            private string _source;
            private int _len;
            private char _curChar;
            private NaturalComparerOptions _naturalComparerOptions;

            public void Init(string source, NaturalComparerOptions naturalComparerOptions)
            {
                _naturalComparerOptions = naturalComparerOptions;
                if (source == null)
                    source = string.Empty;
                _source = source;
                _len = source.Length;
                _idx = -1;
                _numericalValue = 0;
                NextChar();
                NextToken();
            }

            public TokenType TokenType
            {
                get { return _tokenType; }
            }

            public decimal NumericalValue
            {
                get
                {
                    if (_tokenType == TokenType.Numerical)
                    {
                        return _numericalValue;
                    }
                    else
                    {
                        throw new NaturalComparerException("Internal Error: NumericalValue called on a non numerical value.");
                    }
                }
            }

            public string StringValue
            {
                get { return _stringValue; }
            }

            public void NextToken()
            {
                do
                {
                    //CharUnicodeInfo.GetUnicodeCategory 
                    if (_curChar == '\0')
                    {
                        _tokenType = TokenType.Nothing;
                        _stringValue = null;
                        return;
                    }
                    else if (char.IsDigit(_curChar))
                    {
                        ParseNumericalValue();
                        return;
                    }
                    else if (char.IsLetter(_curChar))
                    {
                        ParseString();
                        return;
                    }
                    else
                    {
                        //ignore this character and loop some more 
                        NextChar();
                    }
                }
                while (true);
            }

            private void NextChar()
            {
                _idx += 1;
                if (_idx >= _len)
                {
                    _curChar = '\0';
                }
                else
                {
                    _curChar = _source[_idx];
                }
            }

            private void ParseNumericalValue()
            {
                int start = _idx;
                char numberDecimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0];
                char numberGroupSeparator = NumberFormatInfo.CurrentInfo.NumberGroupSeparator[0];
                do
                {
                    NextChar();
                    if (_curChar == numberDecimalSeparator)
                    {
                        // parse digits after the Decimal Separator 
                        do
                        {
                            NextChar();
                            if (!char.IsDigit(_curChar) && _curChar != numberGroupSeparator)
                                break;

                        }
                        while (true);
                        break;
                    }
                    else
                    {
                        if (!char.IsDigit(_curChar) && _curChar != numberGroupSeparator)
                            break;
                    }
                }
                while (true);
                _stringValue = _source.Substring(start, _idx - start);
                if (decimal.TryParse(_stringValue, out _numericalValue))
                {
                    _tokenType = TokenType.Numerical;
                }
                else
                {
                    // We probably have a too long value 
                    _tokenType = TokenType.String;
                }
            }

            private void ParseString()
            {
                int start = _idx;
                bool roman = (_naturalComparerOptions & NaturalComparerOptions.RomanNumbers) != 0;
                int romanValue = 0;
                int lastRoman = int.MaxValue;
                int cptLastRoman = 0;
                do
                {
                    if (roman)
                    {
                        int thisRomanValue = RomanLetterValue(_curChar);
                        if (thisRomanValue > 0)
                        {
                            bool handled = false;

                            if ((thisRomanValue == 1 || thisRomanValue == 10 || thisRomanValue == 100))
                            {
                                NextChar();
                                int nextRomanValue = RomanLetterValue(_curChar);
                                if (nextRomanValue == thisRomanValue * 10 | nextRomanValue == thisRomanValue * 5)
                                {
                                    handled = true;
                                    if (nextRomanValue <= lastRoman)
                                    {
                                        romanValue += nextRomanValue - thisRomanValue;
                                        NextChar();
                                        lastRoman = thisRomanValue / 10;
                                        cptLastRoman = 0;
                                    }
                                    else
                                    {
                                        roman = false;
                                    }
                                }
                            }
                            else
                            {
                                NextChar();
                            }
                            if (!handled)
                            {
                                if (thisRomanValue <= lastRoman)
                                {
                                    romanValue += thisRomanValue;
                                    if (lastRoman == thisRomanValue)
                                    {
                                        cptLastRoman += 1;
                                        switch (thisRomanValue)
                                        {
                                            case 1:
                                            case 10:
                                            case 100:
                                                if (cptLastRoman > 4)
                                                    roman = false;

                                                break;
                                            case 5:
                                            case 50:
                                            case 500:
                                                if (cptLastRoman > 1)
                                                    roman = false;

                                                break;
                                        }
                                    }
                                    else
                                    {
                                        lastRoman = thisRomanValue;
                                        cptLastRoman = 1;
                                    }
                                }
                                else
                                {
                                    roman = false;
                                }
                            }
                        }
                        else
                        {
                            roman = false;
                        }
                    }
                    else
                    {
                        NextChar();
                    }
                    if (!char.IsLetter(_curChar)) break;
                }
                while (true);
                _stringValue = _source.Substring(start, _idx - start);
                if (roman)
                {
                    _numericalValue = romanValue;
                    _tokenType = TokenType.Numerical;
                }
                else
                {
                    _tokenType = TokenType.String;
                }
            }

        }

        public NaturalComparer(NaturalComparerOptions naturalComparerOptions, StringComparison stringComparison)
        {
            _naturalComparerOptions = naturalComparerOptions;
            _stringComparison = stringComparison;
        }

        public NaturalComparer() : this(NaturalComparerOptions.Default, StringComparison.Ordinal)
        {
        }

        public int Compare(string string1, string string2)
        {
            var parser1 = new StringParser();
            parser1.Init(string1, _naturalComparerOptions);
            var parser2 = new StringParser();
            parser2.Init(string2, _naturalComparerOptions);
            int result;
            do
            {
                if (parser1.TokenType == TokenType.Numerical & parser2.TokenType == TokenType.Numerical)
                {
                    // both string1 and string2 are numerical 
                    result = decimal.Compare(parser1.NumericalValue, parser2.NumericalValue);
                }
                else
                {
                    result = string.Compare(parser1.StringValue, parser2.StringValue, _stringComparison);
                }
                if (result != 0)
                {
                    return result;
                }
                else
                {
                    parser1.NextToken();
                    parser2.NextToken();
                }
            }
            while (!(parser1.TokenType == TokenType.Nothing & parser2.TokenType == TokenType.Nothing));
            //identical 
            return 0;
        }

        private static int RomanLetterValue(char c)
        {
            switch (c)
            {
                case 'I':
                    return 1;
                case 'V':
                    return 5;
                case 'X':
                    return 10;
                case 'L':
                    return 50;
                case 'C':
                    return 100;
                case 'D':
                    return 500;
                case 'M':
                    return 1000;
                default:
                    return 0;
            }
        }

        public int RomanValue(string string1)
        {
            var parser1 = new StringParser();
            parser1.Init(string1, _naturalComparerOptions);

            if (parser1.TokenType == TokenType.Numerical)
            {
                return (int)parser1.NumericalValue;
            }
            else
            {
                return 0;
            }
        }

        int IComparer.Compare(object x, object y)
        {
            return ((IComparer<string>)this).Compare((string)x, (string)y);
        }

        public class NaturalComparerException : System.Exception
        {

            public NaturalComparerException(string msg)
               : base(msg)
            {
            }
        }

        [System.Flags()]
        public enum NaturalComparerOptions
        {
            None,
            RomanNumbers,
            IgnoreCase,
            //DecimalValues <- we could put this as an option 
            //IgnoreSpaces <- we could put this as an option 
            //IgnorePunctuation <- we could put this as an option 
            Default = None
        }
    }
}
