using System;
using System.Collections.Generic;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods
{
    enum CremaEvents
    {
        UserStateChanged,
        UserChanged,
        UserItemCreated,
        UserItemRenamed,
        UserItemMoved,
        UserItemDeleted,
        UserLoggedIn,
        UserLoggedOut,
        UserKicked,
        UserBanChanged,
        MessageReceived,

        DomainCreated,
        DomainDeleted,
        DomainInfoChanged,
        DomainStateChanged,
        DomainUserAdded,
        DomainUserRemoved,
        DomainUserChanged,
        DomainRowAdded,
        DomainRowChanged,
        DomainRowRemoved,
        DomainPropertyChanged,

        DataBaseCreated,
        DataBaseRenamed,
        DataBaseDeleted,
        DataBaseLoaded,
        DataBaseUnloaded,
        DataBaseResetting,
        DataBaseReset,
        DataBaseAuthenticationEntered,
        DataBaseAuthenticationLeft,
        DataBaseInfoChanged,
        DataBaseStateChanged,
        DataBaseAccessChanged,
        DataBaseLockChanged,
    }
}
