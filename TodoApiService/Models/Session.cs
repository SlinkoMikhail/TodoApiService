using System;

namespace TodoApiService.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public RefreshToken RefreshToken{ get; set; }
    }
}