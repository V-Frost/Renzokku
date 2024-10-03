using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Renzoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private int Sizefield = 0;

        private void GenerateGrid(int rows, int columns, int[,] matrix)
        {
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();

            gameGrid.HorizontalAlignment = HorizontalAlignment.Center;

            GenerateGridLayout(rows, columns);
            GenerateTextBoxes(rows, columns);
            GenerateDots(rows, columns, matrix);

            ShowRandomNumbers(matrix);

        }

        // Метод для генерації сітки
        private void GenerateGridLayout(int rows, int columns)
        {
            for (int i = 0; i < rows * 2 - 1; i++)
            {
                if (i % 2 == 0)
                    gameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45) }); // Для текстБоксів
                else
                    gameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) }); // Для крапок
            }

            for (int j = 0; j < columns * 2 - 1; j++)
            {
                if (j % 2 == 0)
                    gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) }); // Для текстБоксів
                else
                    gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10) }); // Для крапок
            }
        }

        // Метод для генерації текстБоксів
        private void GenerateTextBoxes(int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    TextBox textBox = new TextBox
                    {
                        Margin = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,

                        TextAlignment = TextAlignment.Center, 
                        VerticalContentAlignment = VerticalAlignment.Center, 
                        Padding = new Thickness(0),
                        BorderThickness = new Thickness(0.7), 
                        BorderBrush = Brushes.Black, 
                        Height = 40, 
                        FontWeight = FontWeights.SemiBold,
                        FontSize = 18
                    };

                    Grid.SetRow(textBox, i * 2); 
                    Grid.SetColumn(textBox, j * 2); 
                    gameGrid.Children.Add(textBox);
                }
            }
        }

        private void GenerateDots(int rows, int columns, int[,] matrix)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Горизонтальна перевірка на послідовність
                    if (j < columns - 1 && matrix[i, j] + 1 == matrix[i, j + 1])
                    {
                        Ellipse dot = CreateDot();
                        // Додаємо крапку в "непарний" стовпець між текстБоксами
                        Grid.SetRow(dot, i * 2);
                        Grid.SetColumn(dot, j * 2 + 1);
                        gameGrid.Children.Add(dot);
                    }

                    // Вертикальна перевірка на послідовність
                    if (i < rows - 1 && matrix[i, j] + 1 == matrix[i + 1, j])
                    {
                        Ellipse dot = CreateDot();
                        // Додаємо крапку в "непарний" рядок між текстБоксами
                        Grid.SetRow(dot, i * 2 + 1);
                        Grid.SetColumn(dot, j * 2);
                        gameGrid.Children.Add(dot);
                    }
                }
            }
        }

        // Метод для створення крапки
        private Ellipse CreateDot()
        {
            return new Ellipse
            {
                Width = 7,
                Height = 7,
                Fill = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        private int[,] GenerateMatrix(int size)
        {
            int[,] matrix = new int[size, size];
            Random random = new Random();

            // Створюємо базову матрицю
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = (j + i) % size + 1; // Заповнюємо базову матрицю
                }
            }

            // Перемішуємо рядки
            for (int i = 0; i < size; i++)
            {
                int swapIndex = random.Next(size);
                for (int j = 0; j < size; j++)
                {
                    // Обмінюємо рядки
                    int temp = matrix[i, j];
                    matrix[i, j] = matrix[swapIndex, j];
                    matrix[swapIndex, j] = temp;
                }
            }

            // Перемішуємо стовпці
            for (int j = 0; j < size; j++)
            {
                int swapIndex = random.Next(size);
                for (int i = 0; i < size; i++)
                {
                    // Обмінюємо стовпці
                    int temp = matrix[i, j];
                    matrix[i, j] = matrix[i, swapIndex];
                    matrix[i, swapIndex] = temp;
                }
            }

            return matrix;
        }

        private void ShowRandomNumbers(int[,] matrix)
        {
            Random random = new Random();

            int numbersToShow = random.Next(1, 3); 

            for (int i = 0; i < numbersToShow; i++)
            {
                int row, column;

                // Генеруємо випадкові позиції для чисел
                do
                {
                    row = random.Next(0, matrix.GetLength(0)); 
                    column = random.Next(0, matrix.GetLength(1));
                } while (matrix[row, column] == 0);

     
                TextBox textBox = (TextBox)gameGrid.Children.OfType<TextBox>().FirstOrDefault(tb => Grid.GetRow(tb) == row * 2 && Grid.GetColumn(tb) == column * 2);

                if (textBox != null)
                {
                    textBox.Text = matrix[row, column].ToString(); 
                }
            }
        }

        private void Level4x4_Click(object sender, RoutedEventArgs e)
        {
            int[,] matrix = GenerateMatrix(4); 
            GenerateGrid(4, 4, matrix);
            Sizefield = 4;
        }

        private void Level5x5_Click(object sender, RoutedEventArgs e)
        {
            int[,] matrix = GenerateMatrix(5); 
            GenerateGrid(5, 5, matrix);
            Sizefield = 5;
        }

        private void Level7x7_Click(object sender, RoutedEventArgs e)
        {
            int[,] matrix = GenerateMatrix(7); 
            GenerateGrid(7, 7, matrix);
            Sizefield = 7;
        }

        private void Level9x9_Click(object sender, RoutedEventArgs e)
        {
            int[,] matrix = GenerateMatrix(9);
            this.Width = 520;
            this.Height = 700;
            this.CheckButton.Margin = new Thickness(0, 0, 0, -450);

            GenerateGrid(9, 9, matrix);
            Sizefield = 9;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int[,] matrix = GenerateMatrix(4); 
            GenerateGrid(4, 4, matrix);
            Sizefield = 4;

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int[,] matrix = GenerateMatrix(Sizefield);
            GenerateGrid(Sizefield, Sizefield, matrix);
        }

        private void CheckField(int rows, int columns, int[,] matrix)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Отримуємо текст з текстБокса
                    var textBox = (TextBox)gameGrid.Children[i * 2 * columns + j * 2];
                    string userInput = textBox.Text;

                    // Перевірка на коректність введеного числа
                    if (int.TryParse(userInput, out int userNumber))
                    {
                        // Якщо число введено неправильно, підсвічуємо бортик червоним
                        if (userNumber != matrix[i, j])
                        {
                            textBox.BorderBrush = Brushes.Red;
                        }
                        else
                        {
                            textBox.BorderBrush = Brushes.Black; // Якщо правильно, скидаємо колір
                        }
                    }
                    else
                    {
                        textBox.BorderBrush = Brushes.Red; // Підсвічуємо, якщо введено не число
                    }
                }
            }
        }
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            // Перевіряємо значення у текстБоксах за згенерованою матрицею
            
        }
    }
}