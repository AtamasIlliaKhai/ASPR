namespace ASPR_Practice1
{
    internal class SimplexFullResult
    {
        public SimplexResult Result { get; private set; }
        public SimplexTable FinalTable { get; private set; }

        public SimplexFullResult(SimplexResult result, SimplexTable finalTable)
        {
            Result = result;
            FinalTable = finalTable;
        }
    }
}