using System;

namespace CinemaKeeper.Persistence.Models
{
    public class Quote
    {
        public Guid Id { get; set; }
        public ulong UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public ulong AuthorId { get; set; }
    }
}
