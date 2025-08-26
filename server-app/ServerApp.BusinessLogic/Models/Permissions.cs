using ServerApp.DataAccess.Entities;
namespace ServerApp.BusinessLogic.Models;

public class ChatMemberPermissions
{
    public bool CanSendMessages { get; set; } = false;
    public bool CanEdit { get; set; } = false;
    public bool CanDelete { get; set; } = false;
    public bool CanAddMember { get; set; } = false;
    public bool CanRemoveMember { get; set; } = false;
    public bool CanPromote { get; set; } = false;

    public ChatMemberPermissions() { }
    public ChatMemberPermissions(ChatMemberRole role)
    {
        InitWithRole(role);
    }

    public void InitWithRole(ChatMemberRole role)
    {
        switch (role)
        {
            case ChatMemberRole.Owner: InitOwner(); break;
            case ChatMemberRole.Moderator: InitModerator(); break;
            case ChatMemberRole.Regular: InitRegular(); break;
            default: InitRegular(); break;
        }
    }

    private void InitOwner()
    {
        CanSendMessages = true;
        CanEdit = true;
        CanDelete = true;
        CanAddMember = true;
        CanRemoveMember = true;
        CanPromote = true;
    }

    private void InitModerator()
    {
        CanSendMessages = true;
        CanEdit = true;
        CanAddMember = true;
        CanRemoveMember = true;
    }

    private void InitRegular()
    {
        CanSendMessages = true;
    }
}
