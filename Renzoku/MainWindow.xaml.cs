using System.Drawing;
using System.Security.Policy;
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

        private int[,] matrix; // Для зберігання згенерованої матриці
        private int Sizefield = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateGrid()
        {
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();

            gameGrid.HorizontalAlignment = HorizontalAlignment.Center;

            GenerateGridLayout();
            GenerateTextBoxes();
            GenerateDots();

            ShowRandomNumbers(matrix);

        }

        // Метод для генерації сітки
        private void GenerateGridLayout()
        {
            for (int i = 0; i < Sizefield * 2 - 1; i++)
            {
                if (i % 2 == 0)
                    gameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45) }); // Для текстБоксів
                else
                    gameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) }); // Для крапок
            }

            for (int j = 0; j < Sizefield * 2 - 1; j++)
            {
                if (j % 2 == 0)
                    gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) }); // Для текстБоксів
                else
                    gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10) }); // Для крапок
            }
        }

        // Метод для генерації текстБоксів
        private void GenerateTextBoxes()
        {
            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
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

        private void GenerateDots()
        {
            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
                {
                    // Горизонтальна перевірка на зростання та спад
                    if (j < Sizefield - 1)
                    {
                        if ((matrix[i, j] + 1 == matrix[i, j + 1]) || (matrix[i, j] - 1 == matrix[i, j + 1])) // Зростання
                        {
                            Ellipse dot = CreateDot();
                            Grid.SetRow(dot, i * 2);
                            Grid.SetColumn(dot, j * 2 + 1);
                            gameGrid.Children.Add(dot);
                        }
                    }


                    // Вертикальна перевірка на зростання та спад
                    if (i < Sizefield - 1)
                    {
                        if ((matrix[i, j] + 1 == matrix[i + 1, j]) || (matrix[i, j] - 1 == matrix[i + 1, j])) // Зростання
                        {
                            Ellipse dot = CreateDot();
                            Grid.SetRow(dot, i * 2 + 1);
                            Grid.SetColumn(dot, j * 2);
                            gameGrid.Children.Add(dot);
                        }
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
            Sizefield = 4;
            matrix = GenerateMatrix(Sizefield);
            GenerateGrid();

        }

        private void Level5x5_Click(object sender, RoutedEventArgs e)
        {
            Sizefield = 5;
            matrix = GenerateMatrix(Sizefield);
            GenerateGrid();
        }

        private void Level7x7_Click(object sender, RoutedEventArgs e)
        {
            Sizefield = 7;
            matrix = GenerateMatrix(Sizefield);
            GenerateGrid();
        }

        private void Level9x9_Click(object sender, RoutedEventArgs e)
        {
            Sizefield = 9;
            this.Width = 520;
            this.Height = 700;
            this.CheckButton.Margin = new Thickness(0, 0, 0, -450);

            matrix = GenerateMatrix(Sizefield);
            GenerateGrid();

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Sizefield = 4;
            matrix = GenerateMatrix(Sizefield);
            GenerateGrid();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            matrix = GenerateMatrix(Sizefield);
            GenerateGrid();
        }


        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            // Проходження по всіх позиціях матриці
            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
                {
                    // Отримуємо TextBox за позицією
                    var textBox = (TextBox)gameGrid.Children.OfType<TextBox>()
                        .FirstOrDefault(tb => Grid.GetRow(tb) == i * 2 && Grid.GetColumn(tb) == j * 2);

                    // Скидаємо кольори рамки перед перевіркою
                    if (textBox != null)
                    {
                        textBox.BorderBrush = Brushes.Black; // Скидаємо колір

                        // Отримуємо текст з TextBox
                        string userInput = textBox.Text;

                        // Перевірка на коректність введеного числа
                        if (int.TryParse(userInput, out int userNumber))
                        {
                            // Якщо число введено неправильно, підсвічуємо бортик червоним
                            if (userNumber != matrix[i, j])
                            {
                                textBox.BorderBrush = Brushes.Red;
                                textBox.BorderThickness = new Thickness(2);

                            }
                        }
                        else
                        {
                            // Підсвічуємо, якщо введено не число
                            textBox.BorderBrush = Brushes.Red;
                            textBox.BorderThickness = new Thickness(2);

                        }
                    }
                }
            }
        }

    }
}
