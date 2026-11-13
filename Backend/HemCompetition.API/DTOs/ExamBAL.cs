using Dapper;
using HemCompetition.API.DTOs;
using HemCompetition.API.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient; // Or Microsoft.Data.SqlClient
public class ExamBAL
{
    private readonly string _connectionString;

    public ExamBAL(string connectionString)
    {
        _connectionString = connectionString;
    }

    public ExamReviewVM GetUserExamDetials(int examId)
    {
        var examReviewVM = new ExamReviewVM
        {
            ExaminationRecords = new List<ExaminationRecordVM>(),
            Questions = new List<QuestionViewModel1>()
        };

        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            var parameters = new { ExamID = examId };

            // Execute the stored procedure supporting multiple result sets
            using (var multi = db.QueryMultiple("sp_GetExamReviewDetails", parameters, commandType: CommandType.StoredProcedure))
            {
                var examRecords = multi.Read<ExaminationRecordVM>().ToList();
                if (!examRecords.Any()) return examReviewVM;

                examReviewVM.ExaminationRecords = examRecords;

                var questions = multi.Read<QuestionViewModel1>().ToList();
                var options = multi.Read<QuestionOptionViewModel>().ToList();

                // Assemble the view model structure
                foreach (var data in examRecords)
                {
                    var question = questions.FirstOrDefault(q => q.QuestionID == data.QuestionID);
                    if (question != null)
                    {
                        var qvm = new QuestionViewModel1
                        {
                            QuestionID = question.QuestionID,
                            QuestionName = question.QuestionName,
                            Marks = question.Marks,
                            Type = question.Type,
                            ComplexityLevel = question.ComplexityLevel,
                            AnswerDescription = question.AnswerDescription,
                            CourseID = question.CourseID,
                            TopicID = question.TopicID,
                            QuestionOption = options
                                .Where(o => o.QuestionID == question.QuestionID)
                                .Select(option => new QuestionOptionViewModel
                                {
                                    OptionID = option.OptionID,
                                    Options = option.Options,
                                    // Highlights the option selected by the user
                                    IsAnswer = option.OptionID == data.Answer
                                }).ToList()
                        };

                        examReviewVM.Questions.Add(qvm);
                    }
                }
            }
        }

        return examReviewVM;
    }
}

public class ExamReviewVM
{
    public ExamReviewVM()
    {
        Questions = new List<QuestionViewModel1>();
        ExaminationRecords = new List<ExaminationRecordVM>();
    }
    public List<QuestionViewModel1> Questions { get; set; }
    public List<ExaminationRecordVM> ExaminationRecords { get; set; }
}
[Serializable]
public class ExaminationRecordVM
{
    public int RecordID { get; set; }
    public Nullable<int> ExaminationID { get; set; }
    public Nullable<int> QuestionID { get; set; }
    public string? RecordStatus { get; set; }
    public Nullable<int> Answer { get; set; }
    public Nullable<bool> isCorrect { get; set; }
    public Nullable<System.DateTime> SubmitTiming { get; set; }
    public Nullable<int> OrderNo { get; set; }
}