using System.Windows.Forms;

namespace Genetic_Maze
{
    public static class InputHandler
    {
        public static int GetInt(TextBox textBox)
        {
            int value;
            if (int.TryParse(textBox.Text, out value))
            {
                return value;
            }
            else
            {
                // Обработка ошибки ввода, например, вывести сообщение или предоставить значение по умолчанию
                return 0;
            }
        }
    }
}