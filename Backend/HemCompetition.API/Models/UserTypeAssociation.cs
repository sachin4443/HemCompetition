using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class UserTypeAssociation
{
    public int UserId { get; set; }

    public int UserType { get; set; }
}
