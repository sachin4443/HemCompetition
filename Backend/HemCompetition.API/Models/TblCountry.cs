using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class TblCountry
{
    public int Id { get; set; }

    public string Sortname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Phonecode { get; set; }
}
