using System;

namespace TodoApiService.Models
{
    public class RefreshToken
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public Guid SessionId { get; set; }
        public Session Session { get; set; }
    }
}