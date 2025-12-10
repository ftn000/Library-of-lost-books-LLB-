public static class InspectionScore
{
    public static int misplacedBooks = 0;
    public static int dirtiness = 0;
    public static int unhappyVisitors = 0;

    public static void Reset()
    {
        misplacedBooks = 0;
        dirtiness = 0;
        unhappyVisitors = 0;
    }

    public static int FinalScore()
    {
        int score = 100
            - misplacedBooks * 5
            - dirtiness * 3
            - unhappyVisitors * 10;

        if (score < 0) score = 0;
        if (score > 100) score = 100;
        return score;
    }
}
