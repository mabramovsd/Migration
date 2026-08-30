namespace Migration.Contracts.DTO
{
    public class ValidationErrorResponse
    {
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
