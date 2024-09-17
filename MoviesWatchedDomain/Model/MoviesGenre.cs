using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesWatchedDomain.Model;

public partial class MoviesGenre
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Оберіть фільм")]
    public int MovieId { get; set; }
    [Required(ErrorMessage = "Оберіть жанр")]
    public int GenreId { get; set; }

    public virtual Genre? Genre { get; set; } = null!;

    public virtual Movie? Movie { get; set; } = null!;
}
