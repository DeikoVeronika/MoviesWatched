using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesWatchedDomain.Model;

public partial class Actor
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введіть ім'я актора")]
    [Display(Name="Ім'я актора")]
    public string Name { get; set; } = null!;

    [Display(Name = "Фото актора")]
    public byte[]? ActorImage { get; set; }

    public virtual ICollection<MoviesActor> MoviesActors { get; set; } = new List<MoviesActor>();
}
