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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Pr5_CipherTranspos
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Cipher cipher;
        char[,] _curMatrix = new char[0,0];

        Input _input = Input.LeftRight;
        Output _output = Output.UpDown;
        public bool[,] _cellKardanoChosen = new bool[8,8];
        Button[,] _buttons = new Button[8, 8];

        public MainWindow()
        {
            InitializeComponent();
            cipher = new Cipher();
            DefaultValues();
            cbSelectAlgorithm.SelectedIndex = (int)Algorithm.RouteTranspose;
        }

        #region main Algorytms
        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            if (!isCorrectInput())
                return;
            spTables.Children.Clear();
            int rows = int.Parse(tbMatrixRow.Text);
            int cols = int.Parse(tbMatrixColumn.Text);

            //проверка ключей
            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.VerticalTranspose || cbSelectAlgorithm.SelectedIndex == (int)Algorithm.DoubleTranspose)
                if (!areCorrectKeys(rows, cols))
                    return;

            //Шифры перестановки
            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.RouteTranspose)
                tbResult.Text = DoRouteTranspose(rows, cols);
            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.VerticalTranspose)
                tbResult.Text = DoVerticalTranspose(rows, cols);
            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.KardanoTranspose)
                tbResult.Text = DoKardanoTranspose(rows, cols);
            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.DoubleTranspose)
                tbResult.Text = DoDoubleTranspose(rows, cols);
        }

        string DoRouteTranspose(int rows, int cols)
        {
            //формирование первичной матрицы
            _curMatrix = cipher.MakeMatrix(tbMessage.Text, rows, cols, _input);
            //показываем первоначальную матрицу
            ShowCurMatrix(rows, cols);
            return cipher.EncodeMatrix(_curMatrix, rows, cols, _output);
        }

        string DoVerticalTranspose(int rows, int cols)
        {
            //формирование первичной матрицы
            _curMatrix = cipher.MakeMatrix(tbMessage.Text, rows, cols, _input);
            //показываем первоначальную матрицу
            ShowCurMatrix(rows, cols);

            //показываем вторую матрицу
            int[] keysCols = MakeKeys(tbKeyCols.Text);
            _curMatrix = cipher.VerticalTranspose(_curMatrix, rows, cols, keysCols);
            ShowCurMatrix(rows, cols);
            return cipher.EncodeMatrix(_curMatrix, rows, cols, _output);
        }

        string DoDoubleTranspose(int rows, int cols)
        {
            //формирование первичной матрицы
            _curMatrix = cipher.MakeMatrix(tbMessage.Text, rows, cols, _input);
            //показываем первоначальную матрицу
            ShowCurMatrix(rows, cols);

            //показываем вторую матрицу
            int[] keysCols = MakeKeys(tbKeyCols.Text);
            int[] keysRows = MakeKeys(tbKeyRows.Text);
            _curMatrix = cipher.VerticalTranspose(_curMatrix, rows, cols, keysCols);
            ShowCurMatrix(rows, cols);

            //показываем третью матрицу
            _curMatrix = cipher.HorizontalTranspose(_curMatrix, rows, cols, keysRows);
            ShowCurMatrix(rows, cols);

            return cipher.EncodeMatrix(_curMatrix, rows, cols, _output);
        }

        private string DoKardanoTranspose(int rows, int cols)
        {
            _curMatrix = cipher.MakeMatrix("", rows, cols, _input);
            _curMatrix = cipher.KardanoTranspose(_curMatrix, rows, cols, 0, _cellKardanoChosen, tbMessage.Text);
            ShowCurMatrix(rows, cols);
            _curMatrix = cipher.KardanoTranspose(_curMatrix, rows, cols, 90, _cellKardanoChosen, tbMessage.Text);
            ShowCurMatrix(rows, cols);
            _curMatrix = cipher.KardanoTranspose(_curMatrix, rows, cols, 180, _cellKardanoChosen, tbMessage.Text);
            ShowCurMatrix(rows, cols);
            _curMatrix = cipher.KardanoTranspose(_curMatrix, rows, cols, 270, _cellKardanoChosen, tbMessage.Text);
            ShowCurMatrix(rows, cols);

            return cipher.EncodeMatrix(_curMatrix, rows, cols, _output);
        }

        #endregion

        #region Проверка корректности ввода (isCorrectInput, areCorrectKeys)

        private bool isCorrectInput()
        {
            if (cbSelectAlgorithm.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите алгоритм шифрования.");
                return false;
            }
            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.DoubleTranspose && (cbSelectRouteInput.SelectedIndex == -1 || cbSelectRouteOutput.SelectedIndex == -1))
            {
                MessageBox.Show("Выберите маршрут ввода сообщения в матрицу и маршрут вывода из нее.");
                return false;
            }
            if (!int.TryParse(tbMatrixColumn.Text, out int cols))
            {
                MessageBox.Show("Значения в размеров матрицы должны быть целыми числами!");
                return false;
            }

            if (!int.TryParse(tbMatrixRow.Text, out int rows))
            {
                MessageBox.Show("Значения в размеров матрицы должны быть целыми числами!");
                return false;
            }

            string key = "0123456789 ";
            foreach (char c in tbKeyCols.Text)
                if (!key.Contains(c))
                {
                    MessageBox.Show("В ключе должны содеражться только числовые значения и пробелы.");
                    return false;
                }

            foreach (char c in tbKeyRows.Text)
                if (!key.Contains(c))
                {
                    MessageBox.Show("В ключе должны содеражться только числовые значения и пробелы.");
                    return false;
                }
            return true;
        }

        public bool areCorrectKeys(int rows, int cols)
        {
            //Vertical+Double Transpose
            int[] keysCols = MakeKeys(tbKeyCols.Text);
            if (keysCols.Length == 0)
            {
                MessageBox.Show("Ошибка ввода ключа (столбцы).");
                return false;
            }
            if (keysCols.Length != cols)
            {
                MessageBox.Show("Ключи (числа) должны быть введены согласно размерности таблицы (столбцы).");
                return false;
            }
            foreach (int k in keysCols)
                if (k < 1 || k > cols )
                {
                    MessageBox.Show("Числовые значения ключа должны быть числами от 1 до n (n-количество столбцов)");
                    return false;
                }

            //DoubleTranspose
            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.DoubleTranspose)
            {
                int[] keysRows = MakeKeys(tbKeyRows.Text);
                if (keysRows.Length == 0)
                {
                    MessageBox.Show("Ошибка ввода ключа (строки).");
                    return false;
                }
                if (keysRows.Length != rows)
                {
                    MessageBox.Show("Ключи (числа) должны быть введены согласно размерности таблицы (строки).");
                    return false;
                }
                foreach (int k in keysRows)
                    if (k < 1 || k > cols)
                    {
                        MessageBox.Show("Числовые значения ключа должны быть числами от 1 до n (n-количество строк)");
                        return false;
                    }
            }

            return true;
        }
        #endregion

        #region Подготовка параметров (MakeKeys, SelectRoute_Field)
        private int[] MakeKeys(string keyText)
        {
            int[] keys;
            try
            {
                keys = keyText.Split().Select(i => int.Parse(i.ToString())).ToArray();

            }
            catch (Exception e)
            {
                //MessageBox.Show("Проверьте введенный ключ (только числа через пробел).\n\n" + e.ToString());
                return keys = new int[0];
            }
            return keys;
        }

        private void cbSelectRouteInput_SelectChanged(object sender, EventArgs e)
        {
            if (cbSelectRouteInput.SelectedIndex == (int)Input.LeftRight)
                _input = Input.LeftRight;
            if (cbSelectRouteInput.SelectedIndex == (int)Input.RightLeft)
                _input = Input.RightLeft;
            if (cbSelectRouteInput.SelectedIndex == (int)Input.UpDown)
                _input = Input.UpDown;
            if (cbSelectRouteInput.SelectedIndex == (int)Input.DownUp)
                _input = Input.DownUp;

            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.RouteTranspose)
                if (cbSelectRouteInput.SelectedIndex == cbSelectRouteOutput.SelectedIndex)
                    MessageBox.Show("Сделайте маршрут ввода и вывода разными.\n\n" +
                        "При одинаковых маршрутах не будет видно изменений исходного сообщения!");
        }

        private void cbSelectRouteOutput_SelectChanged(object sender, EventArgs e)
        {
            if (cbSelectRouteOutput.SelectedIndex == (int)Output.LeftRight)
                _output = Output.LeftRight;
            if (cbSelectRouteOutput.SelectedIndex == (int)Output.RightLeft)
                _output = Output.RightLeft;
            if (cbSelectRouteOutput.SelectedIndex == (int)Output.UpDown)
                _output = Output.UpDown;
            if (cbSelectRouteOutput.SelectedIndex == (int)Output.DownUp)
                _output = Output.DownUp;
            if (cbSelectRouteInput.SelectedIndex == cbSelectRouteOutput.SelectedIndex)
                MessageBox.Show("Обращаем ваше внимание на то, " +
                   "что если сделать маршрут ввода и вывода (при работе с матрицей) одинаковыми, " +
                   "то сообщение при шифровании может понести минимальные видимые изменения");
        }
       
        #endregion

        #region Работа интерфейса (ShowCurMatrix, MakeKardanoTrafaret, cbSelectAlgorithm_SelectChanged)

        void ShowCurMatrix(int rows, int cols)
        {
            //не забыть в поле добавить ссылку на объект
            Grid newGrid = new Grid { Margin = new Thickness(30, 0, 0, 0)};

            for (int j = 0; j < Math.Max(rows, cols); j++)
            {
                newGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
                newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });
            }

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    TextBox tb = new TextBox
                    {
                        Name = "c" + r + c, //c - cell(ячейка)
                        Text = _curMatrix[r, c].ToString(),
                        BorderBrush = Brushes.Black,
                        Padding = new Thickness(2),
                        IsReadOnly = true
                    };
                    newGrid.Children.Add(tb);
                    Grid.SetRow(tb, r);
                    Grid.SetColumn(tb, c);
                }
            }
            spTables.Children.Add(newGrid);
        }


        void DefaultValues()
        {
            tbMessage.Text = "Жди меня, и я вернусь. Только очень жди.";
            tbMatrixColumn.Text = "8";
            tbMatrixRow.Text = "5";
            
            //ключи строк и столбцов.
            KeyCols.Visibility = Visibility.Hidden;
            KeyRows.Visibility = Visibility.Hidden;

            tbKeyCols.Text = "";
            tbKeyRows.Text = "";

            SelectRoute.Visibility = Visibility.Visible;
            cbSelectRouteInput.SelectedIndex = (int)Input.LeftRight;
            cbSelectRouteOutput.SelectedIndex = (int)Output.UpDown;
            cbSelectRouteInput.Visibility = Visibility.Visible;
            cbSelectRouteOutput.Visibility = Visibility.Visible;

            spKardanoTrafaret.Children.Clear();
            spKardanoTrafaret.Visibility = Visibility.Collapsed;
        }

        //изменение отображения при изменении алгоритма
        private void cbSelectAlgorithm_SelectChanged(object sender, EventArgs e)
        {
            DefaultValues();

            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.VerticalTranspose || cbSelectAlgorithm.SelectedIndex == (int)Algorithm.DoubleTranspose)
            {
                KeyCols.Visibility = Visibility.Visible;
                tbKeyCols.Text = "5 2 1 7 8 4 6 3";
            }

            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.DoubleTranspose)
            {
                KeyRows.Visibility = Visibility.Visible;
                tbKeyRows.Text = "3 5 1 2 4";
            }

            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.KardanoTranspose)
            {
                tbMessage.Text = "У лукоморья дуб зеленый. Златая цепь на дубе том. И днем и ночью";
                tbMatrixColumn.Text = "8";
                tbMatrixRow.Text = "8";
                spKardanoTrafaret.Visibility = Visibility.Visible;
                DefaultCells();
                DefaultClick();
                cbSelectRouteInput.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region AllWithKardano
        private void Cols_Changed(object sender, TextChangedEventArgs e)
        {
            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.KardanoTranspose)
                if (int.TryParse(tbMatrixColumn.Text, out int cols))
                {
                    if (tbMatrixColumn.Text == tbMatrixRow.Text)
                        MakeKardanoTrafaret(cols, cols);
                    else
                        tbMatrixRow.Text = tbMatrixColumn.Text;
                }    
        }
        private void Rows_Changed(object sender, TextChangedEventArgs e)
        {
            if (cbSelectAlgorithm.SelectedIndex == (int)Algorithm.KardanoTranspose)
                if (int.TryParse(tbMatrixRow.Text, out int rows))
                {
                    if (tbMatrixColumn.Text == tbMatrixRow.Text)
                        MakeKardanoTrafaret(rows, rows);
                    else
                        tbMatrixColumn.Text = tbMatrixRow.Text;
                }
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            var button = (Button)sender; //обращаемся к кнопке, которая была нажата.
            int row = int.Parse(button.Name.Split('_')[1]);
            int col = int.Parse(button.Name.Split('_')[2]);
            //если была уже выбрана то сброс
            if (_cellKardanoChosen[row, col])
            {
                _cellKardanoChosen[row, col] = false;
                button.Background = Brushes.LightGray;
            }
            else//иначе - устанавливаем
            {
                _cellKardanoChosen[row, col] = true;
                button.Background = Brushes.Cyan;
            }
        }

        void MakeKardanoTrafaret(int rows, int cols)
        {
            spKardanoTrafaret.Children.Clear();
            spTables.Children.Clear();
            _cellKardanoChosen = new bool[rows, cols];
            _buttons = new Button[rows, cols];
            Grid newGrid = new Grid { Margin = new Thickness(30, 0, 0, 0)};

            for (int j = 0; j < Math.Max(rows, cols); j++)
            {
                newGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
                newGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });
            }

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Button button = new Button
                    {
                        Name = "b_" + r + "_" + c, //c - cell(ячейка)
                        Background = Brushes.LightGray,
                        BorderBrush = Brushes.Black,
                    };
                    _buttons[r, c] = button;
                    button.Click += ButtonOnClick;
                    _cellKardanoChosen[r, c] = false;//изначально ничего не выбрано
                    newGrid.Children.Add(button);
                    Grid.SetRow(button, r);
                    Grid.SetColumn(button, c);
                }
            }
            spKardanoTrafaret.Children.Add(newGrid);
            
        }

        void DefaultCells()
        {
            
            _cellKardanoChosen[0, 1] = true;
            _cellKardanoChosen[0, 5] = true;
            _cellKardanoChosen[1, 3] = true;
            _cellKardanoChosen[1, 6] = true;
            _cellKardanoChosen[2, 4] = true;
            _cellKardanoChosen[2, 7] = true;
            _cellKardanoChosen[3, 0] = true;
            _cellKardanoChosen[4, 2] = true;
            _cellKardanoChosen[4, 4] = true;
            _cellKardanoChosen[4, 6] = true;
            _cellKardanoChosen[5, 1] = true;
            _cellKardanoChosen[5, 5] = true;
            _cellKardanoChosen[6, 2] = true;
            _cellKardanoChosen[6, 7] = true;
            _cellKardanoChosen[7, 0] = true;
            _cellKardanoChosen[7, 4] = true;
        }

        void DefaultClick()
        {
            int n = Convert.ToInt32(Math.Sqrt(_cellKardanoChosen.Length));
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    if (_cellKardanoChosen[r, c])
                        _buttons[r, c].Background = Brushes.Cyan;
        }
        #endregion

    }



}
