
namespace TelegramBotEFCore.Models
{
    public enum UserState
    {
        None,           
        WaitingForRole, 
        BecomingTeacher,
        BecomingStudent,
        GettingTeacherData,
        GettingStudentData,
        GettingGroupData,
        GettingSubjectData,
        Teacher,
        Student
    }
}
