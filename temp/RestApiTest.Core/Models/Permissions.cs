using System;

namespace RestApiTest.Core.Models
{
    //?? Czy tego typu rzeczy też się daje jako model?
    //public class Permissions
    //{
    //    public bool canAsk { get; set; }
    //    public bool canMark { get; set; }
    //    public bool isModerator { get; set; }
    //    public bool isAdministrator { get; set; }
    //} //TODO: wyliczać biznesową logiką

    [Flags]
    public enum Perms
    {
        canAsk = 1,
        canDeleteComments = 2,
        canEditComments = 4,
        canSetMarks = 8,
        canManageUsers = 16
    }
}
