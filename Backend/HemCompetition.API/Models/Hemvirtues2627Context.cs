using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Models;

public partial class Hemvirtues2627Context : DbContext
{
    public Hemvirtues2627Context()
    {
    }

    public Hemvirtues2627Context(DbContextOptions<Hemvirtues2627Context> options)
        : base(options)
    {
    }
   
    public virtual DbSet<ActualExaminationResult> ActualExaminationResults { get; set; }

    public virtual DbSet<AnubhutiMaster> AnubhutiMasters { get; set; }

    public virtual DbSet<AnubhutiTransaction> AnubhutiTransactions { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<CKenKenHeader> CKenKenHeaders { get; set; }

    public virtual DbSet<CKenKenLine> CKenKenLines { get; set; }

    public virtual DbSet<CMasterKenken> CMasterKenkens { get; set; }

    public virtual DbSet<CfAnubhutiImagesUpload> CfAnubhutiImagesUploads { get; set; }

    public virtual DbSet<CommissionLog> CommissionLogs { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<CourseCategoryLookup> CourseCategoryLookups { get; set; }

    public virtual DbSet<CourseCustomerType> CourseCustomerTypes { get; set; }

    public virtual DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }

    public virtual DbSet<CreativityActivitesSubmission2021> CreativityActivitesSubmission2021s { get; set; }

    public virtual DbSet<CreativityActivitesSubmission2025Deletedatum> CreativityActivitesSubmission2025Deletedata { get; set; }

    public virtual DbSet<CreativityActivitesSubmission2026Backup> CreativityActivitesSubmission2026Backups { get; set; }

    public virtual DbSet<CustomerTypeLookup> CustomerTypeLookups { get; set; }

    public virtual DbSet<DbchangeLog> DbchangeLogs { get; set; }

    public virtual DbSet<EventData2022> EventData2022s { get; set; }

    public virtual DbSet<Examination> Examinations { get; set; }

    public virtual DbSet<ExaminationOption> ExaminationOptions { get; set; }

    public virtual DbSet<ExaminationRecord> ExaminationRecords { get; set; }

    public virtual DbSet<FutoshikiAnswer> FutoshikiAnswers { get; set; }

    public virtual DbSet<FutoshikiCompetitionMaster> FutoshikiCompetitionMasters { get; set; }

    public virtual DbSet<FutoshikiCompetitionSolution> FutoshikiCompetitionSolutions { get; set; }

    public virtual DbSet<FutoshikiCompetitionTarget> FutoshikiCompetitionTargets { get; set; }

    public virtual DbSet<FutoshikiMaster> FutoshikiMasters { get; set; }

    public virtual DbSet<FutoshikiSolution> FutoshikiSolutions { get; set; }

    public virtual DbSet<FutoshikiTransaction> FutoshikiTransactions { get; set; }

    public virtual DbSet<FutoshikiUserAnswer> FutoshikiUserAnswers { get; set; }

    public virtual DbSet<Game24Competition> Game24Competitions { get; set; }

    public virtual DbSet<Game24Solution> Game24Solutions { get; set; }

    public virtual DbSet<Game24UserSoln> Game24UserSolns { get; set; }

    public virtual DbSet<GetSelectedStandardList> GetSelectedStandardLists { get; set; }

    public virtual DbSet<GuestsForEvent> GuestsForEvents { get; set; }

    public virtual DbSet<HfAffiliateVisitEvent> HfAffiliateVisitEvents { get; set; }

    public virtual DbSet<HfAffiliateVisitProcessImageUpload> HfAffiliateVisitProcessImageUploads { get; set; }

    public virtual DbSet<HfCryViewGetInstituteStudentSummary> HfCryViewGetInstituteStudentSummaries { get; set; }

    public virtual DbSet<HfDbchangeLog> HfDbchangeLogs { get; set; }

    public virtual DbSet<HfGameCountRecord> HfGameCountRecords { get; set; }

    public virtual DbSet<HfMasCity> HfMasCities { get; set; }

    public virtual DbSet<HfMasMainCourse> HfMasMainCourses { get; set; }

    public virtual DbSet<HfMonthlyBill> HfMonthlyBills { get; set; }

    public virtual DbSet<HfProject> HfProjects { get; set; }

    public virtual DbSet<HfRelManageGroupList> HfRelManageGroupLists { get; set; }

    public virtual DbSet<HfRelManageGroupListDeletedatum> HfRelManageGroupListDeletedata { get; set; }

    public virtual DbSet<HfSchoolVisit> HfSchoolVisits { get; set; }

    public virtual DbSet<HfSchoolVisitSchedule> HfSchoolVisitSchedules { get; set; }

    public virtual DbSet<HfSection> HfSections { get; set; }

    public virtual DbSet<HfSectionSubmission> HfSectionSubmissions { get; set; }

    public virtual DbSet<HfSpInsituteWiseRegistrationSummaryReport> HfSpInsituteWiseRegistrationSummaryReports { get; set; }

    public virtual DbSet<HfSpInstituteWiseAffiliateName> HfSpInstituteWiseAffiliateNames { get; set; }

    public virtual DbSet<HfSpStdWiseInstituteStudentsRankAndTotalPoint> HfSpStdWiseInstituteStudentsRankAndTotalPoints { get; set; }

    public virtual DbSet<HfSpVolunteerSchoolAssociation> HfSpVolunteerSchoolAssociations { get; set; }

    public virtual DbSet<HfSurveyDatum> HfSurveyData { get; set; }

    public virtual DbSet<HfTblAcademicTerm> HfTblAcademicTerms { get; set; }

    public virtual DbSet<HfTblAcademicYear> HfTblAcademicYears { get; set; }

    public virtual DbSet<HfTblAffiliateExpence> HfTblAffiliateExpences { get; set; }

    public virtual DbSet<HfTblAffiliateKraFile> HfTblAffiliateKraFiles { get; set; }

    public virtual DbSet<HfTblAffliateFocusedList> HfTblAffliateFocusedLists { get; set; }

    public virtual DbSet<HfTblCompEligibleUser> HfTblCompEligibleUsers { get; set; }

    public virtual DbSet<HfTblCompTime> HfTblCompTimes { get; set; }

    public virtual DbSet<HfTblCourse> HfTblCourses { get; set; }

    public virtual DbSet<HfTblCourseCategoryLookup> HfTblCourseCategoryLookups { get; set; }

    public virtual DbSet<HfTblCreativityActivitesSubmission2026> HfTblCreativityActivitesSubmission2026s { get; set; }

    public virtual DbSet<HfTblCreativityActivitySubmission> HfTblCreativityActivitySubmissions { get; set; }

    public virtual DbSet<HfTblEmailLog> HfTblEmailLogs { get; set; }

    public virtual DbSet<HfTblEmailReminderLog> HfTblEmailReminderLogs { get; set; }

    public virtual DbSet<HfTblExamConfiguration> HfTblExamConfigurations { get; set; }

    public virtual DbSet<HfTblExceptionLoggingToDataBase> HfTblExceptionLoggingToDatabases { get; set; }

    public virtual DbSet<HfTblFailedUserRecord> HfTblFailedUserRecords { get; set; }

    public virtual DbSet<HfTblFeatureFlag> HfTblFeatureFlags { get; set; }

    public virtual DbSet<HfTblGroupProjectFile> HfTblGroupProjectFiles { get; set; }

    public virtual DbSet<HfTblHeProjectSubmission> HfTblHeProjectSubmissions { get; set; }

    public virtual DbSet<HfTblHegroupInvitation> HfTblHegroupInvitations { get; set; }

    public virtual DbSet<HfTblIndividualProjectFile> HfTblIndividualProjectFiles { get; set; }

    public virtual DbSet<HfTblInstitute> HfTblInstitutes { get; set; }

    public virtual DbSet<HfTblInstituteSubject> HfTblInstituteSubjects { get; set; }

    public virtual DbSet<HfTblLanguage> HfTblLanguages { get; set; }

    public virtual DbSet<HfTblLeadsInstitute> HfTblLeadsInstitutes { get; set; }

    public virtual DbSet<HfTblLevel> HfTblLevels { get; set; }

    public virtual DbSet<HfTblMainCourse> HfTblMainCourses { get; set; }

    public virtual DbSet<HfTblMainCoursesLanguage> HfTblMainCoursesLanguages { get; set; }

    public virtual DbSet<HfTblManageGroupListVacationCompetition> HfTblManageGroupListVacationCompetitions { get; set; }

    public virtual DbSet<HfTblMasStockType> HfTblMasStockTypes { get; set; }

    public virtual DbSet<HfTblMenu> HfTblMenus { get; set; }

    public virtual DbSet<HfTblMenuActionMap> HfTblMenuActionMaps { get; set; }

    public virtual DbSet<HfTblMenuTable> HfTblMenuTables { get; set; }

    public virtual DbSet<HfTblMonthlyBill> HfTblMonthlyBills { get; set; }

    public virtual DbSet<HfTblNotification> HfTblNotifications { get; set; }

    public virtual DbSet<HfTblParent> HfTblParents { get; set; }

    public virtual DbSet<HfTblParentChildAssociation> HfTblParentChildAssociations { get; set; }

    public virtual DbSet<HfTblPractical> HfTblPracticals { get; set; }

    public virtual DbSet<HfTblPracticalTranslation> HfTblPracticalTranslations { get; set; }

    public virtual DbSet<HfTblProject> HfTblProjects { get; set; }

    public virtual DbSet<HfTblQuiz> HfTblQuizzes { get; set; }

    public virtual DbSet<HfTblQuizCourseLevel> HfTblQuizCourseLevels { get; set; }

    public virtual DbSet<HfTblRecordStatus> HfTblRecordStatuses { get; set; }

    public virtual DbSet<HfTblRelManageGroupList2026> HfTblRelManageGroupList2026s { get; set; }

    public virtual DbSet<HfTblReportQueue> HfTblReportQueues { get; set; }

    public virtual DbSet<HfTblReportTable> HfTblReportTables { get; set; }

    public virtual DbSet<HfTblRoleDashboardMapping> HfTblRoleDashboardMappings { get; set; }

    public virtual DbSet<HfTblRolePermission> HfTblRolePermissions { get; set; }

    public virtual DbSet<HfTblSchoolVisitActivity> HfTblSchoolVisitActivities { get; set; }

    public virtual DbSet<HfTblStandardMast> HfTblStandardMasts { get; set; }

    public virtual DbSet<HfTblStockRequisition> HfTblStockRequisitions { get; set; }

    public virtual DbSet<HfTblStockTransfer> HfTblStockTransfers { get; set; }

    public virtual DbSet<HfTblStudent> HfTblStudents { get; set; }

    public virtual DbSet<HfTblStudentActivityLogOld> HfTblStudentActivityLogOlds { get; set; }

    public virtual DbSet<HfTblStudentAssociate> HfTblStudentAssociates { get; set; }

    public virtual DbSet<HfTblStudentMark> HfTblStudentMarks { get; set; }

    public virtual DbSet<HfTblStudentStandard> HfTblStudentStandards { get; set; }

    public virtual DbSet<HfTblSurveyDataTranslation> HfTblSurveyDataTranslations { get; set; }

    public virtual DbSet<HfTblSyllabus> HfTblSyllabi { get; set; }

    public virtual DbSet<HfTblTask> HfTblTasks { get; set; }

    public virtual DbSet<HfTblTaskStatus> HfTblTaskStatuses { get; set; }

    public virtual DbSet<HfTblTeacher> HfTblTeachers { get; set; }

    public virtual DbSet<HfTblTeacherAssociate> HfTblTeacherAssociates { get; set; }

    public virtual DbSet<HfTblTempAffiliateFocusedList> HfTblTempAffiliateFocusedLists { get; set; }

    public virtual DbSet<HfTblTopic> HfTblTopics { get; set; }

    public virtual DbSet<HfTblUploadIndividualProject> HfTblUploadIndividualProjects { get; set; }

    public virtual DbSet<HfTblUserActivityLog> HfTblUserActivityLogs { get; set; }

    public virtual DbSet<HfTblUserAdditionApproval> HfTblUserAdditionApprovals { get; set; }

    public virtual DbSet<HfTblUserNotification> HfTblUserNotifications { get; set; }

    public virtual DbSet<HfTblUserRole> HfTblUserRoles { get; set; }

    public virtual DbSet<HfTblUserSession> HfTblUserSessions { get; set; }

    public virtual DbSet<HfTblUserSessionsAson21122025> HfTblUserSessionsAson21122025s { get; set; }

    public virtual DbSet<HfTblUserType> HfTblUserTypes { get; set; }

    public virtual DbSet<HfTblVacationCompetitionSubmission> HfTblVacationCompetitionSubmissions { get; set; }

    public virtual DbSet<HfTblVolunteer> HfTblVolunteers { get; set; }

    public virtual DbSet<HfTblVolunteerSchoolAssociation> HfTblVolunteerSchoolAssociations { get; set; }

    public virtual DbSet<HfTblWjSyllabus> HfTblWjSyllabi { get; set; }

    public virtual DbSet<HfTbltopicTranslation> HfTbltopicTranslations { get; set; }

    public virtual DbSet<HfUploadAnubhuti> HfUploadAnubhutis { get; set; }

    public virtual DbSet<HfUser> HfUsers { get; set; }

    public virtual DbSet<HfUsersAuth> HfUsersAuths { get; set; }

    public virtual DbSet<HfViewActiveStudentsdetail> HfViewActiveStudentsdetails { get; set; }

    public virtual DbSet<HfViewActiveTeacherUnderSchool> HfViewActiveTeacherUnderSchools { get; set; }

    public virtual DbSet<HfViewAffiliatewiseValueCompletedsummary> HfViewAffiliatewiseValueCompletedsummaries { get; set; }

    public virtual DbSet<HfViewCompEligibleUser> HfViewCompEligibleUsers { get; set; }

    public virtual DbSet<HfViewCompGameSummaryReport> HfViewCompGameSummaryReports { get; set; }

    public virtual DbSet<HfViewCompQuizReport> HfViewCompQuizReports { get; set; }

    public virtual DbSet<HfViewCompUserGameSummaryForNumberGame> HfViewCompUserGameSummaryForNumberGames { get; set; }

    public virtual DbSet<HfViewCompUserGameSummaryForWordGame> HfViewCompUserGameSummaryForWordGames { get; set; }

    public virtual DbSet<HfViewCompWordAndNumberGameDetailReport> HfViewCompWordAndNumberGameDetailReports { get; set; }

    public virtual DbSet<HfViewCompletedStudentsMainCourseCertificate> HfViewCompletedStudentsMainCourseCertificates { get; set; }

    public virtual DbSet<HfViewCompletedStudentsMainCourseCertificateAsperCertificate> HfViewCompletedStudentsMainCourseCertificateAsperCertificates { get; set; }

    public virtual DbSet<HfViewCreativityStudentlist> HfViewCreativityStudentlists { get; set; }

    public virtual DbSet<HfViewDiffrentExaminationRecord> HfViewDiffrentExaminationRecords { get; set; }

    public virtual DbSet<HfViewDiffrentExaminationRecordsSchema> HfViewDiffrentExaminationRecordsSchemas { get; set; }

    public virtual DbSet<HfViewDistinctExaminatioId> HfViewDistinctExaminatioIds { get; set; }

    public virtual DbSet<HfViewFocusedAffiliateList> HfViewFocusedAffiliateLists { get; set; }

    public virtual DbSet<HfViewInstituteAndStdWiseStudentSummary> HfViewInstituteAndStdWiseStudentSummaries { get; set; }

    public virtual DbSet<HfViewInstituteStandardStudentSummary> HfViewInstituteStandardStudentSummaries { get; set; }

    public virtual DbSet<HfViewInstituteStudentPblsummary> HfViewInstituteStudentPblsummaries { get; set; }

    public virtual DbSet<HfViewInstituteTeacherList> HfViewInstituteTeacherLists { get; set; }

    public virtual DbSet<HfViewInstitutewiseCourseCompletionReport> HfViewInstitutewiseCourseCompletionReports { get; set; }

    public virtual DbSet<HfViewInstitutewiseCourseCompletionReportWithdetail> HfViewInstitutewiseCourseCompletionReportWithdetails { get; set; }

    public virtual DbSet<HfViewInstitutewiseStdWiseTop3StudentPerformer> HfViewInstitutewiseStdWiseTop3StudentPerformers { get; set; }

    public virtual DbSet<HfViewInstitutewiseStdWiseTopStudentPerformer> HfViewInstitutewiseStdWiseTopStudentPerformers { get; set; }

    public virtual DbSet<HfViewInstitutewiseStudentsListCompletedSurveyAndCertificateStatus> HfViewInstitutewiseStudentsListCompletedSurveyAndCertificateStatuses { get; set; }

    public virtual DbSet<HfViewInstitutewiseTop10StudentPerformer> HfViewInstitutewiseTop10StudentPerformers { get; set; }

    public virtual DbSet<HfViewInstitutewiseTopStudentPerformer> HfViewInstitutewiseTopStudentPerformers { get; set; }

    public virtual DbSet<HfViewMinandMaxExaminatioId> HfViewMinandMaxExaminatioIds { get; set; }

    public virtual DbSet<HfViewPraticalSubmissionRecord> HfViewPraticalSubmissionRecords { get; set; }

    public virtual DbSet<HfViewSchoolPerformance> HfViewSchoolPerformances { get; set; }

    public virtual DbSet<HfViewSchoolStudentList> HfViewSchoolStudentLists { get; set; }

    public virtual DbSet<HfViewStockRequisitionReport> HfViewStockRequisitionReports { get; set; }

    public virtual DbSet<HfViewStudent> HfViewStudents { get; set; }

    public virtual DbSet<HfViewStudentActivityLog> HfViewStudentActivityLogs { get; set; }

    public virtual DbSet<HfViewStudentMainCoursewiseActivityStatus> HfViewStudentMainCoursewiseActivityStatuses { get; set; }

    public virtual DbSet<HfViewStudentWiseCompletedMainCourseList> HfViewStudentWiseCompletedMainCourseLists { get; set; }

    public virtual DbSet<HfViewStudentWiseCompletedMainCourseListAsPerCertificateWise> HfViewStudentWiseCompletedMainCourseListAsPerCertificateWises { get; set; }

    public virtual DbSet<HfViewStudentWiseandMaincourseWiseActivityCount> HfViewStudentWiseandMaincourseWiseActivityCounts { get; set; }

    public virtual DbSet<HfViewStudentsCourseCompletedList> HfViewStudentsCourseCompletedLists { get; set; }

    public virtual DbSet<HfViewStudentsDetail> HfViewStudentsDetails { get; set; }

    public virtual DbSet<HfViewStudentsMainCourseCertifiedList> HfViewStudentsMainCourseCertifiedLists { get; set; }

    public virtual DbSet<HfViewTeacherActivityLog> HfViewTeacherActivityLogs { get; set; }

    public virtual DbSet<HfViewTeacherCourseSummary> HfViewTeacherCourseSummaries { get; set; }

    public virtual DbSet<HfViewTeacherList> HfViewTeacherLists { get; set; }

    public virtual DbSet<HfViewTeacherWisePerformanceSummary> HfViewTeacherWisePerformanceSummaries { get; set; }

    public virtual DbSet<HfViewTeacherwiseCompletedCourseCount> HfViewTeacherwiseCompletedCourseCounts { get; set; }

    public virtual DbSet<HfViewUsersList> HfViewUsersLists { get; set; }

    public virtual DbSet<HfViewVacationProjectSubmission> HfViewVacationProjectSubmissions { get; set; }

    public virtual DbSet<HfViewValuewisePassTeacher> HfViewValuewisePassTeachers { get; set; }

    public virtual DbSet<HfViewVolunteer> HfViewVolunteers { get; set; }

    public virtual DbSet<HfViewWjCoin> HfViewWjCoins { get; set; }

    public virtual DbSet<HfVisitProcessRunning> HfVisitProcessRunnings { get; set; }

    public virtual DbSet<HfWordCountRecord> HfWordCountRecords { get; set; }

    public virtual DbSet<HvCompetitionConfirmation> HvCompetitionConfirmations { get; set; }

    public virtual DbSet<HvWebinarConfirmation> HvWebinarConfirmations { get; set; }

    public virtual DbSet<KenAnswer> KenAnswers { get; set; }

    public virtual DbSet<KenCage> KenCages { get; set; }

    public virtual DbSet<KenCompCage> KenCompCages { get; set; }

    public virtual DbSet<KenCompMaster> KenCompMasters { get; set; }

    public virtual DbSet<KenCompSolution> KenCompSolutions { get; set; }

    public virtual DbSet<KenCompTarget> KenCompTargets { get; set; }

    public virtual DbSet<KenCompUserAnswer> KenCompUserAnswers { get; set; }

    public virtual DbSet<KenKen> KenKens { get; set; }

    public virtual DbSet<KenMaster> KenMasters { get; set; }

    public virtual DbSet<KenSolution> KenSolutions { get; set; }

    public virtual DbSet<KenTarget> KenTargets { get; set; }

    public virtual DbSet<KnowlegdePartnersList> KnowlegdePartnersLists { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<MainCourse> MainCourses { get; set; }

    public virtual DbSet<MainCoursesLanguage> MainCoursesLanguages { get; set; }

    public virtual DbSet<MigrationHistory> MigrationHistories { get; set; }

    public virtual DbSet<Partnership> Partnerships { get; set; }

    public virtual DbSet<PuzzleCompMaster> PuzzleCompMasters { get; set; }

    public virtual DbSet<PuzzleCompMasterOld> PuzzleCompMasterOlds { get; set; }

    public virtual DbSet<PuzzleCompTransaction> PuzzleCompTransactions { get; set; }

    public virtual DbSet<PuzzleMaster> PuzzleMasters { get; set; }

    public virtual DbSet<PuzzleTransaction> PuzzleTransactions { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<QuizCourseAllocation> QuizCourseAllocations { get; set; }

    public virtual DbSet<QuizCourseLevel> QuizCourseLevels { get; set; }

    public virtual DbSet<StandardList> StandardLists { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentActivityLog> StudentActivityLogs { get; set; }

    public virtual DbSet<StudentRankView> StudentRankViews { get; set; }

    public virtual DbSet<SurveyResponse> SurveyResponses { get; set; }

    public virtual DbSet<TblCity> TblCities { get; set; }

    public virtual DbSet<TblCountry> TblCountries { get; set; }

    public virtual DbSet<TblSchoolAccount> TblSchoolAccounts { get; set; }

    public virtual DbSet<TblState> TblStates { get; set; }

    public virtual DbSet<Templist> Templists { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<TotalActiveStudent> TotalActiveStudents { get; set; }

    public virtual DbSet<TotalStudent> TotalStudents { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAccountDetail> UserAccountDetails { get; set; }

    public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }

    public virtual DbSet<UserGameSummary> UserGameSummaries { get; set; }

    public virtual DbSet<UserTypeAssociation> UserTypeAssociations { get; set; }

    public virtual DbSet<VAnswer> VAnswers { get; set; }

    public virtual DbSet<VGetOptionDatum> VGetOptionData { get; set; }

    public virtual DbSet<VHfAnubhutiImageQuestionBind> VHfAnubhutiImageQuestionBinds { get; set; }

    public virtual DbSet<VOption> VOptions { get; set; }

    public virtual DbSet<VOptionsWithAnswer> VOptionsWithAnswers { get; set; }

    public virtual DbSet<VQuestionOption> VQuestionOptions { get; set; }

    public virtual DbSet<VQuizTimeGrandTotal> VQuizTimeGrandTotals { get; set; }

    public virtual DbSet<VRelUsersAspNetUser> VRelUsersAspNetUsers { get; set; }

    public virtual DbSet<VScoreFutoshikiCal> VScoreFutoshikiCals { get; set; }

    public virtual DbSet<VScoreGameCal> VScoreGameCals { get; set; }

    public virtual DbSet<VScoreKenKenCal> VScoreKenKenCals { get; set; }

    public virtual DbSet<VScorePuzzleCal> VScorePuzzleCals { get; set; }

    public virtual DbSet<VScoreScrambleCal> VScoreScrambleCals { get; set; }

    public virtual DbSet<VScoreWordGameCal> VScoreWordGameCals { get; set; }

    public virtual DbSet<VSpAffiliateAutomail> VSpAffiliateAutomails { get; set; }

    public virtual DbSet<VWjCoin> VWjCoins { get; set; }

    public virtual DbSet<VWorldEnviorment> VWorldEnviorments { get; set; }

    public virtual DbSet<ViewCourse> ViewCourses { get; set; }

    public virtual DbSet<ViewCoursesDetailed> ViewCoursesDetaileds { get; set; }

    public virtual DbSet<ViewDiStinctDIstinctsCamble> ViewDiStinctDIstinctsCambles { get; set; }

    public virtual DbSet<ViewExamDistinctResult> ViewExamDistinctResults { get; set; }

    public virtual DbSet<ViewExamResult> ViewExamResults { get; set; }

    public virtual DbSet<ViewKenCompUserAnswer> ViewKenCompUserAnswers { get; set; }

    public virtual DbSet<ViewQuestion> ViewQuestions { get; set; }

    public virtual DbSet<ViewTopic> ViewTopics { get; set; }

    public virtual DbSet<ViewTransactionsSummary> ViewTransactionsSummaries { get; set; }

    public virtual DbSet<ViewUser> ViewUsers { get; set; }

    public virtual DbSet<ViewWjAffiliateCommissionChart> ViewWjAffiliateCommissionCharts { get; set; }

    public virtual DbSet<ViewWjAffiliateCommissionRate> ViewWjAffiliateCommissionRates { get; set; }

    public virtual DbSet<ViewWjCityStateMapping> ViewWjCityStateMappings { get; set; }

    public virtual DbSet<ViewWjCityWiseInstitute> ViewWjCityWiseInstitutes { get; set; }

    public virtual DbSet<ViewWjCourse> ViewWjCourses { get; set; }

    public virtual DbSet<ViewWjCourseTopic> ViewWjCourseTopics { get; set; }

    public virtual DbSet<ViewWjInstitute> ViewWjInstitutes { get; set; }

    public virtual DbSet<ViewWjInstitutesAdmin> ViewWjInstitutesAdmins { get; set; }

    public virtual DbSet<ViewWjMonthlyAffiliateEarning> ViewWjMonthlyAffiliateEarnings { get; set; }

    public virtual DbSet<ViewWjPracticalList> ViewWjPracticalLists { get; set; }

    public virtual DbSet<ViewWjSchoolRanking> ViewWjSchoolRankings { get; set; }

    public virtual DbSet<ViewWjStudent> ViewWjStudents { get; set; }

    public virtual DbSet<ViewWjStudentProfile> ViewWjStudentProfiles { get; set; }

    public virtual DbSet<ViewWjStudentRanking> ViewWjStudentRankings { get; set; }

    public virtual DbSet<ViewWjStudentsAdminList> ViewWjStudentsAdminLists { get; set; }

    public virtual DbSet<ViewWjStudentsDetail> ViewWjStudentsDetails { get; set; }

    public virtual DbSet<ViewWjStudentsDetailsAdmin> ViewWjStudentsDetailsAdmins { get; set; }

    public virtual DbSet<ViewWjStudentsDetailsInfo> ViewWjStudentsDetailsInfos { get; set; }

    public virtual DbSet<ViewWjStudentsDetailsInfoForActiveStudnet> ViewWjStudentsDetailsInfoForActiveStudnets { get; set; }

    public virtual DbSet<ViewWjStudentsDetailsInfoForActiveStudnetsVy> ViewWjStudentsDetailsInfoForActiveStudnetsVies { get; set; }

    public virtual DbSet<ViewWjStudentsDetailsInfoForVacationCompetitionProject> ViewWjStudentsDetailsInfoForVacationCompetitionProjects { get; set; }

    public virtual DbSet<ViewWjStudentsRank> ViewWjStudentsRanks { get; set; }

    public virtual DbSet<ViewWjStudentsSource> ViewWjStudentsSources { get; set; }

    public virtual DbSet<ViewWjSyllabus> ViewWjSyllabi { get; set; }

    public virtual DbSet<ViewWjTeacherDetail> ViewWjTeacherDetails { get; set; }

    public virtual DbSet<ViewWjTeacherPracticalList> ViewWjTeacherPracticalLists { get; set; }

    public virtual DbSet<ViewWjTeacherRanking> ViewWjTeacherRankings { get; set; }

    public virtual DbSet<ViewWjUserDetail> ViewWjUserDetails { get; set; }

    public virtual DbSet<ViewWjWeeklyPerformer> ViewWjWeeklyPerformers { get; set; }

    public virtual DbSet<WeeklyStudentCoin> WeeklyStudentCoins { get; set; }

    public virtual DbSet<WjActivityLog> WjActivityLogs { get; set; }

    public virtual DbSet<WjAffiliateVoucher> WjAffiliateVouchers { get; set; }

    public virtual DbSet<WjCertification> WjCertifications { get; set; }

    public virtual DbSet<WjCertificationAccess> WjCertificationAccesses { get; set; }

    public virtual DbSet<WjCertificationCourseList> WjCertificationCourseLists { get; set; }

    public virtual DbSet<WjCity> WjCities { get; set; }

    public virtual DbSet<WjCoin> WjCoins { get; set; }

    public virtual DbSet<WjCourseSetup> WjCourseSetups { get; set; }

    public virtual DbSet<WjEarning> WjEarnings { get; set; }

    public virtual DbSet<WjEmailVerification> WjEmailVerifications { get; set; }

    public virtual DbSet<WjEvent> WjEvents { get; set; }

    public virtual DbSet<WjEventTeacherWinner> WjEventTeacherWinners { get; set; }

    public virtual DbSet<WjEventType> WjEventTypes { get; set; }

    public virtual DbSet<WjEventUser> WjEventUsers { get; set; }

    public virtual DbSet<WjFaq> WjFaqs { get; set; }

    public virtual DbSet<WjFileUpload> WjFileUploads { get; set; }

    public virtual DbSet<WjGame24> WjGame24s { get; set; }

    public virtual DbSet<WjGame24Topic> WjGame24Topics { get; set; }

    public virtual DbSet<WjGem> WjGems { get; set; }

    public virtual DbSet<WjGift> WjGifts { get; set; }

    public virtual DbSet<WjInstitute> WjInstitutes { get; set; }

    public virtual DbSet<WjMainCertificate> WjMainCertificates { get; set; }

    public virtual DbSet<WjPaymentPlansLog> WjPaymentPlansLogs { get; set; }

    public virtual DbSet<WjPractical> WjPracticals { get; set; }

    public virtual DbSet<WjPracticalComment> WjPracticalComments { get; set; }

    public virtual DbSet<WjPracticalSubmission> WjPracticalSubmissions { get; set; }

    public virtual DbSet<WjReceiptVoucher> WjReceiptVouchers { get; set; }

    public virtual DbSet<WjReferralCode> WjReferralCodes { get; set; }

    public virtual DbSet<WjScrambleAnswer> WjScrambleAnswers { get; set; }

    public virtual DbSet<WjScrambleAnswerComp> WjScrambleAnswerComps { get; set; }

    public virtual DbSet<WjScrambleMaster> WjScrambleMasters { get; set; }

    public virtual DbSet<WjScrambleQuestion> WjScrambleQuestions { get; set; }

    public virtual DbSet<WjScrambleTopic> WjScrambleTopics { get; set; }

    public virtual DbSet<WjSessionTracking> WjSessionTrackings { get; set; }

    public virtual DbSet<WjState> WjStates { get; set; }

    public virtual DbSet<WjStudent> WjStudents { get; set; }

    public virtual DbSet<WjStudentAffiliateAssociate> WjStudentAffiliateAssociates { get; set; }

    public virtual DbSet<WjStudentAssociate> WjStudentAssociates { get; set; }

    public virtual DbSet<WjStudentCertificationAccess> WjStudentCertificationAccesses { get; set; }

    public virtual DbSet<WjStudentCredit> WjStudentCredits { get; set; }

    public virtual DbSet<WjStudentFeedback> WjStudentFeedbacks { get; set; }

    public virtual DbSet<WjStudentRanking2017> WjStudentRanking2017s { get; set; }

    public virtual DbSet<WjStudentTeacherAssociation> WjStudentTeacherAssociations { get; set; }

    public virtual DbSet<WjSyllabus> WjSyllabi { get; set; }

    public virtual DbSet<WjTaskStudent> WjTaskStudents { get; set; }

    public virtual DbSet<WjTeacher> WjTeachers { get; set; }

    public virtual DbSet<WjTeacherAssociate> WjTeacherAssociates { get; set; }

    public virtual DbSet<WjTeacherCertification> WjTeacherCertifications { get; set; }

    public virtual DbSet<WjTeacherPracticalSubmission> WjTeacherPracticalSubmissions { get; set; }

    public virtual DbSet<WjTeacherStudentPracticalSubmission> WjTeacherStudentPracticalSubmissions { get; set; }

    public virtual DbSet<WjTeachersCoin> WjTeachersCoins { get; set; }

    public virtual DbSet<WjTeam> WjTeams { get; set; }

    public virtual DbSet<WjTeamMember> WjTeamMembers { get; set; }

    public virtual DbSet<WjTechTeacherQuestion> WjTechTeacherQuestions { get; set; }

    public virtual DbSet<WjUserPracticalUnlockStatus> WjUserPracticalUnlockStatuses { get; set; }

    public virtual DbSet<WjValueCertification> WjValueCertifications { get; set; }

    public virtual DbSet<WjValueCertificationBackup> WjValueCertificationBackups { get; set; }

    public virtual DbSet<WjValueCertificationOld> WjValueCertificationOlds { get; set; }

    public virtual DbSet<WjWordGameQuestion> WjWordGameQuestions { get; set; }

    public virtual DbSet<WjWordGameTopic> WjWordGameTopics { get; set; }

    public virtual DbSet<WjZoneWise> WjZoneWises { get; set; }

    public virtual DbSet<WordGameSolution> WordGameSolutions { get; set; }

    public virtual DbSet<WordGameSolutionComp> WordGameSolutionComps { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //      #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //     => optionsBuilder.UseSqlServer("Server=HVLOCAL\\HVSQLEXPRESS;Database=Hemvirtues26-27;User Id=sa;Password=root@123;TrustServerCertificate=True;MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActualExaminationResult>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ActualExaminationResult");

            entity.Property(e => e.ExamTopicId).HasColumnName("ExamTopicID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<AnubhutiMaster>(entity =>
        {
            entity.HasKey(e => e.AnbId);

            entity.ToTable("AnubhutiMaster");

            entity.Property(e => e.AnbId).HasColumnName("AnbID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AnubhutiTransaction>(entity =>
        {
            entity.HasKey(e => e.AnbTranId);

            entity.ToTable("AnubhutiTransaction");

            entity.HasIndex(e => e.IsResolvedBy, "IX_AnubhutiTransaction_IsResolvedBy");

            entity.HasIndex(e => new { e.IsResolvedBy, e.AnbId }, "IX_AnubhutiTransaction_IsResolvedBy_AnbID");

            entity.Property(e => e.AnbTranId).HasColumnName("AnbTranID");
            entity.Property(e => e.Action1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Action2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AnbId).HasColumnName("AnbID");
            entity.Property(e => e.IsResolvedDate)
                .HasDefaultValueSql("(getdate())", "DF_AnubhutiTransaction_IsResolvedDate")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Anb).WithMany(p => p.AnubhutiTransactions)
                .HasForeignKey(d => d.AnbId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AnubhutiTransaction_AnubhutiMaster");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.AspNetRoles");

            entity.HasIndex(e => e.Name, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.AspNetUsers");

            entity.HasIndex(e => e.UserName, "IX_AspNetUsers_UserName_Login");

            entity.HasIndex(e => e.UserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.LockoutEndDateUtc).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK_dbo.AspNetUserRoles");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_RoleId");
                        j.HasIndex(new[] { "UserId" }, "IX_UserId");
                        j.IndexerProperty<string>("UserId").HasMaxLength(128);
                        j.IndexerProperty<string>("RoleId").HasMaxLength(128);
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.AspNetUserClaims");

            entity.HasIndex(e => e.UserId, "IX_UserId");

            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId }).HasName("PK_dbo.AspNetUserLogins");

            entity.HasIndex(e => e.UserId, "IX_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);
            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId");
        });

        modelBuilder.Entity<CKenKenHeader>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.C_KenKenHeader");

            entity.ToTable("C_KenKenHeader");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Difficulty).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasMaxLength(1);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Operation).HasMaxLength(10);
        });

        modelBuilder.Entity<CKenKenLine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.C_KenKenLine");

            entity.ToTable("C_KenKenLine");

            entity.Property(e => e.Column1).HasMaxLength(10);
            entity.Property(e => e.Column10).HasMaxLength(10);
            entity.Property(e => e.Column2).HasMaxLength(10);
            entity.Property(e => e.Column3).HasMaxLength(10);
            entity.Property(e => e.Column4).HasMaxLength(10);
            entity.Property(e => e.Column5).HasMaxLength(10);
            entity.Property(e => e.Column6).HasMaxLength(10);
            entity.Property(e => e.Column7).HasMaxLength(10);
            entity.Property(e => e.Column8).HasMaxLength(10);
            entity.Property(e => e.Column9).HasMaxLength(10);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LineType).HasMaxLength(10);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<CMasterKenken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__C_Master__3214EC075AE4759F");

            entity.ToTable("C_MasterKenken");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasMaxLength(1);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<CfAnubhutiImagesUpload>(entity =>
        {
            entity.HasKey(e => e.Qmid).HasFillFactor(90);

            entity.ToTable("CF_Anubhuti_Images_Upload");

            entity.HasIndex(e => e.Qid, "IX_CF_Anubhuti_Images_Upload_QID_");

            entity.Property(e => e.Qmid).HasColumnName("QMID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.Qid).HasColumnName("QID");
            entity.Property(e => e.SadImage).HasColumnName("Sad_Image");
            entity.Property(e => e.SmileImage).HasColumnName("Smile_Image");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<CommissionLog>(entity =>
        {
            entity.ToTable("Commission_log");

            entity.Property(e => e.CommissionLogId).HasColumnName("commission_log_id");
            entity.Property(e => e.CommissionAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CommissionPerAffiliate).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.CreditCommission).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.FreeStudentCommssion).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modifiedDate");
            entity.Property(e => e.RegistrationAmount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("registration_amount");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => new { e.CourseId, e.IsActive }, "IX_Courses_MainCourseID_IsActive");

            entity.Property(e => e.ApprovalRemark)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.CourseAllocatedTo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Student");
            entity.Property(e => e.CourseId)
                .ValueGeneratedOnAdd()
                .HasColumnName("CourseID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseParentId).HasColumnName("CourseParentID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ExpectedDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.FeaturedImage)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Fees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsPublished).HasColumnName("isPublished");
            entity.Property(e => e.MainCategory)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PublishDate).HasColumnType("datetime");
            entity.Property(e => e.StudentExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentResourse)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerResource)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(e => new { e.CategoryId, e.CourseId })
                .HasName("PK__CourseCa__759BED338374C0CE")
                .HasFillFactor(90);

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");

            entity.HasOne(d => d.Category).WithMany(p => p.CourseCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseCat__Categ__1E6F845E");
        });

