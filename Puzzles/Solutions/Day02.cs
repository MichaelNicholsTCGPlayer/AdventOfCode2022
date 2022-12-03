namespace Puzzles.Solutions
{
    public sealed class Day02 : IPuzzle
    {
        public int Day => 2;

        public string Puzzle1(string[] input)
        {
            var rounds = input.Select(r => Parse_RPS_RPS(r)).ToList();

            long totalScore = 0;
            foreach (var round in rounds)
            {
                totalScore += ScoreForRPS(round.yours);
                totalScore += ScoreForLDW(CalculateLDW(round.theirs, round.yours));
            }

            return totalScore.ToString();
        }

        public string Puzzle2(string[] input)
        {
            var rounds = input.Select(r => Parse_RPS_LDW(r)).ToList();

            long totalScore = 0;
            foreach (var round in rounds)
            {
                totalScore += ScoreForRPS(CalculateYourRPS(round.theirs, round.outcome));
                totalScore += ScoreForLDW(round.outcome);
            }

            return totalScore.ToString();

        }

        private (RPS theirs, RPS yours) Parse_RPS_RPS(string row)
        {
            return (ParseRPS(row[0]), ParseRPS(row[2]));
        }

        private (RPS theirs, LDW outcome) Parse_RPS_LDW(string row)
        {
            return (ParseRPS(row[0]), ParseLDW(row[2]));
        }

        private RPS ParseRPS(char c)
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

        private LDW ParseLDW(char c)
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

        private LDW CalculateLDW(RPS theirs, RPS yours)
        {
            if (theirs == yours)
            {
                return LDW.D;
            }
            else if (((int)theirs + 1) % 3 == (int)yours)
            {
                return LDW.W;
            }
            else
            {
                return LDW.L;
            }



            // Brute Force Logic
            //switch (theirs)
            //{
            //    case RPS.R:
            //        if (yours == RPS.P)
            //            return LDW.W;
            //        else
            //            return LDW.L;

            //    case RPS.P:
            //        if (yours == RPS.S)
            //            return LDW.W;
            //        else
            //            return LDW.L;

            //    case RPS.S:
            //        if (yours == RPS.R)
            //            return LDW.W;
            //        else
            //            return LDW.L;
            //}

            //throw new Exception("Invalid Input");
        }

        private RPS CalculateYourRPS(RPS theirs, LDW outcome)
        {
            return (RPS)(((int)theirs + (int)outcome + 2) % 3);

            // Brute Force Logic
            //RPS yours = RPS.P;

            //if (round.outcome == LDW.D)
            //{
            //    yours = round.theirs;
            //}
            //else if (round.outcome == LDW.W)
            //{
            //    switch (round.theirs)
            //    {
            //        case RPS.R:
            //            yours = RPS.P;
            //            break;

            //        case RPS.P:
            //            yours = RPS.S;
            //            break;

            //        case RPS.S:
            //            yours = RPS.R;
            //            break;
            //    }
            //}
            //else
            //{
            //    switch (round.theirs)
            //    {
            //        case RPS.R:
            //            yours = RPS.S;
            //            break;

            //        case RPS.P:
            //            yours = RPS.R;
            //            break;

            //        case RPS.S:
            //            yours = RPS.P;
            //            break;
            //    }
            //}
        }

        private int ScoreForLDW(LDW outcome)
        {
            // Loss: 0 points
            // Draw: 3 points
            // Win: 6 points
            return (int)outcome * 3;
        }

        private int ScoreForRPS(RPS yours)
        {
            // Rock: 1 point
            // Paper: 2 points
            // Scissors: 3 points
            return (int)yours + 1;
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
