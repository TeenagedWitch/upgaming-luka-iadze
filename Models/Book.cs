namespace upgaming_luka_iadze.Models;

public class Book
{
    public int ID { get; set; }
    public string Title { get; set; }
    public int AuthorID { get; set; }
    public int PublicationYear { get; set; }
}