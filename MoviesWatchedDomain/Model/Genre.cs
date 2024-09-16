using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesWatchedDomain.Model;

public partial class Genre
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введіть назву жанру")]
    [Display(Name = "Назва жанру")]
    public string Name { get; set; } = null!;

    public byte[]? GenreImage { get; set; }

    public virtual ICollection<MoviesGenre> MoviesGenres { get; set; } = new List<MoviesGenre>();
}
