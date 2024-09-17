using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesWatchedDomain.Model;

public partial class Movie
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введіть назву фільму")]
    [Display(Name = "Назва фільму")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "Введіть дату виходу фільму")]
    [Display(Name = "Дата виходу фільму")]
    public short ReleaseDate { get; set; }
    [Required(ErrorMessage = "Оберіть мову якою переглядали")]
    [Display(Name = "Мова якою переглядали")]
    public int LanguageId { get; set; }

    [Display(Name = "Тривалість (в хв)")]
    public short? Duration { get; set; }

    public byte[]? MovieImage { get; set; }

    [Display(Name = "Дата перегляду, оцінка, коментар")]
    public string? ReviewDate { get; set; }

    public virtual Language? Language { get; set; } = null!;

    public virtual ICollection<MoviesActor> MoviesActors { get; set; } = new List<MoviesActor>();

    public virtual ICollection<MoviesGenre> MoviesGenres { get; set; } = new List<MoviesGenre>();
}
