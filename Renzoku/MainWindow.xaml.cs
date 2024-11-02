using Renzokku;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Renzoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int[,] matrix = null!;
        private int Sizefield = 0;
        private TextBox[,] textBoxGrid = null!;
        public TextBox lastFocusedTextBox { get; private set; }
        public Dictionary<TextBox, BorderInfo> textBoxBorders = new Dictionary<TextBox, BorderInfo>();
        private DispatcherTimer timer = null!;
        private TimeSpan timeElapsed;
        private bool isPaused = false;
        private bool highlightErrors = false;
        private bool highlightRowAndColumn = false;
        public bool highlightSameNumbers = false;
        public bool areNumberButtonsVisible = true;
        public Button activeNumberButton = null; 
        private Setting settingsWindow;
        public bool highlightActiveButton = false; // Змінна для контролю підсвічування

        public MainWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
        }

        private void GenerateGrid()
        {
            // Очищаємо і перегенеруємо поле
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();
            buttonPanel.Children.Clear();

            gameGrid.HorizontalAlignment = HorizontalAlignment.Center;
            gameGrid.VerticalAlignment = VerticalAlignment.Center;

            GenerateGridLayout();
            GeneratePlayingField();
            GenerateDots();
            ShowNumberButtons(areNumberButtonsVisible); 
            UpdateGameFieldAlignment(areNumberButtonsVisible); 
            ShowRandomNumbers();

            ResetTimer();
        }

        private void GenerateGridLayout()
        {
            for (int i = 0; i < Sizefield * 2 - 1; i++)
            {
                if (i % 2 == 0)
                    gameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45) });
                else
                    gameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });
            }

            for (int j = 0; j < Sizefield * 2 - 1; j++)
            {
                if (j % 2 == 0)
                    gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
                else
                    gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10) });
            }

            gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
        }

        private void GeneratePlayingField()
        {
            textBoxGrid = new TextBox[Sizefield, Sizefield];

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

                    textBox.GotFocus += TextBox_GotFocus;
                    textBox.LostFocus += TextBox_LostFocus; 
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    textBox.TextChanged += TextBox_TextChanged; 

                    Grid.SetRow(textBox, i * 2);
                    Grid.SetColumn(textBox, j * 2);
                    gameGrid.Children.Add(textBox);
                    textBoxGrid[i, j] = textBox;
                }

                Button numberButton = new Button
                {
                    Height = 35,
                    Width = 35,
                    Content = (i + 1).ToString(),
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,

                };
                numberButton.Click += numberButton_Click;

                Grid.SetRow(numberButton, i * 2);
                Grid.SetColumn(numberButton, Sizefield * 2);
                gameGrid.Children.Add(numberButton);
            }

            Button settingsGameField = new Button
            {
                Height = 28,
                Width = 28,
                Margin = new Thickness(5)
            };

            settingsGameField.Click += SettingsGameField_Click;


            ImageSource setting = new BitmapImage(new Uri("pack://application:,,,/Image/settings.png"));
            settingsGameField.Content = new Image { Source = setting };

            Button checkGamingField = new Button
            {
                Height = 28,
                Width = 100,
                Content = "Перевірити",
                Margin = new Thickness(5)
            };

            checkGamingField.Click += CheckButton_Click;

            Button resetButton = new Button
            {
                Height = 28,
                Width = 28,
                Margin = new Thickness(5)
            };

            resetButton.Click += ResetButton_Click;
            ImageSource reset = new BitmapImage(new Uri("pack://application:,,,/Image/refresh.png"));
            resetButton.Content = new Image { Source = reset };

            buttonPanel.Children.Add(settingsGameField);
            buttonPanel.Children.Add(checkGamingField);
            buttonPanel.Children.Add(resetButton);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                int row = -1, col = -1;

                for (int i = 0; i < Sizefield; i++)
                {
                    for (int j = 0; j < Sizefield; j++)
                    {
                        if (textBoxGrid[i, j] == textBox)
                        {
                            row = i;
                            col = j;
                            break;
                        }
                    }
                    if (row != -1) break;
                }

                if (highlightErrors && row != -1 && col != -1)
                {
                    if (int.TryParse(textBox.Text, out int value))
                    {
                        if (value != matrix[row, col]) textBox.Foreground = Brushes.Red;
                        else textBox.Foreground = Brushes.Black; 
                    }
                    else textBox.Foreground = Brushes.Black;
                }
                else textBox.Foreground = Brushes.Black;
            }
        }


        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (Char.IsDigit(e.Text, 0))
                {
                    int enteredNumber = int.Parse(e.Text);

                    if (enteredNumber > 0 && enteredNumber <= Sizefield)
                    {
                        textBox.Text = e.Text;
                        e.Handled = true;
                    }
                    else e.Handled = true; 
                }
                else e.Handled = true;
            }
        }

        public void SetTimerVisibility(bool isVisible)
        {
            if (isVisible)
            {
                timerTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                timerTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
            }

            TextBox currentTextBox = lastFocusedTextBox;
            if (currentTextBox == null) return;

            int row = -1, column = -1;
            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
                {
                    if (textBoxGrid[i, j] == currentTextBox)
                    {
                        row = i;
                        column = j;
                        break;
                    }
                }
                if (row != -1) break;
            }

            switch (e.Key)
            {
                case Key.Left:
                    if (column > 0) textBoxGrid[row, column - 1].Focus();
                    break;
                case Key.Right:
                    if (column < Sizefield - 1) textBoxGrid[row, column + 1].Focus();
                    break;
                case Key.Up:
                    if (row > 0) textBoxGrid[row - 1, column].Focus();
                    break;
                case Key.Down:
                    if (row < Sizefield - 1) textBoxGrid[row + 1, column].Focus();
                    break;
            }
        }

        private void SettingsGameField_Click(object sender, RoutedEventArgs e)
        {
            if (settingsWindow == null || !settingsWindow.IsVisible)
            {
                settingsWindow = new Setting(this);

                settingsWindow.Left = this.Left + this.Width;
                settingsWindow.Top = this.Top;

                settingsWindow.Closed += (s, args) => { settingsWindow = null; };

                settingsWindow.Show();
            }
            else
            {
                settingsWindow.Focus();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            matrix = GenerateMatrix();
            GenerateGrid();
        }

        private void ResetTextBoxBorders()
        {
            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
                {
                    TextBox textBox = textBoxGrid[i, j];
                    if (textBox != null)
                    {
                        textBox.BorderBrush = Brushes.Black;
                        textBox.BorderThickness = new Thickness(0.7);
                    }
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox focusedTextBox = sender as TextBox;

            // Зберігаємо початкові властивості, якщо це ще не зроблено
            if (!textBoxBorders.ContainsKey(focusedTextBox))
            {
                textBoxBorders[focusedTextBox] = new BorderInfo
                {
                    BorderBrush = focusedTextBox.BorderBrush,
                    BorderThickness = focusedTextBox.BorderThickness
                };
            }

            lastFocusedTextBox = focusedTextBox;

            // Логіка виділення рядка і стовпця
            if (highlightRowAndColumn)
            {
                int row = -1, col = -1;
                for (int i = 0; i < Sizefield; i++)
                {
                    for (int j = 0; j < Sizefield; j++)
                    {
                        if (textBoxGrid[i, j] == focusedTextBox)
                        {
                            row = i;
                            col = j;
                            break;
                        }
                    }
                    if (row != -1) break;
                }

                if (row != -1 && col != -1)
                {
                    HighlightRowAndColumn(row, col, Brushes.LightBlue);
                }
            }

            if (highlightSameNumbers && !string.IsNullOrWhiteSpace(focusedTextBox.Text))
            {
                string selectedNumber = focusedTextBox.Text;

                ResetHighlightForAllTextBoxes();

                for (int i = 0; i < Sizefield; i++)
                {
                    for (int j = 0; j < Sizefield; j++)
                    {
                        var textBox = textBoxGrid[i, j];

                        if (textBox != null && textBox.Text == selectedNumber)
                        {
                            textBox.Background = Brushes.LightCoral;
                        }
                    }
                }
            }
        }

        public void ResetHighlightForAllTextBoxes()
        {
            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
                {
                    var textBox = textBoxGrid[i, j];
                    if (textBox != null)
                    {
                        textBox.Background = Brushes.White;
                    }
                }
            }
        }

        public void ShowNumberButtons(bool show)
        {
            for (int i = 0; i < Sizefield; i++)
            {
                var numberButton = gameGrid.Children
                    .OfType<Button>()
                    .FirstOrDefault(b => Grid.GetColumn(b) == Sizefield * 2 && Grid.GetRow(b) == i * 2);

                if (numberButton != null)
                {
                    numberButton.Visibility = show ? Visibility.Visible : Visibility.Collapsed; // Показати або приховати
                }
            }
        }

        public void UpdateGameFieldAlignment(bool showButtons)
        {
            if (showButtons)
            {
                gameGrid.Margin = new Thickness(0, 0, 0, 10);
                gameGrid.HorizontalAlignment = HorizontalAlignment.Center; // Поле зміщене ліворуч, коли кнопки увімкнені
            }
            else
            {
                gameGrid.Margin = new Thickness(50, 0, 0, 10); 
            }
        }


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox focusedTextBox = sender as TextBox;

            int row = -1, col = -1;
            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
                {
                    if (textBoxGrid[i, j] == focusedTextBox)
                    {
                        row = i;
                        col = j;
                        break;
                    }
                }
                if (row != -1) break;
            }

            if (row != -1 && col != -1)
            {
                HighlightRowAndColumn(row, col, Brushes.White); 
            }
        }

        private void HighlightRowAndColumn(int row, int col, Brush color)
        {
            for (int j = 0; j < Sizefield; j++)
            {
                if (textBoxGrid[row, j] != null)
                {
                    textBoxGrid[row, j].Background = color;
                }
            }

            for (int i = 0; i < Sizefield; i++)
            {
                if (textBoxGrid[i, col] != null)
                {
                    textBoxGrid[i, col].Background = color;
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

        private int[,] GenerateMatrix()
        {
            int[,] matrix = new int[Sizefield, Sizefield];
            Random random = new Random();

            // Створюємо базову матрицю
            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
                {
                    matrix[i, j] = (j + i) % Sizefield + 1; // Заповнюємо базову матрицю
                }
            }

            for (int i = 0; i < Sizefield; i++)
            {
                int swapIndex = random.Next(Sizefield);
                for (int j = 0; j < Sizefield; j++)
                {
                    int temp = matrix[i, j];
                    matrix[i, j] = matrix[swapIndex, j];
                    matrix[swapIndex, j] = temp;
                }
            }

            for (int j = 0; j < Sizefield; j++)
            {
                int swapIndex = random.Next(Sizefield);
                for (int i = 0; i < Sizefield; i++)
                {
                    int temp = matrix[i, j];
                    matrix[i, j] = matrix[i, swapIndex];
                    matrix[i, swapIndex] = temp;
                }
            }

            return matrix;
        }

        private void ShowRandomNumbers()
        {
            Random random = new Random();

            int numbersToShow = random.Next(1, 3);

            for (int i = 0; i < numbersToShow; i++)
            {
                int row, column;

                do
                {
                    row = random.Next(0, matrix.GetLength(0));
                    column = random.Next(0, matrix.GetLength(1));
                } while (matrix[row, column] == 0);

                TextBox textBox = textBoxGrid[row, column];

                if (textBox != null)
                {
                    textBox.Text = matrix[row, column].ToString();
                }
            }
        }

        private void numberButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (activeNumberButton != null)
            {
                activeNumberButton.Background = Brushes.LightGray; 
            }

            activeNumberButton = clickedButton;

            if (highlightActiveButton)
            {
                activeNumberButton.Background = Brushes.Blue;
            }

            if (lastFocusedTextBox != null)
            {
                lastFocusedTextBox.Text = clickedButton.Content.ToString();
            }
        }


        private void Level4x4_Click(object sender, RoutedEventArgs e)
        {
            this.Height = 460;
            this.Width = 473;
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;
            buttonPanel.Margin = new Thickness(0, this.Height - 135, 0, 0);
            pauseButton.Margin = new Thickness(0, this.Height - 90, 0, 0);
            Sizefield = 4;
            matrix = GenerateMatrix();

            GenerateGrid();

        }

        private void Level5x5_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 472;
            this.Height = 510;
            buttonPanel.Margin = new Thickness(0, this.Height - 135, 0, 0);
            pauseButton.Margin = new Thickness(0, this.Height - 90, 0, 0);

            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;

            Sizefield = 5;
            matrix = GenerateMatrix();
            GenerateGrid();
        }

        private void Level7x7_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 540;
            this.Height = 630;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;

            buttonPanel.Margin = new Thickness(0, this.Height - 135, 0, 0);
            pauseButton.Margin = new Thickness(0, this.Height - 90, 0, 0);

            Sizefield = 7;
            matrix = GenerateMatrix();
            GenerateGrid();
        }

        private void Level9x9_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 630;
            this.Height = 740;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;

            buttonPanel.Margin = new Thickness(0, this.Height - 135, 0, 0);
            pauseButton.Margin = new Thickness(0, this.Height - 90, 0, 0);

            Sizefield = 9;

            matrix = GenerateMatrix();
            GenerateGrid();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Sizefield = 4;
            matrix = GenerateMatrix();
            GenerateGrid();
            timerTextBlock.Text = "00:00";
            timeElapsed = TimeSpan.Zero;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);  
            timer.Tick += Timer_Tick;
            timer.Start();         
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeElapsed = timeElapsed.Add(TimeSpan.FromSeconds(1));  // Збільшуємо час
            timerTextBlock.Text = $"{timeElapsed:mm\\:ss}";    // Оновлюємо текстовий блок
        }

        private void ResetTimer()
        {
            if (timer == null) timer = new DispatcherTimer();
            if (timer.IsEnabled) timer.Stop();

            timeElapsed = TimeSpan.Zero;
            timerTextBlock.Text = $"{timeElapsed:mm\\:ss}";

            timer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPaused)
            {
                timer.Stop(); 
                pauseButton.Content = "Продовжити"; 

                SetGameFieldEnabled(false);
            }
            else
            {
                timer.Start(); 
                pauseButton.Content = "Пауза"; 

                SetGameFieldEnabled(true);
            }

            isPaused = !isPaused; 
        }

        private void SetGameFieldEnabled(bool isEnabled)
        {
            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
                {
                    if (textBoxGrid[i, j] != null)
                    {
                        textBoxGrid[i, j].IsEnabled = isEnabled; 
                    }
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            matrix = GenerateMatrix();
            GenerateGrid();
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            bool isLevelCompleted = true; 

            for (int i = 0; i < Sizefield; i++)
            {
                for (int j = 0; j < Sizefield; j++)
                {
                    var textBox = textBoxGrid[i, j];

                    if (textBox != null)
                    {
                        textBox.BorderThickness = new Thickness(1);
                        textBox.BorderBrush = Brushes.Black; 

                        string userInput = textBox.Text;

                        if (int.TryParse(userInput, out int userNumber))
                        {
                            if (userNumber != matrix[i, j])
                            {
                                textBox.BorderBrush = Brushes.Red; 
                                textBox.BorderThickness = new Thickness(2);
                                isLevelCompleted = false; 
                            }
                        }
                        else
                        {
                            isLevelCompleted = false; 
                        }
                    }
                }
            }

            if (isLevelCompleted)
            {
                timerTextBlock.Foreground = Brushes.Green; 
                MessageBox.Show("Вітаємо! Ви успішно завершили рівень.", "Рівень завершено", MessageBoxButton.OK, MessageBoxImage.Information);
                if (timer != null && timer.IsEnabled)
                {
                    timer.Stop(); 
                }
            }
           
        }

        public void EnableNightMode(bool isEnabled)
        {
            if (isEnabled)
            {
                this.Background = new SolidColorBrush(Colors.DarkGray);
                gameGrid.Background = new SolidColorBrush(Colors.DarkGray);
            }
            else
            {
                this.Background = new SolidColorBrush(Colors.White);
                gameGrid.Background = new SolidColorBrush(Colors.White);
            }
        }

        public void EnableErrorHighlighting(bool isEnabled)
        {
            highlightErrors = isEnabled;

            if (highlightErrors)
            {
                for (int i = 0; i < Sizefield; i++)
                {
                    for (int j = 0; j < Sizefield; j++)
                    {
                        TextBox textBox = textBoxGrid[i, j];
                        if (textBox != null && int.TryParse(textBox.Text, out int value))
                        {
                            if (value != matrix[i, j]) textBox.Foreground = Brushes.Red; 
                            else textBox.Foreground = Brushes.Black; 
                        }
                        else textBox.Foreground = Brushes.Black; 
                    }
                }
            }
            else
            {
                for (int i = 0; i < Sizefield; i++)
                {
                    for (int j = 0; j < Sizefield; j++)
                    {
                        TextBox textBox = textBoxGrid[i, j];
                        if (textBox != null)
                        {
                            textBox.Foreground = Brushes.Black; 
                        }
                    }
                }
            }
        }

        public void EnableRowColumnHighlighting(bool isEnabled)
        {
            highlightRowAndColumn = isEnabled;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            //var mousePosition = Mouse.GetPosition(this);
            //var cursorX = mousePosition.X.ToString("F3");
            //var cursorY = mousePosition.Y.ToString("F3");

            //var windowWidth = this.ActualWidth.ToString("F3");
            //var windowHeight = this.ActualHeight.ToString("F3");

            //this.Title = $"Курсор: ({cursorX}, {cursorY}) | Розмір: {windowWidth}x{windowHeight}";
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void RulesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RulesWindow rulesWindow = new RulesWindow();
            rulesWindow.Owner = this; 
            rulesWindow.ShowDialog(); 
        }

    }

}
