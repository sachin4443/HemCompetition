using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class WjScrambleMaster
{
    public int Id { get; set; }

    public string? ScrambleWord { get; set; }

    public string? UnscrambleWord { get; set; }

    public int NoofShowLetter { get; set; }

    public int FirstIndex { get; set; }

    public int SecondIndex { get; set; }

    public int ThirdIndex { get; set; }

    public int FourthIndex { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int TopicId { get; set; }

    public int QuestionId { get; set; }
}
