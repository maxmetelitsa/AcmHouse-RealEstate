using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace AcmHouse.Functions
{
    public class MaskedTextBox : TextBox
    {
        // Регулярное выражение для маски
        private string _mask;

        public string Mask
        {
            get { return _mask; }
            set
            {
                _mask = value;
                Text = ApplyMask(Text);
            }
        }

        public MaskedTextBox()
        {
            // Обработчики событий для ввода текста
            PreviewTextInput += MaskedTextBox_PreviewTextInput;
            PreviewKeyDown += MaskedTextBox_PreviewKeyDown;
        }

        private void MaskedTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Применяем маску при вводе нового символа
            Text = ApplyMask(Text + e.Text);
            e.Handled = true;
        }

        private void MaskedTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Удаление символов при нажатии Backspace или Delete
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                Text = ApplyMask(string.Empty);
                e.Handled = true;
            }
        }

        // Применяем маску к тексту
        private string ApplyMask(string input)
        {
            if (string.IsNullOrEmpty(_mask))
                return input;

            string result = string.Empty;
            int maskIndex = 0;

            foreach (char maskChar in _mask)
            {
                if (maskIndex >= input.Length)
                    break;

                if (maskChar == '#')
                {
                    if (char.IsDigit(input[maskIndex]))
                    {
                        result += input[maskIndex];
                        maskIndex++;
                    }
                    else
                    {
                        break; // Пропускаем нечисловые символы
                    }
                }
                else
                {
                    result += maskChar; // Копируем символ из маски
                }
            }

            return result;
        }
    }
}
