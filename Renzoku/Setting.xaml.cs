using Renzoku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Renzokku
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        private MainWindow mainWindow;
        public Setting(MainWindow owner)
        {
            InitializeComponent();
            mainWindow = owner;
            ButtonUpForm();
            GenerateSettingGame();
        }

        private void ButtonUpForm()
        {
            Button note = new Button()
            {
                Width = 45,
                Height = 45,
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Right 


            };
            ImageSource noteImg = new BitmapImage(new Uri("D:\\4\\ІЗВП\\Renzoku\\Renzoku\\Image\\mark.png"));
            note.Content = new Image { Source = noteImg };

            note.Click += Note_Click;

            Button eraser = new Button()
            {
                Width = 45,
                Height = 45,
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Left 

            };
            ImageSource eraserImg = new BitmapImage(new Uri("D:\\4\\ІЗВП\\Renzoku\\Renzoku\\Image\\eraser.png"));
            eraser.Content = new Image { Source = eraserImg };
            eraser.Click += Eraser_Click;

            Grid.SetColumn(note, 0);  
            Grid.SetColumn(eraser, 1);  

            btnPanelSetting.Children.Add(note);
            btnPanelSetting.Children.Add(eraser);
        }



        private void Note_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.lastFocusedTextBox != null)
            {
                mainWindow.lastFocusedTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                mainWindow.lastFocusedTextBox.BorderThickness = new Thickness(3);
            }
        }
        private void Eraser_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.lastFocusedTextBox != null)
            {
                if (mainWindow.textBoxBorders.ContainsKey(mainWindow.lastFocusedTextBox))
                {
                    BorderInfo originalBorder = mainWindow.textBoxBorders[mainWindow.lastFocusedTextBox];

                    mainWindow.lastFocusedTextBox.BorderBrush = originalBorder.BorderBrush;
                    mainWindow.lastFocusedTextBox.BorderThickness = originalBorder.BorderThickness;
                }
            }
        }

        private void GenerateSettingGame()
        {
            // Чекбокс для закріплення налаштувань
            CheckBox fixSetting = new CheckBox()
            {
                Margin = new Thickness(15, 5, 0, 0),
                Content = "Закріпити налаштування"
            };
            fixSetting.Checked += FixSetting_Checked;
            fixSetting.Unchecked += FixSetting_Unchecked;
            settingPanel.Children.Add(fixSetting);

            // Чекбокс для приховування/показу таймера
            CheckBox hideTimerCheckBox = new CheckBox()
            {
                Margin = new Thickness(15, 5, 0, 0),
                Content = "Приховати таймер"
            };
            hideTimerCheckBox.Checked += HideTimerCheckBox_Checked;
            hideTimerCheckBox.Unchecked += HideTimerCheckBox_Unchecked;
            settingPanel.Children.Add(hideTimerCheckBox);

            // Чекбокс для нічного режиму
            CheckBox nightModeCheckBox = new CheckBox()
            {
                Margin = new Thickness(15, 5, 0, 0),
                Content = "Нічний режим"
            };
            nightModeCheckBox.Checked += NightModeCheckBox_Checked;
            nightModeCheckBox.Unchecked += NightModeCheckBox_Unchecked;
            settingPanel.Children.Add(nightModeCheckBox);

            CheckBox highlightErrorsCheckBox = new CheckBox()
            {
                Margin = new Thickness(15, 5, 0, 0),
                Content = "Виділяти помилки"
            };
            highlightErrorsCheckBox.Checked += HighlightErrorsCheckBox_Checked;
            highlightErrorsCheckBox.Unchecked += HighlightErrorsCheckBox_Unchecked;
            settingPanel.Children.Add(highlightErrorsCheckBox);

            CheckBox highlightRowColumnCheckBox = new CheckBox()
            {
                Margin = new Thickness(15, 5, 0, 0),
                Content = "Виділяти поточний рядок і стовпець"
            };
            highlightRowColumnCheckBox.Checked += HighlightRowColumnCheckBox_Checked;
            highlightRowColumnCheckBox.Unchecked += HighlightRowColumnCheckBox_Unchecked;
            settingPanel.Children.Add(highlightRowColumnCheckBox);

            CheckBox highlightSameNumbersCheckBox = new CheckBox()
            {
                Margin = new Thickness(15, 5, 0, 0),
                Content = "Виділяти однакові числа"
            };

            highlightSameNumbersCheckBox.Checked += HighlightSameNumbersCheckBox_Checked;
            highlightSameNumbersCheckBox.Unchecked += HighlightSameNumbersCheckBox_Unchecked;
            settingPanel.Children.Add(highlightSameNumbersCheckBox);

            // Основний чекбокс "Показувати кнопки чисел"
            CheckBox showNumberButtonsCheckBox = new CheckBox()
            {
                Margin = new Thickness(15, 5, 0, 0),
                Content = "Показувати кнопки чисел",
                IsChecked = true // За замовчуванням увімкнено
            };

            showNumberButtonsCheckBox.Checked += ShowNumberButtonsCheckBox_Checked;
            showNumberButtonsCheckBox.Unchecked += ShowNumberButtonsCheckBox_Unchecked;
            settingPanel.Children.Add(showNumberButtonsCheckBox);

            // Підпанель для додаткових налаштувань кнопок чисел (зокрема "Підсвічувати активну кнопку")
            StackPanel numberButtonOptionsPanel = new StackPanel()
            {
                Margin = new Thickness(30, 5, 0, 0),
                Visibility = Visibility.Visible // Панель видима за замовчуванням
            };

            // Підчекбокс для підсвічування активної кнопки
            CheckBox highlightActiveButtonCheckBox = new CheckBox()
            {
                Margin = new Thickness(15, 5, 0, 0),
                Content = "Підсвічувати активну кнопку"
            };

            highlightActiveButtonCheckBox.Checked += (s, e) =>
            {
                mainWindow.highlightActiveButton = true; 
            };

            highlightActiveButtonCheckBox.Unchecked += (s, e) =>
            {
                mainWindow.highlightActiveButton = false; 
                if (mainWindow.activeNumberButton != null)
                {
                    mainWindow.activeNumberButton.Background = Brushes.LightGray; 
                }
            };

            numberButtonOptionsPanel.Children.Add(highlightActiveButtonCheckBox); 
            settingPanel.Children.Add(numberButtonOptionsPanel); 

            showNumberButtonsCheckBox.Checked += (s, e) =>
            {
                numberButtonOptionsPanel.Visibility = Visibility.Visible; 
                mainWindow.areNumberButtonsVisible = true;
                mainWindow.ShowNumberButtons(true); 
                mainWindow.UpdateGameFieldAlignment(true); 
            };

            showNumberButtonsCheckBox.Unchecked += (s, e) =>
            {
                numberButtonOptionsPanel.Visibility = Visibility.Collapsed; 
                mainWindow.areNumberButtonsVisible = false;
                mainWindow.ShowNumberButtons(false); 
                mainWindow.UpdateGameFieldAlignment(false); 
            };

        }

        private void ShowNumberButtonsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.areNumberButtonsVisible = true;
            mainWindow.ShowNumberButtons(true); 
            mainWindow.UpdateGameFieldAlignment(true); 
        }

        private void ShowNumberButtonsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mainWindow.areNumberButtonsVisible = false;
            mainWindow.ShowNumberButtons(false); 
            mainWindow.UpdateGameFieldAlignment(false); 
        }




        private void HighlightSameNumbersCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.highlightSameNumbers = true; 
        }

        private void HighlightSameNumbersCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mainWindow.highlightSameNumbers = false; 
            mainWindow.ResetHighlightForAllTextBoxes(); 
        }



        private void NightModeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.EnableNightMode(true); 
        }

        private void NightModeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mainWindow.EnableNightMode(false); 
        }

        private void HighlightErrorsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.EnableErrorHighlighting(true); 
        }

        private void HighlightErrorsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mainWindow.EnableErrorHighlighting(false); 
        }

        private void HighlightRowColumnCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.EnableRowColumnHighlighting(true); 
        }

        private void HighlightRowColumnCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mainWindow.EnableRowColumnHighlighting(false); 
        }


        private void HideTimerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.SetTimerVisibility(false);
        }

        private void HideTimerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mainWindow.SetTimerVisibility(true);
        }


        private void FixSetting_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            this.Activate(); 
        }

        private void FixSetting_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            this.Owner?.Activate(); 
        }


    }
}
