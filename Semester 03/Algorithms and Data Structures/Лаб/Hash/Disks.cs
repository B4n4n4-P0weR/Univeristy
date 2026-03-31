namespace Hash;

public class Disk
{
    public string AlbumName { get; set; }
    public string ArtistName { get; set; }
    public int Year { get; set; }

    public override string ToString() => $"{ArtistName} / {AlbumName} / {Year}";
    
    public Disk(string albumName, string artistName, int year)
    {
        this.AlbumName = albumName;
        this.ArtistName = artistName;
        this.Year = year;
    }

    public Disk() { }
    
}