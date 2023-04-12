public class TicTacToe
{
    private static void Main()
    {
        var game = new Game();

        game.Play();
    }

    private enum Signs
    {
        X,
        O,
        Empty
    }

    private enum State
    {
        XWin,
        OWin,
        Draw,
        NotFinished
    }

    private class Game
    {
        private Signs[,] _board;
        private bool _isX;

        public Game()
        {
            _board = new[,]
            {
                { Signs.Empty, Signs.Empty, Signs.Empty },
                { Signs.Empty, Signs.Empty, Signs.Empty },
                { Signs.Empty, Signs.Empty, Signs.Empty }
            };
            _isX = true;
        }

        public void MakeMove((int x, int y) move)
        {
            if (_isX)
                _board[move.x, move.y] = Signs.X;
            else
                _board[move.x, move.y] = Signs.O;
            _isX = !_isX;
        }

        private void RevertMove((int x, int y) move)
        {
            _board[move.x, move.y] = Signs.Empty;
            _isX = !_isX;
        }


        private bool IsFull()
        {
            foreach (var tile in _board)
                if (tile == Signs.Empty)
                    return false;

            return true;
        }

        public void DrawBoard()
        {
            Console.WriteLine(" -------------");
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                    if (_board[x, y] == Signs.Empty)
                        Console.Write(" |  ");
                    else
                        Console.Write(" | " + _board[x, y]);
                Console.WriteLine(" |");
                Console.WriteLine(" -------------");
            }
        }

        private State CheckState()
        {
            for (var x = 0; x < 3; x++)
                if (_board[x, 0] == _board[x, 1] && _board[x, 1] == _board[x, 2] && _board[x, 0] != Signs.Empty)
                {
                    if (_board[x, 0] == Signs.X)
                        return State.XWin;
                    return State.OWin;
                }

            for (var y = 0; y < 3; y++)
                if (_board[0, y] == _board[1, y] && _board[1, y] == _board[2, y] && _board[0, y] != Signs.Empty)
                {
                    if (_board[0, y] == Signs.X)
                        return State.XWin;
                    return State.OWin;
                }

            if (_board[0, 0] == _board[1, 1] && _board[1, 1] == _board[2, 2] && _board[1, 1] != Signs.Empty)
            {
                if (_board[1, 1] == Signs.X)
                    return State.XWin;
                return State.OWin;
            }

            if (_board[0, 2] == _board[1, 1] && _board[1, 1] == _board[2, 0] && _board[1, 1] != Signs.Empty)
            {
                if (_board[1, 1] == Signs.X)
                    return State.XWin;
                return State.OWin;
            }

            if (IsFull())
                return State.Draw;
            return State.NotFinished;
        }

        private List<(int x, int y)> GenLegalMoves()
        {
            var legalMoves = new List<(int, int)>();
            for (var x = 0; x < 3; x++)
            for (var y = 0; y < 3; y++)
                if (_board[x, y] == Signs.Empty)
                    legalMoves.Add((x, y));

            return legalMoves;
        }

        private int Evaluate()
        {
            var state = CheckState();

            if (state == State.Draw)
                return 0;
            if (state == State.XWin)
                return 1000;
            return -1000;
        }

        private int Negamax()
        {
            if (IsFull())
            {
                if (_isX)
                    return Evaluate();
                return Evaluate() * -1;
            }

            var value = -2000;
            var legalMoves = GenLegalMoves();
            foreach (var move in legalMoves)
            {
                MakeMove(move);
                value = Math.Max(value, -Negamax());
                RevertMove(move);
            }

            return value;
        }

        private void AiMove()
        {
            var value = -1000;
            var legalMoves = GenLegalMoves();
            var bestMove = (0, 0);
            foreach (var move in legalMoves)
            {
                MakeMove(move);
                var score = -Negamax();
                RevertMove(move);
                if (score > value)
                {
                    bestMove = move;
                    value = score;
                }
            }

            MakeMove(bestMove);
        }

        private bool IsEmpty((int x, int y) move)
        {
            if (_board[move.x, move.y] == Signs.Empty)
                return true;
            return false;
        }

        private void Multiplayer()
        {
            var state = State.NotFinished;
            while (state == State.NotFinished)
            {
                DrawBoard();
                Console.Write("Enter the tile you want to use(1-9): ");
                var input = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException()) - 1;
                var move = (input / 3, input % 3);
                if (IsEmpty(move))
                    MakeMove(move);
                else
                    Console.WriteLine("Please select an empty tile!");
                state = CheckState();
            }

            DrawBoard();
            Console.WriteLine(state.ToString());
        }

        private void Solo()
        {
            var state = State.NotFinished;
            while (state == State.NotFinished)
            {
                DrawBoard();
                Console.Write("Enter the tile you want to use(1-9): ");
                var input = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException()) - 1;
                var move = (input / 3, input % 3);
                if (IsEmpty(move))
                    MakeMove(move);
                else
                    Console.WriteLine("Please select an empty tile!");
                state = CheckState();
                if (state != State.NotFinished) break;
                AiMove();
                state = CheckState();
            }

            DrawBoard();
            Console.WriteLine(state.ToString());
        }

        private void Clear()
        {
            _board = new[,]
            {
                { Signs.Empty, Signs.Empty, Signs.Empty },
                { Signs.Empty, Signs.Empty, Signs.Empty },
                { Signs.Empty, Signs.Empty, Signs.Empty }
            };
        }

        public void Play()
        {
            while (true)
            {
                Console.Write("Type M for Multiplayer and S for Solo play: ");
                var input = Console.ReadLine()?.ToLower();
                if (input == "m")
                    Multiplayer();
                else if (input == "s") Solo();
                Console.Write("Type Q to Quit or any letter to Continue : ");
                var quit = Console.ReadLine()?
                    .ToLower();
                if (quit == "q")
                    break;
                Clear();
            }
        }
    }
}