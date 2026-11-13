using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjUserDetail
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }
}
