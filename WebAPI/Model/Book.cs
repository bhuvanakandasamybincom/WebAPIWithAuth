namespace BoardCasterWebAPI.Model
{
    public class Book
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        [MaxLength(150)]
        public string Title { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        [StringLength(150)]
        [MaxLength(150)]
        public string ISBN { get; set; }
        [Required]
        public int CopiesAvailable { get; set; }

    }
}
