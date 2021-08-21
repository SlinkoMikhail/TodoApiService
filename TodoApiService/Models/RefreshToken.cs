using System;

namespace TodoApiService.Models
{
    public class RefreshToken
    {
        public long Id { get; set; }
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public string Token { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
    }
}