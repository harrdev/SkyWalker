namespace WalkerAcademy.Models
{
    public class ApiResponse<T>
    {
        public Info Info { get; set; }
        public List<T> Data { get; set; }
    }
}
