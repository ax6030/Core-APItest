namespace Todo.Dto
{
    public class ReturnJson
    {
        public dynamic Data { get; set; } 
        public int HttpCode { get; set; } 
        public string ErrorMessage { get; set; } 

        public dynamic Error { get; set; } 

        public string User {  get; set; }
    }
}
