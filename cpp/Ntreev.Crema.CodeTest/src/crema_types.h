#pragma once

namespace CremaCode
{
    // Creator: admin
    // CreatedDateTime: 2017-10-25 오전 8:40:06
    // Modifier: admin
    // ModifiedDateTime: 2017-11-01 오전 8:15:33
    enum KindType
    {
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:40:25
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 8:40:25
        KindType_Common = 0x00000000,
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:40:28
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 8:40:28
        KindType_Client = 0x00000001,
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:40:31
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 8:40:31
        KindType_Server = 0x00000002,
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:40:36
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 8:40:36
        KindType_Tools = 0x00000003,
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:41:45
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 8:41:45
        KindType_SubModule = 0x00000004,
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 8:15:33
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 8:15:33
        KindType_Executable = 0x00000005,
    };
    // Creator: admin
    // CreatedDateTime: 2017-11-01 오전 8:18:26
    // Modifier: admin
    // ModifiedDateTime: 2017-11-01 오전 8:21:51
    enum ProjectType
    {
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 8:19:07
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 8:19:07
        ProjectType_Library = 0x00000000,
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 8:21:30
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 8:21:50
        ProjectType_Executable = 0x00000001,
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 8:21:24
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 8:21:51
        ProjectType_SharedLibrary = 0x00000002,
    };
    // Creator: admin
    // CreatedDateTime: 2017-10-23 오전 6:51:02
    // Modifier: admin
    // ModifiedDateTime: 2017-11-01 오전 8:26:13
    enum StringType
    {
        // Creator: admin
        // CreatedDateTime: 2017-10-24 오전 4:56:17
        // Modifier: admin
        // ModifiedDateTime: 2017-10-24 오전 4:58:56
        StringType_None = 0x00000000,
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 6:51:13
        // Modifier: admin
        // ModifiedDateTime: 2017-10-24 오전 4:58:57
        StringType_Text = 0x00000001,
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 6:59:47
        // Modifier: admin
        // ModifiedDateTime: 2017-10-24 오전 4:58:58
        StringType_Title = 0x00000002,
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 7:00:01
        // Modifier: admin
        // ModifiedDateTime: 2017-10-24 오전 4:58:58
        StringType_Label = 0x00000003,
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 7:00:19
        // Modifier: admin
        // ModifiedDateTime: 2017-10-24 오전 4:58:58
        StringType_Button = 0x00000004,
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 7:00:23
        // Modifier: admin
        // ModifiedDateTime: 2017-10-24 오전 4:58:59
        StringType_Comment = 0x00000005,
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 7:00:27
        // Modifier: admin
        // ModifiedDateTime: 2017-10-24 오전 4:58:59
        StringType_Message = 0x00000006,
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 4:50:40
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 4:50:40
        StringType_MenuItem = 0x00000007,
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 5:55:13
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 5:55:13
        StringType_Exception = 0x00000008,
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 7:54:19
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 7:54:19
        StringType_Command = 0x00000009,
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 7:27:01
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 7:27:01
        StringType_Summary = 0x0000000a,
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 8:26:13
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 8:26:13
        StringType_Tooltip = 0x0000000b,
    };
}/*namespace CremaCode*/

