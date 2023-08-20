using System;

namespace WalkerAcademy.Models
{
    public class Info
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public string? Next { get; set; }
        public string? Prev { get; set; }
    }
}
