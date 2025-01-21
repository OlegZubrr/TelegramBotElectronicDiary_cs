using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotEFCore.Models
{
    public enum UserState
    {
        None,           
        WaitingForRole, 
        BecomingTeacher,
        BecomingStudent,
        GettingTeacherData,
        GettiStudentData,
        GettingGroupData,
        GettingSubjectData,
        Teacher,
        Student
    }
}
