namespace DAL_TestSystem
{
    public class Question
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Difficalty { get; set; }
        public int? TestId { get; set; }
        public Test Test { get; set; }
    }
}