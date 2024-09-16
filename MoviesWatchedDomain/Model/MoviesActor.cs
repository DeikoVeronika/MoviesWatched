using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesWatchedDomain.Model;

public partial class MoviesActor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Оберіть фільм")]
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Оберіть актора")]
    public int ActorId { get; set; }

    public virtual Actor Actor { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
