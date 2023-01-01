using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PawnShop.Forms.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsDigit(this string text)
        {
            foreach (var ch in text)
            {
                if (char.IsDigit(ch))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsSpecialChar(this string text)
        {
            foreach (var ch in text)
            {
                if (((ch != '`' && ch != '-') && char.IsPunctuation(ch) || char.IsSeparator(ch) || char.IsControl(ch) || char.IsSymbol(ch)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsEmail(this string text)
        {
            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            return regex.IsMatch(text);
        }

        public static bool IsPasswordValid(this string text, out string errorMessage)
        {
            errorMessage = string.Empty;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(text))
            {
                errorMessage = "Password should contain At least one lower case letter";
                return false;
            }

            if (!hasUpperChar.IsMatch(text))
            {
                errorMessage = "Password should contain At least one upper case letter";
                return false;
            }

            if (!hasMiniMaxChars.IsMatch(text))
            {
                errorMessage = "Password should not be less than or greater than 12 characters";
                return false;
            }

            if (!hasNumber.IsMatch(text))
            {
                errorMessage = "Password should contain At least one numeric value";
                return false;
            }

            if (!hasSymbols.IsMatch(text))
            {
                errorMessage = "Password should contain At least one special case characters";
                return false;
            }

            return true;
        }

        public static bool ContainsDigitOrSpecialChar(this string text)
        {
            return string.IsNullOrEmpty(text) || text.ContainsDigit() || text.ContainsSpecialChar();
        }
    }
}
