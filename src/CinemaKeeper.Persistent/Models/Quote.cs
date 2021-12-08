using System;

namespace CinemaKeeper.Persistence.Models
{
    public class Quote
    {
        public Quote(Guid id, ulong userId, string text, DateTime createDate, ulong authorId)
        {
            Id = id;
            UserId = userId;
            Text = text;
            CreateDate = createDate;
            AuthorId = authorId;
        }

        public Quote(ulong userId, string text, DateTime createDate, ulong authorId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Text = text;
            CreateDate = createDate;
            AuthorId = authorId;
        }

        public Guid Id { get; set; }
        public ulong UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public ulong AuthorId { get; set; }
    }
}
