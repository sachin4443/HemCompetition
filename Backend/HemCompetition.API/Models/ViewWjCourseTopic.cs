using System;
using System.Collections.Generic;

namespace HemCompetition.API.Models;

public partial class ViewWjCourseTopic
{
    public int TopicId { get; set; }

    public int CourseId { get; set; }

    public string? CourseName { get; set; }

    public string? ParentTopicName { get; set; }

    public string? TopicName { get; set; }

    public string? ParentTopicDescription { get; set; }

    public string? VideoLink { get; set; }

    public int? ParentTopicOrder { get; set; }

    public int? Order { get; set; }
}
