using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class HfTblTopic
{
    public int TopicId { get; set; }

    public int CourseId { get; set; }

    public int? ParentTopicId { get; set; }

    public string? TopicName { get; set; }

    public string? Description { get; set; }

    public string? TrainerVideo { get; set; }

    public string? StudentVideo { get; set; }

    public string? HindiVideo { get; set; }

    public string? TrainerNote { get; set; }

    public string? StudentNote { get; set; }

    public int? Order { get; set; }

    public bool? IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? TrainerResource { get; set; }

    public string? StudentResourse { get; set; }

    public string? TrainerExercise { get; set; }

    public string? StudentExercise { get; set; }

    public int? LanguageId { get; set; }

    public int? NoOfQuestion { get; set; }

    public virtual HfTblCourse Course { get; set; } = null!;
}
