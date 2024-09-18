using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesWatchedDomain.Model;

public partial class Language
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введіть мову перегляду фільму")]
    [Display(Name = "Мова перегляду фільму")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
