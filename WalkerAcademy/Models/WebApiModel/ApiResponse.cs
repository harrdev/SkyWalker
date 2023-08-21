namespace WalkerAcademy.Models.WebApiModel
{
    public class ApiResponse<T>
    {
        public Info Info { get; set; }
        public List<T> Data { get; set; }
    }
}
