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
        return 100
            - misplacedBooks * 5
            - dirtiness * 3
            - unhappyVisitors * 10;
    }
}
