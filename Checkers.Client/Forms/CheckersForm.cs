using Checkers.Client.Networking;
using Checkers.Forms.Extensions;
using Checkers.Forms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers.Forms.Forms
{
    public partial class CheckersForm : Form
    {
        private const string PicturePath = @"C:\Users\EgorKuzmin\Pictures\Saved Pictures\";
        private readonly Size PictureSize;
        private const int Port = 7024;

        private int _boardSize;
        private const int CellSize = 100;
        public Board Board { get; set; }

        private List<Button> _simpleSteps = new List<Button>();
        private int _beatStepsCount = 0;
        private bool _hasContinue = false;
        private Button[,] _checkers;

        private NetworkStream _stream;

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
            //SetGame();
        }
        public void SetGame()
        {
            //if (isHost)
            //{
            //    CurrentPlayer = 1;
            //    //_server = new TcpListener(IPAddress.Any, Port);
            //    //_server.Start();
            //    //_socket = _server.AcceptSocket();
            //}
            //else
            //{
            //    try
            //    {
            //        CurrentPlayer = 2;
            //        //_client = new TcpClient(ip, Port);
            //        //_socket = _client.Client;
            //        //_receiver.RunWorkerAsync();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //        Close();
            //    }
            //}

            if (CurrentPlayer == 1)
            {
                label2.Text = "Your Turn";
            }
            else
            {
                label2.Text = "Opponent`s Turn";
            }

            IsInTurn = false;
            PreviousButton = null;

            _blackChecker = new Bitmap(new Bitmap(PicturePath + "black-removebg-preview.png"), PictureSize);
            _whiteChecker = new Bitmap(new Bitmap(PicturePath + "red-removebg-preview.png"), PictureSize);

            CreateBoard(this.Board);
        }

        public void CreateBoard(Board board)
        {
            panel1.Width = (_boardSize + 1) * (CellSize - 5);
            panel1.Height = (_boardSize + 1) * (CellSize - 5);

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * CellSize, i * CellSize);
                    button.Size = new Size(CellSize, CellSize);
                    button.Click += OnCheckerPressed;

                    if (board[i, j] == 1)
                    {
                        button.Image = _whiteChecker;
                    }

                    if (board[i, j] == 2)
                    {
                        button.Image = _blackChecker;
                    }

                    button.BackColor = GetPrevButtonColor(button);
                    button.ForeColor = Color.White;

                    _checkers[i, j] = button;

                    if (IsHandleCreated)
                    {
                        this.BeginInvoke((Action)(() =>
                        {
                            panel1.Controls.Add(button);
                        }));
                    }
                    else
                    {
                        panel1.Controls.Add(button);
                    }
                 
                }
            }
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

                    //var pressedButtonIndexes = _checkers.Find(PressedButton);
                    //var prevButtonIndexes = _checkers.Find(PreviousButton);

                    //if (PressedButton.Image == _whiteChecker)
                    //{
                    //    buffer = new byte[] { 0, (byte)pressedButtonIndexes.Item1, (byte)pressedButtonIndexes.Item2, (byte)prevButtonIndexes.Item1, (byte)prevButtonIndexes.Item2 };
                    //}
                    //else
                    //{
                    //    buffer = new byte[] { 1, (byte)pressedButtonIndexes.Item1, (byte)pressedButtonIndexes.Item2, (byte)prevButtonIndexes.Item1, (byte)prevButtonIndexes.Item2 };
                    //}

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

                        backgroundWorker2.RunWorkerAsync();

                        //IFormatter formatter = new BinaryFormatter();
                        //using (NetworkStream stream = _client.GetStream())
                        //{
                        //    formatter.Serialize(stream, Board);
                        //    //var buffer = stream;
                        //    //_socket.Send(buffer);

                        //    if (!_receiver.IsBusy)
                        //    {
                        //        _receiver.RunWorkerAsync();
                        //    }
                        //}

                        ShowPossibleSteps();
                        _hasContinue = false;
                    }
                    else if (_hasContinue)
                    {
                        PressedButton.BackColor = Color.Red;
                        PressedButton.Enabled = true;
                        IsInTurn = true;
                    }
                }
            }

            PreviousButton = PressedButton;
        }

        public void SwitchPlayer()
        {
            if(CurrentPlayer == 1)
            {
                label2.Text = "Your Turn";
            }
            else
            {
                label2.Text = "Opponent`s Turn";
            }

            CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;
            //ResetGame();
        }

        //public void ResetGame()
        //{
        //    bool isPlayer1HasCheckers = false;
        //    bool isPlayer2HasCheckers = false;

        //    for (int i = 0; i < _boardSize; i++)
        //    {
        //        for (int j = 0; j < _boardSize; j++)
        //        {
        //            if (Board[i, j] == 1)
        //            {
        //                isPlayer1HasCheckers = true;
        //            }
                        
        //            if (Board[i, j] == 2)
        //            {
        //                isPlayer2HasCheckers = true;
        //            }
        //        }
        //    }

        //    if (!isPlayer1HasCheckers || !isPlayer2HasCheckers)
        //    {
        //        this.Controls.Clear();
        //    }
        //}


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

        public void ShowSteps(int rowIndex, int columnIndex, bool isKing = true)
        {
            _simpleSteps.Clear();
            ShowDiagonal(rowIndex, columnIndex, isKing);

            if (_beatStepsCount > 0)
            {
                CloseSimpleSteps(_simpleSteps);
            }
        }

        public void ShowDiagonal(int rowIndex, int columnIndex, bool isKing = false)
        {
            int j = columnIndex + 1;
            for (int i = rowIndex - 1; i >= 0; i--)
            {
                if (CurrentPlayer == 1 && isKing && !_hasContinue)
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

                if (isKing)
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

        private void ReceiveMove()
        {
            //    byte[] buffer = new byte[byte.MaxValue];
            //    _socket.Receive(buffer);

            //    IFormatter formatter = new BinaryFormatter();
            //    using (MemoryStream stream = new MemoryStream(buffer))
            //    {
            //        var board = formatter.Deserialize(stream);
            //        Board = board as Board;

            //        if (!_receiver.IsBusy)
            //        {
            //            _receiver.RunWorkerAsync();
            //        }

            //        CreateBoard(this.Board);
            //    }

            try
            {
                //if(_client?.Connected == true)
                //{

                //    IFormatter formatter = new BinaryFormatter();
                //    using (NetworkStream stream = _client.GetStream())
                //    {
                //        var board = formatter.Deserialize(stream);
                //        Board = board as Board;

                //        if (!_receiver.IsBusy)
                //        {
                //            _receiver.RunWorkerAsync();
                //        }

                //        CreateBoard(this.Board);
                //    }
                //}
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
                _stream = TCPClient.Instance.Client.GetStream();
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            timer1.Start();
        }

        private void CheckersForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConnectionForm connection = new ConnectionForm();
            connection.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!TCPClient.Instance.Client.Connected)
            {
                MessageBox.Show("Connection failed.");
                ConnectionForm connection = new ConnectionForm();
                connection.Show();
                Close();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (TCPClient.Instance.Client.Connected)
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    var board = formatter.Deserialize(_stream);
                    Board = board as Board;
                    CreateBoard(this.Board);
                    _stream.Flush();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (TCPClient.Instance.Client.Connected)
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(_stream, this.Board);
            }
            else
            {
                MessageBox.Show("Message could not be sent!!");
            }

            backgroundWorker2.CancelAsync();
        }
    }
}
