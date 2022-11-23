using Checkers.Forms.Extensions;
using Checkers.Forms.Models;
using Checkers.Server.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Checkers.Forms.Forms
{
    public partial class CheckersForm : Form
    {
        private const string PicturePath = @"C:\Users\EgorKuzmin\Pictures\Saved Pictures\";
        private readonly Size PictureSize;

        public string CurrentTurn { get; set; } = "Your Turn";
        public string AlternativeTurn { get; set; } = "Opponent`s Turn";

        private int _boardSize;
        private const int CellSize = 100;

        //private Socket _socket;
        //private TcpListener _server;
        //private Stream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        public Board Board { get; set; }

        private List<Button> _simpleSteps = new List<Button>();
        private int _beatStepsCount = 0;
        private bool _hasContinue = false;
        private Button[,] _checkers;

        public int CurrentPlayer { get; private set; }
        public bool IsInTurn { get; private set; }
        public Button PreviousButton { get; private set; }
        public Button PressedButton { get; private set; }

        private Image _blackChecker;
        private Image _whiteChecker;
        public CheckersForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            PictureSize = new Size(CellSize - 10, CellSize - 10);
            Board = new Board();
            _boardSize = Board.GetSize();

            _checkers = new Button[_boardSize, _boardSize];
            CurrentPlayer = 1;
        }

        public void SetGame()
        {
            IsInTurn = false;
            PreviousButton = null;

            _blackChecker = new Bitmap(new Bitmap(PicturePath + "black-removebg-preview.png"), PictureSize);
            _whiteChecker = new Bitmap(new Bitmap(PicturePath + "red-removebg-preview.png"), PictureSize);

            CreateBoard();
        }

        public void CreateBoard()
        {
            Width = (_boardSize + 1) * (CellSize - 5);
            Height = (_boardSize + 1) * (CellSize - 5);

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * CellSize, i * CellSize);
                    button.Size = new Size(CellSize, CellSize);
                    button.Click += OnCheckerPressed;

                    if (Board[i, j] == 1)
                    {
                        button.Image = _whiteChecker;
                    }

                    if (Board[i, j] == 2)
                    {
                        button.Image = _blackChecker;
                    }

                    button.BackColor = GetPrevButtonColor(button);
                    button.ForeColor = Color.White;

                    _checkers[i, j] = button;
                    Controls.Add(button);
                }
            }
        }

        public void CreateBoardWithBackgroundWorker()
        {
            this.Invoke(new Action(() =>
            {
                foreach (var checker in _checkers)
                {
                    checker.Image = null;
                }

                for (int i = 0; i < _boardSize; i++)
                {
                    for (int j = 0; j < _boardSize; j++)
                    {
                        if (Board[i, j] == 1)
                        {
                            _checkers[i, j].Image = _whiteChecker;
                        }

                        if (Board[i, j] == 2)
                        {
                            _checkers[i, j].Image = _blackChecker;
                        }
                    }
                }
            }));
        }

        public Color GetPrevButtonColor(Button prevButton)
        {
            if (prevButton.GetRelativeY(CellSize) % 2 != 0 && prevButton.GetRelativeX(CellSize) % 2 == 0)
            {
                return Color.DimGray;
            }

            if (prevButton.GetRelativeY(CellSize) % 2 == 0 && prevButton.GetRelativeX(CellSize) % 2 != 0)
            {
                return Color.DimGray;
            }

            return Color.White;
        }


        public void OnCheckerPressed(object sender, EventArgs args)
        {
            //this.Invoke(new Action(() =>
            //{
             
            //}));

            if (PreviousButton != null)
            {
                PreviousButton.BackColor = GetPrevButtonColor(PreviousButton);
            }

            PressedButton = sender as Button;

            if (Board[PressedButton.GetRelativeY(CellSize), PressedButton.GetRelativeX(CellSize)] != 0
                && Board[PressedButton.GetRelativeY(CellSize), PressedButton.GetRelativeX(CellSize)] == CurrentPlayer)
            {
                CloseSteps();
                PressedButton.BackColor = Color.Red;
                DeactivateAllButtons();
                PressedButton.Enabled = true;
                _beatStepsCount = 0;

                if (PressedButton.Text == "King")
                {
                    ShowSteps(PressedButton.GetRelativeY(CellSize), PressedButton.GetRelativeX(CellSize), false);
                }
                else
                {
                    ShowSteps(PressedButton.GetRelativeY(CellSize), PressedButton.GetRelativeX(CellSize));
                }

                if (IsInTurn)
                {
                    CloseSteps();
                    PressedButton.BackColor = GetPrevButtonColor(PressedButton);
                    ShowPossibleSteps();
                    IsInTurn = false;
                }
                else
                {
                    IsInTurn = true;
                }
            }
            else
            {
                if (IsInTurn)
                {
                    _hasContinue = false;
                    if (Math.Abs(PressedButton.GetRelativeX(CellSize) - PreviousButton.GetRelativeX(CellSize)) > 1)
                    {
                        _hasContinue = true;
                        RemoveBeaten(PressedButton, PreviousButton);
                    }

                    (Board[PressedButton.GetRelativeY(CellSize), PressedButton.GetRelativeX(CellSize)], Board[PreviousButton.GetRelativeY(CellSize), PreviousButton.GetRelativeX(CellSize)])
                        = (Board[PreviousButton.GetRelativeY(CellSize), PreviousButton.GetRelativeX(CellSize)], Board[PressedButton.GetRelativeY(CellSize), PressedButton.GetRelativeX(CellSize)]);

                    PressedButton.Image = PreviousButton.Image;
                    PreviousButton.Image = null;
                    PressedButton.Text = PreviousButton.Text;
                    PreviousButton.Text = "";

                    SwitchButtonToKing(PressedButton);
                    _beatStepsCount = 0;
                    IsInTurn = false;

                    CloseSteps();
                    DeactivateAllButtons();

                    if (PressedButton.Text == "King")
                    {
                        ShowSteps(PressedButton.GetRelativeY(CellSize), PressedButton.GetRelativeX(CellSize), false);
                    }
                    else
                    {
                        ShowSteps(PressedButton.GetRelativeY(CellSize), PressedButton.GetRelativeX(CellSize));
                    }

                    if (_beatStepsCount == 0 || !_hasContinue)
                    {
                        CloseSteps();
                        SwitchPlayer();
                        ShowPossibleSteps();
                        _hasContinue = false;
                    }
                    else if (_hasContinue)
                    {
                        PressedButton.BackColor = Color.Red;
                        PressedButton.Enabled = true;
                        IsInTurn = true;
                    }

                    backgroundWorker2.RunWorkerAsync();
                }
            }

            PreviousButton = PressedButton;
        }

        public void SwitchPlayer()
        {
            //this.Text = AlternativeTurn;
            //(CurrentTurn, AlternativeTurn) = (AlternativeTurn, CurrentTurn);

            //ResetGame();
        }

        public void ResetGame()
        {
            bool isPlayer1HasCheckers = false;
            bool isPlayer2HasCheckers = false;

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    if (Board[i, j] == 1)
                    {
                        isPlayer1HasCheckers = true;
                    }

                    if (Board[i, j] == 2)
                    {
                        isPlayer2HasCheckers = true;
                    }
                }
            }

            if (!isPlayer1HasCheckers)
            {
                MessageBox.Show("You lose!");
            }
            else if (!isPlayer2HasCheckers)
            {
                MessageBox.Show("You win!");
            }
        }

        public void ShowPossibleSteps()
        {
            bool isNotKing;
            bool isBeatStep = false;
            DeactivateAllButtons();

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    if (Board[i, j] == CurrentPlayer)
                    {
                        if (_checkers[i, j].Text == "King")
                        {
                            isNotKing = false;
                        }
                        else
                        {
                            isNotKing = true;
                        }

                        if (IsButtonHasBeatStep(i, j, isNotKing, new int[2] { 0, 0 }))
                        {
                            isBeatStep = true;
                            _checkers[i, j].Enabled = true;
                        }
                    }
                }
            }

            if (!isBeatStep)
            {
                ActivateAllButtons();
            }
        }

        public void SwitchButtonToKing(Button button)
        {
            if (Board[button.GetRelativeY(CellSize), button.GetRelativeX(CellSize)] == 1 && button.GetRelativeY(CellSize) == _boardSize - 1)
            {
                button.Text = "King";

            }
            if (Board[button.GetRelativeY(CellSize), button.GetRelativeX(CellSize)] == 2 && button.GetRelativeY(CellSize) == 0)
            {
                button.Text = "King";
            }
        }

        public void RemoveBeaten(Button endButton, Button startButton)
        {
            int count = Math.Abs(endButton.GetRelativeY(CellSize) - startButton.GetRelativeY(CellSize));
            int startIndexY = endButton.GetRelativeY(CellSize) - startButton.GetRelativeY(CellSize);
            int startIndexX = endButton.GetRelativeX(CellSize) - startButton.GetRelativeX(CellSize);

            startIndexY = startIndexY < 0 ? -1 : 1;
            startIndexX = startIndexX < 0 ? -1 : 1;

            int rowIndex = startButton.GetRelativeY(CellSize) + startIndexY;
            int columnIndex = startButton.GetRelativeX(CellSize) + startIndexX;

            for (int i = 0; i < count - 1; i++)
            {
                Board[rowIndex, columnIndex] = 0;
                _checkers[rowIndex, columnIndex].Image = null;
                _checkers[rowIndex, columnIndex].Text = "";
                rowIndex += startIndexY;
                columnIndex += startIndexX;
            }

        }

        public void ShowSteps(int rowIndex, int columnIndex, bool isNotKing = true)
        {
            _simpleSteps.Clear();
            ShowDiagonal(rowIndex, columnIndex, isNotKing);

            if (_beatStepsCount > 0)
            {
                CloseSimpleSteps(_simpleSteps);
            }
        }

        public void ShowDiagonal(int rowIndex, int columnIndex, bool isNotKing = false)
        {
            int j = columnIndex + 1;
            for (int i = rowIndex - 1; i >= 0; i--)
            {
                if (CurrentPlayer == 1 && isNotKing && !_hasContinue)
                {
                    break;
                }

                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                    {
                        break;
                    }
                }

                if (j < 7)
                {
                    j++;
                }
                else
                {
                    break;
                }

                if (isNotKing)
                {
                    break;
                }
            }

            j = columnIndex - 1;
            for (int i = rowIndex - 1; i >= 0; i--)
            {
                if (CurrentPlayer == 1 && isNotKing && !_hasContinue)
                { 
                    break; 
                }

                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                    {
                        break;
                    }
                }

                if (j > 0)
                {
                    j--;
                }
                else 
                { 
                    break; 
                }

                if (isNotKing)
                {
                    break;
                }
            }

            j = columnIndex - 1;
            for (int i = rowIndex + 1; i < 8; i++)
            {
                if (CurrentPlayer == 2 && isNotKing && !_hasContinue)
                { 
                    break;
                }

                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                    {
                        break;
                    }
                }

                if (j > 0)
                {
                    j--;
                }
                else
                { 
                    break;
                }

                if (isNotKing)
                {
                    break;
                }
            }

            j = columnIndex + 1;
            for (int i = rowIndex + 1; i < 8; i++)
            {
                if (CurrentPlayer == 2 && isNotKing && !_hasContinue) 
                { 
                    break;
                }

                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                    {
                        break;
                    }
                }

                if (j < 7)
                {
                    j++;
                }
                else
                {
                    break;
                }

                if (isNotKing)
                {
                    break;
                }
            }
        }

        public bool DeterminePath(int rowIndex, int columnIndex)
        {
            if (Board[rowIndex, columnIndex] == 0 && !_hasContinue)
            {
                _checkers[rowIndex, columnIndex].BackColor = Color.Yellow;
                _checkers[rowIndex, columnIndex].Enabled = true;
                _simpleSteps.Add(_checkers[rowIndex, columnIndex]);
            }
            else if (Board[rowIndex, columnIndex] != CurrentPlayer)
            {
                if (PressedButton.Text == "King")
                {
                    ShowMandatoryStep(rowIndex, columnIndex, false);
                }
                else
                { 
                    ShowMandatoryStep(rowIndex, columnIndex);
                }
                return false;
            }

            return true;
        }

        public void CloseSimpleSteps(List<Button> simpleSteps)
        {
            if (simpleSteps.Count > 0)
            {
                for (int i = 0; i < simpleSteps.Count; i++)
                {
                    simpleSteps[i].BackColor = GetPrevButtonColor(simpleSteps[i]);
                    simpleSteps[i].Enabled = false;
                }
            }
        }
        public void ShowMandatoryStep(int rowIndex, int columnIndex, bool isKing = true)
        {
            int dirX = rowIndex - PressedButton.GetRelativeY(CellSize);
            int dirY = columnIndex - PressedButton.GetRelativeX(CellSize);

            dirX = dirX < 0 ? -1 : 1;
            dirY = dirY < 0 ? -1 : 1;

            int i = rowIndex;
            int j = columnIndex;
            bool isEmpty = true;

            while (IsInsideBorders(i, j))
            {
                if (Board[i, j] != 0 && Board[i, j] != CurrentPlayer)
                {
                    isEmpty = false;
                    break;
                }

                i += dirX;
                j += dirY;

                if (isKing)
                {
                    break;
                }
            }

            if (isEmpty)
            {
                return;
            }
                
            List<Button> toClose = new List<Button>();
            bool closeSimple = false;
            int k = i + dirX;
            int t = j + dirY;

            while (IsInsideBorders(k, t))
            {
                if (Board[k, t] == 0)
                {
                    if (IsButtonHasBeatStep(k, t, isKing, new int[2] { dirX, dirY }))
                    {
                        closeSimple = true;
                    }
                    else
                    {
                        toClose.Add(_checkers[k, t]);
                    }

                    _checkers[k, t].BackColor = Color.Yellow;
                    _checkers[k, t].Enabled = true;
                    _beatStepsCount++;
                }
                else 
                { 
                    break;
                }

                if (isKing)
                {
                    break;
                }
                    
                t += dirY;
                k += dirX;
            }

            if (closeSimple && toClose.Count > 0)
            {
                CloseSimpleSteps(toClose);
            }
        }

        public bool IsButtonHasBeatStep(int rowIndex, int columnIndex, bool isKing, int[] direction)
        {
            bool isBeatStep = false;
            int j = columnIndex + 1;

            for (int i = rowIndex - 1; i >= 0; i--)
            {
                if (CurrentPlayer == 1 && isKing && !_hasContinue)
                { 
                    break; 
                }

                if (direction[0] == 1 && direction[1] == -1 && !isKing)
                {
                    break;
                }

                if (IsInsideBorders(i, j))
                {
                    if (Board[i, j] != 0 && Board[i, j] != CurrentPlayer)
                    {
                        isBeatStep = true;
                        if (!IsInsideBorders(i - 1, j + 1))
                        {
                            isBeatStep = false;
                        }
                        else if (Board[i - 1, j + 1] != 0)
                        {
                            isBeatStep = false;
                        }
                        else
                        { 
                            return isBeatStep;
                        }
                    }
                }

                if (j < 7)
                {
                    j++;
                }
                else
                { 
                    break; 
                }

                if (isKing)
                {
                    break;
                }
            }

            j = columnIndex - 1;
            for (int i = rowIndex - 1; i >= 0; i--)
            {
                if (CurrentPlayer == 1 && isKing && !_hasContinue)
                {
                    break;
                }

                if (direction[0] == 1 && direction[1] == 1 && !isKing)
                { 
                    break;
                }

                if (IsInsideBorders(i, j))
                {
                    if (Board[i, j] != 0 && Board[i, j] != CurrentPlayer)
                    {
                        isBeatStep = true;
                        if (!IsInsideBorders(i - 1, j - 1))
                        {
                            isBeatStep = false;
                        }
                        else if (Board[i - 1, j - 1] != 0)
                        {
                            isBeatStep = false;
                        }
                        else
                        { 
                            return isBeatStep;
                        }
                    }
                }

                if (j > 0)
                {
                    j--;
                }
                else
                { 
                    break; 
                }

                if (isKing)
                {
                    break;
                }
            }

            j = columnIndex - 1;
            for (int i = rowIndex + 1; i < 8; i++)
            {
                if (CurrentPlayer == 2 && isKing && !_hasContinue)
                {
                    break;
                }

                if (direction[0] == -1 && direction[1] == 1 && !isKing) 
                { 
                    break;
                }

                if (IsInsideBorders(i, j))
                {
                    if (Board[i, j] != 0 && Board[i, j] != CurrentPlayer)
                    {
                        isBeatStep = true;
                        if (!IsInsideBorders(i + 1, j - 1))
                        {
                            isBeatStep = false;
                        }
                        else if (Board[i + 1, j - 1] != 0)
                        {
                            isBeatStep = false;
                        }
                        else
                        {
                            return isBeatStep;
                        }
                    }
                }

                if (j > 0)
                {
                    j--;
                }
                else
                { 
                    break;
                }

                if (isKing)
                {
                    break;
                }
            }

            j = columnIndex + 1;
            for (int i = rowIndex + 1; i < 8; i++)
            {
                if (CurrentPlayer == 2 && isKing && !_hasContinue)
                { 
                    break;
                }

                if (direction[0] == -1 && direction[1] == -1 && !isKing) 
                { 
                    break;
                }

                if (IsInsideBorders(i, j))
                {
                    if (Board[i, j] != 0 && Board[i, j] != CurrentPlayer)
                    {
                        isBeatStep = true;
                        if (!IsInsideBorders(i + 1, j + 1))
                        {
                            isBeatStep = false;
                        }
                        else if (Board[i + 1, j + 1] != 0)
                        {
                            isBeatStep = false;
                        }
                        else
                        {
                            return isBeatStep;
                        }
                    }
                }

                if (j < 7)
                {
                    j++;
                }
                else
                {
                    break;
                }

                if (isKing)
                {
                    break;
                }
            }

            return isBeatStep;
        }

        public void CloseSteps()
        {
            foreach (var control in Controls)
            {
                if (control is Button b)
                {
                    b.BackColor = GetPrevButtonColor(b);
                }
            }
        }

        public bool IsInsideBorders(int i, int j)
        {
            return !(i >= _boardSize || j >= _boardSize || i < 0 || j < 0);
        }

        public void ActivateAllButtons()
        {
            foreach (var control in Controls)
            {
                if (control is Button b)
                {
                    b.Enabled = true;
                }
            }
        }

        public void DeactivateAllButtons()
        {
            foreach (var control in Controls)
            {
                if(control is Button b)
                {
                    b.Enabled = false;
                }
            }
        }

        private void ReceiveMove(string unparsedBoard)
        {
            try
            {
                //CurrentTurn = "Your Turn";
                //AlternativeTurn = "Opponent`s Turn";
                //this.Text = CurrentTurn;
                Board.Parse(unparsedBoard);
                CreateBoardWithBackgroundWorker();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void CheckersForm_Load(object sender, EventArgs e)
        {
            SetGame();

            try
            {
                _reader = new StreamReader(TCPServer.Instance.Client.GetStream());
                _writer = new StreamWriter(TCPServer.Instance.Client.GetStream());
                _writer.AutoFlush = true;
                //_stream = TCPServer.Instance.Client.GetStream();
                //TCPServer.Instance.Client.GetStream().CopyTo(_stream);
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker1.WorkerSupportsCancellation = false;
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TCPServer.Instance.Listener != null && (TCPServer.Instance.Client == null && !TCPServer.Instance.Client.Connected))
            {
                MessageBox.Show("Client connection failed.");
            }
        }

        private void CheckersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConnectionForm connection = new ConnectionForm();
            connection.Show();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (TCPServer.Instance.Client.Connected)
            {
                try
                {
                    var unparsedBoard = _reader.ReadLine();

                    if (!string.IsNullOrEmpty(unparsedBoard))
                    {
                        ReceiveMove(unparsedBoard);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (TCPServer.Instance.Client.Connected)
            {
                Board.WriteToStream(_writer);
            }
            else
            {
                MessageBox.Show("Message could not be sent!!");
            }

            backgroundWorker2.CancelAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(TCPServer.Instance.Client.Connected)
                backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
    }
}
