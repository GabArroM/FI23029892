using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;


public class Bookcontext : DbContext
{
    //tablas
    public DbSet<Author> Authors { get; set; }
    public DbSet<Title> Titles { get; set; }    
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TitleTag> TitleTags { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {      
        var folder = Environment.CurrentDirectory;
        var path = Path.Join(folder, "BooksCF.db");
        optionsBuilder.UseSqlite($"Data Source={path}");
    }
}


public class Author
{
    [NotNull]
    public int AuthorId { get; set; }
    [NotNull]
    public string? AuthorName { get; set; } = string.Empty;

}

public class Title
{
    [NotNull]
    public int TitleId { get; set; }

    // Foreign Key
    [NotNull]
    public int AuthorId { get; set; }   
    [NotNull]
    public Author? Author { get; set; }    

     [NotNull]
    public string? TitleName { get; set; } = string.Empty;
  
}

public class Tag
{
    [NotNull]
    public int TagId { get; set; }
    [NotNull]
    public string? TagName { get; set; } = string.Empty;

}

public class TitleTag
{
    [NotNull]
    public int TitleTagId { get; set; }    
  
  // Foreign Keys
    [NotNull]
    public int TitleId { get; set; } 
    [NotNull]
    public int TagId { get; set; }      
     
    [NotNull]
    public Title? Title { get; set; }
    [NotNull]
    public Tag? Tag { get; set; }
}