        modelBuilder.Entity<CourseCategoryLookup>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__CourseCa__19093A2BCE760A63");

            entity.ToTable("CourseCategoryLookup");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
        });

        modelBuilder.Entity<CourseCustomerType>(entity =>
        {
            entity.HasKey(e => new { e.CustomerTypeId, e.CourseId }).HasName("PK__CourseCu__F919B65481CE40CF");

            entity.ToTable("CourseCustomerType");

            entity.Property(e => e.CustomerTypeId).HasColumnName("CustomerTypeID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
        });

        modelBuilder.Entity<CoursePrerequisite>(entity =>
        {
            entity.HasKey(e => new { e.PrerequisiteId, e.CourseId }).HasName("PK__CoursePr__493B84E1B3E2A479");

            entity.Property(e => e.PrerequisiteId).HasColumnName("PrerequisiteID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
        });

        modelBuilder.Entity<CreativityActivitesSubmission2021>(entity =>
        {
            entity.HasKey(e => e.SubmissionId);

            entity.ToTable("CreativityActivitesSubmission2021");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CreativityActivitesSubmission2025Deletedatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CreativityActivitesSubmission2025_deletedata");

            entity.Property(e => e.ActivityType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deletedDate");
            entity.Property(e => e.Deletedby).HasColumnName("deletedby");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SubmissionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("SubmissionID");
        });

        modelBuilder.Entity<CreativityActivitesSubmission2026Backup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CreativityActivitesSubmission2026_Backup");

            entity.Property(e => e.ActivityType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
        });

        modelBuilder.Entity<CustomerTypeLookup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CustomerTypeLookup");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerTypeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("CustomerTypeID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DbchangeLog>(entity =>
        {
            entity.HasKey(e => e.DbchangeLogId).HasName("PK__DBChange__A7FEF0BDE7851888");

            entity.ToTable("DBChangeLog");

            entity.Property(e => e.DbchangeLogId).HasColumnName("DBChangeLogID");
            entity.Property(e => e.DatabaseName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LoginName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.ObjectName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.ObjectType)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.SqlCommand).IsUnicode(false);
        });

        modelBuilder.Entity<EventData2022>(entity =>
        {
            entity.HasKey(e => e.Qrid);

            entity.ToTable("EventData2022");

            entity.Property(e => e.Qrid).HasColumnName("QRID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PhysicalQrcodePath).HasColumnName("PhysicalQRCodePath");
            entity.Property(e => e.QrcodeImage).HasColumnName("QRCodeImage");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.EventData2022s)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventData2022_Users");
        });

        modelBuilder.Entity<Examination>(entity =>
        {
            entity.ToTable("Examination");

            entity.HasIndex(e => new { e.UserId, e.EndTime }, "IX_Examination_UserID_EndTime");

            entity.HasIndex(e => new { e.UserId, e.ExamTopicId, e.EndTime }, "IX_Examination_UserID_ExamTopicID_EndTime");

            entity.HasIndex(e => new { e.UserId, e.ExamTopicId, e.MarksObtain }, "IX_Examination_UserID_ExamTopicID_MarksObtain");

            entity.HasIndex(e => new { e.UserId, e.MarksObtain }, "IX_Examination_UserID_MarksObtain");

            entity.HasIndex(e => new { e.UserId, e.QuizId, e.ExamTopicId }, "IX_Examination_UserID_QuizID_ExamTopicID");

            entity.HasIndex(e => new { e.UserId, e.QuizId, e.LevelId, e.MarksObtain }, "IX_Examination_UserID_QuizID_LevelID_MarksObtain");

            entity.Property(e => e.ExaminationId).HasColumnName("ExaminationID");
            entity.Property(e => e.CoinsEarned).HasDefaultValue(0);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.ExamTopicId).HasColumnName("ExamTopicID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<ExaminationOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Examinat__3214EC270C9FDDE1");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ExaminationId).HasColumnName("ExaminationID");
            entity.Property(e => e.OptionId).HasColumnName("OptionID");

            entity.HasOne(d => d.Examination).WithMany(p => p.ExaminationOptions)
                .HasForeignKey(d => d.ExaminationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Examinati__Exami__05AEC38C");
        });

        modelBuilder.Entity<ExaminationRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK_Record");

            entity.HasIndex(e => e.CreatedBy, "IX_ExaminationRecords_CreatedBy_QuestionID");

            entity.HasIndex(e => e.ExaminationId, "IX_ExaminationRecords_ExaminationID");

            entity.HasIndex(e => new { e.ExaminationId, e.QuestionId }, "IX_ExaminationRecords_ExaminationID_QuestionID");

            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExaminationId).HasColumnName("ExaminationID");
            entity.Property(e => e.IsCorrect).HasColumnName("isCorrect");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.RecordStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubmitTiming).HasColumnType("datetime");
        });

        modelBuilder.Entity<FutoshikiAnswer>(entity =>
        {
            entity.ToTable("Futoshiki_Answer");

            entity.HasIndex(e => new { e.IsCreatedBy, e.Fid }, "IX_Futoshiki_Answer_IsCreatedBy_FID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Fid).HasColumnName("FID");
            entity.Property(e => e.SubmitTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<FutoshikiCompetitionMaster>(entity =>
        {
            entity.HasKey(e => e.Fcmid);

            entity.ToTable("Futoshiki_Competition_Master");

            entity.Property(e => e.Fcmid).HasColumnName("FCMID");
            entity.Property(e => e.DisplayDate).HasColumnType("datetime");
            entity.Property(e => e.FcLevel)
                .HasMaxLength(50)
                .HasColumnName("FC_Level");
            entity.Property(e => e.GameType).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasMaxLength(50);
            entity.Property(e => e.Size).HasMaxLength(50);
        });

        modelBuilder.Entity<FutoshikiCompetitionSolution>(entity =>
        {
            entity.HasKey(e => e.Fcsid);

            entity.ToTable("Futoshiki_Competition_Solution");

            entity.Property(e => e.Fcsid).HasColumnName("FCSID");
            entity.Property(e => e.Fcmid).HasColumnName("FCMID");
            entity.Property(e => e.S1).HasMaxLength(50);
            entity.Property(e => e.S2).HasMaxLength(50);
            entity.Property(e => e.S3).HasMaxLength(50);
            entity.Property(e => e.S4).HasMaxLength(50);

            entity.HasOne(d => d.Fcm).WithMany(p => p.FutoshikiCompetitionSolutions)
                .HasForeignKey(d => d.Fcmid)
                .HasConstraintName("FK_Futoshiki_Competition_Solution_Futoshiki_Competition_Master");
        });

        modelBuilder.Entity<FutoshikiCompetitionTarget>(entity =>
        {
            entity.HasKey(e => e.Fctid);

            entity.ToTable("Futoshiki_Competition_Target");

            entity.Property(e => e.Fctid).HasColumnName("FCTID");
            entity.Property(e => e.Fcmid).HasColumnName("FCMID");
            entity.Property(e => e.Target1).HasMaxLength(50);
            entity.Property(e => e.Target2).HasMaxLength(50);
            entity.Property(e => e.Target3).HasMaxLength(50);
            entity.Property(e => e.Target4).HasMaxLength(50);
            entity.Property(e => e.Target5).HasMaxLength(50);
            entity.Property(e => e.Target6).HasMaxLength(50);
            //entity.Property(e => e.Target7).HasMaxLength(50);

            entity.HasOne(d => d.Fcm).WithMany(p => p.FutoshikiCompetitionTargets)
                .HasForeignKey(d => d.Fcmid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Futoshiki_Competition_Target_Futoshiki_Competition_Master");
        });

        modelBuilder.Entity<FutoshikiMaster>(entity =>
        {
            entity.HasKey(e => e.Fid);

            entity.ToTable("Futoshiki_Master");

            entity.Property(e => e.Fid).HasColumnName("FID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Fdifficulty)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FDifficulty");
            entity.Property(e => e.Fisactive).HasColumnName("FISActive");
            entity.Property(e => e.Fsize)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FSize");
            entity.Property(e => e.FtopicName)
                .HasMaxLength(50)
                .HasColumnName("FTopicName");
        });

        modelBuilder.Entity<FutoshikiSolution>(entity =>
        {
            entity.HasKey(e => e.Fsid).HasFillFactor(90);

            entity.ToTable("Futoshiki_Solution");

            entity.Property(e => e.Fsid).HasColumnName("FSID");
            entity.Property(e => e.Fid).HasColumnName("FID");

            entity.HasOne(d => d.FidNavigation).WithMany(p => p.FutoshikiSolutions)
                .HasForeignKey(d => d.Fid)
                .HasConstraintName("FK_Futoshiki_Solution_Futoshiki_Master");
        });

        modelBuilder.Entity<FutoshikiTransaction>(entity =>
        {
            entity.HasKey(e => e.Ftid);

            entity.ToTable("Futoshiki_Transaction");

            entity.Property(e => e.Ftid).HasColumnName("FTID");
            entity.Property(e => e.Fid).HasColumnName("FID");
            entity.Property(e => e.Ftarget1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FTarget1");
            entity.Property(e => e.Ftarget2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FTarget2");
            entity.Property(e => e.Ftarget3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FTarget3");
            entity.Property(e => e.Ftarget4)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FTarget4");
            entity.Property(e => e.Ftarget5)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FTarget5");
            entity.Property(e => e.Ftarget6)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FTarget6");
            entity.Property(e => e.Ftarget7)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FTarget7");

            entity.HasOne(d => d.FidNavigation).WithMany(p => p.FutoshikiTransactions)
                .HasForeignKey(d => d.Fid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Futoshiki_Transaction_Futoshiki_Master");
        });

        modelBuilder.Entity<FutoshikiUserAnswer>(entity =>
        {
            entity.HasKey(e => e.AnsId);

            entity.ToTable("Futoshiki_User_Answer");

            entity.Property(e => e.AnsId).HasColumnName("AnsID");
            entity.Property(e => e.Fcid).HasColumnName("FCID");
            entity.Property(e => e.Remark).HasMaxLength(50);
            entity.Property(e => e.TimeDiff).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Fc).WithMany(p => p.FutoshikiUserAnswers)
                .HasForeignKey(d => d.Fcid)
                .HasConstraintName("FK_Futoshiki_User_Answer_Futoshiki_Competition_Master");
        });

        modelBuilder.Entity<Game24Competition>(entity =>
        {
            entity.HasKey(e => e.Gcid);

            entity.ToTable("Game24_Competition");

            entity.Property(e => e.Gcid).HasColumnName("GCID");
            entity.Property(e => e.Answer).HasMaxLength(50);
            entity.Property(e => e.DisplayDate).HasColumnType("datetime");
            entity.Property(e => e.GameType).HasMaxLength(50);
            entity.Property(e => e.GcLevel)
                .HasMaxLength(50)
                .HasColumnName("GC_Level");
            entity.Property(e => e.Number1).HasMaxLength(50);
            entity.Property(e => e.Number2).HasMaxLength(50);
            entity.Property(e => e.Number3).HasMaxLength(50);
            entity.Property(e => e.Number4).HasMaxLength(50);
        });

        modelBuilder.Entity<Game24Solution>(entity =>
        {
            entity.HasKey(e => e.Sid);

            entity.ToTable("Game24Solution");

            entity.HasIndex(e => new { e.IsCreatedBy, e.CourseId, e.Game24Id }, "IX_Game24Solution_IsCreatedBy_CourseID_Game24ID");

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.Game24Id).HasColumnName("Game24ID");
            entity.Property(e => e.SolutionAnswers)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Game24UserSoln>(entity =>
        {
            entity.HasKey(e => e.AnsId);

            entity.ToTable("Game24_User_Soln");

            entity.Property(e => e.AnsId).HasColumnName("AnsID");
            entity.Property(e => e.Gcid).HasColumnName("GCID");
            entity.Property(e => e.Remark).HasMaxLength(50);
            entity.Property(e => e.UserAnswer).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Gc).WithMany(p => p.Game24UserSolns)
                .HasForeignKey(d => d.Gcid)
                .HasConstraintName("FK_Game24_User_Soln_Game24_Competition");
        });

        modelBuilder.Entity<GetSelectedStandardList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GetSelec__3214EC2796FDC37A");

            entity.ToTable("GetSelected_StandardList");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Standards)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<GuestsForEvent>(entity =>
        {
            entity.HasKey(e => e.GuestId);

            entity.ToTable("GuestsForEvent");

            entity.Property(e => e.GuestId).HasColumnName("GuestID");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.MobileNo).HasMaxLength(50);
            entity.Property(e => e.ReferredBy).HasMaxLength(50);
            entity.Property(e => e.RegisteredDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HfAffiliateVisitEvent>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_Affiliate_Visit_Events");

            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<HfAffiliateVisitProcessImageUpload>(entity =>
        {
            entity.HasKey(e => e.VlImgId);

            entity.ToTable("HF_Affiliate_Visit_Process_Image_Upload");

            entity.Property(e => e.VlImgId).HasColumnName("VL_Img_ID");
            entity.Property(e => e.EntryDatetTime).HasColumnType("datetime");
            entity.Property(e => e.ProcessId).HasColumnName("ProcessID");
            entity.Property(e => e.SchoolVisitImage).HasColumnName("School_Visit_Image");
            entity.Property(e => e.StudentVisitImage).HasColumnName("Student_Visit_Image");
            entity.Property(e => e.TeacherVisitImage).HasColumnName("Teacher_Visit_Image");
            entity.Property(e => e.UploadByAffiliatedUserId).HasColumnName("Upload_by_Affiliated_User_ID");
        });

        modelBuilder.Entity<HfCryViewGetInstituteStudentSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_CRY_View_GetInstituteStudentSummary");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
        });

        modelBuilder.Entity<HfDbchangeLog>(entity =>
        {
            entity.HasKey(e => e.DbchangeLogId).HasName("PK__HF_DBCha__A7FEF0BDDA59AD7D");

            entity.ToTable("HF_DBChangeLog");

            entity.HasIndex(e => e.EventDate, "IX_HF_DBChangeLog__EventDate");

            entity.Property(e => e.DbchangeLogId).HasColumnName("DBChangeLogID");
            entity.Property(e => e.DatabaseName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LoginName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.ObjectName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.ObjectType)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.SqlCommand).IsUnicode(false);
        });

        modelBuilder.Entity<HfGameCountRecord>(entity =>
        {
            entity.HasKey(e => e.GhfId);

            entity.ToTable("HF_Game_Count_Record");

            entity.Property(e => e.GhfId).HasColumnName("GHF_ID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.GameName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GhfCount).HasColumnName("GHF_Count");
            entity.Property(e => e.GhfOver).HasColumnName("GHF_Over");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<HfMasCity>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_MAS_Cities");

            entity.Property(e => e.CityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("City_ID");
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("City_Name");
            entity.Property(e => e.EntDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Ent_Date");
            entity.Property(e => e.StateName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("State_Name");
        });

        modelBuilder.Entity<HfMasMainCourse>(entity =>
        {
            entity.HasKey(e => e.MainCourseId);

            entity.ToTable("HF_MAS_MainCourses");

            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.CertificateTemplateUrl).HasColumnName("Certificate_Template_Url");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EntBy).HasColumnName("ENT_By");
            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("ENT_Date");
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.UpdBy).HasColumnName("UPD_By");
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("UPD_Date");
        });

        modelBuilder.Entity<HfMonthlyBill>(entity =>
        {
            entity.HasKey(e => e.SubmissionId);

            entity.ToTable("HF_MonthlyBill");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.AttendenceDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HfProject>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__HF_Proje__761ABED09D768EE8");

            entity.ToTable("HF_Projects");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.ProjectName).HasMaxLength(100);
            entity.Property(e => e.ProjectTypeId).HasColumnName("Project_Type_Id");
        });

        modelBuilder.Entity<HfRelManageGroupList>(entity =>
        {
            entity.HasKey(e => e.Mgslid);

            entity.ToTable("HF_Rel_Manage_Group_List");

            entity.Property(e => e.Mgslid).HasColumnName("MGSLID");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfRelManageGroupListDeletedatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_Rel_Manage_Group_List_deletedata");

            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deletedDate");
            entity.Property(e => e.Deletedby).HasColumnName("deletedby");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.Mgslid)
                .ValueGeneratedOnAdd()
                .HasColumnName("MGSLID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfSchoolVisit>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_School_Visit");

            entity.Property(e => e.DelBy).HasColumnName("DEL_BY");
            entity.Property(e => e.DelDate)
                .HasColumnType("datetime")
                .HasColumnName("Del_DATE");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("END_DATE");
            entity.Property(e => e.EntBy).HasColumnName("Ent_BY");
            entity.Property(e => e.InstituteId).HasColumnName("Institute_ID");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.ScheduleId).HasColumnName("Schedule_ID");
            entity.Property(e => e.UpdBy).HasColumnName("Upd_BY");
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("UPD_DATE");
            entity.Property(e => e.VisitId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Visit_ID");
            entity.Property(e => e.VisitedDate).HasColumnName("Visited_Date");
        });

        modelBuilder.Entity<HfSchoolVisitSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_School_Visit_Schedule");

            entity.Property(e => e.CancelledReason)
                .IsUnicode(false)
                .HasColumnName("Cancelled_Reason");
            entity.Property(e => e.DelBy).HasColumnName("DEL_BY");
            entity.Property(e => e.DelDate)
                .HasColumnType("datetime")
                .HasColumnName("Del_DATE");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("END_DATE");
            entity.Property(e => e.EntBy).HasColumnName("Ent_BY");
            entity.Property(e => e.InstituteId).HasColumnName("Institute_ID");
            entity.Property(e => e.IsvisitCancelled).HasColumnName("ISVisitCancelled");
            entity.Property(e => e.Isvisited).HasColumnName("ISVisited");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.ScheduleDate)
                .HasColumnType("datetime")
                .HasColumnName("Schedule_Date");
            entity.Property(e => e.ScheduleId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Schedule_ID");
            entity.Property(e => e.UpdBy).HasColumnName("Upd_BY");
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("UPD_DATE");
            entity.Property(e => e.VisitedBy)
                .HasColumnType("datetime")
                .HasColumnName("Visited_BY");
            entity.Property(e => e.VisitedDate)
                .HasColumnType("datetime")
                .HasColumnName("Visited_Date");
        });

        modelBuilder.Entity<HfSection>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__HF_Secti__80EF08924BE4D333");

            entity.ToTable("HF_Sections");

            entity.Property(e => e.SectionId).HasColumnName("SectionID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.SectionTitle).HasMaxLength(100);
        });

        modelBuilder.Entity<HfSectionSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__HF_Secti__449EE10518F81DB3");

            entity.ToTable("HF_SectionSubmissions");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("Mobile_NO");
            entity.Property(e => e.SectionId).HasColumnName("SectionID");
            entity.Property(e => e.SubmittedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<HfSpInsituteWiseRegistrationSummaryReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_SP_InsituteWiseRegistrationSummaryReport");

            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.AffiliateName)
                .HasMaxLength(101)
                .HasColumnName("Affiliate_Name");
            entity.Property(e => e.HigherStandardProvider)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<HfSpInstituteWiseAffiliateName>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_SP_InstituteWiseAffiliateName");

            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.AffiliateName).HasMaxLength(100);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<HfSpStdWiseInstituteStudentsRankAndTotalPoint>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_SP_StdWiseInstituteStudentsRankAndTotalPoints");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfSpVolunteerSchoolAssociation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_SP_VolunteerSchoolAssociation");

            entity.Property(e => e.EntBy).HasColumnName("Ent_by");
            entity.Property(e => e.EntDatetime)
                .HasColumnType("datetime")
                .HasColumnName("Ent_Datetime");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_status");
            entity.Property(e => e.SchoolId).HasColumnName("School_ID");
            entity.Property(e => e.VolunteerId).HasColumnName("Volunteer_ID");
        });

        modelBuilder.Entity<HfSurveyDatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_SurveyData");

            entity.HasIndex(e => e.CourseId, "IX_HF_SurveyData_CourseID_");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.LanguageId).HasColumnName("LanguageID");
            entity.Property(e => e.Option1).HasMaxLength(50);
            entity.Property(e => e.Option2).HasMaxLength(50);
            entity.Property(e => e.Option3).HasMaxLength(50);
            entity.Property(e => e.Option4).HasMaxLength(50);
            entity.Property(e => e.Option5).HasMaxLength(50);
            entity.Property(e => e.SubTopicId).HasColumnName("SubTopicID");
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
        });

        modelBuilder.Entity<HfTblAcademicTerm>(entity =>
        {
            entity.HasKey(e => e.TermId).HasName("PK__HF_TBL_A__410A2E45E5DA3B15");

            entity.ToTable("HF_TBL_AcademicTerms");

            entity.Property(e => e.TermId).HasColumnName("TermID");
            entity.Property(e => e.AcademicYear)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TermName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Institute).WithMany(p => p.HfTblAcademicTerms)
                .HasForeignKey(d => d.InstituteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Terms_Institute");
        });

        modelBuilder.Entity<HfTblAcademicYear>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_AcademicYear");

            entity.Property(e => e.AcademicYearId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AcademicYearID");
            entity.Property(e => e.YearLabel)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfTblAffiliateExpence>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_AffiliateExpences");

            entity.Property(e => e.ApprovedAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ApvlDate).HasColumnName("APVL_DATE");
            entity.Property(e => e.ApvlDate2).HasColumnName("APVL_DATE2");
            entity.Property(e => e.ApvlDate3).HasColumnName("APVL_DATE3");
            entity.Property(e => e.ApvlId1).HasColumnName("Apvl_ID1");
            entity.Property(e => e.ApvlId2).HasColumnName("Apvl_ID2");
            entity.Property(e => e.ApvlId3).HasColumnName("Apvl_ID3");
            entity.Property(e => e.ApvlRemarks1).HasColumnName("Apvl_Remarks1");
            entity.Property(e => e.ApvlRemarks2).HasColumnName("Apvl_Remarks2");
            entity.Property(e => e.ApvlRemarks3).HasColumnName("Apvl_Remarks3");
            entity.Property(e => e.ApvlStatus1).HasColumnName("Apvl_Status1");
            entity.Property(e => e.ApvlStatus2).HasColumnName("Apvl_Status2");
            entity.Property(e => e.ApvlStatus3).HasColumnName("Apvl_Status3");
            entity.Property(e => e.AttachmentPath).IsUnicode(false);
            entity.Property(e => e.DelDate)
                .HasColumnType("datetime")
                .HasColumnName("DEL_DATE");
            entity.Property(e => e.DelUserId).HasColumnName("DEL_USER_ID");
            entity.Property(e => e.EntBy).HasColumnName("ENT_BY");
            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("ENT_Date");
            entity.Property(e => e.ExpencesAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Expences_Amount");
            entity.Property(e => e.ExpencesDate)
                .HasColumnType("datetime")
                .HasColumnName("Expences_Date");
            entity.Property(e => e.ExpencesId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Expences_ID");
            entity.Property(e => e.FromLocation)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("Institute_ID");
            entity.Property(e => e.PaymentAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Payment_Amount");
            entity.Property(e => e.PaymentBy).HasColumnName("Payment_By");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("Payment_Date");
            entity.Property(e => e.PaymentStatus).HasColumnName("Payment_Status");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.ToLocation)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.TravelMode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdBy).HasColumnName("Upd_BY");
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("Upd_Date");
            entity.Property(e => e.VisitId).HasColumnName("Visit_ID");
        });

        modelBuilder.Entity<HfTblAffiliateKraFile>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_Affiliate_KRA_Files");

            entity.HasIndex(e => new { e.AffiliateId, e.IsLatest }, "IX_affiliate_kra_files_affiliate");

            entity.Property(e => e.AffiliateId).HasColumnName("Affiliate_id");
            entity.Property(e => e.FileId)
                .ValueGeneratedOnAdd()
                .HasColumnName("file_id");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .HasColumnName("file_path");
            entity.Property(e => e.IsLatest).HasColumnName("is_latest");
            entity.Property(e => e.UploadLog).HasColumnName("upload_log");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.VersionNumber).HasColumnName("version_number");
        });

        modelBuilder.Entity<HfTblAffliateFocusedList>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_AffliateFocusedList");

            entity.HasIndex(e => e.InstituteId, "IX_FocusedList_InstituteID");

            entity.Property(e => e.AffliateId).HasColumnName("Affliate_ID");
            entity.Property(e => e.EntDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Ent_Date");
            entity.Property(e => e.EntId).HasColumnName("Ent_ID");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.InstituteId).HasColumnName("Institute_ID");
        });

        modelBuilder.Entity<HfTblCompEligibleUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_CompEligibleUser");

            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.UserRoles)
                .IsUnicode(false)
                .HasColumnName("User_Roles");
        });

        modelBuilder.Entity<HfTblCompTime>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_CompTime");
        });

        modelBuilder.Entity<HfTblCourse>(entity =>
        {
            entity.HasKey(e => e.CourseId)
                .HasName("PK_Course")
                .HasFillFactor(90);

            entity.ToTable("HF_TBL_Courses");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.ApprovalRemark)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.CourseAllocatedTo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Student");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseParentId).HasColumnName("CourseParentID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ExpectedDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.FeaturedImage)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Fees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Courses_IsActive");
            entity.Property(e => e.IsPublished).HasColumnName("isPublished");
            entity.Property(e => e.MainCategory)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PublishDate).HasColumnType("datetime");
            entity.Property(e => e.StudentExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentResourse)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerResource)
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.HasOne(d => d.CourseParent).WithMany(p => p.InverseCourseParent)
                .HasForeignKey(d => d.CourseParentId)
                .HasConstraintName("FK__HF_TBL_Co__Cours__58D1301D");
        });

        modelBuilder.Entity<HfTblCourseCategoryLookup>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__HF_TBL_C__19093A2BAADC1F5A");

            entity.ToTable("HF_TBL_CourseCategoryLookup");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
        });

        modelBuilder.Entity<HfTblCreativityActivitesSubmission2026>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_CreativityActivitesSubmission2026");

            entity.Property(e => e.ActivityType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
        });

        modelBuilder.Entity<HfTblCreativityActivitySubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__HF_TBL_C__449EE105FBA94483");

            entity.ToTable("HF_TBL_CreativityActivitySubmission");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfTblEmailLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__HF_TBL_E__5E5499A82AB59241");

            entity.ToTable("HF_TBL_Email_Log");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.ApplicationName)
                .HasMaxLength(100)
                .HasColumnName("Application_Name");
            entity.Property(e => e.CcName)
                .HasMaxLength(255)
                .HasColumnName("CC_NAME");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RecipientEmail).HasMaxLength(255);
            entity.Property(e => e.RetryCount).HasDefaultValue(0);
            entity.Property(e => e.SentBy).HasMaxLength(100);
            entity.Property(e => e.SentDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Subject).HasMaxLength(500);
            entity.Property(e => e.TemplateId)
                .HasMaxLength(100)
                .HasColumnName("TemplateID");
        });

        modelBuilder.Entity<HfTblEmailReminderLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__HF_tbl_E__5E5499A819CFEE89");

            entity.ToTable("HF_tbl_EmailReminderLog");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.CourseName).HasMaxLength(255);
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfTblExamConfiguration>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK__HF_TBL_E__C3BC333C8DBCE8B7");

            entity.ToTable("HF_TBL_ExamConfigurations");

            entity.Property(e => e.ConfigId).HasColumnName("ConfigID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MaxMarks)
                .HasDefaultValue(100.00m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PassingMarks)
                .HasDefaultValue(35.00m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Standard)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TermId).HasColumnName("TermID");

            entity.HasOne(d => d.Institute).WithMany(p => p.HfTblExamConfigurations)
                .HasForeignKey(d => d.InstituteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Config_Institute");

            entity.HasOne(d => d.Subject).WithMany(p => p.HfTblExamConfigurations)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Config_Subject");

            entity.HasOne(d => d.Term).WithMany(p => p.HfTblExamConfigurations)
                .HasForeignKey(d => d.TermId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Config_Term");
        });

        modelBuilder.Entity<HfTblExceptionLoggingToDataBase>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_Tbl_ExceptionLoggingToDataBase");

            entity.Property(e => e.ExceptionMsg)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ExceptionType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ExceptionUrl)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ExceptionURL");
            entity.Property(e => e.FeatureId).HasColumnName("Feature_ID");
            entity.Property(e => e.HttpXForwardedFor)
                .IsUnicode(false)
                .HasColumnName("HTTP_X_FORWARDED_FOR");
            entity.Property(e => e.IpAddress)
                .IsUnicode(false)
                .HasColumnName("IP_Address");
            entity.Property(e => e.Logdate).HasColumnType("datetime");
            entity.Property(e => e.Logid).ValueGeneratedOnAdd();
            entity.Property(e => e.Methods).IsUnicode(false);
            entity.Property(e => e.OthererrorMessage).IsUnicode(false);
            entity.Property(e => e.ProjectName).IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("User_ID");
        });

        modelBuilder.Entity<HfTblFailedUserRecord>(entity =>
        {
            entity.HasKey(e => e.FailedRecordId).HasName("PK__HF_TBL_F__4130E3EA9C0B970A");

            entity.ToTable("HF_TBL_FailedUserRecords");

            entity.Property(e => e.FailedRecordId).HasColumnName("FailedRecordID");
            entity.Property(e => e.ApprovalId).HasColumnName("ApprovalID");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<HfTblFeatureFlag>(entity =>
        {
            entity.HasKey(e => e.FeatureKey).HasName("PK__HF_TBL_F__FCEFA3313569A5A0");

            entity.ToTable("HF_TBL_FeatureFlags");

            entity.Property(e => e.FeatureKey)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.FeatureId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Feature_ID");
            entity.Property(e => e.FeatureValue).HasMaxLength(200);
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
        });

        modelBuilder.Entity<HfTblGroupProjectFile>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_GroupProjectFile");

            entity.Property(e => e.FileId)
                .ValueGeneratedOnAdd()
                .HasColumnName("FileID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
        });

        modelBuilder.Entity<HfTblHeProjectSubmission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_HeProjectSubmission");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedBy).HasColumnName("CreatedBY");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StudentsId).HasColumnName("StudentsID");
            entity.Property(e => e.SubmissionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("SubmissionID");
        });

        modelBuilder.Entity<HfTblHegroupInvitation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_HEGroupInvitation");

            entity.Property(e => e.AcceptDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Accept_Date");
            entity.Property(e => e.AcceptedStudentId).HasColumnName("Accepted_Student_ID");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.InvitedBy).HasColumnName("Invited_BY");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
        });

        modelBuilder.Entity<HfTblIndividualProjectFile>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_IndividualProjectFile");

            entity.Property(e => e.FileId)
                .ValueGeneratedOnAdd()
                .HasColumnName("FileID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
        });

        modelBuilder.Entity<HfTblInstitute>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_Institute");

            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("ENT_DATE");
            entity.Property(e => e.EntId).HasColumnName("Ent_ID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteType)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.RecStatus).HasColumnName("REC_Status");
            entity.Property(e => e.ReferralId).HasColumnName("ReferralID");
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("UPD_DATE");
            entity.Property(e => e.UpdId).HasColumnName("UPD_ID");
        });

        modelBuilder.Entity<HfTblInstituteSubject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__HF_TBL_I__AC1BA3881D7702EA");

            entity.ToTable("HF_TBL_InstituteSubjects");

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SubjectName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Institute).WithMany(p => p.HfTblInstituteSubjects)
                .HasForeignKey(d => d.InstituteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subjects_Institute");
        });

        modelBuilder.Entity<HfTblLanguage>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__HF_TBL_L__12696A425FDF1909");

            entity.ToTable("HF_TBL_Language");

            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LanguageName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("languageName");
        });

        modelBuilder.Entity<HfTblLeadsInstitute>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_LeadsInstitute");

            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.DocumentPath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("Ent_Date");
            entity.Property(e => e.EntId).HasColumnName("Ent_ID");
            entity.Property(e => e.InstituteName).HasColumnName("Institute_Name");
            entity.Property(e => e.LeadsId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Leads_ID");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("UPD_Date");
            entity.Property(e => e.UpdId).HasColumnName("UPD_ID");
        });

        modelBuilder.Entity<HfTblLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK_Levels_1");

            entity.ToTable("HF_TBL_Levels");

            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.HfTblLevels)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("HF_TBL_Levels");
        });

        modelBuilder.Entity<HfTblMainCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBl_MainCourses");

            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EntBy).HasColumnName("ENT_BY");
            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("ENT_DATE");
            entity.Property(e => e.MainCourseId)
                .ValueGeneratedOnAdd()
                .HasColumnName("MainCourseID");
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.UpdBy).HasColumnName("UPD_BY");
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("UPD_DATE");
        });

        modelBuilder.Entity<HfTblMainCoursesLanguage>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_MainCoursesLanguage");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.LanguageDescription).HasMaxLength(1000);
            entity.Property(e => e.LanguageImagePath).HasMaxLength(500);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HfTblManageGroupListVacationCompetition>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_Manage_Group_List_VacationCompetition");

            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.Mgslid)
                .ValueGeneratedOnAdd()
                .HasColumnName("MGSLID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfTblMasStockType>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_MAS_StockType");

            entity.Property(e => e.StockType)
                .IsUnicode(false)
                .HasColumnName("Stock_Type");
            entity.Property(e => e.StockTypeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Stock_Type_ID");
        });

        modelBuilder.Entity<HfTblMenu>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_Tbl_Menu");

            entity.Property(e => e.CoreMenuUrl).IsUnicode(false);
            entity.Property(e => e.Icon)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("icon");
            entity.Property(e => e.Isactive).HasColumnName("ISActive");
            entity.Property(e => e.IscoreMenu).HasColumnName("ISCoreMenu");
            entity.Property(e => e.MenuCode)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("Menu_Code");
            entity.Property(e => e.MenuId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Menu_Id");
            entity.Property(e => e.MenuLevel).HasColumnName("Menu_Level");
            entity.Property(e => e.MenuName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Menu_Name");
            entity.Property(e => e.ParentMenuCode)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("Parent_Menu_code");
            entity.Property(e => e.ParentMenuCodeCore).HasColumnName("Parent_Menu_code_Core");
            entity.Property(e => e.Target)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("target");
            entity.Property(e => e.Url)
                .IsUnicode(false)
                .HasColumnName("url");
        });

        modelBuilder.Entity<HfTblMenuActionMap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HF_Tbl_M__3214EC2778813818");

            entity.ToTable("HF_Tbl_MenuActionMap");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActionName)
                .HasMaxLength(100)
                .HasColumnName("Action_Name");
            entity.Property(e => e.ControllerName)
                .HasMaxLength(100)
                .HasColumnName("Controller_Name");
            entity.Property(e => e.MenuId).HasColumnName("MENU_ID");
        });

        modelBuilder.Entity<HfTblMenuTable>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_MenuTable");

            entity.Property(e => e.ApvlDate)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("APVL_DATE");
            entity.Property(e => e.ApvlDate2)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("APVL_DATE2");
            entity.Property(e => e.ApvlDate3)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("APVL_DATE3");
            entity.Property(e => e.ApvlId1Column)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Apvl_ID1_Column");
            entity.Property(e => e.ApvlId2Column)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Apvl_ID2_Column");
            entity.Property(e => e.ApvlId3Column)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Apvl_ID3_Column");
            entity.Property(e => e.ApvlRemarks1Column)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Apvl_Remarks1_Column");
            entity.Property(e => e.ApvlRemarks2Column)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Apvl_Remarks2_Column");
            entity.Property(e => e.ApvlRemarks3Column)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Apvl_Remarks3_Column");
            entity.Property(e => e.ApvlStatus1Column)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Apvl_Status1_Column");
            entity.Property(e => e.ApvlStatus2Column)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Apvl_Status2_Column");
            entity.Property(e => e.ApvlStatus3Column)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Apvl_Status3_Column");
            entity.Property(e => e.DelDateColumn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DEL_DATE_Column");
            entity.Property(e => e.DelUserIdColumn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DEL_USER_ID__Column");
            entity.Property(e => e.KeyIdColumn)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("KeyID_Column");
            entity.Property(e => e.MenuId).HasColumnName("Menu_ID");
            entity.Property(e => e.StatusColumn)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Status_column");
            entity.Property(e => e.TableDesc)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Table_Desc");
            entity.Property(e => e.TableId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Table_ID");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Table_Name");
        });

        modelBuilder.Entity<HfTblMonthlyBill>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_MonthlyBill");

            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.AttendenceDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SubmissionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("SubmissionID");
        });

        modelBuilder.Entity<HfTblNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HF_TBL_N__3214EC0786FB3375");

            entity.ToTable("HF_TBL_Notifications");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TargetType).HasMaxLength(50);
            entity.Property(e => e.TargetValue).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(150);
        });

        modelBuilder.Entity<HfTblParent>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_PARENTS");

            entity.Property(e => e.CreatedBy).HasColumnName("Created_By");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Created_Date");
            entity.Property(e => e.Occupation)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ParentId).HasColumnName("Parent_ID");
        });

        modelBuilder.Entity<HfTblParentChildAssociation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_Tbl_ParentChildAssociation");

            entity.Property(e => e.ChildId).HasColumnName("ChildID");
            entity.Property(e => e.MappedDate).HasColumnType("datetime");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
        });

        modelBuilder.Entity<HfTblPractical>(entity =>
        {
            entity.HasKey(e => e.PracticalId)
                .HasName("PK_WJ_Practicals")
                .HasFillFactor(90);

            entity.ToTable("HF_TBL_Practicals");

            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsTask).HasColumnName("isTask");
            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PracticalStandard).HasMaxLength(10);
            entity.Property(e => e.PracticalSubtopicId).HasColumnName("PracticalSubtopicID");
            entity.Property(e => e.PracticalTopicId).HasColumnName("PracticalTopicID");
            entity.Property(e => e.Resources)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ScrambleTopicId).HasColumnName("ScrambleTopicID");
            entity.Property(e => e.StudentGroup)
                .HasMaxLength(100)
                .HasColumnName("Student_Group");
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Type).HasMaxLength(500);
            entity.Property(e => e.VideoLink)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.HfTblPracticals)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WJ_Practicals_Courses");
        });

        modelBuilder.Entity<HfTblPracticalTranslation>(entity =>
        {
            entity.HasKey(e => e.TranslationId).HasName("PK__HF_TBL_P__663DA0ACBBC3F4CE");

            entity.ToTable("HF_TBL_PracticalTranslations");

            entity.Property(e => e.TranslationId).HasColumnName("TranslationID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LanguageId).HasColumnName("LanguageID");
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.VisibleTitle).HasMaxLength(500);
        });

        modelBuilder.Entity<HfTblProject>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__HF_TBL_P__761ABED086E01931");

            entity.ToTable("HF_TBL_Projects");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.HfTblProjects)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK__HF_TBL_Pr__Creat__6596F9B5");
        });

        modelBuilder.Entity<HfTblQuiz>(entity =>
        {
            entity.HasKey(e => e.QuizId)
                .HasName("PK_Quiz")
                .HasFillFactor(90);

            entity.ToTable("HF_TBL_Quiz");

            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.CourseAllocatedTo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.Mcqlevel)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCQLevel");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WhizJuniorSyllabus).HasDefaultValue(false, "DF_Quiz_WhizJuniorSyllabus");
        });

        modelBuilder.Entity<HfTblQuizCourseLevel>(entity =>
        {
            entity.HasKey(e => new { e.QuizId, e.LevelId, e.CourseId })
                .HasName("PK_QuizLevels")
                .HasFillFactor(90);

            entity.ToTable("HF_TBL_QuizCourseLevels");

            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");

            entity.HasOne(d => d.Course).WithMany(p => p.HfTblQuizCourseLevels)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuizCourseLevels_Courses");

            entity.HasOne(d => d.Level).WithMany(p => p.HfTblQuizCourseLevels)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuizLevels_KDS");

            entity.HasOne(d => d.Quiz).WithMany(p => p.HfTblQuizCourseLevels)
                .HasForeignKey(d => d.QuizId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuizLevels_Quiz");
        });

        modelBuilder.Entity<HfTblRecordStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_RecordStatus");

            entity.Property(e => e.RecStatus)
                .ValueGeneratedOnAdd()
                .HasColumnName("Rec_Status");
            entity.Property(e => e.StatusDescription).HasMaxLength(200);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<HfTblRelManageGroupList2026>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_Rel_Manage_Group_List_2026");

            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.Mgslid).HasColumnName("MGSLID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfTblReportQueue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HF_TBL_R__3214EC0722CBC35F");

            entity.ToTable("HF_TBL_ReportQueue");

            entity.Property(e => e.FileUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsProcessed).HasDefaultValue(false);
            entity.Property(e => e.ProcessedDate).HasColumnType("datetime");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfTblReportTable>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_ReportTable");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<HfTblRoleDashboardMapping>(entity =>
        {
            entity.HasKey(e => e.RoleDashboardId).HasName("PK__HF_TBL_R__A308F0676F39BF79");

            entity.ToTable("HF_TBL_RoleDashboardMapping");

            entity.Property(e => e.RoleDashboardId).HasColumnName("RoleDashboard_ID");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DashboardName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Dashboard_Name");
            entity.Property(e => e.DashboardUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Dashboard_URL");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RecStatus)
                .HasDefaultValue(5)
                .HasColumnName("Rec_Status");
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
        });

        modelBuilder.Entity<HfTblRolePermission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_Tbl_RolePermission");

            entity.Property(e => e.AddPer).HasColumnName("ADD_PER");
            entity.Property(e => e.DelPer).HasColumnName("DEL_PER");
            entity.Property(e => e.EupdPer).HasColumnName("EUPD_PER");
            entity.Property(e => e.GroupID).HasColumnName("Group_iD");
            entity.Property(e => e.MenuId).HasColumnName("MENU_ID");
            entity.Property(e => e.UpdPer).HasColumnName("UPD_PER");
            entity.Property(e => e.ViewPer).HasColumnName("VIEW_PER");
        });

        modelBuilder.Entity<HfTblSchoolVisitActivity>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_SchoolVisitActivities");

            entity.Property(e => e.ActivityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Activity_ID");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Activity_Type");
            entity.Property(e => e.EntDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ENT_Date");
            entity.Property(e => e.EntId).HasColumnName("ENT_ID");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Remarks).HasColumnType("text");
            entity.Property(e => e.VisitId).HasColumnName("Visit_ID");
        });

        modelBuilder.Entity<HfTblStandardMast>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_Standard_Mast");

            entity.Property(e => e.Standard).IsUnicode(false);
            entity.Property(e => e.StandardId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Standard_ID");
        });

        modelBuilder.Entity<HfTblStockRequisition>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_StockRequisition");

            entity.Property(e => e.DelDate)
                .HasColumnType("datetime")
                .HasColumnName("DEL_DATE");
            entity.Property(e => e.DelUserId).HasColumnName("DEL_USER_ID");
            entity.Property(e => e.EntBy).HasColumnName("ENT_BY");
            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("ENT_Date");
            entity.Property(e => e.ItemType)
                .HasMaxLength(100)
                .HasColumnName("Item_Type");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.Remarks).IsUnicode(false);
            entity.Property(e => e.RequestedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RequisitionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("RequisitionID");
            entity.Property(e => e.ShippingAddress).HasColumnName("Shipping_Address");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdBy).HasColumnName("Upd_BY");
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("Upd_Date");
        });

        modelBuilder.Entity<HfTblStockTransfer>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_StockTransfer");

            entity.Property(e => e.DelDate)
                .HasColumnType("datetime")
                .HasColumnName("DEL_DATE");
            entity.Property(e => e.DelUserId).HasColumnName("DEL_USER_ID");
            entity.Property(e => e.EntBy).HasColumnName("ENT_BY");
            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("ENT_Date");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.RequisitionId).HasColumnName("RequisitionID");
            entity.Property(e => e.TransferDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TransferId)
                .ValueGeneratedOnAdd()
                .HasColumnName("TransferID");
            entity.Property(e => e.TransferMode).HasMaxLength(100);
            entity.Property(e => e.UpdBy).HasColumnName("Upd_BY");
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("Upd_Date");
        });

        modelBuilder.Entity<HfTblStudent>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_STUDENTS");

            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GardianName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("Institute_ID");
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("STUDENT_ID");
        });

        modelBuilder.Entity<HfTblStudentActivityLogOld>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_StudentActivityLog_OLD");

            entity.HasIndex(e => e.StudentId, "IX_HF_TBL_StudentActivityLog_OLD_StudentID_");

            entity.Property(e => e.AcedamicYear).IsUnicode(false);
            entity.Property(e => e.ActivityDate)
                .HasColumnType("datetime")
                .HasColumnName("Activity_Date");
            entity.Property(e => e.ActivityType)
                .IsUnicode(false)
                .HasColumnName("Activity_Type");
            entity.Property(e => e.CourseName).IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).IsUnicode(false);
            entity.Property(e => e.MainCourseName).IsUnicode(false);
            entity.Property(e => e.Standard).IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .IsUnicode(false)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfTblStudentAssociate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_StudentAssociates");

            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("ENT_DATE");
            entity.Property(e => e.EntId).HasColumnName("Ent_ID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfTblStudentMark>(entity =>
        {
            entity.HasKey(e => e.MarkId).HasName("PK__HF_TBL_S__4E30D346870C36ED");

            entity.ToTable("HF_TBL_StudentMarks");

            entity.HasIndex(e => new { e.InstituteId, e.StudentId }, "IX_Tenant_Student_Marks_Lookup");

            entity.HasIndex(e => new { e.StudentId, e.ConfigId }, "UQ_Student_Config_Tenant").IsUnique();

            entity.Property(e => e.MarkId).HasColumnName("MarkID");
            entity.Property(e => e.ConfigId).HasColumnName("ConfigID");
            entity.Property(e => e.CreatedTimestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.IsAbsent).HasDefaultValue(false);
            entity.Property(e => e.MarksObtained).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ModifiedTimestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Remarks)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Config).WithMany(p => p.HfTblStudentMarks)
                .HasForeignKey(d => d.ConfigId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Marks_Config");

            entity.HasOne(d => d.Institute).WithMany(p => p.HfTblStudentMarks)
                .HasForeignKey(d => d.InstituteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Marks_Institute");

            entity.HasOne(d => d.Student).WithMany(p => p.HfTblStudentMarks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Marks_Student");
        });

        modelBuilder.Entity<HfTblStudentStandard>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_Student_Standard");

            entity.Property(e => e.AcademicYearId).HasColumnName("AcademicYearID");
            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("ENT_DATE");
            entity.Property(e => e.EntId).HasColumnName("Ent_ID");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.StdId).HasColumnName("Std_ID");
            entity.Property(e => e.StudentId).HasColumnName("Student_ID");
        });

        modelBuilder.Entity<HfTblSurveyDataTranslation>(entity =>
        {
            entity.HasKey(e => e.TranslationId).HasName("PK__HF_TBL_S__663DA0AC3DD541A4");

            entity.ToTable("HF_TBL_SurveyDataTranslation");

            entity.Property(e => e.TranslationId).HasColumnName("TranslationID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.LanguageId).HasColumnName("LanguageID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.SubTopicId).HasColumnName("SubTopicID");
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
        });

        modelBuilder.Entity<HfTblSyllabus>(entity =>
        {
            entity.HasKey(e => e.SyllabusId).HasName("PK__WJ_Sylla__9B801746010A0A00");

            entity.ToTable("HF_TBL_Syllabus");

            entity.Property(e => e.SyllabusId).HasColumnName("SyllabusID");
            entity.Property(e => e.CompetitionYear)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Standard_");
        });

        modelBuilder.Entity<HfTblTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__HF_TBL_T__7C6949D1578091C0");

            entity.ToTable("HF_TBL_Tasks");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.AssignedToUserId).HasColumnName("AssignedToUserID");
            entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.HfTblTaskAssignedToUsers)
                .HasForeignKey(d => d.AssignedToUserId)
                .HasConstraintName("FK__HF_TBL_Ta__Assig__68736660");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.HfTblTaskCreatedByUsers)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK__HF_TBL_Ta__Creat__69678A99");

            entity.HasOne(d => d.Project).WithMany(p => p.HfTblTasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HF_TBL_Ta__Proje__6A5BAED2");
        });

        modelBuilder.Entity<HfTblTaskStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__HF_TBL_T__C8EE2043468207DA");

            entity.ToTable("HF_TBL_TaskStatus");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<HfTblTeacher>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_Tbl_Teacher");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Standard)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
        });

        modelBuilder.Entity<HfTblTeacherAssociate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_TeacherAssociates");

            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("ENT_DATE");
            entity.Property(e => e.EntId).HasColumnName("Ent_ID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
        });

        modelBuilder.Entity<HfTblTempAffiliateFocusedList>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_TempAffiliateFocusedList");

            entity.Property(e => e.AffiliateCode).HasColumnName("Affiliate_Code");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
        });

        modelBuilder.Entity<HfTblTopic>(entity =>
        {
            entity.HasKey(e => e.TopicId)
                .HasName("PK_Topic")
                .HasFillFactor(90);

            entity.ToTable("HF_TBL_Topics");

            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.HindiVideo).IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Topics_IsActive");
            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ParentTopicId).HasColumnName("ParentTopicID");
            entity.Property(e => e.StudentExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentNote).IsUnicode(false);
            entity.Property(e => e.StudentResourse)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentVideo).IsUnicode(false);
            entity.Property(e => e.TrainerExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerNote).IsUnicode(false);
            entity.Property(e => e.TrainerResource)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerVideo).IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.HfTblTopics)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Topics__CourseID__6CF8245B");
        });

        modelBuilder.Entity<HfTblUploadIndividualProject>(entity =>
        {
            entity.ToTable("HF_TBL_UploadIndividualProject");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FileExtension)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FileLink)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsCreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsModifiledDate)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("Student_ID");
        });

        modelBuilder.Entity<HfTblUserActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserActivityLog");

            entity.ToTable("HF_TBL_UserActivityLog");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ApplicationName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeviceDetails)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HtmlLog)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HtmlTemplate)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LogSubType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LogType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.Remark)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfTblUserAdditionApproval>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_UserAddition_Approvals");

            entity.Property(e => e.ApprovalDate).HasColumnType("datetime");
            entity.Property(e => e.ApprovalId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ApprovalID");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.InstituteId).HasColumnName("Institute_ID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SubmissionDate).HasColumnType("datetime");
            entity.Property(e => e.SubmittedByUserId).HasColumnName("SubmittedByUserID");
            entity.Property(e => e.UserRole)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("User_Role");
        });

        modelBuilder.Entity<HfTblUserNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HF_TBL_U__3214EC07F1DAB6B9");

            entity.ToTable("HF_TBL_UserNotifications");

            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.ReadAt).HasColumnType("datetime");

            entity.HasOne(d => d.Notification).WithMany(p => p.HfTblUserNotifications)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__HF_TBL_Us__Notif__61E66462");
        });

        modelBuilder.Entity<HfTblUserRole>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_UserRoles");

            entity.Property(e => e.AccessSiteId).HasColumnName("Access_Site_ID");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Role_ID");
            entity.Property(e => e.Roles).IsUnicode(false);
            entity.Property(e => e.UserTypeId).HasColumnName("User_Type_ID");
        });

        modelBuilder.Entity<HfTblUserSession>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__HF_TBL_U__1788CCAC1CBAB0EF");

            entity.ToTable("HF_TBL_UserSessions");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.LastActivity).HasColumnType("datetime");
            entity.Property(e => e.SessionId)
                .HasMaxLength(100)
                .HasColumnName("SessionID");
        });

        modelBuilder.Entity<HfTblUserSessionsAson21122025>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_UserSessionsASon21122025");

            entity.Property(e => e.LastActivity).HasColumnType("datetime");
            entity.Property(e => e.SessionId)
                .HasMaxLength(100)
                .HasColumnName("SessionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<HfTblUserType>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_User_type");

            entity.Property(e => e.UTypeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("U_type_id");
            entity.Property(e => e.UserDesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("User_Desc");
        });

        modelBuilder.Entity<HfTblVacationCompetitionSubmission>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_VacationCompetitionSubmission");

            entity.Property(e => e.ActivityType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SubmissionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("SubmissionID");
        });

        modelBuilder.Entity<HfTblVolunteer>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_Volunteers");

            entity.Property(e => e.ReferralCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VolunteerId).HasColumnName("VolunteerID");
            entity.Property(e => e.VolunteerType)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfTblVolunteerSchoolAssociation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_VolunteerSchoolAssociation");

            entity.Property(e => e.EntBy).HasColumnName("Ent_by");
            entity.Property(e => e.EntDatetime)
                .HasColumnType("datetime")
                .HasColumnName("Ent_Datetime");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_status");
            entity.Property(e => e.SchoolId).HasColumnName("School_ID");
            entity.Property(e => e.VolunteerId).HasColumnName("Volunteer_ID");
        });

        modelBuilder.Entity<HfTblWjSyllabus>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_TBL_WJ_Syllabus");

            entity.Property(e => e.CompetitionYear)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Standard_");
            entity.Property(e => e.SyllabusId)
                .ValueGeneratedOnAdd()
                .HasColumnName("SyllabusID");
        });

        modelBuilder.Entity<HfTbltopicTranslation>(entity =>
        {
            entity.HasKey(e => e.TranslationId).HasName("PK__HF_TBLTo__663DA0ACB49D28DE");

            entity.ToTable("HF_TBLTopicTranslations");

            entity.Property(e => e.TranslationId).HasColumnName("TranslationID");
            entity.Property(e => e.LanguageId).HasColumnName("LanguageID");
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
        });

        modelBuilder.Entity<HfUploadAnubhuti>(entity =>
        {
            entity.ToTable("HF_UploadAnubhuti");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.FileExtension)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FileLink)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsCreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsModifiledDate)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_Users");

            entity.Property(e => e.AddressLine1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine3)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.City).HasMaxLength(200);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("Ent_Date");
            entity.Property(e => e.EntId).HasColumnName("Ent_ID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LanguagePreference)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("Upd_Date");
            entity.Property(e => e.UpdId).HasColumnName("Upd_ID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<HfUsersAuth>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HF_Users_Auth");

            entity.Property(e => e.EmailId)
                .HasMaxLength(256)
                .HasColumnName("Email_Id");
            entity.Property(e => e.LockoutEndDateUtc).HasColumnType("datetime");
            entity.Property(e => e.RecStatus).HasColumnName("Rec_Status");
            entity.Property(e => e.Salt).HasMaxLength(100);
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("User_Id");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<HfViewActiveStudentsdetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_ActiveStudentsdetails");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .HasColumnName("Institute_Name");
            entity.Property(e => e.MainCourseDetailswithPoints)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
            entity.Property(e => e.TotalMarks)
                .HasMaxLength(3)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewActiveTeacherUnderSchool>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_ActiveTeacherUnderSchool");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
        });

        modelBuilder.Entity<HfViewAffiliatewiseValueCompletedsummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_AffiliatewiseValueCompletedsummary");

            entity.Property(e => e.AffiliateName).HasMaxLength(101);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewCompEligibleUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_Comp_Eligible_User");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfViewCompGameSummaryReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_COMP_GameSummaryReports");

            entity.Property(e => e.Categories)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FutoshikiScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.Game24Score).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.KenKenScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.Minutes)
                .HasColumnType("decimal(13, 2)")
                .HasColumnName("minutes");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PuzzleScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.SchoolName).HasMaxLength(100);
            entity.Property(e => e.ScrambleScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.TTimeInMinutes).HasColumnName("T_Time_IN_Minutes");
            entity.Property(e => e.TTimeInSecond).HasColumnName("T_Time_IN_SECOND");
            entity.Property(e => e.TotalScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(101);
            entity.Property(e => e.WordGameScore).HasColumnType("numeric(38, 2)");
        });

        modelBuilder.Entity<HfViewCompQuizReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_COMP_QuizReports");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Quiz1).HasColumnName("Quiz_1");
            entity.Property(e => e.Quiz1TimeScore).HasColumnName("Quiz1_Time_Score");
            entity.Property(e => e.Quiz2).HasColumnName("Quiz_2");
            entity.Property(e => e.Quiz2TimeScore).HasColumnName("Quiz2_Time_Score");
            entity.Property(e => e.Quiz3).HasColumnName("Quiz_3");
            entity.Property(e => e.Quiz3TimeScore).HasColumnName("Quiz3_Time_Score");
            entity.Property(e => e.Quiz4).HasColumnName("Quiz_4");
            entity.Property(e => e.Quiz4TimeScore).HasColumnName("Quiz4_Time_Score");
            entity.Property(e => e.SchoolName).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(101);
            entity.Property(e => e.UserRoles)
                .HasMaxLength(5000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewCompUserGameSummaryForNumberGame>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_Comp_UserGameSummaryForNumberGame");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FutoshikiScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.Game24Score).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.KenKenScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SchoolName).HasMaxLength(100);
            entity.Property(e => e.TotalScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.UserName).HasMaxLength(101);
            entity.Property(e => e.UserRoles)
                .HasMaxLength(5000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewCompUserGameSummaryForWordGame>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_Comp_UserGameSummaryForWordGame");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PuzzleScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.SchoolName).HasMaxLength(100);
            entity.Property(e => e.ScrambleScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.TotalScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.UserName).HasMaxLength(101);
            entity.Property(e => e.UserRoles)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.WordGameScore).HasColumnType("numeric(38, 2)");
        });

        modelBuilder.Entity<HfViewCompWordAndNumberGameDetailReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_Comp_WordAndNumberGameDetailReport");

            entity.Property(e => e.Answer)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.GameName)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("Game_Name");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SchoolName).HasMaxLength(100);
            entity.Property(e => e.Score).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.UserName).HasMaxLength(101);
            entity.Property(e => e.UserRoles)
                .HasMaxLength(5000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewCompletedStudentsMainCourseCertificate>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_Completed_StudentsMainCourseCertificate");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
            entity.Property(e => e.TotalMarks)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("Total_Marks");
        });

        modelBuilder.Entity<HfViewCompletedStudentsMainCourseCertificateAsperCertificate>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_Completed_StudentsMainCourseCertificateASPerCertificate");

            entity.Property(e => e.InstituteName).HasMaxLength(50);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Standard).HasMaxLength(50);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
            entity.Property(e => e.TotalMarks).HasColumnName("Total_Marks");
        });

        modelBuilder.Entity<HfViewCreativityStudentlist>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_CreativityStudentlist");

            entity.Property(e => e.City).HasMaxLength(200);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("Institute_ID");
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("STUDENT_ID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(102)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.ValueName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Value_Name");
        });

        modelBuilder.Entity<HfViewDiffrentExaminationRecord>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_DiffrentExaminationRecords");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.ExamTopicId).HasColumnName("ExamTopicID");
            entity.Property(e => e.ExaminationId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ExaminationID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.Rn).HasColumnName("rn");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<HfViewDiffrentExaminationRecordsSchema>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_DiffrentExaminationRecordsSCHEMA");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.ExamTopicId).HasColumnName("ExamTopicID");
            entity.Property(e => e.ExaminationId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ExaminationID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.Rn).HasColumnName("rn");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<HfViewDistinctExaminatioId>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_DistinctExaminatioID");

            entity.Property(e => e.ExamTopicId).HasColumnName("ExamTopicID");
            entity.Property(e => e.ExaminationId).HasColumnName("ExaminationID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<HfViewFocusedAffiliateList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_FocusedAffiliateList");

            entity.Property(e => e.AffiliateName).HasMaxLength(101);
            entity.Property(e => e.FamilyPbl).HasColumnName("Family_PBL");
            entity.Property(e => e.GroupPbl).HasColumnName("Group_PBL");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewInstituteAndStdWiseStudentSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstituteAndStdWiseStudentSummary");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewInstituteStandardStudentSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstituteStandardStudentSummary");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewInstituteStudentPblsummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstituteStudentPBLSummary");

            entity.Property(e => e.FamilyPblcompleted).HasColumnName("FamilyPBLCompleted");
            entity.Property(e => e.GroupPblcompleted).HasColumnName("GroupPBLCompleted");
            entity.Property(e => e.IndividualPblcompleted).HasColumnName("IndividualPBLCompleted");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewInstituteTeacherList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstituteTeacherList");

            entity.Property(e => e.City).HasMaxLength(200);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.RegisteredDate).HasColumnType("datetime");
            entity.Property(e => e.Standard)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.TeacherName)
                .HasMaxLength(102)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<HfViewInstitutewiseCourseCompletionReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstitutewiseCourseCompletionReport");

            entity.Property(e => e.CompletedCourseName)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CompletedCourse_name");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PendingCourseName)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PendingCourse_name");
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewInstitutewiseCourseCompletionReportWithdetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstitutewiseCourseCompletionReportWithdetails");

            entity.Property(e => e.CompletedCourseName)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("CompletedCourse_name");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PendingCourseIncompleteItems).HasColumnName("PendingCourse_IncompleteItems");
            entity.Property(e => e.PendingCourseName)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("PendingCourse_name");
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewInstitutewiseStdWiseTop3StudentPerformer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstitutewiseStdWiseTop3StudentPerformer");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfViewInstitutewiseStdWiseTopStudentPerformer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstitutewiseStdWiseTopStudentPerformer");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfViewInstitutewiseStudentsListCompletedSurveyAndCertificateStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstitutewiseStudentsListCompletedSurveyAndCertificateStatus");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EligibleForCertificate).HasColumnName("Eligible_For_Certificate");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfViewInstitutewiseTop10StudentPerformer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstitutewiseTop10StudentPerformer");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfViewInstitutewiseTopStudentPerformer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_InstitutewiseTopStudentPerformer");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfViewMinandMaxExaminatioId>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_MINandMAxExaminatioID");

            entity.Property(e => e.ExamTopicId).HasColumnName("ExamTopicID");
            entity.Property(e => e.MinExaminationId).HasColumnName("MinExaminationID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<HfViewPraticalSubmissionRecord>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_PraticalSubmissionRecords");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileExtension)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FileLink)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsDeleteDate).HasColumnType("datetime");
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Rn).HasColumnName("rn");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
        });

        modelBuilder.Entity<HfViewSchoolPerformance>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_SchoolPerformance");

            entity.Property(e => e.ActiveStudentPercentage).HasMaxLength(4000);
            entity.Property(e => e.ActiveTeacherPercentage).HasMaxLength(4000);
            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.AffiliateName).HasMaxLength(100);
            entity.Property(e => e.EligibleForCertificateStudentPercentage).HasMaxLength(4000);
            entity.Property(e => e.EligibleForCertificateTeacherPercentage).HasMaxLength(4000);
            entity.Property(e => e.GroupSubmissionPercentage).HasMaxLength(4000);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TotalValues)
                .HasMaxLength(2)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewSchoolStudentList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_SchoolStudentList");

            entity.Property(e => e.City).HasMaxLength(200);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.InstituteId).HasColumnName("Institute_ID");
            entity.Property(e => e.Points)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("STUDENT_ID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<HfViewStockRequisitionReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_StockRequisitionReport");

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.ItemType)
                .HasMaxLength(100)
                .HasColumnName("Item_Type");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Remarks).IsUnicode(false);
            entity.Property(e => e.RequestedDate).HasColumnType("datetime");
            entity.Property(e => e.RequisitionId).HasColumnName("RequisitionID");
            entity.Property(e => e.ShippingAddress).HasColumnName("Shipping_Address");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<HfViewStudent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_Student");

            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GardianName).HasMaxLength(50);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfViewStudentActivityLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_StudentActivityLog");

            entity.Property(e => e.ActivityDate)
                .HasColumnType("datetime")
                .HasColumnName("Activity_Date");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(14)
                .IsUnicode(false)
                .HasColumnName("Activity_Type");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfViewStudentMainCoursewiseActivityStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_StudentMainCoursewiseActivityStatus");

            entity.Property(e => e.GroupPblstatus)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("GroupPBLStatus");
            entity.Property(e => e.IndividualPblstatus)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("IndividualPBLStatus");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PostValueSurveyStatus)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
            entity.Property(e => e.TotalPoints)
                .HasMaxLength(3)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewStudentWiseCompletedMainCourseList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_StudentWiseCompletedMainCourseList");

            entity.Property(e => e.CoursesCompleted)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("Courses_Completed");
            entity.Property(e => e.CoursesPending)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("Courses_Pending");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.NoOfCoursesCompleted).HasColumnName("No_of_Courses_Completed");
            entity.Property(e => e.NoOfCoursesPending).HasColumnName("No_of_Courses_Pending");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Standard_");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
            entity.Property(e => e.TotalMarks)
                .HasMaxLength(3)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewStudentWiseCompletedMainCourseListAsPerCertificateWise>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_StudentWiseCompletedMainCourseListAsPerCertificateWise");

            entity.Property(e => e.CoursesCompleted)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("Courses_Completed");
            entity.Property(e => e.CoursesPending)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("Courses_Pending");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(50);
            entity.Property(e => e.NoOfCoursesCompleted).HasColumnName("No_of_Courses_Completed");
            entity.Property(e => e.NoOfCoursesPending).HasColumnName("No_of_Courses_Pending");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Standard_");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
            entity.Property(e => e.TotalMarks)
                .HasMaxLength(3)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HfViewStudentWiseandMaincourseWiseActivityCount>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_StudentWiseandMaincourseWiseActivityCount");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfViewStudentsCourseCompletedList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_StudentsCourseCompletedList");

            entity.Property(e => e.CoursesCompleted)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("Courses_Completed");
            entity.Property(e => e.CoursesPending)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("Courses_Pending");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(50);
            entity.Property(e => e.NoOfCoursesCompleted).HasColumnName("No_of_Courses_Completed");
            entity.Property(e => e.NoOfCoursesPending).HasColumnName("No_of_Courses_Pending");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Standard_");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfViewStudentsDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_StudentsDetails");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Div)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName).HasMaxLength(101);
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<HfViewStudentsMainCourseCertifiedList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_StudentsMainCourseCertifiedList");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .HasColumnName("Student_Name");
        });

        modelBuilder.Entity<HfViewTeacherActivityLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_TeacherActivityLog");

            entity.Property(e => e.ActivityDate)
                .HasColumnType("datetime")
                .HasColumnName("Activity_Date");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("Activity_Type");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.TeacherName)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Teacher_Name");
        });

        modelBuilder.Entity<HfViewTeacherCourseSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_TeacherCourseSummary");

            entity.Property(e => e.CourseCompleted)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.CoursePending)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.NoOfcourseCompleted).HasColumnName("NoOFCourseCompleted");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.TeacherName)
                .HasMaxLength(101)
                .HasColumnName("Teacher_Name");
        });

        modelBuilder.Entity<HfViewTeacherList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_TeacherList");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<HfViewTeacherWisePerformanceSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_VIew_TeacherWisePerformanceSummary");

            entity.Property(e => e.ActiveStudents).HasColumnName("Active_Students");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.StudentsCompleted5Values).HasColumnName("Students_Completed_5_Values");
            entity.Property(e => e.StudentsCompletedValue).HasColumnName("Students_Completed_Value");
            entity.Property(e => e.TeacherAssignedStandards)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TeacherCompletedCourses).HasColumnName("Teacher_Completed_Courses");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.TeacherName).HasMaxLength(101);
            entity.Property(e => e.TotalRegisteredStudents).HasColumnName("Total_Registered_Students");
        });

        modelBuilder.Entity<HfViewTeacherwiseCompletedCourseCount>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_TeacherwiseCompletedCourseCount");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.TeacherName)
                .HasMaxLength(101)
                .HasColumnName("Teacher_Name");
        });

        modelBuilder.Entity<HfViewUsersList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_UsersList");

            entity.Property(e => e.AddressLine1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine3)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.City).HasMaxLength(200);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(256)
                .HasColumnName("Email_Id");
            entity.Property(e => e.EntDate)
                .HasColumnType("datetime")
                .HasColumnName("Ent_Date");
            entity.Property(e => e.EntId).HasColumnName("Ent_ID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LanguagePreference)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdDate)
                .HasColumnType("datetime")
                .HasColumnName("Upd_Date");
            entity.Property(e => e.UpdId).HasColumnName("Upd_ID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<HfViewVacationProjectSubmission>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_VIEW_VacationProjectSubmission");

            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.AffiliateName).HasMaxLength(101);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CoOrdinatorName)
                .HasMaxLength(101)
                .HasColumnName("Co-Ordinator Name");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferredBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SchoolName)
                .HasMaxLength(100)
                .HasColumnName("School Name");
            entity.Property(e => e.SubmissionDate)
                .HasColumnType("datetime")
                .HasColumnName("Submission Date");
        });

        modelBuilder.Entity<HfViewValuewisePassTeacher>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_ValuewisePassTeacher");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.TeacherName)
                .HasMaxLength(101)
                .HasColumnName("Teacher_Name");
        });

        modelBuilder.Entity<HfViewVolunteer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_Volunteer");

            entity.Property(e => e.AddressLine1).HasMaxLength(250);
            entity.Property(e => e.AddressLine2).HasMaxLength(250);
            entity.Property(e => e.AddressLine3).HasMaxLength(250);
            entity.Property(e => e.AspNetUserId)
                .HasMaxLength(128)
                .HasColumnName("AspNetUserID");
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.RefInstituteName).HasMaxLength(100);
            entity.Property(e => e.RefUserId).HasColumnName("RefUserID");
            entity.Property(e => e.ReferralByCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(256);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<HfViewWjCoin>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HF_View_WJ_Coins");

            entity.Property(e => e.CoinFor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Coins).HasColumnName("coins");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<HfVisitProcessRunning>(entity =>
        {
            entity.HasKey(e => e.ProcessId).HasName("PK_HF_Visit_Process_Runing");

            entity.ToTable("HF_Visit_Process_Running");

            entity.Property(e => e.ProcessId).HasColumnName("ProcessID");
            entity.Property(e => e.AffileatedUserId).HasColumnName("AffileatedUserID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.FullyProcessStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_Table_1_Full_Process_Status")
                .HasColumnName("Fully_Process_Status");
            entity.Property(e => e.IsSchoolFeedBack)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_HF_Visit_Process_Running_IsSchool_FeedBack")
                .HasColumnName("IsSchool_FeedBack");
            entity.Property(e => e.Kpid).HasColumnName("KPID");
            entity.Property(e => e.KpidPartnerUploadSignStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("Y", "DF_Table_1_KPID_PartnerShip_Sign")
                .HasColumnName("KPID_Partner_Upload_Sign_Status");
            entity.Property(e => e.SchoolOrientationOnVisitLocationName)
                .IsUnicode(false)
                .HasColumnName("School_Orientation_OnVisit_LocationName");
            entity.Property(e => e.SchoolOrientationRemarks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("School_Orientation_Remarks");
            entity.Property(e => e.SchoolOrientationVisitDateTime)
                .HasColumnType("datetime")
                .HasColumnName("School_Orientation_Visit_DateTime");
            entity.Property(e => e.SchoolOrientationVisitImageStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_HF_Visit_Process_Running_School_Orientation_Visit_Image_Status")
                .HasColumnName("School_Orientation_Visit_Image_Status");
            entity.Property(e => e.SchoolOrientationVisitStatusCompleted)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_Table_1_School_Orientation_Vistit_Status_Completed")
                .HasColumnName("School_Orientation_Visit_Status_Completed");
            entity.Property(e => e.StudentOrientationRemarks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Student_Orientation_Remarks");
            entity.Property(e => e.StudentOrientationVisitDateTime)
                .HasColumnType("datetime")
                .HasColumnName("Student_Orientation_Visit_DateTime");
            entity.Property(e => e.StudentOrientationVisitImageStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_HF_Visit_Process_Running_Student_Orientation_Visit_Image_Status")
                .HasColumnName("Student_Orientation_Visit_Image_Status");
            entity.Property(e => e.StudentOrientationVisitStatusCompleted)
                .HasColumnType("datetime")
                .HasColumnName("Student_Orientation_Visit_Status_Completed");
            entity.Property(e => e.StudnetOrientationOnVisitLocationName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Studnet_Orientation_OnVisit_LocationName");
            entity.Property(e => e.TeacherOrientationOnVistiLocationName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Teacher_Orientation_OnVisti_LocationName");
            entity.Property(e => e.TeacherOrientationRemarks)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Teacher_Orientation_Remarks");
            entity.Property(e => e.TeacherOrientationVisitDateTime)
                .HasColumnType("datetime")
                .HasColumnName("Teacher_Orientation_Visit_DateTime");
            entity.Property(e => e.TeacherOrientationVisitImageStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_HF_Visit_Process_Running_Teacher_Orientation_Visit_Image_Status")
                .HasColumnName("Teacher_Orientation_Visit_Image_Status");
            entity.Property(e => e.TeacherOrientationVisitStatusCompleted)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_HF_Visit_Process_Running_Teacher_Orientation_Visit_Status_Completed")
                .HasColumnName("Teacher_Orientation_Visit_Status_Completed");
        });

        modelBuilder.Entity<HfWordCountRecord>(entity =>
        {
            entity.HasKey(e => e.GhfId).HasName("PK_HF_word_Count_Record");

            entity.ToTable("HF_Word_Count_Record");

            entity.Property(e => e.GhfId).HasColumnName("GHF_ID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.GameNames)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GhfCount).HasColumnName("GHF_Count");
            entity.Property(e => e.GhfOver).HasColumnName("GHF_Over");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<HvCompetitionConfirmation>(entity =>
        {
            entity.HasKey(e => e.CompId);

            entity.ToTable("HV_Competition_Confirmation");

            entity.Property(e => e.CompId).HasColumnName("CompID");
            entity.Property(e => e.CompetitionSelected).HasColumnName("Competition_Selected");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HvWebinarConfirmation>(entity =>
        {
            entity.HasKey(e => e.WebinarId);

            entity.ToTable("HV_Webinar_Confirmation");

            entity.Property(e => e.WebinarId).HasColumnName("WebinarID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.WebinarTopicSelected).HasColumnName("Webinar_Topic_Selected");
        });

        modelBuilder.Entity<KenAnswer>(entity =>
        {
            entity.ToTable("Ken_Answers");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Kmid).HasColumnName("KMID");
            entity.Property(e => e.SubmitTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<KenCage>(entity =>
        {
            entity.HasKey(e => e.Kcid)
                .HasName("PK_KenCage")
                .HasFillFactor(90);

            entity.ToTable("Ken_Cage");

            entity.Property(e => e.Kcid).HasColumnName("KCID");
            entity.Property(e => e.C1).HasMaxLength(50);
            entity.Property(e => e.C2).HasMaxLength(50);
            entity.Property(e => e.C3).HasMaxLength(50);
            entity.Property(e => e.C4).HasMaxLength(50);
            entity.Property(e => e.C5).HasMaxLength(50);
            entity.Property(e => e.Kmid).HasColumnName("KMID");
            entity.Property(e => e.Ktid).HasColumnName("KTID");
        });

        modelBuilder.Entity<KenCompCage>(entity =>
        {
            entity.HasKey(e => e.Kccid);

            entity.ToTable("Ken_Comp_Cage");

            entity.Property(e => e.Kccid).HasColumnName("KCCID");
            entity.Property(e => e.Cage1).HasMaxLength(50);
            entity.Property(e => e.Cage2).HasMaxLength(50);
            entity.Property(e => e.Cage3).HasMaxLength(50);
            entity.Property(e => e.Cage4).HasMaxLength(50);
            entity.Property(e => e.Cage5).HasMaxLength(50);
            entity.Property(e => e.Kcmid).HasColumnName("KCMID");
            entity.Property(e => e.Kctid).HasColumnName("KCTID");

            entity.HasOne(d => d.Kcm).WithMany(p => p.KenCompCages)
                .HasForeignKey(d => d.Kcmid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ken_Comp_Cage_Ken_Comp_Master");

            entity.HasOne(d => d.Kct).WithMany(p => p.KenCompCages)
                .HasForeignKey(d => d.Kctid)
                .HasConstraintName("FK_Ken_Comp_Cage_Ken_Comp_Target");
        });

        modelBuilder.Entity<KenCompMaster>(entity =>
        {
            entity.HasKey(e => e.Kcmid);

            entity.ToTable("Ken_Comp_Master");

            entity.Property(e => e.Kcmid).HasColumnName("KCMID");
            entity.Property(e => e.DisplayDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasMaxLength(50);
            entity.Property(e => e.KenLevel)
                .HasMaxLength(50)
                .HasColumnName("Ken_Level");
            entity.Property(e => e.Operation).HasMaxLength(50);
            entity.Property(e => e.QuizType).HasMaxLength(50);
            entity.Property(e => e.Size).HasMaxLength(50);
        });

        modelBuilder.Entity<KenCompSolution>(entity =>
        {
            entity.HasKey(e => e.Kcsid);

            entity.ToTable("Ken_Comp_Solution");

            entity.Property(e => e.Kcsid).HasColumnName("KCSID");
            entity.Property(e => e.Kcmid).HasColumnName("KCMID");
            entity.Property(e => e.S1).HasMaxLength(50);
            entity.Property(e => e.S2).HasMaxLength(50);
            entity.Property(e => e.S3).HasMaxLength(50);
            entity.Property(e => e.S4).HasMaxLength(50);
            entity.Property(e => e.S5).HasMaxLength(50);

            entity.HasOne(d => d.Kcm).WithMany(p => p.KenCompSolutions)
                .HasForeignKey(d => d.Kcmid)
                .HasConstraintName("FK_Ken_Comp_Solution_Ken_Comp_Master");
        });

        modelBuilder.Entity<KenCompTarget>(entity =>
        {
            entity.HasKey(e => e.Kctid);

            entity.ToTable("Ken_Comp_Target");

            entity.Property(e => e.Kctid).HasColumnName("KCTID");
            entity.Property(e => e.Kcmid).HasColumnName("KCMID");
            entity.Property(e => e.Target1).HasMaxLength(50);
            entity.Property(e => e.Target2).HasMaxLength(50);
            entity.Property(e => e.Target3).HasMaxLength(50);
            entity.Property(e => e.Target4).HasMaxLength(50);
            entity.Property(e => e.Target5).HasMaxLength(50);

            entity.HasOne(d => d.Kcm).WithMany(p => p.KenCompTargets)
                .HasForeignKey(d => d.Kcmid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ken_Comp_Target_Ken_Comp_Master");
        });

        modelBuilder.Entity<KenCompUserAnswer>(entity =>
        {
            entity.HasKey(e => e.AnsId);

            entity.ToTable("Ken_Comp_User_Answer");

            entity.Property(e => e.AnsId).HasColumnName("AnsID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Kcmid).HasColumnName("KCMID");
            entity.Property(e => e.Remark).HasMaxLength(50);
            entity.Property(e => e.TimeDiff).HasMaxLength(50);
            entity.Property(e => e.UserAnswer).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<KenKen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KenKen__3214EC27530856BE");

            entity.ToTable("KenKen");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Createdon).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<KenMaster>(entity =>
        {
            entity.HasKey(e => e.Kmid).HasName("PK_KenMaster");

            entity.ToTable("Ken_Master");

            entity.Property(e => e.Kmid).HasColumnName("KMID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Difficulty)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Operation)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<KenSolution>(entity =>
        {
            entity.HasKey(e => e.Ksid)
                .HasName("PK_KenSolution")
                .HasFillFactor(90);

            entity.ToTable("Ken_Solution");

            entity.Property(e => e.Ksid).HasColumnName("KSID");
            entity.Property(e => e.Kmid).HasColumnName("KMID");
        });

        modelBuilder.Entity<KenTarget>(entity =>
        {
            entity.HasKey(e => e.Ktid)
                .HasName("PK_KenTarget")
                .HasFillFactor(90);

            entity.ToTable("Ken_Target");

            entity.Property(e => e.Ktid).HasColumnName("KTID");
            entity.Property(e => e.Kmid).HasColumnName("KMID");
            entity.Property(e => e.T1).HasMaxLength(50);
            entity.Property(e => e.T2).HasMaxLength(50);
            entity.Property(e => e.T3).HasMaxLength(50);
            entity.Property(e => e.T4).HasMaxLength(50);
            entity.Property(e => e.T5).HasMaxLength(50);
        });

        modelBuilder.Entity<KnowlegdePartnersList>(entity =>
        {
            entity.HasKey(e => e.Kpid).HasFillFactor(90);

            entity.ToTable("Knowlegde_Partners_List");

            entity.Property(e => e.Kpid).HasColumnName("KPID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).IsUnicode(false);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__language__12696A42AC39C037");

            entity.ToTable("language");

            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LanguageName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("languageName");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.LevelId)
                .ValueGeneratedOnAdd()
                .HasColumnName("LevelID");
            entity.Property(e => e.Levels)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MainCourse>(entity =>
        {
            entity.HasKey(e => e.MainCourseId).HasName("PK_MainCourse");

            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MainCoursesLanguage>(entity =>
        {
            entity.HasKey(e => e.Id).HasFillFactor(90);

            entity.ToTable("MainCoursesLanguage");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.LanguageDescription).HasMaxLength(1000);
            entity.Property(e => e.LanguageImagePath).HasMaxLength(500);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MigrationHistory>(entity =>
        {
            entity.HasKey(e => new { e.MigrationId, e.ContextKey }).HasName("PK_dbo.__MigrationHistory");

            entity.ToTable("__MigrationHistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ContextKey).HasMaxLength(300);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Partnership>(entity =>
        {
            entity.HasKey(e => e.PartnershipId)
                .HasName("PK__Partners__AD185E799BD5F2A9")
                .HasFillFactor(90);

            entity.ToTable("Partnership");

            entity.Property(e => e.Company)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedById).HasColumnName("CreatedByID");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EditedBy)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EditedById).HasColumnName("EditedByID");
            entity.Property(e => e.EditedOn).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Mobile)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Status_");
            entity.Property(e => e.Work)
                .HasMaxLength(550)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PuzzleCompMaster>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Puzzle_Comp_Master");

            entity.Property(e => e.Answer)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Difficulty)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.Pzid).HasColumnName("PZID");
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PuzzleCompMasterOld>(entity =>
        {
            entity.HasKey(e => e.Pzid).HasName("PK_Puzzle_Comp_Master_2");

            entity.ToTable("Puzzle_Comp_Master-old");

            entity.Property(e => e.Pzid).HasColumnName("PZID");
            entity.Property(e => e.Answer)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Difficulty)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PuzzleCompTransaction>(entity =>
        {
            entity.HasKey(e => e.PztranId);

            entity.ToTable("Puzzle_Comp_Transaction");

            entity.Property(e => e.PztranId).HasColumnName("PZTranID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.GameNames).HasMaxLength(50);
            entity.Property(e => e.IsResolved)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_Puzzle_Comp_Transaction_IsResolved");
            entity.Property(e => e.IsResolvedDate).HasColumnType("datetime");
            entity.Property(e => e.Remark).HasMaxLength(50);
        });

        modelBuilder.Entity<PuzzleMaster>(entity =>
        {
            entity.HasKey(e => e.Pzid).HasFillFactor(90);

            entity.ToTable("PuzzleMaster");

            entity.Property(e => e.Pzid).HasColumnName("PZID");
            entity.Property(e => e.Answer)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Difficulty)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PuzzleTransaction>(entity =>
        {
            entity.HasKey(e => e.PztranId).HasFillFactor(90);

            entity.ToTable("PuzzleTransaction");

            entity.Property(e => e.PztranId).HasColumnName("PZTranID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.IsResolved)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("N", "DF_PuzzleTransaction_IsResolved");
            entity.Property(e => e.IsResolvedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId)
                .HasName("PK_Question")
                .HasFillFactor(90);

            entity.HasIndex(e => e.CourseId, "IX_Questions_CourseID_");

            entity.HasIndex(e => new { e.SubTopicId, e.LanguageId }, "IX_Questions_SubTopicID_languageID");

            entity.HasIndex(e => e.LanguageId, "IX_Questions_languageID_");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.ComplexityLevel)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsAdminStatus).HasColumnName("isAdminStatus");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SubTopicId).HasColumnName("SubTopicID");
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.OptionId);

            entity.ToTable("QuestionOption");

            entity.HasIndex(e => e.QuestionId, "IX_QuestionOption_QuestionID_");

            entity.Property(e => e.OptionId).HasColumnName("OptionID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Quiz");

            entity.Property(e => e.CourseAllocatedTo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.Mcqlevel)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MCQLevel");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.QuizId)
                .ValueGeneratedOnAdd()
                .HasColumnName("QuizID");
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<QuizCourseAllocation>(entity =>
        {
            entity.HasKey(e => new { e.QuizId, e.CourseId });

            entity.ToTable("QuizCourseAllocation");

            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
        });

        modelBuilder.Entity<QuizCourseLevel>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
        });

        modelBuilder.Entity<StandardList>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("StandardList");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Standards)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId)
                .HasName("PK__Students__32C52A79DBDE90AB")
                .HasFillFactor(90);

            entity.Property(e => e.StudentId)
                .ValueGeneratedNever()
                .HasColumnName("StudentID");
            entity.Property(e => e.AddressLine1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine3)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OldTallyName)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.TallyName)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserPassword)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<StudentActivityLog>(entity =>
        {
            entity.HasKey(e => e.ActivityLogId);

            entity.ToTable("student_activity_log");

            entity.Property(e => e.ActivityLogId).HasColumnName("activity_log_id");
            entity.Property(e => e.ActivityDetails).HasColumnName("activity_details");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.IsChat).HasColumnName("isChat");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
        });

        modelBuilder.Entity<StudentRankView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StudentRankView");

            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<SurveyResponse>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SurveyResponse");

            entity.HasIndex(e => new { e.CourseId, e.UserId }, "IX_SurveyResponse_CourseID, UserID_");

            entity.HasIndex(e => new { e.TopicId, e.UserId }, "IX_SurveyResponse_TopicID_UserID");

            entity.HasIndex(e => e.UserId, "IX_SurveyResponse_UserID");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.SubmitTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<TblCity>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbl_cities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.StateId).HasColumnName("state_id");
        });

        modelBuilder.Entity<TblCountry>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbl_countries");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phonecode).HasColumnName("phonecode");
            entity.Property(e => e.Sortname)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("sortname");
        });

        modelBuilder.Entity<TblSchoolAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId);

            entity.ToTable("tbl_school_accounts");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AddressLine1).HasColumnName("address_line_1");
            entity.Property(e => e.AddressLine2).HasColumnName("address_line_2");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.ChannelPartner).HasColumnName("channel_partner");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.ContactPersonId).HasColumnName("contact_person_id");
            entity.Property(e => e.Country).HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Pincode).HasColumnName("pincode");
            entity.Property(e => e.RegisteredBy).HasColumnName("registered_by");
            entity.Property(e => e.RemainderStatus).HasColumnName("remainder_status");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.SchoolBoard)
                .HasMaxLength(100)
                .HasColumnName("school_board");
            entity.Property(e => e.SchoolFees)
                .HasMaxLength(100)
                .HasColumnName("school_fees");
            entity.Property(e => e.SchoolName).HasColumnName("school_name");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalStudents).HasColumnName("total_students");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<TblState>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbl_states");

            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Templist>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("templist");

            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.AffiliateName).HasMaxLength(101);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => new { e.CourseId, e.NoOfQuestion }, "IX_Topics_CourseID_NoOfQuestion");

            entity.HasIndex(e => e.ParentTopicId, "IX_Topics_ParentTopicID_");

            entity.HasIndex(e => e.TopicId, "IX_Topics_TopicID_");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.HindiVideo).IsUnicode(false);
            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ParentTopicId).HasColumnName("ParentTopicID");
            entity.Property(e => e.StudentExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentNote).IsUnicode(false);
            entity.Property(e => e.StudentResourse)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentVideo).IsUnicode(false);
            entity.Property(e => e.TopicId)
                .ValueGeneratedOnAdd()
                .HasColumnName("TopicID");
            entity.Property(e => e.TrainerExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerNote).IsUnicode(false);
            entity.Property(e => e.TrainerResource)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerVideo).IsUnicode(false);
        });

        modelBuilder.Entity<TotalActiveStudent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TotalActiveStudents");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
        });

        modelBuilder.Entity<TotalStudent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TotalStudents");

            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.Property(e => e.TrainerId)
                .ValueGeneratedNever()
                .HasColumnName("TrainerID");
            entity.Property(e => e.AddressIdentityProofUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AlternateContactPerson)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ApprovalRemark)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.ContactPersonContactNo)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateOfJoining).HasColumnType("datetime");
            entity.Property(e => e.DateOfResignation).HasColumnType("datetime");
            entity.Property(e => e.InterestedInLearning).HasColumnName("Interested_In_Learning");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PermanentAddressLine1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.PermanentAddressLine2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.PermanentAddressLine3)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.PermanentPincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.PhotoIdentityProofUrl).IsUnicode(false);
            entity.Property(e => e.PreferedCalltimeFrom)
                .HasColumnType("datetime")
                .HasColumnName("PreferedCalltime_From");
            entity.Property(e => e.PreferedCalltimeTo)
                .HasColumnType("datetime")
                .HasColumnName("PreferedCalltime_To");
            entity.Property(e => e.ReferalId).HasColumnName("ReferalID");
            entity.Property(e => e.RegistrationMode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ResidentialLatitude).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.ResidentialLongitude).HasColumnType("decimal(10, 8)");

            entity.HasOne(d => d.Referal).WithMany(p => p.TrainerReferals)
                .HasForeignKey(d => d.ReferalId)
                .HasConstraintName("FK__Trainers__Refera__67DE6983");

            entity.HasOne(d => d.ReportToNavigation).WithMany(p => p.TrainerReportToNavigations)
                .HasForeignKey(d => d.ReportTo)
                .HasConstraintName("FK__Trainers__Report__68D28DBC");

            entity.HasOne(d => d.TrainerNavigation).WithOne(p => p.TrainerTrainerNavigation)
                .HasForeignKey<Trainer>(d => d.TrainerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Trainers__UserID__66EA454A");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId)
                .HasName("PK__Transact__55433A4BA9D8B2B9")
                .HasFillFactor(90);

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.BasicAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountAmt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountMode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DiscountRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Narration)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxId).HasColumnName("TaxID");
            entity.Property(e => e.TaxMode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmountInWords)
                .HasMaxLength(2500)
                .IsUnicode(false);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
            entity.Property(e => e.TransactionNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC0ED98992");

            entity.HasIndex(e => e.AspNetUserId, "IX_Users_AspNetUserID_");

            entity.HasIndex(e => e.CityName, "IX_Users_CityName");

            entity.HasIndex(e => e.CreatedDate, "IX_Users_CreatedDate");

            entity.HasIndex(e => e.EmailId, "IX_Users_EmailID_");

            entity.HasIndex(e => e.MobileNo, "IX_Users_MobileNo_");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine1).HasMaxLength(250);
            entity.Property(e => e.AddressLine2).HasMaxLength(250);
            entity.Property(e => e.AddressLine3).HasMaxLength(250);
            entity.Property(e => e.AspNetUserId)
                .HasMaxLength(128)
                .HasColumnName("AspNetUserID");
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsUserDeactivated).HasColumnName("isUserDeactivated");
            entity.Property(e => e.LanguagePreference)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.RegisterFrom)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.UserCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<UserAccountDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("User_account_details");

            entity.Property(e => e.AccountId)
                .ValueGeneratedOnAdd()
                .HasColumnName("account_id");
            entity.Property(e => e.AccountNumber).HasColumnName("account_number");
            entity.Property(e => e.AdharcardPath).HasColumnName("adharcard_path");
            entity.Property(e => e.BankName).HasColumnName("Bank_name");
            entity.Property(e => e.ChequePath)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Cheque_Path");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.IfscCode).HasColumnName("IFSC_code");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modified_date");
            entity.Property(e => e.PancardPath).HasColumnName("pancard_path");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");
        });

        modelBuilder.Entity<UserActivityLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserActivityLog");

            entity.Property(e => e.ApplicationName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeviceDetails)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HtmlLog)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HtmlTemplate)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LogSubType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LogType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.Remark)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserGameSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("UserGameSummary");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FutoshikiScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.Game24Score).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.KenKenScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PuzzleScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.SchoolName).HasMaxLength(100);
            entity.Property(e => e.ScrambleScore).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.UserName).HasMaxLength(101);
            entity.Property(e => e.UserRoles)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.WordGameScore).HasColumnType("numeric(38, 2)");
        });

        modelBuilder.Entity<UserTypeAssociation>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.UserType });

            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<VAnswer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_Answers");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
        });

        modelBuilder.Entity<VGetOptionDatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_GetOptionData");

            entity.Property(e => e.OptionId).HasColumnName("OptionID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
        });

        modelBuilder.Entity<VHfAnubhutiImageQuestionBind>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_HF_Anubhuti_ImageQuestionBind");

            entity.Property(e => e.AnbId).HasColumnName("AnbID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.SadImage).HasColumnName("Sad_Image");
            entity.Property(e => e.SmileImage).HasColumnName("Smile_Image");
        });

        modelBuilder.Entity<VOption>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_Options");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
        });

        modelBuilder.Entity<VOptionsWithAnswer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_OptionsWithAnswers");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
        });

        modelBuilder.Entity<VQuestionOption>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_QuestionOption");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
        });

        modelBuilder.Entity<VQuizTimeGrandTotal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_Quiz_Time_Grand_Total");

            entity.Property(e => e.TotalSeconds).HasPrecision(0);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<VRelUsersAspNetUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_Rel_Users_AspNetUsers");

            entity.Property(e => e.AspNetUserId)
                .HasMaxLength(128)
                .HasColumnName("AspNetUserID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<VScoreFutoshikiCal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_ScoreFutoshikiCal");

            entity.Property(e => e.AnsId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AnsID");
            entity.Property(e => e.Fcid).HasColumnName("FCID");
            entity.Property(e => e.FutushikiScorePoint)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Remark).HasMaxLength(50);
            entity.Property(e => e.TimeDiff).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<VScoreGameCal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_ScoreGameCal");

            entity.Property(e => e.AnsId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AnsID");
            entity.Property(e => e.Game24ScorePoint)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Gcid).HasColumnName("GCID");
            entity.Property(e => e.Remark).HasMaxLength(50);
            entity.Property(e => e.UserAnswer).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<VScoreKenKenCal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_ScoreKenKenCal");

            entity.Property(e => e.Kcmid).HasColumnName("KCMID");
            entity.Property(e => e.KenKenScorePoint)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TimeDiff).HasMaxLength(50);
            entity.Property(e => e.UserAnswer)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<VScorePuzzleCal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_ScorePuzzleCal");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.GameNames).HasMaxLength(50);
            entity.Property(e => e.IsResolved)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsResolvedDate).HasColumnType("datetime");
            entity.Property(e => e.PuzzleScorePoint)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PztranId)
                .ValueGeneratedOnAdd()
                .HasColumnName("PZTranID");
            entity.Property(e => e.Remark).HasMaxLength(50);
        });

        modelBuilder.Entity<VScoreScrambleCal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_ScoreScrambleCal");

            entity.Property(e => e.AnsId).ValueGeneratedOnAdd();
            entity.Property(e => e.Answer)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GameNames).HasMaxLength(50);
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Remark).HasMaxLength(50);
            entity.Property(e => e.ScrambleScorePoint)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VScoreWordGameCal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_ScoreWordGameCal");

            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.GameNames).HasMaxLength(50);
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Remark).HasMaxLength(50);
            entity.Property(e => e.TimeDiff).HasMaxLength(50);
            entity.Property(e => e.UserAnswer)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Wgid).HasColumnName("WGID");
            entity.Property(e => e.Wgsid)
                .ValueGeneratedOnAdd()
                .HasColumnName("WGSID");
            entity.Property(e => e.WgtopicId).HasColumnName("WGTopicID");
            entity.Property(e => e.WordGameScorePoint)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VSpAffiliateAutomail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_SP_Affiliate_Automail");

            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VWjCoin>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_WJ_Coins");

            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.SchoolName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("School Name");
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Student Name");
        });

        modelBuilder.Entity<VWorldEnviorment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_WorldEnviorment");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.TimeScore)
                .HasPrecision(0)
                .HasColumnName("Time_Score");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName)
                .HasMaxLength(101)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_Courses");

            entity.Property(e => e.ApprovalRemark)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.AuthorName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.CourseAllocatedTo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseCategorys)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseManager)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseParentId).HasColumnName("CourseParentID");
            entity.Property(e => e.CreatedByName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ExpectedDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.FeaturedImage)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Fees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsPublished).HasColumnName("isPublished");
            entity.Property(e => e.MainCategory)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedByName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PublishDate).HasColumnType("datetime");
            entity.Property(e => e.RootCategory)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.StudentExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentResourse)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerResource)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewCoursesDetailed>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_Courses_Detailed");

            entity.Property(e => e.ApprovalRemark)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.AuthorName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.CourseAllocatedTo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseCategorys)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseManager)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseParentId).HasColumnName("CourseParentID");
            entity.Property(e => e.CreatedByName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ExpectedDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.FeaturedImage)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Fees).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsPublished).HasColumnName("isPublished");
            entity.Property(e => e.MainCategory)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Mcqcount).HasColumnName("MCQCount");
            entity.Property(e => e.ModifiedByName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PublishDate).HasColumnType("datetime");
            entity.Property(e => e.RootCategory)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.StudentExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentResourse)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerResource)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewDiStinctDIstinctsCamble>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VIEW_diSTINCT_dISTINCTsCAMBLE");

            entity.Property(e => e.Answer)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<ViewExamDistinctResult>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_ExamDistinctResult");

            entity.Property(e => e.AppearedDate).HasColumnType("datetime");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.QuizName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<ViewExamResult>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_ExamResult");

            entity.Property(e => e.ExaminationId).HasColumnName("ExaminationID");
            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.RoWno).HasColumnName("RoWNo");
        });

        modelBuilder.Entity<ViewKenCompUserAnswer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_Ken_Comp_User_Answer");

            entity.Property(e => e.Kcmid).HasColumnName("KCMID");
            entity.Property(e => e.TimeDiff).HasMaxLength(50);
            entity.Property(e => e.UserAnswer)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<ViewQuestion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_Questions");

            entity.Property(e => e.ComplexityLevel)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.CourseDescription)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FeaturedImage)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewTopic>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_Topic");

            entity.Property(e => e.CourseAllocatedTo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseApprovalStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseDescription)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CourseFeaturedImage)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.HindiVideo).IsUnicode(false);
            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ParentTopicId).HasColumnName("ParentTopicID");
            entity.Property(e => e.StudentExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentNote).IsUnicode(false);
            entity.Property(e => e.StudentResourse)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.StudentVideo).IsUnicode(false);
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.TrainerExercise)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerNote).IsUnicode(false);
            entity.Property(e => e.TrainerResource)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TrainerVideo).IsUnicode(false);
        });

        modelBuilder.Entity<ViewTransactionsSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_Transactions_Summary");

            entity.Property(e => e.BasicAmount).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.DiscountAmt).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ViewUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_Users");

            entity.Property(e => e.AccessToken)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine3)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AspNetUserId)
                .HasMaxLength(128)
                .HasColumnName("AspNetUserID");
            entity.Property(e => e.AspNetUsersId).HasMaxLength(128);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IsUserDeactivated).HasColumnName("isUserDeactivated");
            entity.Property(e => e.LanguagePreference)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.RegisterFrom)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.UserCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.UserRoles)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.UserRolesId)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewWjAffiliateCommissionChart>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_AffiliateCommissionChart");

            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SchoolCommissionAmount).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.TotalCommissionAmount).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<ViewWjAffiliateCommissionRate>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_AffiliateCommissionRates");

            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.CommissionAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Source)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SourceId).HasColumnName("SourceID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<ViewWjCityStateMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_City_State_Mapping");

            entity.Property(e => e.CityName)
                .HasMaxLength(2500)
                .IsUnicode(false);
            entity.Property(e => e.FkCityId).HasColumnName("FK_CityID");
            entity.Property(e => e.FkStateId).HasColumnName("FK_StateID");
            entity.Property(e => e.StateName)
                .HasMaxLength(2500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewWjCityWiseInstitute>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_CityWiseInstitutes");

            entity.Property(e => e.Cities)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewWjCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Courses");

            entity.Property(e => e.CourseCategorys)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewWjCourseTopic>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Course_Topics");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.VideoLink).IsUnicode(false);
        });

        modelBuilder.Entity<ViewWjInstitute>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Institutes");

            entity.Property(e => e.AddressLine1).HasMaxLength(250);
            entity.Property(e => e.AffilliateCount).HasColumnName("affilliate_count");
            entity.Property(e => e.BannerImageUrl)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.InstituteType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ReferredBy)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SchoolCount).HasColumnName("School_count");
            entity.Property(e => e.Status)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentCount).HasColumnName("Student_count");
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjInstitutesAdmin>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Institutes_Admin");

            entity.Property(e => e.AddressLine1).HasMaxLength(250);
            entity.Property(e => e.BannerImageUrl)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.InstituteType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Isverify).HasColumnName("isverify");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjMonthlyAffiliateEarning>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Monthly_Affiliate_Earnings");

            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.AffiliateReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Affiliate_Referral_Code");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TotalPayment)
                .HasColumnType("decimal(29, 2)")
                .HasColumnName("Total_Payment");
        });

        modelBuilder.Entity<ViewWjPracticalList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_PracticalList");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsTask).HasColumnName("isTask");
            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LevelName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Std)
                .HasMaxLength(56)
                .IsUnicode(false);
            entity.Property(e => e.StudentGroup)
                .HasMaxLength(100)
                .HasColumnName("Student_Group");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.SubmitDate).HasColumnType("datetime");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.TeacherName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VideoLink)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewWjSchoolRanking>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_SchoolRanking");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewWjStudent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Students");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GardianName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.LevelPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TrialPeriodTill).HasColumnType("datetime");
        });

        modelBuilder.Entity<ViewWjStudentProfile>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Student_Profile");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<ViewWjStudentRanking>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_StudentRanking");

            entity.Property(e => e.SChoolRank).HasColumnName("S\r\n  \r\nchoolRank");
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentLevel)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewWjStudentsAdminList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Students_AdminList");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GardianName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.IsGoogler).HasColumnName("isGoogler");
            entity.Property(e => e.IsUserDeactivated).HasColumnName("isUserDeactivated");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegistrationDate).HasColumnName("Registration Date");
            entity.Property(e => e.RegistrationDisplayDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjStudentsDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Students_Details");

            entity.Property(e => e.AddressLine1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GardianName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LevelPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegistrationDate).HasColumnName("Registration Date");
            entity.Property(e => e.RegistrationDisplayDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TrialPeriodTill).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjStudentsDetailsAdmin>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Students_Details_Admin");

            entity.Property(e => e.AddressLine1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GardianName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LevelPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegistrationDate).HasColumnName("Registration Date");
            entity.Property(e => e.RegistrationDisplayDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TrialPeriodTill).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjStudentsDetailsInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Students_Details_Info");

            entity.Property(e => e.AddressLine1).HasMaxLength(250);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GardianName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.IsUserDeactivated).HasColumnName("isUserDeactivated");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LevelPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegistrationDate).HasColumnName("Registration Date");
            entity.Property(e => e.RegistrationDisplayDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TrialPeriodTill).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjStudentsDetailsInfoForActiveStudnet>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Students_Details_Info_for_Active_Studnets");

            entity.Property(e => e.AddressLine1).HasMaxLength(250);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GardianName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.IsUserDeactivated).HasColumnName("isUserDeactivated");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LevelPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegistrationDate).HasColumnName("Registration_Date");
            entity.Property(e => e.RegistrationDisplayDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TrialPeriodTill).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjStudentsDetailsInfoForActiveStudnetsVy>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Students_Details_Info_for_Active_Studnets_VY");

            entity.Property(e => e.AddressLine1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GardianName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsUserDeactivated).HasColumnName("isUserDeactivated");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LevelPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegistrationDate).HasColumnName("Registration_Date");
            entity.Property(e => e.RegistrationDisplayDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TrialPeriodTill).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjStudentsDetailsInfoForVacationCompetitionProject>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Students_Details_Info_for_VacationCompetitionProject");

            entity.Property(e => e.AddressLine1).HasMaxLength(250);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GardianName).HasMaxLength(50);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.InstituteName).HasMaxLength(100);
            entity.Property(e => e.IsUserDeactivated).HasColumnName("isUserDeactivated");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LevelPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegistrationDate).HasColumnName("Registration_Date");
            entity.Property(e => e.RegistrationDisplayDate)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TrialPeriodTill).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjStudentsRank>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_StudentsRank");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<ViewWjStudentsSource>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Students_Sources");

            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GardianName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.LevelPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Source)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SourceId).HasColumnName("SourceID");
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TrialPeriodTill).HasColumnType("datetime");
        });

        modelBuilder.Entity<ViewWjSyllabus>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_Syllabus");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CompetitionYear)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseDescription)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.MainCourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.QuizName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Standard_");
            entity.Property(e => e.SubCategoryName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SyllabusId).HasColumnName("SyllabusID");
        });

        modelBuilder.Entity<ViewWjTeacherDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_TeacherDetails");

            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.HemVirtueStudentStudentMcqcount).HasColumnName("HemVirtueStudentStudentMCQCount");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjTeacherPracticalList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_TeacherPracticalList");

            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileLink)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LevelName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentGroup)
                .HasMaxLength(100)
                .HasColumnName("Student_Group");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.SubmitDate).HasColumnType("datetime");
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.VideoLink)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewWjTeacherRanking>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_TeacherRanking");

            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
        });

        modelBuilder.Entity<ViewWjUserDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_UserDetails");

            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<ViewWjWeeklyPerformer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_WJ_WeeklyPerformer");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<WeeklyStudentCoin>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("WeeklyStudentCoins");

            entity.Property(e => e.StudentId).HasColumnName("studentID");
            entity.Property(e => e.WeeklyCoins).HasColumnName("Weekly Coins");
        });

        modelBuilder.Entity<WjActivityLog>(entity =>
        {
            entity.ToTable("WJ_ActivityLog");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Browser)
                .HasMaxLength(1500)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(1500)
                .IsUnicode(false)
                .HasColumnName("IPAddress");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ModuleName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.ReferenceId2)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("ReferenceID_2");
            entity.Property(e => e.ReturnUrl)
                .HasMaxLength(1500)
                .IsUnicode(false)
                .HasColumnName("ReturnURL");
        });

        modelBuilder.Entity<WjAffiliateVoucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Affil__3214EC275CB97A0C");

            entity.ToTable("WJ_AffiliateVoucher");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AffiliateId).HasColumnName("Affiliate_ID");
            entity.Property(e => e.AffiliateName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Affiliate_Name");
            entity.Property(e => e.FkRequestId).HasColumnName("Fk_RequestID");
            entity.Property(e => e.GeneratedDate)
                .HasColumnType("datetime")
                .HasColumnName("Generated_Date");
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ValidEndDate).HasColumnType("datetime");
            entity.Property(e => e.ValidStartDate).HasColumnType("datetime");
            entity.Property(e => e.VoucherCode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Voucher_Code");
        });

        modelBuilder.Entity<WjCertification>(entity =>
        {
            entity.HasKey(e => e.CertificationId);

            entity.ToTable("WJ_Certification");

            entity.Property(e => e.CertificationId).HasColumnName("CertificationID");
            entity.Property(e => e.CertificationId1)
                .HasMaxLength(50)
                .HasColumnName("Certification_ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DifficultyLevelMarks).HasColumnName("Difficulty_Level_Marks");
            entity.Property(e => e.EngagementMarks).HasColumnName("Engagement_Marks");
            entity.Property(e => e.ExamDate).HasColumnType("datetime");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasColumnName("File_Name");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.PlanningMarks).HasColumnName("Planning_Marks");
            entity.Property(e => e.ProjectMark).HasColumnName("Project_mark");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.ResourcesUsageMarks).HasColumnName("Resources_Usage_Marks");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.UploadDate).HasColumnType("datetime");
            entity.Property(e => e.VideoDescription).HasColumnName("Video_Description");
        });

        modelBuilder.Entity<WjCertificationAccess>(entity =>
        {
            entity.HasKey(e => e.VipId).HasName("PK__WJ_Certi__C30A9C1F229393D6");

            entity.ToTable("WJ_Certification_Access");

            entity.Property(e => e.VipId).HasColumnName("VipID");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EndtO)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasColumnName("ImageURl");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.StartFrom)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("startFrom");
        });

        modelBuilder.Entity<WjCertificationCourseList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Certi__3214EC27A893E057");

            entity.ToTable("WJ_CertificationCourseList");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.Role)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("S")
                .IsFixedLength();
        });

        modelBuilder.Entity<WjCity>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__WJ_City__F2D21A96EFAA7566");

            entity.ToTable("WJ_City");

            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CityName)
                .HasMaxLength(2500)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<WjCoin>(entity =>
        {
            entity.HasKey(e => e.CoinId).HasName("PK__WJ_Coins__3CD32F814106E02D");

            entity.ToTable("WJ_Coins");

            entity.HasIndex(e => e.StudentId, "IX_WJ_Coins_StudentID");

            entity.Property(e => e.CoinId).HasColumnName("CoinID");
            entity.Property(e => e.CoinFor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExamTopicId).HasColumnName("ExamTopicID");
            entity.Property(e => e.QuizLevelId).HasColumnName("QuizLevelID");
            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<WjCourseSetup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Cours__3214EC075B5A895B");

            entity.ToTable("WJ_Course_Setup");

            entity.Property(e => e.CourseName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.PdfimageUrl)
                .HasMaxLength(500)
                .HasColumnName("PDFImageUrl");
            entity.Property(e => e.Pdfurl)
                .HasMaxLength(500)
                .HasColumnName("PDFUrl");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);
            entity.Property(e => e.WebinarLink).HasMaxLength(500);
        });

        modelBuilder.Entity<WjEarning>(entity =>
        {
            entity.HasKey(e => e.EarningId).HasName("PK__WJ_Earni__2418A1C2A76126C2");

            entity.ToTable("WJ_Earnings");

            entity.Property(e => e.EarningId).HasColumnName("EarningID");
            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EarningMode)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WjEmailVerification>(entity =>
        {
            entity.HasKey(e => e.EmailVerificationId);

            entity.ToTable("WJ_EmailVerification");

            entity.Property(e => e.EmailVerificationId).HasColumnName("EmailVerificationID");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("Create_date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_date");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("Update_date");
        });

        modelBuilder.Entity<WjEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__WJ_Event__7944C870C2D163F0");

            entity.ToTable("WJ_Events");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.EventAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.EventEndDate).HasColumnType("datetime");
            entity.Property(e => e.EventMarketingEndDate).HasColumnType("datetime");
            entity.Property(e => e.EventParticipantEndDate).HasColumnType("datetime");
            entity.Property(e => e.EventParticipantStartDate).HasColumnType("datetime");
            entity.Property(e => e.EventStartDate).HasColumnType("datetime");
            entity.Property(e => e.FkEventTypeId).HasColumnName("FK_EventTypeID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Message)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.FkEventType).WithMany(p => p.WjEvents)
                .HasForeignKey(d => d.FkEventTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_Events__FK_Ev__45C948A1");
        });

        modelBuilder.Entity<WjEventTeacherWinner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Event__3214EC2732E12EC5");

            entity.ToTable("WJ_EventTeacherWinner");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Coins)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.EmailId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FkEventIddetails).HasColumnName("FK_EventIDDetails");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.WinningDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<WjEventType>(entity =>
        {
            entity.HasKey(e => e.EventTypeId).HasName("PK__WJ_Event__A9216B1F487A767B");

            entity.ToTable("WJ_EventType");

            entity.Property(e => e.EventTypeId).HasColumnName("EventTypeID");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Message)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WjEventUser>(entity =>
        {
            entity.HasKey(e => e.EventUserId).HasName("PK__WJ_Event__75F924A3BF7D0D14");

            entity.ToTable("WJ_EventUsers");

            entity.Property(e => e.EventUserId).HasColumnName("EventUserID");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.ExpiredOn).HasColumnType("datetime");
            entity.Property(e => e.FkeventId).HasColumnName("FKEventID");
            entity.Property(e => e.Message)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<WjFaq>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_FAQ__3214EC27D2E0553B");

            entity.ToTable("WJ_FAQ");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsTechathon)
                .HasDefaultValue(false)
                .HasColumnName("isTechathon");
            entity.Property(e => e.IsWhizjuniors)
                .HasDefaultValue(false)
                .HasColumnName("isWhizjuniors");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(false);
        });

        modelBuilder.Entity<WjFileUpload>(entity =>
        {
            entity.HasKey(e => e.FileUploadId).HasName("PK__Wj_FileU__6F30D8D6E49A351E");

            entity.ToTable("Wj_FileUpload");

            entity.Property(e => e.FileUploadId).HasColumnName("FileUploadID");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<WjGame24>(entity =>
        {
            entity.HasKey(e => e.Game24Id).HasFillFactor(90);

            entity.ToTable("WJ_Game24");

            entity.Property(e => e.Game24Id).HasColumnName("Game24ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(100);
            entity.Property(e => e.Game24Level)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GameTopicId).HasColumnName("GameTopicID");

            entity.HasOne(d => d.GameTopic).WithMany(p => p.WjGame24s)
                .HasForeignKey(d => d.GameTopicId)
                .HasConstraintName("FK_WJ_Game24_WJ_Game24");
        });

        modelBuilder.Entity<WjGame24Topic>(entity =>
        {
            entity.HasKey(e => e.GameTopicId);

            entity.ToTable("WJ_Game24Topic");

            entity.Property(e => e.GameTopicId).HasColumnName("GameTopicID");
            entity.Property(e => e.GameTopicName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WjGem>(entity =>
        {
            entity.HasKey(e => e.GemsId)
                .HasName("PK__WJ_Gems__85F40455457DF21E")
                .HasFillFactor(90);

            entity.ToTable("WJ_Gems");

            entity.Property(e => e.GemsId).HasColumnName("GemsID");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.GemsFor)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsApp)
                .HasDefaultValue(false)
                .HasColumnName("isApp");
            entity.Property(e => e.QuizLevelId).HasColumnName("QuizLevelID");
            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.Status).HasDefaultValue(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<WjGift>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK__WJ_Gifts__3214EC27B1ACC646")
                .HasFillFactor(90);

            entity.ToTable("WJ_Gifts");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.FkWinnerId).HasColumnName("Fk_WinnerID");
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false)
                .HasColumnName("isUsed");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.Type).HasColumnName("type");
        });

        modelBuilder.Entity<WjInstitute>(entity =>
        {
            entity.HasKey(e => e.InstituteId)
                .HasName("PK__WJ_Insti__09EC0D9B515AF6DE")
                .HasFillFactor(90);

            entity.ToTable("WJ_Institutes");

            entity.HasIndex(e => e.InstituteType, "IX_WJ_Institutes_InstituteType_");

            entity.HasIndex(e => new { e.InstituteType, e.ReferralCode }, "IX_WJ_Institutes_InstituteType_ReferralCode");

            entity.HasIndex(e => new { e.InstituteType, e.ReferralCode }, "IX_WJ_Institutes_Type_Referral");

            entity.HasIndex(e => e.ProfileUrl, "UQ__WJ_Insti__B3B7E2F054376389")
                .IsUnique()
                .HasFillFactor(90);

            entity.Property(e => e.InstituteId)
                .ValueGeneratedNever()
                .HasColumnName("InstituteID");
            entity.Property(e => e.BannerImageUrl)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InstituteType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__WJ_Instit__IsAct__5713D034");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Profession).HasMaxLength(100);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SocialOrgName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.TypeOfOrganisation)
                .HasMaxLength(256)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WjMainCertificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId);

            entity.ToTable("WJ_Main_Certificates");

            entity.Property(e => e.CertificateId).HasColumnName("CertificateID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");

            entity.HasOne(d => d.Institute).WithMany(p => p.WjMainCertificates)
                .HasForeignKey(d => d.InstituteId)
                .HasConstraintName("FK_WJ_Main_Certificates_WJ_Institutes");
        });

        modelBuilder.Entity<WjPaymentPlansLog>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__WJ_Payme__9B556A58267D4701");

            entity.ToTable("WJ_Payment_Plans_Logs");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.ExpiredOn)
                .HasColumnType("datetime")
                .HasColumnName("Expired On");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpadtedOn)
                .HasColumnType("datetime")
                .HasColumnName("Upadted_On");
            entity.Property(e => e.Validity).HasColumnType("datetime");
        });

        modelBuilder.Entity<WjPractical>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WJ_Practicals");

            entity.HasIndex(e => e.CourseId, "IX_WJ_Practicals_CourseID_");

            entity.HasIndex(e => e.PracticalId, "IX_WJ_Practicals_PracticalID");

            entity.HasIndex(e => e.StudentGroup, "IX_WJ_Practicals__Student_Group");

            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsTask).HasColumnName("isTask");
            entity.Property(e => e.LanguageId).HasColumnName("languageID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PracticalId)
                .ValueGeneratedOnAdd()
                .HasColumnName("PracticalID");
            entity.Property(e => e.PracticalStandard).HasMaxLength(10);
            entity.Property(e => e.PracticalSubtopicId).HasColumnName("PracticalSubtopicID");
            entity.Property(e => e.PracticalTopicId).HasColumnName("PracticalTopicID");
            entity.Property(e => e.Resources)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ScrambleTopicId).HasColumnName("ScrambleTopicID");
            entity.Property(e => e.StudentGroup)
                .HasMaxLength(100)
                .HasColumnName("Student_Group");
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Type).HasMaxLength(500);
            entity.Property(e => e.VideoLink)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WjPracticalComment>(entity =>
        {
            entity.HasKey(e => e.PracticalCommentId).HasName("PK_PracticalComments");

            entity.ToTable("WJ_PracticalComments");

            entity.Property(e => e.PracticalCommentId).HasColumnName("PracticalCommentID");
            entity.Property(e => e.Comments)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Submission).WithMany(p => p.WjPracticalComments)
                .HasForeignKey(d => d.SubmissionId)
                .HasConstraintName("FK_PracticalComments_submission");
        });

        modelBuilder.Entity<WjPracticalSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__WJ_Pract__449EE1056AC77C01");

            entity.ToTable("WJ_PracticalSubmission");

            entity.HasIndex(e => e.StudentId, "IX_WJ_PracticalSubmission_StudentID");

            entity.HasIndex(e => new { e.StudentId, e.PracticalId }, "IX_WJ_PracticalSubmission_StudentID_PracticalID");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())", "DF__WJ_Practi__Creat__2DA7A64D")
                .HasColumnType("datetime");
            entity.Property(e => e.FileExtension)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FileLink)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsDeleteDate).HasColumnType("datetime");
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.WjPracticalSubmissions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_Practi__Stude__2BBF5DDB");
        });

        modelBuilder.Entity<WjReceiptVoucher>(entity =>
        {
            entity.HasKey(e => e.ReceiptVoucherId).HasName("PK__WJ_Recei__F7C6A4CDD758C5E6");

            entity.ToTable("WJ_ReceiptVoucher");

            entity.Property(e => e.ReceiptVoucherId).HasColumnName("ReceiptVoucherID");
            entity.Property(e => e.ChqBankName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ChqDate).HasColumnType("datetime");
            entity.Property(e => e.ChqNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Narration)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.RcptAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RcptAmountInWords)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.RcptDate).HasColumnType("datetime");
            entity.Property(e => e.RcptMode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RcptNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Remark)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.WjReceiptVouchers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_Receip__UserI__16F94B1F");
        });

        modelBuilder.Entity<WjReferralCode>(entity =>
        {
            entity.HasKey(e => e.ReferralId).HasName("PK__WJ_Refer__A2C4A946683E5C36");

            entity.ToTable("WJ_ReferralCodes");

            entity.HasIndex(e => e.UserId, "IX_WJ_ReferralCodes_UserID_");

            entity.HasIndex(e => e.ReferralCode, "UQ__WJ_Refer__7E0678126B1AC8E1")
                .IsUnique()
                .HasFillFactor(90);

            entity.Property(e => e.ReferralId).HasColumnName("ReferralID");
            entity.Property(e => e.ApplicableFrom).HasColumnType("datetime");
            entity.Property(e => e.ApplicableTill).HasColumnType("datetime");
            entity.Property(e => e.CommissionAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CommisssionPerAffiliate)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("commisssion_per_affiliate");
            entity.Property(e => e.CreditCommission)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("credit_commission");
            entity.Property(e => e.FreeStudentCommision).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__WJ_Referr__IsAct__6D031153");
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrailDays).HasColumnName("Trail_Days");
            entity.Property(e => e.TrailType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Trail_Type");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.WjReferralCodes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__WJ_Referr__UserI__6DF7358C");
        });

        modelBuilder.Entity<WjScrambleAnswer>(entity =>
        {
            entity.HasKey(e => e.AnsId)
                .HasName("PK__WJ_Scram__135B838D8E4FEC1F")
                .HasFillFactor(90);

            entity.ToTable("WJ_ScrambleAnswer");

            entity.Property(e => e.Answer)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<WjScrambleAnswerComp>(entity =>
        {
            entity.HasKey(e => e.AnsId).HasName("PK__WJ_Scram__135B838DB6F24E5D");

            entity.ToTable("WJ_ScrambleAnswer_Comp");

            entity.Property(e => e.Answer)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GameNames).HasMaxLength(50);
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Remark).HasMaxLength(50);
        });

        modelBuilder.Entity<WjScrambleMaster>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_dbo.WJ_ScrambleMaster")
                .HasFillFactor(90);

            entity.ToTable("WJ_ScrambleMaster");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.ScrambleWord).HasMaxLength(20);
            entity.Property(e => e.UnscrambleWord).HasMaxLength(20);
        });

        modelBuilder.Entity<WjScrambleQuestion>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_dbo.WJ_ScrambleQuestion")
                .HasFillFactor(90);

            entity.ToTable("WJ_ScrambleQuestion");

            entity.Property(e => e.Answer).HasMaxLength(20);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.JumbledWord).HasMaxLength(20);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Question).HasMaxLength(1024);
            entity.Property(e => e.ScrambleType).HasMaxLength(20);
        });

        modelBuilder.Entity<WjScrambleTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.WJ_ScrambleTopic");

            entity.ToTable("WJ_ScrambleTopic");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.ScrambleTopic).HasMaxLength(100);
        });

        modelBuilder.Entity<WjSessionTracking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Sessi__3214EC278696A43E");

            entity.ToTable("WJ_SessionTracking");

            entity.HasIndex(e => new { e.UserId, e.CreatedOn }, "IX_WJ_SessionTracking_UserID_Created_On");

            entity.HasIndex(e => new { e.UserId, e.Urlaccessed }, "IX_WJ_SessionTracking_UserID_URLAccessed");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IPAddress");
            entity.Property(e => e.TimeAccessed).HasColumnType("datetime");
            entity.Property(e => e.Urlaccessed)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("URLAccessed");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WjsessionId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("WJSessionID");
        });

        modelBuilder.Entity<WjState>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("PK__WJ_State__C3BA3B5A32BFAF15");

            entity.ToTable("WJ_State");

            entity.Property(e => e.StateId).HasColumnName("StateID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.StateName)
                .HasMaxLength(2500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WjStudent>(entity =>
        {
            entity.HasKey(e => e.StudentId)
                .HasName("PK__WJ_Stude__32C52A7948C5B0DD")
                .HasFillFactor(90);

            entity.ToTable("WJ_Students");

            entity.HasIndex(e => e.IsPaid, "IX_WJ_Students_IsPaid_");

            entity.HasIndex(e => e.ProfileUrl, "UQ__WJ_Stude__B3B7E2F04BA21D88").IsUnique();

            entity.Property(e => e.StudentId)
                .ValueGeneratedNever()
                .HasColumnName("StudentID");
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GardianName).HasMaxLength(50);
            entity.Property(e => e.GuardianEmailId)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("GuardianEmailID");
            entity.Property(e => e.GuardianMobileNo)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__WJ_Studen__IsAct__4E7E8A33");
            entity.Property(e => e.LevelPath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RegistrationAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RollNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.TrialPeriodTill).HasColumnType("datetime");
        });

        modelBuilder.Entity<WjStudentAffiliateAssociate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Stude__3214EC277682736E");

            entity.ToTable("WJ_StudentAffiliateAssociates");

            entity.HasIndex(e => e.AffiliateId, "IX_WJ_StudentAffiliateAssociates_AffiliateID_");

            entity.HasIndex(e => e.StudentId, "IX_WJ_StudentAffiliateAssociates_StudentID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AffiliateId).HasColumnName("AffiliateID");
            entity.Property(e => e.Createdon).HasColumnType("datetime");
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<WjStudentAssociate>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.InstituteId }).HasFillFactor(90);

            entity.ToTable("WJ_StudentAssociates");

            entity.HasIndex(e => e.InstituteId, "IX_WJ_StudentAssociates_InstituteID_");

            entity.HasIndex(e => new { e.InstituteId, e.StudentId }, "IX_WJ_StudentAssociates_InstituteID_StudentID");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");

            entity.HasOne(d => d.Institute).WithMany(p => p.WjStudentAssociates)
                .HasForeignKey(d => d.InstituteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_Studen__Insti__5CCCA98A");

            entity.HasOne(d => d.Student).WithMany(p => p.WjStudentAssociates)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_Studen__Stude__5BD88551");
        });

        modelBuilder.Entity<WjStudentCertificationAccess>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_stude__3214EC2709D6307F");

            entity.ToTable("WJ_student_Certification_Access");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.VipId).HasColumnName("VipID");

            entity.HasOne(d => d.Vip).WithMany(p => p.WjStudentCertificationAccesses)
                .HasForeignKey(d => d.VipId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_studen__VipID__2FDA0782");
        });

        modelBuilder.Entity<WjStudentCredit>(entity =>
        {
            entity.HasKey(e => e.CreditId).HasName("PK__WJ_Stude__0BAAE0B30191AAD8");

            entity.ToTable("WJ_Student_Credit");

            entity.Property(e => e.CreditId).HasColumnName("creditID");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Credit).HasColumnName("credit");
            entity.Property(e => e.CreditFor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("creditFor");
            entity.Property(e => e.ModifyOn).HasColumnType("datetime");
            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<WjStudentFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId)
                .HasName("PK__WJ_Stude__6A4BEDF6C08E8F5F")
                .HasFillFactor(90);

            entity.ToTable("WJ_StudentFeedbacks");

            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.Comments)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreateOn).HasColumnType("datetime");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.WjStudentFeedbacks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WJ_StudentFeedbacks_Users");
        });

        modelBuilder.Entity<WjStudentRanking2017>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WJ_StudentRanking2017");

            entity.Property(e => e.AddressLine1)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AddressLine3)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Division)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GardianName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.InstituteName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentLevel)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WjStudentTeacherAssociation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Stude__3214EC273DDA4636");

            entity.ToTable("WJ_Student_Teacher_Association");

            entity.HasIndex(e => e.StudentId, "IX_WJ_Student_Teacher_Association_StudentID_");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
        });

        modelBuilder.Entity<WjSyllabus>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WJ_Syllabus");

            entity.HasIndex(e => new { e.CourseId, e.OrderNo }, "IX_WJ_Syllabus_CourseID_OrderNo");

            entity.Property(e => e.CompetitionYear)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");
            entity.Property(e => e.MainCourseId).HasColumnName("MainCourseID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.Standard)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Standard_");
            entity.Property(e => e.SyllabusId)
                .ValueGeneratedOnAdd()
                .HasColumnName("SyllabusID");

            entity.HasOne(d => d.Institute).WithMany()
                .HasForeignKey(d => d.InstituteId)
                .HasConstraintName("FK__WJ_Syllab__Insti__44C015CF");
        });

        modelBuilder.Entity<WjTaskStudent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Task___3214EC27C8B62147");

            entity.ToTable("WJ_Task_Student");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CanShow)
                .HasDefaultValue(true)
                .HasColumnName("canShow");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TaskId).HasColumnName("TaskID");
        });

        modelBuilder.Entity<WjTeacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId);

            entity.ToTable("WJ_Teacher");

            entity.Property(e => e.TeacherId)
                .ValueGeneratedNever()
                .HasColumnName("TeacherID");
            entity.Property(e => e.Division)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.ProfileUrl)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Standard)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Teacher).WithOne(p => p.WjTeacher)
                .HasForeignKey<WjTeacher>(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WJ_Teacher_Users");
        });

        modelBuilder.Entity<WjTeacherAssociate>(entity =>
        {
            entity.HasKey(e => new { e.TeacherId, e.InstituteId });

            entity.ToTable("WJ_TeacherAssociates");

            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.InstituteId).HasColumnName("InstituteID");

            entity.HasOne(d => d.Institute).WithMany(p => p.WjTeacherAssociates)
                .HasForeignKey(d => d.InstituteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WJ_TeacherAssociates_WJ_Institutes");

            entity.HasOne(d => d.Teacher).WithMany(p => p.WjTeacherAssociates)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WJ_TeacherAssociates_WJ_TeacherAssociates");
        });

        modelBuilder.Entity<WjTeacherCertification>(entity =>
        {
            entity.HasKey(e => e.CertificationId);

            entity.ToTable("WJ_Teacher_Certification");

            entity.Property(e => e.CertificationId).HasColumnName("CertificationID");
            entity.Property(e => e.CertificationId1)
                .HasMaxLength(50)
                .HasColumnName("Certification_ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DifficultyLevelMarks).HasColumnName("Difficulty_Level_Marks");
            entity.Property(e => e.EngagementMarks).HasColumnName("Engagement_Marks");
            entity.Property(e => e.ExamDate).HasColumnType("datetime");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasColumnName("File_Name");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.PlanningMarks).HasColumnName("Planning_Marks");
            entity.Property(e => e.ProjectMark).HasColumnName("Project_mark");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.ResourcesUsageMarks).HasColumnName("Resources_Usage_Marks");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.UploadDate).HasColumnType("datetime");
            entity.Property(e => e.VideoDescription).HasColumnName("Video_Description");
        });

        modelBuilder.Entity<WjTeacherPracticalSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__WJ_Teach__449EE1055A055D03");

            entity.ToTable("WJ_Teacher_PracticalSubmission");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileExtension)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FileLink)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsDeleteDate).HasColumnType("datetime");
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Teacher).WithMany(p => p.WjTeacherPracticalSubmissions)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_Teache__Teach__3469B275");
        });

        modelBuilder.Entity<WjTeacherStudentPracticalSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__WJ_Teach__449EE105E54A80E3");

            entity.ToTable("WJ_TeacherStudent_PracticalSubmission");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())", "DF__WJ_Teache__Creat__55174480")
                .HasColumnType("datetime");
            entity.Property(e => e.FileExtension)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FileLink)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsDeleteDate).HasColumnType("datetime");
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
        });

        modelBuilder.Entity<WjTeachersCoin>(entity =>
        {
            entity.HasKey(e => e.CoinId).HasName("PK__WJ_Teach__3CD32F819465CE3B");

            entity.ToTable("WJ_Teachers_Coins");

            entity.Property(e => e.CoinId).HasColumnName("CoinID");
            entity.Property(e => e.CoinFor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuizLevelId).HasColumnName("QuizLevelID");
            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Teacher).WithMany(p => p.WjTeachersCoins)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_Teache__Teach__224B023A");
        });

        modelBuilder.Entity<WjTeam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Team__3214EC276D2F8B2F");

            entity.ToTable("WJ_Team");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.Logo)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.StatusDate).HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WjTeamMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_TeamM__3214EC2714BDB944");

            entity.ToTable("WJ_TeamMembers");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.FkTeamId).HasColumnName("FK_TeamID");
            entity.Property(e => e.Message)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.StatusDate).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.FkTeam).WithMany(p => p.WjTeamMembers)
                .HasForeignKey(d => d.FkTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_TeamMe__FK_Te__66010E09");
        });

        modelBuilder.Entity<WjTechTeacherQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_Tech___3214EC27350A1423");

            entity.ToTable("WJ_Tech_Teacher_Questions");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsApp)
                .HasDefaultValue(false)
                .HasColumnName("isApp");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.Status)
                .HasDefaultValue(false)
                .HasColumnName("status");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
        });

        modelBuilder.Entity<WjUserPracticalUnlockStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_UserP__3214EC07CA4645BA");

            entity.ToTable("WJ_UserPractical_UnlockStatus");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<WjValueCertification>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WJ_Value_Certification");

            entity.Property(e => e.CertificationNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasComputedColumnSql("('HEM'+right('000000000000'+CONVERT([varchar](15),[ID],(0)),(200)))", true);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Division).HasMaxLength(50);
            entity.Property(e => e.EmailId).HasMaxLength(50);
            entity.Property(e => e.ExpiredOn).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GeneratedOn).HasColumnType("datetime");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.InstituteName).HasMaxLength(255);
            entity.Property(e => e.IssuedOn).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Lavel).HasMaxLength(255);
            entity.Property(e => e.MobileNo).HasMaxLength(15);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Standard).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<WjValueCertificationBackup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WJ_Value_Certification_BACKUP");

            entity.Property(e => e.AcedamicYear)
                .IsUnicode(false)
                .HasColumnName("Acedamic_Year");
            entity.Property(e => e.CertificationNo).IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Division).HasMaxLength(50);
            entity.Property(e => e.EmailId).HasMaxLength(50);
            entity.Property(e => e.ExpiredOn).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GeneratedOn).HasColumnType("datetime");
            entity.Property(e => e.IssuedOn).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Lavel).HasMaxLength(50);
            entity.Property(e => e.MobileNo).HasMaxLength(15);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Standard).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<WjValueCertificationOld>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WJ_Value_CertificationOLD");

            entity.ToTable("WJ_Value_Certification_OLD");

            entity.Property(e => e.AcedamicYear)
                .IsUnicode(false)
                .HasColumnName("Acedamic_Year");
            entity.Property(e => e.CertificationNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasComputedColumnSql("('HEM'+right('000000000000'+CONVERT([varchar](15),[ID],(0)),(200)))", true);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Division).HasMaxLength(50);
            entity.Property(e => e.EmailId).HasMaxLength(50);
            entity.Property(e => e.ExpiredOn).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.GeneratedOn).HasColumnType("datetime");
            entity.Property(e => e.InstituteName).HasMaxLength(50);
            entity.Property(e => e.IssuedOn).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Lavel).HasMaxLength(50);
            entity.Property(e => e.MobileNo).HasMaxLength(15);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Standard).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<WjWordGameQuestion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WJ_WordGameQuestions");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.WgAnswer).HasColumnName("WG_Answer");
            entity.Property(e => e.WgQuestion).HasColumnName("WG_Question");
            entity.Property(e => e.Wgid).HasColumnName("WGID");
            entity.Property(e => e.WgtopicId).HasColumnName("WGTopicID");
        });

        modelBuilder.Entity<WjWordGameTopic>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WJ_WordGameTopic");

            entity.Property(e => e.WgTopicId)
                .ValueGeneratedOnAdd()
                .HasColumnName("WG_TopicID");
            entity.Property(e => e.WgTopicName).HasColumnName("WG_TopicName");
        });

        modelBuilder.Entity<WjZoneWise>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WJ_ZoneW__3214EC271AC0EB42");

            entity.ToTable("WJ_ZoneWise");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("Created_On");
            entity.Property(e => e.FkCityId).HasColumnName("FK_CityID");
            entity.Property(e => e.FkStateId).HasColumnName("FK_StateID");
            entity.Property(e => e.FkZoneId).HasColumnName("FK_ZoneID");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.FkCity).WithMany(p => p.WjZoneWises)
                .HasForeignKey(d => d.FkCityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_ZoneWi__FK_Ci__6AC5C326");

            entity.HasOne(d => d.FkState).WithMany(p => p.WjZoneWises)
                .HasForeignKey(d => d.FkStateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WJ_ZoneWi__FK_St__6CAE0B98");
        });

        modelBuilder.Entity<WordGameSolution>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WordGameSolution");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.SolutionAnswers)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Wgid).HasColumnName("WGID");
            entity.Property(e => e.Wgsid)
                .ValueGeneratedOnAdd()
                .HasColumnName("WGSID");
        });

        modelBuilder.Entity<WordGameSolutionComp>(entity =>
        {
            entity.HasKey(e => e.Wgsid);

            entity.ToTable("WordGameSolution_Comp");

            entity.Property(e => e.Wgsid).HasColumnName("WGSID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.GameNames).HasMaxLength(50);
            entity.Property(e => e.PracticalId).HasColumnName("PracticalID");
            entity.Property(e => e.Remark).HasMaxLength(50);
            entity.Property(e => e.TimeDiff).HasMaxLength(50);
            entity.Property(e => e.UserAnswer)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Wgid).HasColumnName("WGID");
            entity.Property(e => e.WgtopicId).HasColumnName("WGTopicID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
