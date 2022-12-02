using System.IO.Pipes;
using System.Reflection.Emit;

namespace Puzzles.Solutions
{
    public sealed class Day02 : IPuzzle
    {
        public int Day => 2;

        public string Puzzle1(string[] input)
        {
            var rounds = input.Select(r => Parse_1(r)).ToList();

            long totalScore = 0;
            foreach (var round in rounds)
            {
                totalScore += Score_1(round);
            }

            return totalScore.ToString();
        }

        private int Score_1((RPS theirs, RPS yours) round)
        {
            int score = 0;
            switch (round.yours)
            {
                case RPS.R:
                    score += 1; // Rock: 1 point
                    break;

                case RPS.P:
                    score += 2; // Paper: 2 points
                    break;

                case RPS.S:
                    score += 3; // Scissors: 3 points
                    break;
            }

            // Loss: 0 points
            // Draw: 3 points
            // Win: 6 points
            var roundResult = RoundResult_1(round.theirs, round.yours);
            score += (int)roundResult * 3;

            return score;
        }

        private (RPS theirs, RPS yours) Parse_1(string row)
        {
            return (Parse_1(row[0]), Parse_1(row[2]));
        }

        private RPS Parse_1(char c)
        {
            switch (c)
            {
                case 'A':
                case 'X':
                    return RPS.R;

                case 'B':
                case 'Y':
                    return RPS.P;

                case 'C':
                case 'Z':
                    return RPS.S;

            }

            throw new Exception("Unknown Type");
        }

        private LDW RoundResult_1(RPS theirs, RPS yours)
        {
            if (theirs == yours)
            {
                return LDW.D;
            }

            switch (theirs)
            {
                case RPS.R:
                    if (yours == RPS.P)
                        return LDW.W;
                    else
                        return LDW.L;

                case RPS.P:
                    if (yours == RPS.S)
                        return LDW.W;
                    else
                        return LDW.L;

                case RPS.S:
                    if (yours == RPS.R)
                        return LDW.W;
                    else
                        return LDW.L;
            }

            throw new Exception("Invalid Input");
        }





        public string Puzzle2(string[] input)
        {
            var rounds = input.Select(r => Parse_2(r)).ToList();

            long totalScore = 0;
            foreach (var round in rounds)
            {
                totalScore += Score_2(round);
            }

            return totalScore.ToString();

        }

        private (RPS theirs, LDW yours) Parse_2(string row)
        {
            return (Parse_1(row[0]), Parse_2(row[2]));
        }

        private LDW Parse_2(char c)
        {
            switch (c)
            {
                case 'X':
                    return LDW.L;

                case 'Y':
                    return LDW.D;

                case 'Z':
                    return LDW.W;

            }

            throw new Exception("Unknown Type");
        }

        private int Score_2((RPS theirs, LDW outcome) round)
        {
            int score = 0;
            RPS yours = RPS.P;

            if (round.outcome == LDW.D)
            {
                yours = round.theirs;
            }
            else if (round.outcome == LDW.W)
            {
                switch (round.theirs)
                {
                    case RPS.R:
                        yours = RPS.P;
                        break;

                    case RPS.P:
                        yours = RPS.S;
                        break;

                    case RPS.S:
                        yours = RPS.R;
                        break;
                }
            }
            else
            {
                switch (round.theirs)
                {
                    case RPS.R:
                        yours = RPS.S;
                        break;

                    case RPS.P:
                        yours = RPS.R;
                        break;

                    case RPS.S:
                        yours = RPS.P;
                        break;
                }
            }

            switch (yours)
            {
                case RPS.R:
                    score += 1; // Rock: 1 point
                    break;

                case RPS.P:
                    score += 2; // Paper: 2 points
                    break;

                case RPS.S:
                    score += 3; // Scissors: 3 points
                    break;
            }

            // Loss: 0 points
            // Draw: 3 points
            // Win: 6 points
            score += (int)round.outcome * 3;

            return score;
        }


        private enum RPS
        {
            R = 0,
            P = 1,
            S = 2,
        }

        private enum LDW
        {
            L = 0,
            D = 1,
            W = 2,
        }
    }
}
