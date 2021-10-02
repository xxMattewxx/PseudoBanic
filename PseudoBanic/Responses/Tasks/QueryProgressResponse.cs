namespace PseudoBanic.Responses
{
    class QueryProgressResponse : BaseResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int TotalDone { get; set; }
        public int TotalGenerated { get; set; }
    }
}
